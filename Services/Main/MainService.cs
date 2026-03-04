using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json;
using TamaApi.clsMod;
using TamaApi.Controllers;
using static TamaApi.General;
using static TamaApi.Services.db.DbService;

namespace TamaApi.Services.Main
{
    public class ApiResponse
    {
        // ملاحظة: يجب أن تتطابق أسماء الخصائص مع أسماء مفاتيح JSON
        public required string Status { get; set; }
        public required string Text { get; set; }
        public required string Message_ID { get; set; }
    }
    public class MainService(ILogger<MainController> _logger) : IMainService
    {
        readonly ILogger<MainController> logger = _logger;

        [Obsolete]
        public dynamic ExecStoredProcedure(Request tamaRequest)
        {
            return ExecSp(tamaRequest).ToDynamic();
        }

        [Obsolete]
        public dynamic DoSomeThings(Request tamaRequest)
        {
            return ExecDoSomeThings(tamaRequest,logger);
        }
        [Obsolete]
        public dynamic ExecCmd(RequestCmd tamaRequestCmd)
        {
            return ExecCommand(tamaRequestCmd).ToDynamic();
        }
        [Obsolete]
        private async Task<dynamic> SendNotification(Message message,EnumNotificationType NotificationType,int UserId,ILogger<MainController> _logger)
        {

            //Save Notification To Database
            List<Para>? paras = [];
            paras.Add(new Para() { Name = "NotificationTypeId", Value = ((int)NotificationType).ToString(), DataType = SqlDbType.Int });
            paras.Add(new Para() { Name = "UserId", Value = UserId.ToString(), DataType = SqlDbType.Int });
            paras.Add(new Para() { Name="Title",Value=message.Notification.Title,DataType= SqlDbType.NVarChar });
            paras.Add(new Para() { Name = "Body", Value = message.Notification.Body, DataType = SqlDbType.NVarChar });
            if (message.Data != null) paras.Add(new Para() { Name = "Data", Value = JsonConvert.SerializeObject(message.Data), DataType = SqlDbType.NVarChar });

            ExecSp(new Request() { SpName = "sp_Ins_Notification", Paras = paras });

            //Start To Send Notification
            string serviceAccountKeyPath = Path.Combine(AppContext.BaseDirectory, "tamam-2acdd-firebase-adminsdk.json");

            if (!File.Exists(serviceAccountKeyPath))
            {
                logger.LogInformation($"XXX_SendNotification ===> Service account key file not found.");
                return new
                {
                    MsgId = 2,
                    MsgAr = "Service account key file not found.",
                    MsgEn = "Service account key file not found."
                };
            }

            try
            {
                if (FirebaseApp.DefaultInstance == null) { 
                    FirebaseApp.Create(new AppOptions(){Credential = GoogleCredential.FromFile(serviceAccountKeyPath)});
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"XXX_SendNotification ===> Error initializing Firebase Admin SDK: {ex.Message}");
                return new
                {
                    MsgId = 2,
                    MsgAr = $"Error initializing Firebase Admin SDK: {ex.Message}",
                    MsgEn = $"Error initializing Firebase Admin SDK: {ex.Message}",
                };
            }

            try
            {
                // إرسال الرسالة
                string responseDevice = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                logger.LogInformation($"Successfully sent message to device: {responseDevice}");

                return new
                {
                    MsgId = 1,
                    MsgAr = "Successfully sent message to device.",
                    MsgEn = "Successfully sent message to device."
                };
            }
            catch (FirebaseMessagingException fcmEx)
            {
                logger.LogError($"Firebase Messaging Error: {fcmEx.Message}");
                logger.LogError($"Error Code: {fcmEx.MessagingErrorCode}");
                return new
                {
                    MsgId = 2,
                    MsgAr = $"Firebase Messaging Error: {fcmEx.Message}.",
                    MsgEn = $"Firebase Messaging Error: {fcmEx.Message}.",
                };
            }
        }

        [Obsolete]
        DataTable GetNamaAdminsTokens() {
            List<Para> paras = [ 
                new Para(){ Name="TableName",Value="NamaAdmin",DataType= SqlDbType.NVarChar}
            ];
            DataTable tblNamaAdmin = ExecSp(new Request() {SpName= "sp_Get_Table",Paras=paras});
            return tblNamaAdmin;
        }

        [Obsolete]
        public async Task<dynamic> Notification(ApiNotification apiNotification, ILogger<MainController> _logger)
        {
            Message message;
            dynamic msgRes = new
            {
                MsgId = 1,
                MsgAr = "تمام",
                MsgEn = "تمام"
            };
            switch (apiNotification.NotificationType) {
                case EnumNotificationType.contactUs:
                    apiNotification.Data = [];
                    apiNotification.Data!.Add($"NotificationType", ((int)EnumNotificationType.contactUs).ToString()!);
                   
                    DataTable adminsTokens = GetNamaAdminsTokens();
                    foreach (DataRow row in adminsTokens.Rows)
                    {
                        apiNotification.Token = GetValueString(row, "firebaseToken");
                        message = new Message()
                        {
                            Notification = apiNotification.Notification,
                            Data = apiNotification.Data,
                            Token = apiNotification.Token
                        };
                        msgRes=await SendNotification(message, EnumNotificationType.contactUs, GetValueInt(row, "UserId"), _logger);
                    }
                    break;
            }
            return msgRes;
        }
    }
}
