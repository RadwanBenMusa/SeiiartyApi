using FirebaseAdmin.Messaging;
using Serilog.Parsing;

namespace TamaApi.clsMod
{
    
    public enum EnumNotificationType
    {
        contactUs,
        searchMedecine,
        clinicAdmins,
        examWaitinglist,
        analysisResPatient,
        analysisResDoc
    }
    public class ApiNotification
    {
        public Notification? Notification { get; set; }
        public Dictionary<string, string>? Data { get; set; }
        public string? Token { get; set; }
        public EnumNotificationType NotificationType { get; set; }
    }
    
    
}

