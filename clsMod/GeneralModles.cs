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
        public EnumNotificationType NotificationType { get; set; }
        // Token removed — the API reads FCM tokens from the DB, not from the app
    }

    public class NotificationRequest
    {
        // Token removed — the API reads FCM tokens from the DB based on
        // NotificationTypeId (1 = admins, 2 = store owners, 3 = admins).
        // The Flutter app never sends or knows about FCM tokens.
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int NotificationTypeId { get; set; }
        public int UserId { get; set; }
        public object? Data { get; set; }
    }
}