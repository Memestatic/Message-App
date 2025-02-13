using Microsoft.AspNetCore.Identity;

namespace Message_App.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual ICollection<Friendship> Friends { get; set; }
    }
}
