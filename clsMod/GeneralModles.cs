using FirebaseAdmin.Messaging;

namespace Seiiarty.clsMod
{
    
    public enum EnumNotificationType
    {
        request,
        contactUs
    }
    public class ApiNotification
    {
        public Notification? Notification { get; set; }
        public Dictionary<string, string>? Data { get; set; }
        public string? Token { get; set; }
        public EnumNotificationType NotificationType { get; set; }
    }

    public class NotificationRequest
    {
        public string? Token { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int NotificationTypeId { get; set; }
        public int UserId { get; set; }
        public object? Data { get; set; } // Changed from dynamic to object for better serialization
    }
}

