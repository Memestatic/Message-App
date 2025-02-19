namespace Message_App.Models
{
    public class UserSearchResultViewModel
    {
        public required ApplicationUser User { get; set; }
        public bool IsFriend { get; set; }
    }
}
