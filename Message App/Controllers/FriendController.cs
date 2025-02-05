using Message_App.Data;
using Message_App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Message_App.Controllers
{
    public class FriendController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public FriendController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Show the list of friends
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var friends = _context.Friendships
                .Where(f => f.UserId == user.Id || f.IsAccepted)
                .Select(f => f.Friend)
                .ToList();

            return View(friends);
        }

        // Search Users
        [Authorize]
        public async Task<IActionResult> Search(string query)
        {
            var users = await _context.Users
                .Where(u => u.UserName.Contains(query))
                .ToListAsync();

            return View(users);
        }

        // Send a friend request
        public async Task<IActionResult> AddFriend(string friendId)
        {
            var user = await _userManager.GetUserAsync(User);
            var friend = await _context.Users.FindAsync(friendId);

            if (friend != null && user.Id != friendId)
            {
                _context.Friendships.Add(new Friendship
                {
                    UserId = user.Id,
                    FriendId = friendId,
                    IsAccepted = false
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // Accept a friend request
        public async Task<IActionResult> AcceptFriend(int friendshipId)
        {
            var friendship = await _context.Friendships.FindAsync(friendshipId);
            if (friendship != null)
            {
                friendship.IsAccepted = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

    }
}
