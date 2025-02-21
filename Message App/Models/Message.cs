using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Message_App.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Sender")]
        public string SenderId { get; set; }
        public virtual ApplicationUser Sender { get; set; }

        [ForeignKey("Receiver")]
        public string ReceiverId { get; set; }
        public virtual ApplicationUser Receiver { get; set; }

        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public string? AttachmentUrl { get; set; }
    }
}
