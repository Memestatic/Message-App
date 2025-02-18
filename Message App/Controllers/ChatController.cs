﻿using Message_App.Data;
using Message_App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Message_App.Controllers
{
    public class ChatController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IHubContext<ChatHub> hubContext)
        {
            _userManager = userManager;
            _context = context;
            _hubContext = hubContext;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var currentUserId = (await _userManager.GetUserAsync(User)).Id;

            ViewBag.CurrentUserId = currentUserId;

            var friendsWithMessages = _context.Friendships
                .Where(f => (f.UserId == currentUserId || f.FriendId == currentUserId) && f.IsAccepted)
                .Select(f => new FriendWithLastMessageViewModel
                {
                    Id = f.UserId == currentUserId ? f.Friend.Id : f.User.Id,
                    FirstName = f.UserId == currentUserId ? f.Friend.FirstName : f.User.FirstName,
                    LastName = f.UserId == currentUserId ? f.Friend.LastName : f.User.LastName,
                    AvatarUrl = f.UserId == currentUserId ? f.Friend.AvatarUrl : f.User.AvatarUrl,
                    LastMessageContent = f.LastMessageContent,
                    LastMessageDate = string.IsNullOrEmpty(f.LastMessageDate)
                        ? (DateTime?)null
                        : DateTime.Parse(f.LastMessageDate),
                    LastMessageSenderName = f.LastMessageSenderId == currentUserId
                    ? "You" : (f.UserId == currentUserId ? f.Friend.FirstName : f.User.FirstName),
                    UnreadCount = f.UserId == currentUserId ? 0 : f.UnreadCount
                })
                .ToList();

            int unreadChatsCount = friendsWithMessages.Count(f => f.UnreadCount > 0);
            ViewBag.UnreadChatsCount = unreadChatsCount;

            return View(friendsWithMessages);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(string friendId)
        {
            var user = await _userManager.GetUserAsync(User);

            var friendship = _context.Friendships.FirstOrDefault(f =>
                (f.UserId == user.Id && f.FriendId == friendId) ||
                (f.UserId == friendId && f.FriendId == user.Id));

            if (friendship != null)
            {
                friendship.UnreadCount = 0;  // Reset licznika
                _context.Friendships.Update(friendship);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }


        [Authorize]
        [HttpGet]   
        public async Task<IActionResult> ChatWindow(string friendId)
        {
            var user = await _userManager.GetUserAsync(User);

            // Fetch chat messages between the current user and the friend.
            var messages = _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => (m.SenderId == user.Id && m.ReceiverId == friendId) ||
                        (m.SenderId == friendId && m.ReceiverId == user.Id))
            .OrderBy(m => m.Timestamp)
            .ToList();

            ViewBag.FriendId = friendId;
            return PartialView("_ChatWindow", messages);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendMessage(string friendId, string messageContent)
        {
            var user = await _userManager.GetUserAsync(User);

            if (string.IsNullOrWhiteSpace(messageContent))
            {
                return BadRequest("Message content is empty.");
            }

            var message = new Message
            {
                SenderId = user.Id,
                ReceiverId = friendId,
                Content = messageContent,
                Timestamp = DateTime.Now,
                Sender = user
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Update the last message in the friendship
            var friendship = _context.Friendships.FirstOrDefault(f =>
            (f.UserId == user.Id && f.FriendId == friendId) ||
            (f.UserId == friendId && f.FriendId == user.Id));

            if (friendship != null)
            {
                friendship.SetLastMessage(messageContent, message.Timestamp.ToString(), user.Id);
                ViewBag.friendship = friendship;

                if (friendship.FriendId == friendId)
                {
                    friendship.UnreadCount += 1;
                }

                _context.Friendships.Update(friendship);
                await _context.SaveChangesAsync();
            }

            await _hubContext.Clients.User(user.Id)
                .SendAsync("UpdateFriendList", friendId, messageContent, message.Timestamp, user.Id, user.FirstName);

            await _hubContext.Clients.User(friendId)
                .SendAsync("UpdateFriendList", user.Id, messageContent, message.Timestamp, user.Id, user.FirstName);


            // Send the message to the friend via SignalR
            var friend = await _context.FindAsync<ApplicationUser>(friendId);

            await _hubContext.Clients.Users(user.Id, friendId)
                .SendAsync("ReceiveMessage", user.Id, friendId, friend.FirstName, messageContent);

            // Optionally, return a JSON response indicating success
            return Json(new { success = true, message = "Message sent successfully." });
        }
    }
}
