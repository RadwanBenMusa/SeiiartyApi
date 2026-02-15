using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Management;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TamaApi.clsMod;
using TamaApi.Controllers;
using TamaApi.Services.db;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static TamaApi.Controllers.MainController;
using static TamaApi.General;
using static TamaApi.Services.db.DbService;
using Component = TamaApi.clsMod.Component;
using Document = TamaApi.clsMod.Document;
using Parameter = TamaApi.clsMod.Parameter;

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
        public int GetRemoteContractCoId(string PhoneNumber,int clinicId) 
        {
            string cmd = "SELECT dbo.UserInsurance.* " +
                "FROM dbo.UserInsurance INNER JOIN " +
                "dbo.[User] ON dbo.UserInsurance.UserId = dbo.[User].ID " +
                $"WHERE (dbo.UserInsurance.ClinicId = {clinicId}) AND (dbo.[User].PhoneNumber = N'{PhoneNumber}')";
            DataTable dt = ExecCommand(new RequestCmd(){Cmd=cmd});
            if(dt.Rows.Count>0)
                return (int)dt.Rows[0]["RemoteContractCoId"];
            else
                return 0;
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
        private async Task<dynamic> SendNotification(Message message,
            EnumNotificationType NotificationType,
            int UserId,
            ILogger<MainController> _logger)
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
        DataTable GetClinicAdminsTokens(int ClinicNo) {
            List<Para> paras = [
                new(){ Name="PermissionId",Value="1",DataType= SqlDbType.Int},
                new(){ Name="ClinicNo",Value=ClinicNo.ToString(),DataType= SqlDbType.Int}
            ];
            DataTable tblNamaAdmin = ExecSp(new Request() { SpName = "sp_Get_ClinicUserPermission", Paras = paras });
            return tblNamaAdmin;
        }

        [Obsolete]
        DataTable GetTamaPharmAppTokens() {
            DataTable tblPharmacyUser = ExecSp(new Request() { SpName = "sp_Get_PharmacyUser", Paras = [] });
            return tblPharmacyUser;
        }

        [Obsolete]
        private static string GetPatientFireBaseToken(int RemoteCustomerId,int clinicNo, out int UserId,out int clinicId,out int PatientId) {
            DataTable dtData= ExecSp(new Request() { SpName = "sp_Get_ClinicPatient", Paras = [
                new Para(){Name="RemoteCustomerId",DataType=SqlDbType.Int,Value=RemoteCustomerId.ToString()},
                new Para(){Name="ClinicNo",DataType=SqlDbType.Int,Value=clinicNo.ToString()}
                ] });
            UserId = 0;
            clinicId = 0;
            PatientId = 0;
            if (dtData.Rows.Count != 0) {
                clinicId= GetValueInt(dtData.Rows[0], "ClinicId");
                UserId = GetValueInt(dtData.Rows[0], "UserId");
                PatientId = GetValueInt(dtData.Rows[0], "PatientId");
                return GetValueString(dtData.Rows[0], "firebaseTokenPatient");
            }
            else
                return "";
        }
        
        [Obsolete]
        private static string GetDoctorFireBaseToken(int RemoteCustomerId, int clinicNo, out int UserId)
        {
            string cmd = $"SELECT        dbo.Doctor.firebaseToken,dbo.Person.UserId\r\nFROM            dbo.ClinicUser INNER JOIN\r\n                         dbo.Clinic ON dbo.ClinicUser.ClinicId = dbo.Clinic.ID INNER JOIN\r\n                         dbo.Doctor INNER JOIN\r\n                         dbo.Person ON dbo.Doctor.PersonId = dbo.Person.ID ON dbo.ClinicUser.UserId = dbo.Person.UserId\r\nWHERE        (dbo.Clinic.ClinicNo = {clinicNo}) AND (dbo.ClinicUser.RemoteClinicUserId = {RemoteCustomerId}) AND (dbo.ClinicUser.DeletionDate IS NULL) AND (dbo.Clinic.DeletionDate IS NULL)";
            DataTable dtData = ExecCommand(new RequestCmd()
            {
                Cmd = cmd
            });

            UserId = 0;
            if (dtData.Rows.Count != 0) {
                UserId = GetValueInt(dtData.Rows[0], "UserId");
                return GetValueString(dtData.Rows[0], "firebaseToken");
            }
            else
                return "";
        }

        private static string HoursToStr(int Hours) {
            return Hours switch
            {
                // حالة المفرد
                1 => "ساعة واحدة",

                // حالة المثنى
                2 => "ساعتين",

                // حالة الجمع (من 3 إلى 10) باستخدام شرط التخلص (when)
                // هذا هو ما يميز Switch Expression عن Switch Statement القديمة
                _ when Hours >= 3 && Hours <= 10 => $"{Hours} ساعات",

                // الحالة الافتراضية (11 وما فوق)
                _ => $"{Hours} ساعة"
            };
        }

        [Obsolete]
        private static bool LastNotificationSame(int UserId, Dictionary<string, string>? Data) {
            try
            { 
                string cmd = $"SELECT TOP (1) * FROM dbo.Notification WHERE (UserId = {UserId}) AND (NotificationTypeId = 3) ORDER BY ID DESC";
                DataTable dtData = ExecCommand(new RequestCmd(){Cmd = cmd});
                if (dtData.Rows.Count == 0)
                    return false;
                else {
                    string SavedData = (string)dtData.Rows[0]["Data"]!;
                    //dynamic SavedDataClass = JsonConvert.DeserializeObject<dynamic>(SavedData)!;
                    using JsonDocument doc = JsonDocument.Parse(SavedData);
                    JsonElement root = doc.RootElement;
                    if (root.GetProperty("ClinicNo").GetString()! == Data!["ClinicNo"])
                        if (root.GetProperty("PatientId").GetString()! == Data["PatientId"])
                            if (root.GetProperty("OpdScheduleId").GetString()! == Data["OpdScheduleId"])
                                if (root.GetProperty("AfterHours").GetString()! == Data["AfterHours"])
                                    return true;
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
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

            int clinicNo;
            int UserId = 0;
            int clinicId=0;
            string patientFireBaseToken;
            switch (apiNotification.NotificationType) {
                
                case EnumNotificationType.analysisResPatient:
                    clinicNo = int.Parse(apiNotification.Data!["ClinicNo"]);
                    int CustomerId = int.Parse(apiNotification.Data!["CustomerId"]);
                    //int OrdersId = int.Parse(apiNotification.Data!["OrdersId"]);

                    patientFireBaseToken = GetPatientFireBaseToken(CustomerId, clinicNo, out UserId,out clinicId,out int Patient);
                    apiNotification.Data.Add($"ClinicId", clinicId.ToString());

                    apiNotification.Data.Add($"NotificationType", EnumNotificationType.analysisResPatient.ToString()!);

                    if (patientFireBaseToken != "")
                    {
                        apiNotification.Token = patientFireBaseToken;
                        apiNotification.Notification!.Body = "نتائج تحاليل";
                        message = new Message()
                        {
                            Notification = apiNotification.Notification,
                            Data = apiNotification.Data,
                            Token = apiNotification.Token
                        };
                        msgRes = await SendNotification(message, EnumNotificationType.analysisResPatient, UserId, _logger);
                    }

                    break;

                case EnumNotificationType.analysisResDoc:
                    clinicNo = int.Parse(apiNotification.Data!["ClinicNo"]);
                    int DoctorUserId = int.Parse(apiNotification.Data!["DoctorUserId"]);

                    apiNotification.Data.Add($"NotificationType", EnumNotificationType.analysisResDoc.ToString()!);

                    UserId = 0;
                    string doctorFireBaseToken = GetDoctorFireBaseToken(DoctorUserId, clinicNo,out UserId);
                    if (doctorFireBaseToken != "")
                    {
                        apiNotification.Token = doctorFireBaseToken;
                        apiNotification.Notification!.Body = "نتائج تحاليل";
                        message = new Message()
                        {
                            Notification = apiNotification.Notification,
                            Data = apiNotification.Data,
                            Token = apiNotification.Token
                        };
                        msgRes = await SendNotification(message,EnumNotificationType.analysisResDoc,UserId, _logger);
                    }
                    break;

                case EnumNotificationType.examWaitinglist:
                    string[] CustomerIds = apiNotification.Data!["CustomerIds"].Split("^");
                    clinicNo = int.Parse(apiNotification.Data!["ClinicNo"]);
                    string docName="";
                    if (apiNotification.Data.ContainsKey("docName"))
                        docName=apiNotification.Data!["docName"];

                    foreach (string strCustomerId in CustomerIds)
                    {
                        UserId = 0;
                        patientFireBaseToken = GetPatientFireBaseToken(int.Parse(strCustomerId), clinicNo,out UserId,out clinicId,out int patientId);
                        if (patientFireBaseToken != "")
                        {

                            apiNotification.Token = patientFireBaseToken;
                            _ = int.TryParse(apiNotification.Data![$"HoursAfter{strCustomerId}"], out int afterHours);

                            Dictionary<string, string>? Data = [];
                            Data.Add($"ClinicNo", clinicNo.ToString()!);
                            Data.Add($"PatientId", patientId.ToString()!);
                            string patientName = "";
                            if (apiNotification.Data.ContainsKey($"patientName{strCustomerId}")) {
                                patientName = apiNotification.Data![$"patientName{strCustomerId}"];
                                Data.Add($"patientName", patientName);
                            }

                            Data.Add($"docName", docName);

                            if (apiNotification.Data.ContainsKey("OpdScheduleId")) Data.Add($"OpdScheduleId", int.Parse(apiNotification.Data!["OpdScheduleId"]).ToString());
                            if (apiNotification.Data.ContainsKey("AppointmentDate")) Data.Add($"AppointmentDate", apiNotification.Data!["AppointmentDate"]);
                            Data.Add($"AfterHours", afterHours.ToString());
                            if (apiNotification.Data.ContainsKey("HowMuch_PerHour")) Data.Add($"HowMuch_PerHour", apiNotification.Data!["HowMuchPersonPerHour"]);
                            string Body = "";
                            if(docName!="") Body +="الطبيب : " + docName + "\n";
                            if(patientName!="") Body += "الاسم : " + patientName + "\n";

                            if (afterHours == 0)
                                Body += "موعد الكشف -- خلال هذه الساعة";
                            else
                                if (afterHours == -1)
                                    Body += "الطبيب بدأ في الكشف" + "\n" + "ليس لديك رقم في القائمة يرجى التوجه الى الاستعلامات";
                                else
                                    Body += $"موعد الكشف تقريبا -- بعد {HoursToStr(afterHours)}";

                            apiNotification.Notification!.Body = Body;
                            message = new Message()
                            {
                                Notification = apiNotification.Notification,
                                Data = Data,
                                Token = apiNotification.Token
                            };

                            if(!LastNotificationSame(UserId,Data))
                                msgRes = await SendNotification(message, EnumNotificationType.examWaitinglist,UserId, _logger);
                        }
                    }
                    break;

                case EnumNotificationType.contactUs:
                    apiNotification.Data = [];
                    apiNotification.Data!.Add($"NotificationType", ((int)EnumNotificationType.contactUs).ToString()!);
                    UserId = 0;
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
                
                case EnumNotificationType.searchMedecine:
                    apiNotification.Data!.Add($"NotificationType", EnumNotificationType.searchMedecine.ToString()!);
                    DataTable tamaPharmAppTokens = GetTamaPharmAppTokens();
                    foreach (DataRow row in tamaPharmAppTokens.Rows)
                    {
                        apiNotification.Token = GetValueString(row, "firebaseTokenPharm");
                        message = new Message()
                        {
                            Notification = apiNotification.Notification,
                            Data = apiNotification.Data,
                            Token = apiNotification.Token
                        };
                        msgRes = await SendNotification(message, EnumNotificationType.searchMedecine, GetValueInt(row, "UserId"), _logger);
                    }
                    break;
                
                case EnumNotificationType.clinicAdmins:
                    int ClinicNo = int.Parse(apiNotification.Data!["ClinicNo"]);
                    apiNotification.Data!.Add($"NotificationType", EnumNotificationType.clinicAdmins.ToString()!);

                    DataTable tamaClinicAdmins = GetClinicAdminsTokens(ClinicNo);
                    foreach (DataRow row in tamaClinicAdmins.Rows)
                    {
                        apiNotification.Token = GetValueString(row, "firebaseToken");
                        message = new Message()
                        {
                            Notification = apiNotification.Notification,
                            Data = apiNotification.Data,
                            Token = apiNotification.Token
                        };
                        msgRes = await SendNotification(message, EnumNotificationType.clinicAdmins, GetValueInt(row, "UserId"), _logger);
                    }
                    break;
            }
            return msgRes;
        }
    }
}
