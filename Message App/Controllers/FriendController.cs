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

            // Pobranie listy zaakceptowanych znajomych.
            var friends = _context.Friendships
                .Where(f => (f.UserId == user.Id || f.FriendId == user.Id) && f.IsAccepted)
                .Select(f => f.UserId == user.Id ? f.Friend : f.User)
                .ToList();

            // Pobranie listy oczekujących zaproszeń, gdzie użytkownik jest odbiorcą.
            var pendingInvitations = _context.Friendships
                .Where(f => f.FriendId == user.Id && !f.IsAccepted)
                .ToList();

            // Utworzenie ViewModelu i przekazanie do widoku.
            var viewModel = new FriendsViewModel
            {
                Friends = friends,
                PendingInvitations = pendingInvitations
            };

            return View(viewModel);
        }


        // Search Users
        [Authorize]
        public async Task<IActionResult> Search(string query)
        {
            var currentUserId = _userManager.GetUserId(User);

            var users = await _context.Users
                .Where(u => u.UserName.Contains(query) && u.Id != currentUserId)
                .Select(u => new UserSearchResultViewModel
                {
                    User = u,
                    IsFriend = _context.Friendships.Any(f =>
                        ((f.UserId == currentUserId && f.FriendId == u.Id) ||
                         (f.FriendId == currentUserId && f.UserId == u.Id))
                        && f.IsAccepted)
                })
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

                // Check if the friendship already exists, if so accept friend instead of create new friendship
                var friendship = await GetFriendshipAsync(user.Id, friendId);

                if (friendship != null)
                {
                    AcceptFriendInvitation(friendship.Id);
                }

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
        public async Task<IActionResult> AcceptFriendInvitation(int invitationId)
        {
            var friendship = await _context.Friendships.FindAsync(invitationId);
            if (friendship != null)
            {
                friendship.IsAccepted = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RejectFriendInvitation(int invitationId)
        {
            var friendship = await _context.Friendships.FindAsync(invitationId);
            if (friendship != null)
            {
                _context.Friendships.Remove(friendship);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteFriend(string friendId)
        {
            var user = await _userManager.GetUserAsync(User);
            var friend = await _context.Users.FindAsync(friendId);
            
            var friendship = await GetFriendshipAsync(user.Id.ToString(), friendId);


            if (friendship != null)
            {
                _context.Friendships.Remove(friendship);
                await _context.SaveChangesAsync();
            } 

            return RedirectToAction("Index");
        }

        public async Task<Friendship> GetFriendshipAsync(string userId, string friendId)
        {
            return await _context.Friendships.FirstOrDefaultAsync(f =>
                (f.UserId == userId && f.FriendId == friendId) ||
                (f.UserId == friendId && f.FriendId == userId));
        }

    }
}
