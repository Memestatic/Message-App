namespace Message_App.Models
{
    public class FriendWithLastMessageViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AvatarUrl { get; set; }
        public string LastMessageContent { get; set; }
        public DateTime? LastMessageDate { get; set; }
        public string LastMessageSenderName { get; set; }
        public int UnreadCount { get; set; }
    }
}
