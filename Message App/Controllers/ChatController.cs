using Message_App.Data;
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
            var user = await _userManager.GetUserAsync(User);
            var friends = _context.Friendships
                .Where(f => (f.UserId == user.Id || f.FriendId == user.Id) && f.IsAccepted)
                .Select(f => f.UserId == user.Id ? f.Friend : f.User)
                .ToList();

            return View(friends);
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

            // Send the message to the friend via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", user.UserName, messageContent);

            // Optionally, return a JSON response indicating success
            return Json(new { success = true, message = "Message sent successfully." });
        }
    }
}
