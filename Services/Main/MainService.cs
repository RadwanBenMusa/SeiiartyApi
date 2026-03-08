using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using System.Data;
using Seiiarty.clsMod;
using Seiiarty.Controllers;
using static Seiiarty.General;
using static Seiiarty.Services.db.DbService;
using Seiiarty.StoredProcedures;

namespace Seiiarty.Services.Main
{
    public class ApiResponse
    {
        // ملاحظة: يجب أن تتطابق أسماء الخصائص مع أسماء مفاتيح JSON
        public required string Status { get; set; }
        public required string Text { get; set; }
        public required string Message_ID { get; set; }
    }
    public class MainService() : IMainService
    {
        

  

        [Obsolete]
        public dynamic DoSomeThings(Request request)
        {
            return ExecDoSomeThings(request);
        }
        [Obsolete]
        public dynamic ExecCmd(RequestCmd requestCmd)
        {
            return ExecCommand(requestCmd).ToDynamic();
        }
        [Obsolete]
        private async Task<dynamic> SendNotification(Message message,int NotificationTypeId,int UserId)
        {

            //Save Notification To Database
            SpNotification.Insert(new InsNotification { 
                NotificationTypeId = NotificationTypeId,
                UserId = UserId,
                Title = message.Notification.Title,
                Body = message.Notification.Body,
                Data = message.Data.ToString()

            });
            //Start To Send Notification
            string serviceAccountKeyPath = Path.Combine(AppContext.BaseDirectory, "seiiarty-54af7-firebase-adminsdk.json");

            if (!File.Exists(serviceAccountKeyPath))
            {
                
                return new
                {
                    MsgId = 2,
                    MsgAr = "Service account key file not found.",
                    MsgEn = "Service account key file not found."
                };
            }

            try
            {
                // إرسال الرسالة
                string responseDevice = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                

                return new
                {
                    MsgId = 1,
                    MsgAr = "Successfully sent message to device.",
                    MsgEn = "Successfully sent message to device."
                };
            }
            catch (FirebaseMessagingException fcmEx)
            {
                return new
                {
                    MsgId = 2,
                    MsgAr = $"Firebase Messaging Error: {fcmEx.Message}.",
                    MsgEn = $"Firebase Messaging Error: {fcmEx.Message}.",
                };
            }
        }

        [Obsolete]
        private async void sendToAdmins(ApiNotification apiNotification) {
            Message message;
            dynamic msgRes;
            apiNotification.Data = [];
            apiNotification.Data!.Add($"NotificationType", "1");

            DataTable adminsTokens = GetNamaAdminsTokens();
            foreach (DataRow row in adminsTokens.Rows)
            {
                apiNotification.Token = GetValueString(row, "FirebaseToken");
                message = new Message()
                {
                    Notification = apiNotification.Notification,
                    Data = apiNotification.Data,
                    Token = apiNotification.Token
                };
                msgRes = await SendNotification(message, 1, GetValueInt(row, "ID"));
            }
        }
        [Obsolete]
        DataTable GetNamaAdminsTokens() {
            GetUser getUser = new GetUser()
            {
                Admin = true,
            };
            DataTable AdminTokens = SpUser.Get(getUser);
            return AdminTokens;
        }

        [Obsolete]
        public async Task<dynamic> Notification(ApiNotification apiNotification)
        {
            Message message;
            dynamic msgRes = new
            {
                MsgId = 1,
                MsgAr = "تمام",
                MsgEn = "تمام"
            };
            
            switch (apiNotification.NotificationType) {
                case EnumNotificationType.request:
                    sendToAdmins(apiNotification);
                    break;
                case EnumNotificationType.contactUs:
                    sendToAdmins(apiNotification);
                    break;
            }
            return msgRes;
        }
    }
}
