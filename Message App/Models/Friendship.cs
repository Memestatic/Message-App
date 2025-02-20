using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Message_App.Models
{
    public class Friendship
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        [MaxLength(5)]
        public required string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("Friend")]
        [MaxLength(5)]
        public required string FriendId { get; set; }
        public ApplicationUser Friend { get; set; }

        public bool IsAccepted { get; set; }
        
        [MaxLength(200)]
        public string? LastMessageContent { get; set; }
        [MaxLength(20)]
        public string? LastMessageDate { get; set; }
        [MaxLength(5)]
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
