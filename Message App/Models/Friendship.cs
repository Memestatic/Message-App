using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Message_App.Models;

namespace Message_App.Models
{
    public class Friendship
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("Friend")]
        public string FriendId { get; set; }
        public virtual ApplicationUser Friend { get; set; }

        public bool IsAccepted { get; set; }
    }
}
