namespace Message_App.Models
{
    public class FriendsViewModel
    {
        public List<ApplicationUser> Friends { get; set; }
        public List<Friendship> PendingInvitations { get; set; }
    }

}
