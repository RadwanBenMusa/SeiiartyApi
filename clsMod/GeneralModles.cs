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
    
    
}

