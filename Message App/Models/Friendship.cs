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
        public string? LastMessageContent { get; set; }
        public string? LastMessageDate { get; set; }
        public string? LastMessageSenderId { get; set; }

        public int UnreadCount { get; set; }

        public void SetLastMessage(string content, string date, string senderId)
        {
            LastMessageContent = content;
            LastMessageDate = date;
            LastMessageSenderId = senderId;
        }
    }
}
