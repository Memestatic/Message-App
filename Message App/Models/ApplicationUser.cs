using Microsoft.AspNetCore.Identity;

namespace Message_App.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Friendship> Friends { get; set; }
    }
}
