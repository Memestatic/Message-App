﻿using Microsoft.AspNetCore.Identity;

namespace Message_App.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? AvatarUrl { get; set; }
        public virtual ICollection<Friendship> Friends { get; set; }

        public ApplicationUser()
        {
            this.AvatarUrl = "/avatars/default.jpg";
        }
    }
}
