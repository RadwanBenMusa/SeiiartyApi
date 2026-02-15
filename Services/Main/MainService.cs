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
    public class MainService : IMainService
    {
        ILogger<MainController> logger;
        public MainService(ILogger<MainController> _logger)
        {
            logger=_logger;
        }

        [Obsolete]
        public dynamic ExecStoredProcedure(TamaRequest tamaRequest)
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
            DataTable dt = ExecCommand(new TamaRequestCmd(){Cmd=cmd});
            if(dt.Rows.Count>0)
                return (int)dt.Rows[0]["RemoteContractCoId"];
            else
                return 0;
        }

        [Obsolete]
        public dynamic DoSomeThings(TamaRequest tamaRequest)
        {
            return ExecDoSomeThings(tamaRequest,logger);
        }
        [Obsolete]
        public dynamic ExecCmd(TamaRequestCmd tamaRequestCmd)
        {
            return ExecCommand(tamaRequestCmd).ToDynamic();
        }
        [Obsolete]
        public dynamic GetSetupTable()
        {
            return DbService.GetTable("SetupTable").ToDynamic();
        }

        [Obsolete]
        public dynamic GetTable(GetTableCP getTableCP)
        {
            return DbService.GetTable(getTableCP:getTableCP).ToDynamic();
        }

        private string CheckPoneNoForWhatsApp(string PhoneNo)
        {
            PhoneNo = PhoneNo.Replace("-", string.Empty).Replace("/", string.Empty).Replace("\\", string.Empty).Replace("_", string.Empty).Replace(".", string.Empty);

            string newPhone = "";
            if (PhoneNo.Length == 12)
                newPhone = PhoneNo;
            else
            if (PhoneNo.Length == 10 && PhoneNo[0] == '0')
                newPhone = "218" + PhoneNo.Substring(1, PhoneNo.Length - 1);
            else
            if (PhoneNo.Length == 9)
                newPhone = "218" + PhoneNo;
            else
            if (PhoneNo.Length == 13 && PhoneNo[0] == '+')
                newPhone = "218" + PhoneNo.Substring(4, PhoneNo.Length - 4);
            else
            if (PhoneNo.Length == 14 && PhoneNo[0] == '0')
                newPhone = "218" + PhoneNo.Substring(5, PhoneNo.Length - 5);

            return newPhone;
        }

        private string CheckPoneNoForSms(string PhoneNo)
        {
            PhoneNo = PhoneNo.Replace("-", string.Empty).Replace("/", string.Empty).Replace("\\", string.Empty).Replace("_", string.Empty).Replace(".", string.Empty);

            string newPhone = "";
            if (PhoneNo.Length == 12)
                newPhone = PhoneNo;
            else
            if (PhoneNo.Length == 10 && PhoneNo[0] == '0')
                newPhone = "218" + PhoneNo.Substring(1, PhoneNo.Length - 1);
            else
            if (PhoneNo.Length == 9)
                newPhone = "218" + PhoneNo;
            else
            if (PhoneNo.Length == 13 && PhoneNo[0] == '+')
                newPhone = "218" + PhoneNo.Substring(4, PhoneNo.Length - 4);
            else
            if (PhoneNo.Length == 14 && PhoneNo[0] == '0')
                newPhone = "218" + PhoneNo.Substring(5, PhoneNo.Length - 5);

            return newPhone;
        }

        public async Task<dynamic> SendSmsMsgAsync(string phoneNo, string message)
        {
            try
            {
                string ApiKey = "78512214deafed6a";
                //string ApiKey = "785145642214deafed6a";
                string SecretKet = "bce5825b";
                string Sender_Id = "Tamam";

                string apiUrl = "";

                apiUrl = "https://api.libyasms.com:3236/sendtext?";
                apiUrl = apiUrl + "apikey=" + ApiKey;
                apiUrl = apiUrl + "&secretkey=" + SecretKet;
                apiUrl = apiUrl + "&callerID=" + Sender_Id;
                apiUrl = apiUrl + "&toUser=" + phoneNo;
                apiUrl = apiUrl + "&messageContent=" + message;

                var client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                // التحقق من حالة الاستجابة
                if (response.IsSuccessStatusCode)
                {
                    // قراءة الاستجابة
                    dynamic result = await response.Content.ReadAsStringAsync();
                    JsonDocument document = JsonDocument.Parse(result);
                    JsonElement root = document.RootElement;
                    root.TryGetProperty("Status", out JsonElement statusElement);
                    if (statusElement.GetString() == "0")

                        return new
                        {
                            MsgId = 1,
                            MsgAr = "Sccess",
                            MsgEn = "Sccess",
                            Data = result
                        };
                    else {
                        root.TryGetProperty("Text", out JsonElement errorElement);
                        return new
                        {
                            MsgId = 2,
                            MsgAr = errorElement.GetString(),
                            MsgEn = errorElement.GetString(),
                            Data = result
                        };
                    }
                        
                }
                else
                {
                    //Console.WriteLine("حدث خطأ: " + response.StatusCode);
                    return new
                    {
                        MsgId = 2,
                        MsgAr = response.ReasonPhrase!,
                        MsgEn = response.ReasonPhrase!
                    };
                }

            }
            catch (Exception ex)
            {
                return new
                {
                    MsgId = 2,
                    MsgAr = ex.Message,
                    MsgEn = ex.Message

                };
            }
        }
        [Obsolete]
        public async Task<dynamic> SmsLinkAnalysisRes(ClsSmsLinkAnalysisRes clsSmsLinkAnalysisRes, ILogger<MainController> logger)
        {
            try
            {
                DataTable tblSetupTable = ExecSp(new TamaRequest() { spName = "sp_Get_Table", paras = [new Para() { Name = "TableName", DataType = SqlDbType.NVarChar, Value = "SetupTable" }] });
                decimal msgAmount = (tblSetupTable.Rows[0]["SmsMsgPrice"] == DBNull.Value) ? 0 : (decimal)tblSetupTable.Rows[0]["SmsMsgPrice"];

                DataTable tblClinic = ExecSp(new TamaRequest() { spName = "sp_Get_Clinic", paras = [new Para() { Name = "ClinicNo", DataType = SqlDbType.Int, Value = clsSmsLinkAnalysisRes.ClinicNo.ToString() }] });
                if (tblClinic.Rows.Count == 0) return "Can't find a clinic";
                string ClinicName = (string)tblClinic.Rows[0]["ClinicName"];

                decimal SmsBalance = (tblClinic.Rows[0]["SmsBalance"] == DBNull.Value) ? 0 : (decimal)tblClinic.Rows[0]["SmsBalance"];
                //هل يوجد رصيد يسمح
                if (SmsBalance < msgAmount) return new
                {
                    MsgId = 2,
                    MsgAr = "There is not enough balance.",
                    MsgEn = "There is not enough balance."
                };

                clsSmsLinkAnalysisRes.customerPhoneNo = CheckPoneNoForSms(clsSmsLinkAnalysisRes.customerPhoneNo);

                if (clsSmsLinkAnalysisRes.customerPhoneNo == "") return new
                {
                    MsgId = 2,
                    MsgAr = "The phone number not correct",
                    MsgEn = "The phone number not correct"
                };
                
                string message = $"السلام عليكم -- {ClinicName}";
                message += $"\nمرفق اليكم نتيجة التحاليل";
                message += $"\n{clsSmsLinkAnalysisRes.OtherInfo}";
                message += $"\nhttps://tamam.ly/AnalysisResult/{clsSmsLinkAnalysisRes.FileName}";
                dynamic res = await SendSmsMsgAsync(clsSmsLinkAnalysisRes.customerPhoneNo, message);
                if (res.MsgId==1)
                    //خصم قيمة ارسال الرسالة
                    ExecCmd(new TamaRequestCmd() { Cmd = $"Update Clinic Set SmsBalance=SmsBalance-{msgAmount} Where ClinicNo={clsSmsLinkAnalysisRes.ClinicNo}" });

                return res;
            }
            catch (Exception ex) { return ex.Message; }
        }

        [Obsolete]
        public async Task<dynamic> WhatsAppMsgAnalysisRes(int ClinicNo, string FileName,string? OtherInfo,string customerPhoneNo,ILogger<MainController> logger)
        {
            try
            {
                DataTable tblSetupTable = ExecSp(new TamaRequest() { spName = "sp_Get_Table", paras = [new Para() { Name = "TableName", DataType = SqlDbType.NVarChar, Value = "SetupTable" }] });
                decimal msgAmount = (tblSetupTable.Rows[0]["WhatsappMsgPrice"] == DBNull.Value) ? 0 : (decimal)tblSetupTable.Rows[0]["WhatsappMsgPrice"];

                DataTable tblClinic = ExecSp(new TamaRequest() { spName= "sp_Get_Clinic", paras = [new Para(){ Name= "ClinicNo", DataType= SqlDbType.Int, Value = ClinicNo.ToString() }] });
                if (tblClinic.Rows.Count == 0) return "Can't find a clinic";
                string ClinicName =(string)tblClinic.Rows[0]["ClinicName"];

                decimal WhatsAppBalance =(tblClinic.Rows[0]["WhatsAppBalance"]==DBNull.Value)?0: (decimal)tblClinic.Rows[0]["WhatsAppBalance"];
                //هل يوجد رصيد يسمح
                if (WhatsAppBalance < msgAmount) return new
                {
                    MsgId = 2,
                    MsgAr = "There is not enough balance.",
                    MsgEn = "There is not enough balance."
                };

                // معلومات API 
                string apiUrl = "https://graph.facebook.com/v20.0/435425779648819/messages";
                string token = "EAARBrWpzZCQ8BO4WGlKBB7ZBNUs3sr4bGfZBBzAQZAZCpcsGV8ZCxLHczauW0tRqSn0Gpscsns7bHURqUOTidiQucQG13B7ISyjzdqmHY04JZAVGR8sbRApnXEjbtl5U4VS2ksvx67iJjDxyZBGE9QFQovQw1aavxLxqwbZAAmexW0lekiwPZCCswZC5IZBZA7OxzMLfK6VmAq7sIaDJNZCKVj";

                // إعداد جسم الطلب (body)
                List<Component> components = new(
                [
                    new Component
                    { type = "header", parameters = new List<Parameter>
                    ([
                        new Parameter { type= "document", document=new Document {link= $"https://tamam.ly/AnalysisResult/{FileName}",filename=$"{FileName}" } }
                    ])
                    },

                    new Component
                    {
                        type = "body",
                        parameters = new List<Parameter>([
                            new Parameter { type= "text", text= ClinicName },
                            //new Parameter { type= "text", text= "لأي استفسار التواصل مسجات واتس اب 0925313000" }
                            new Parameter { type= "text", text= OtherInfo??"" }
                            ])
                    }
                ]);

                customerPhoneNo = CheckPoneNoForWhatsApp(customerPhoneNo);

                if (customerPhoneNo == "") return new
                {
                    MsgId = 2,
                    MsgAr = "The phone number not correct",
                    MsgEn = "The phone number not correct"
                }; 
                
                Template template = new Template(name: "analysis_result", language: new Language { code = "ar" }, components: components);
                ClsModWhatsAppMessage clsModWhatsAppMessage = new ClsModWhatsAppMessage(messaging_product: "whatsapp", to: customerPhoneNo, type: "template", template: template);

                string json = JsonConvert.SerializeObject(clsModWhatsAppMessage);
                // إعداد عميل HTTP
                using (var client = new HttpClient())
                {
                    // إعداد رأس التراخيص (headers)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // إعداد المحتوى
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // إرسال الطلب (POST في هذا المثال)
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    // التحقق من حالة الاستجابة
                    if (response.IsSuccessStatusCode)
                    {
                        // قراءة الاستجابة
                        string result = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine(result);
                        
                        logger.LogInformation($"XXX_Whatsapp Result===> { JsonConvert.SerializeObject(result)}");

                        //خصم قيمة ارسال الرسالة
                        ExecCmd(new TamaRequestCmd() { Cmd = $"Update Clinic Set WhatsAppBalance=WhatsAppBalance-{msgAmount} Where ClinicNo={ClinicNo}" });
                        return new
                        {
                            MsgId = 1,
                            MsgAr = "Ok",
                            MsgEn = "Ok"

                        };
                    }
                    else
                    {
                        logger.LogInformation($"XXX_Whatsapp Error===> {response.StatusCode}");
                        //Console.WriteLine("حدث خطأ: " + response.StatusCode);
                        return new
                        {
                            MsgId = 2,
                            MsgAr = response.ReasonPhrase!,
                            MsgEn = response.ReasonPhrase!

                        };
                    }
                }
            }
            catch (Exception ex) { return ex.Message; }
        }

        public static string DecryptInvoiceNo(string encryptedInvoiceNoCombined, string keyString)
        {
            // 1. فصل IV عن النص المشفر
            string[] parts = encryptedInvoiceNoCombined.Split(':');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid encrypted string format. Expected 'IV:EncryptedData'.");
            }

            byte[] ivBytes = Convert.FromBase64String(parts[0]);
            byte[] encryptedBytes = Convert.FromBase64String(parts[1]);

            // 2. تحضير المفتاح (يجب أن يكون بنفس طريقة التشفير)
            byte[] keyBytes = new byte[32];
            byte[] keyStringBytes = Encoding.UTF8.GetBytes(keyString);
            Array.Copy(keyStringBytes, keyBytes, Math.Min(keyStringBytes.Length, keyBytes.Length));

            // 3. إنشاء نسخة AES
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256; // 256 bits
                aesAlg.Mode = CipherMode.CBC; // Cipher Block Chaining
                aesAlg.Padding = PaddingMode.PKCS7; // PKCS7 Padding
                aesAlg.Key = keyBytes;
                aesAlg.IV = ivBytes; // استخدام IV المستخرج من النص المشفر

                // 4. إنشاء decryptor لأداء تحويل الدفق
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                //byte[] decryptedBytes;
                using (var msDecrypt = new System.IO.MemoryStream(encryptedBytes))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        // قراءة البيانات من CryptoStream
                        using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                        {
                            string decryptedString = srDecrypt.ReadToEnd();
                            return decryptedString;
                        }
                    }
                }
            }
        }
        static string EncryptInvoiceNo(string invoiceNo, string keyString)
        {
            // Prepare the key: Pad or truncate to 32 bytes (256 bits)
            byte[] keyBytes = new byte[32];
            byte[] keyStringBytes = Encoding.UTF8.GetBytes(keyString);

            // Copy keyStringBytes to keyBytes, padding with zeros if necessary
            Array.Copy(keyStringBytes, keyBytes, Math.Min(keyStringBytes.Length, keyBytes.Length));

            // Create a new AES instance
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256; // 256 bits
                aesAlg.Mode = CipherMode.CBC; // Cipher Block Chaining
                aesAlg.Padding = PaddingMode.PKCS7; // PKCS7 Padding
                aesAlg.Key = keyBytes;

                // Generate a random IV (16 bytes for AES)
                aesAlg.GenerateIV();
                byte[] ivBytes = aesAlg.IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Convert the invoiceNo string to bytes
                byte[] invoiceNoBytes = Encoding.UTF8.GetBytes(invoiceNo);

                byte[] encryptedBytes;
                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(invoiceNoBytes, 0, invoiceNoBytes.Length);
                        csEncrypt.FlushFinalBlock();
                    }
                    encryptedBytes = msEncrypt.ToArray();
                }

                // Encode IV and encrypted bytes to Base64 and combine with a colon
                string ivBase64 = Convert.ToBase64String(ivBytes);
                string encryptedInvoiceNoBase64 = Convert.ToBase64String(encryptedBytes);

                return $"{ivBase64}:{encryptedInvoiceNoBase64}";
            }
        }

        [Obsolete]
        static int getPaymentGateIdForAppointment(ClsUrlPay clsUrlPay,DataTable tblSetupTable) {
            DataTable tblOpdSchedule = ExecCommand(new TamaRequestCmd()
            {
                ClinicId = clsUrlPay.ClinicId,
                Cmd = $"SELECT Item.DefaultSellingPrice FROM OpdSchedule INNER JOIN Employee ON OpdSchedule.EmployeeId = Employee.ID INNER JOIN Item ON Employee.ExaminationItemId = Item.ID WHERE (OpdSchedule.ID = {clsUrlPay.Appointment!.RemoteOpdScheduleId})"
            });
            if (tblOpdSchedule == null) return 0;
            if (tblOpdSchedule.Rows.Count == 0) return 0;
            //check amount ok
            if (Convert.ToDouble(tblOpdSchedule.Rows[0]["DefaultSellingPrice"].ToString()) != clsUrlPay.Amount) return 0;

            double Commission = Convert.ToDouble(tblSetupTable.Rows[0]["GetAwayCommission"]);

            //Ins_PayGateAppointment
            DataTable tblPayGateAppointment = ExecSp(new TamaRequest()
            {
                spName = "sp_Ins_PayGateAppointment",
                paras = [
                    new Para() { Name = "Amount", DataType = SqlDbType.Money, Value = clsUrlPay.Amount.ToString() },
                        new Para() { Name = "Commission", DataType = SqlDbType.Money, Value = Commission.ToString() },
                        new Para() { Name = "ClinicId", DataType = SqlDbType.Money, Value = clsUrlPay.ClinicId.ToString() },
                        new Para() { Name = "CreationUserId", DataType = SqlDbType.Money, Value = clsUrlPay.CreationUserId.ToString() },
                        new Para() { Name = "realEnvironment", DataType = SqlDbType.Bit, Value = clsUrlPay.RealEnvironment.ToString() },
                        new Para() { Name = "PatientId", DataType = SqlDbType.Money, Value = clsUrlPay.Appointment.PatientId.ToString() },
                        new Para() { Name = "RemoteOpdScheduleId", DataType = SqlDbType.Money, Value = clsUrlPay.Appointment.RemoteOpdScheduleId.ToString() },
                        new Para() { Name = "AppointmentDate", DataType = SqlDbType.DateTime, Value = clsUrlPay.Appointment.AppointmentDate.ToString() },
                        new Para() { Name = "DocName", DataType = SqlDbType.NVarChar, Value = clsUrlPay.Appointment.DocName },
                        ]
            });

            if ((int)tblPayGateAppointment.Rows[0]["MsgId"] != 1) return 0;
            return (int)tblPayGateAppointment.Rows[0]["PaymentGateId"]!;
        }
        [Obsolete]
        static int insPayPaymentGate(ClsUrlPay clsUrlPay, DataTable tblSetupTable, string paymentGateTypeId)
        {
            double Commission = Convert.ToDouble(tblSetupTable.Rows[0]["GetAwayCommission"]);

            //Ins_PayGateAppointment
            DataTable tblPayGate = ExecSp(new TamaRequest()
            {
                spName = "sp_Ins_PaymentGate",
                paras = [
                    new Para() { Name = "Amount", DataType = SqlDbType.Money, Value = clsUrlPay.Amount.ToString() },
                    new Para() { Name = "Commission", DataType = SqlDbType.Money, Value = Commission.ToString() },
                    new Para() { Name = "ClinicId", DataType = SqlDbType.Int, Value = clsUrlPay.ClinicId.ToString() },
                    new Para() { Name = "CreationUserId", DataType = SqlDbType.Int, Value = clsUrlPay.CreationUserId.ToString() },
                    new Para() { Name = "realEnvironment", DataType = SqlDbType.Bit, Value = clsUrlPay.RealEnvironment.ToString() },
                    new Para() { Name = "PaymentGateTypeId", DataType = SqlDbType.Int, Value = paymentGateTypeId},
                ]
            });

            if ((int)tblPayGate.Rows[0]["MsgId"] != 1) return 0;
            return (int)tblPayGate.Rows[0]["Id"]!;
        }

        [Obsolete]
        static int getPaymentGateIdForOrderDet(ClsUrlPay clsUrlPay, DataTable tblSetupTable)
        {
            int PaymentGateId = insPayPaymentGate(clsUrlPay, tblSetupTable, ((int)EnumPaymentGateType.orderPayment + 1).ToString());

            foreach (int i in clsUrlPay.OrderDetailsIds!)
            {
                string cmd = $"Insert Into PayGateOrderDetIds(PaymentGateId,RemoteOrderDetId)Values({PaymentGateId},{i})";
                ExecCommand(new TamaRequestCmd() { Cmd = cmd, });
            }
            return PaymentGateId;
        }

        [Obsolete]
        static int getPaymentGateIdForWalletRecharge(ClsUrlPay clsUrlPay, DataTable tblSetupTable)
        {
            return insPayPaymentGate(clsUrlPay, tblSetupTable, ((int)EnumPaymentGateType.walletRecharge + 1).ToString());
        }

        [Obsolete]
        int getPaymentGateIdForActiveClinic(ClsUrlPay clsUrlPay, DataTable tblSetupTable)
        {
            double Commission = Convert.ToDouble(tblSetupTable.Rows[0]["GetAwayCommission"]);
            int PaymentGateId = 0;
            //Ins_PayGate
            DataTable tblPaymentGate = ExecSp(new TamaRequest()
            {
                spName = "sp_Ins_PaymentGate",
                paras = [
                    new Para() { Name = "Amount", DataType = SqlDbType.Money, Value = clsUrlPay.Amount.ToString() },
                    new Para() { Name = "Commission", DataType = SqlDbType.Money, Value = Commission.ToString() },
                    new Para() { Name = "ClinicId", DataType = SqlDbType.Int, Value = clsUrlPay.ClinicId.ToString() },
                    new Para() { Name = "PaymentGateTypeId", DataType = SqlDbType.Int, Value = "4" },
                    new Para() { Name = "CreationUserId", DataType = SqlDbType.Int, Value = clsUrlPay.CreationUserId.ToString() },
                    new Para() { Name = "realEnvironment", DataType = SqlDbType.Bit, Value = clsUrlPay.RealEnvironment.ToString() },

                ]
            });
            if ((int)tblPaymentGate.Rows[0]["MsgId"] != 1) return 0;
            PaymentGateId = General.getValueInt(tblPaymentGate.Rows[0], "ID");
            //Ins_PayGateActiveClinic
            if (clsUrlPay.ActiveClinicList != null)
            {
                foreach (ClsActiveClinic activeClinic in clsUrlPay.ActiveClinicList)
                {
                    DataTable tblPayGateActiveClinic = ExecSp(new TamaRequest()
                    {
                        spName = "sp_Ins_PayGateActiveClinic",
                        paras = [
                            new Para() { Name = "PaymentGateId", DataType = SqlDbType.Int, Value = PaymentGateId.ToString() },
                            new Para() { Name = "MonthNo", DataType = SqlDbType.Int, Value = activeClinic.MonthNo.ToString() },
                            new Para() { Name = "YearNo", DataType = SqlDbType.Int, Value = activeClinic.YearNo.ToString() },
                        ]
                    });
                    if ((int)tblPayGateActiveClinic.Rows[0]["MsgId"] != 1) return 0;

                }
            }
            return PaymentGateId;

        }

        [Obsolete]
        static int getPaymentGateIdForSubscribeMachine(ClsUrlPay clsUrlPay, DataTable tblSetupTable)
        {
            double Commission = Convert.ToDouble(tblSetupTable.Rows[0]["GetAwayCommission"]);

            //Ins_PayGateMachine
            DataTable tblPayGateMachine = ExecSp(new TamaRequest()
            {
                spName = "sp_Ins_PayGateMachine",
                paras = [
                    new Para() { Name = "Amount", DataType = SqlDbType.Money, Value = clsUrlPay.Amount.ToString() },
                    new Para() { Name = "Commission", DataType = SqlDbType.Money, Value = Commission.ToString() },
                    new Para() { Name = "ClinicId", DataType = SqlDbType.Money, Value = clsUrlPay.ClinicId.ToString() },
                    new Para() { Name = "CreationUserId", DataType = SqlDbType.Money, Value = clsUrlPay.CreationUserId.ToString() },
                    new Para() { Name = "realEnvironment", DataType = SqlDbType.Bit, Value = clsUrlPay.RealEnvironment.ToString() },
                    new Para() { Name = "MachineId", DataType = SqlDbType.Int, Value = clsUrlPay.Machine!.MachineId.ToString() },
                    new Para() { Name = "ExDate", DataType = SqlDbType.DateTime, Value = Convert.ToString(clsUrlPay.Machine.ExDate) },
                    new Para() { Name = "Com", DataType = SqlDbType.Bit, Value = Convert.ToString(clsUrlPay.Machine.Com) },

                ]
            });

            if ((int)tblPayGateMachine.Rows[0]["MsgId"] != 1) return 0;
            return (int)tblPayGateMachine.Rows[0]["PaymentGateId"]!;
        }
        [Obsolete]
        static int getPaymentGateIdForSubscribeWhatsApp(ClsUrlPay clsUrlPay, DataTable tblSetupTable)
        {
            return insPayPaymentGate(clsUrlPay, tblSetupTable, ((int)EnumPaymentGateType.subscribeWhatsApp + 1).ToString());
        }
        [Obsolete]
        static int getPaymentGateIdForSubscribeSms(ClsUrlPay clsUrlPay, DataTable tblSetupTable)
        {
            return insPayPaymentGate(clsUrlPay, tblSetupTable, ((int)EnumPaymentGateType.subscribeSms + 1).ToString());
        }
        [Obsolete]
        public async Task<dynamic> GetUrlPay(ClsUrlPay clsUrlPay, ILogger<MainController> _logger) {
            try
            {
                var emptyRes = new
                {
                    url = "",
                    error = "",
                    customRef = ""
                };

                if (clsUrlPay.Amount==0) return emptyRes;

                DataTable tblSetupTable = ExecSp(new TamaRequest() { 
                    spName = "sp_Get_Table", paras = [
                        new Para() { Name = "TableName", DataType = SqlDbType.NVarChar, Value = "SetupTable" }
                        ] });
                int PaymentGateId=0;

                //Test Environment
                string storeId = "Ard3qY4BYXoBLezR9gdOabMVlwDmrQ8Lx18x7kjA5EGWy6PJK13Nqn204KW1JzGj";
                string storeToken = "8PnJT4WMYs8XiEn9x3KeTZpqPnZ2VCHLOu3y7nJL";
                

                DataTable tblClinic = ExecSp(new TamaRequest() { spName = "sp_Get_Clinic", paras = [new Para() { Name = "ID", DataType = SqlDbType.Int, Value = clsUrlPay.ClinicId.ToString() }] });
                bool TestClinic = (tblClinic.Rows[0]["TestClinic"] as bool?) ?? false;
                bool TamaAdmin = false;
                if (clsUrlPay.PaymentGateType == EnumPaymentGateType.walletRecharge ||
                        clsUrlPay.PaymentGateType == EnumPaymentGateType.subscribeWhatsApp ||
                        clsUrlPay.PaymentGateType == EnumPaymentGateType.subscribeMachine ||
                        clsUrlPay.PaymentGateType == EnumPaymentGateType.activeClinic ||
                        clsUrlPay.PaymentGateType == EnumPaymentGateType.subscribeSms)
                    TamaAdmin = true;
                //Check Real Environment
                if (clsUrlPay.RealEnvironment)
                {
                    if (TamaAdmin)
                    {
                        //Nama Store
                        storeId = (string)tblSetupTable.Rows[0]["TadawelStoreId"]!;
                        storeToken = (string)tblSetupTable.Rows[0]["TadawelToken"];
                    }
                    
                    else {
                        //Clinic Store
                        bool OnLinePay = (tblClinic.Rows[0]["OnLinePay"] as bool?) ?? false;
                        if (!TamaAdmin) {

                            if (TestClinic)
                            {
                                logger.LogError($"XXX_GetUrlPay ===> هذه مصحة اختبار لا يمكن العمل عليها في البيئة الحقيقية");
                                return new
                                {
                                    url = "",
                                    error = "*الدفع الالكتروني مقفل من المصحة*"
                                };
                            }

                            if (!OnLinePay)
                            {
                                logger.LogError($"XXX_GetUrlPay ===> الدفع الالكتروني مقفل من المصحة");
                                return new
                                {
                                    url = "",
                                    error = "الدفع الالكتروني مقفل من المصحة"
                                };
                            }
                            else
                            {
                                if (clsUrlPay.Moamalat)
                                {
                                    string MoamalatMerchantID = getValueString(tblClinic.Rows[0], "MoamalatMerchantID");
                                    string MoamalatTerminalID = getValueString(tblClinic.Rows[0], "MoamalatTerminalID");
                                    string MoamalatSecretkey = getValueString(tblClinic.Rows[0], "MoamalatSecretkey");
                                    if (MoamalatMerchantID == "" || MoamalatMerchantID == "" || MoamalatSecretkey == "")
                                    {
                                        logger.LogError($"XXX_GetUrlPay ===> بوابة الدفع الالكتروني غير مجهزة بالمصحة");
                                        return new
                                        {
                                            url = "",
                                            error = "بوابة الدفع الالكتروني غير مجهزة بالمصحة"
                                        };
                                    }
                                }
                                else
                                {
                                    //Tadawel
                                    storeId = (tblClinic.Rows[0]["PaymentGateway"] == DBNull.Value) ? "" : tblClinic.Rows[0]["PaymentGateway"].ToString()!;
                                    storeToken = (tblClinic.Rows[0]["PaymentGatewayToken"] == DBNull.Value) ? "" : tblClinic.Rows[0]["PaymentGatewayToken"].ToString()!;

                                    if (storeId == "" || storeToken == "")
                                    {
                                        logger.LogError($"XXX_GetUrlPay ===> بوابة الدفع الالكتروني غير مجهزة بالمصحة");
                                        return new
                                        {
                                            url = "",
                                            error = "بوابة الدفع الالكتروني غير مجهزة بالمصحة"
                                        };
                                    }
                                }
                            }
                        }
                    }
                }
                else{
                    if (!TestClinic){
                        logger.LogError($"XXX_GetUrlPay ===> هذه مصحة حقيقية لا يمكن العمل عليها في بيئة التجربة");
                        return new{
                            url = "",
                            error = "هذه مصحة حقيقية لا يمكن العمل عليها في بيئة التجربة"
                        };
                    }
                }

                switch (clsUrlPay.PaymentGateType) {
                    case EnumPaymentGateType.walletRecharge:

                        PaymentGateId = getPaymentGateIdForWalletRecharge(clsUrlPay, tblSetupTable);
                        break;

                    case EnumPaymentGateType.patientAppointment:
                        if (clsUrlPay.Appointment == null) return emptyRes;
                        if (clsUrlPay.Appointment.RemoteOpdScheduleId == 0) return emptyRes;

                        PaymentGateId = getPaymentGateIdForAppointment(clsUrlPay, tblSetupTable);
                        break;

                    case EnumPaymentGateType.orderPayment:
                        if (clsUrlPay.OrderDetailsIds== null) return emptyRes;

                        PaymentGateId = getPaymentGateIdForOrderDet(clsUrlPay, tblSetupTable);
                        break;

                    case EnumPaymentGateType.activeClinic:
                        PaymentGateId = getPaymentGateIdForActiveClinic(clsUrlPay, tblSetupTable);
                        break;

                    case EnumPaymentGateType.subscribeMachine:
                        PaymentGateId = getPaymentGateIdForSubscribeMachine(clsUrlPay, tblSetupTable);
                        break;

                    case EnumPaymentGateType.subscribeWhatsApp:
                        PaymentGateId = getPaymentGateIdForSubscribeWhatsApp(clsUrlPay, tblSetupTable);
                        break;

                    case EnumPaymentGateType.subscribeSms:
                        PaymentGateId = getPaymentGateIdForSubscribeSms(clsUrlPay, tblSetupTable);
                        break;
                    default:
                        
                        return emptyRes;
                }
                if (PaymentGateId == 0) return emptyRes;


                string PaymentGateIdEncrypt = EncryptInvoiceNo(PaymentGateId.ToString(), "TamamKeyEncrypt");
                if (!clsUrlPay.Moamalat) PaymentGateIdEncrypt = Uri.EscapeDataString(PaymentGateIdEncrypt);

                string customRef = $"@!@!@Tamam@!@!@_{PaymentGateIdEncrypt}";
                if (clsUrlPay.Moamalat)
                {
                    return new
                    {
                        url = "",
                        error = "",
                        customRef
                    };
                }

                string backendUrl = "https://mainApi.tamam.ly/PaymentCallBack/TDSPPaymentCallBack";

                double Commission = Convert.ToDouble(tblSetupTable.Rows[0]["GetAwayCommission"]);

                double amountWithCommission = clsUrlPay.Amount+(clsUrlPay.Amount*Commission/100);

                string apiUrl = clsUrlPay.RealEnvironment ? "https://api.tlync.ly/api/v1/intiate/apipayment" :
                    "https://uat-api.tlync.ly/api/v1/intiate/apipayment";

                string url = $"{apiUrl}?id={storeId}&amount={amountWithCommission}&backend_url={backendUrl}&custom_ref={customRef}";
                //url += "&frontend_url=https://www.google.com/";
                url += $"&phone={clsUrlPay.UserPhoneNo}";
                _logger.LogInformation($"GetUrlPay ===> URL = {url}");

                //if (email.isNotEmpty) url += '&email=$email';

                // إعداد رأس التراخيص (headers)
                var headers = new Dictionary<string, string>
                    {
                        { "Accept", "application/json" },
                        //{ "Content-Type", "application/x-www-form-urlencoded" },
                        { "Authorization", $"Bearer {storeToken}" }
                    };

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
                HttpClient _httpClient = new HttpClient();
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JsonDocument doc = JsonDocument.Parse(responseBody);
                    if (doc.RootElement.TryGetProperty("url", out JsonElement urlElement))
                    {
                        return new
                        {
                            url = urlElement.GetString(),
                            error = ""
                        };

                    }
                    else
                    {
                        // Handle case where "url" property is not found
                        //Console.WriteLine("Response JSON does not contain a 'url' property.");
                        logger.LogError($"XXX_GetUrlPay ===> Unknown error");
                        return new
                        {
                            url = "",
                            error = "Unknown error"
                        };
                    }
                }
                else {
                    return response;
                }
            }
            catch (Exception ex) { 
                return ex.Message;
            }
        }

        [Obsolete]
        public async Task<dynamic> WhatsAppMsg(ClsWhatsAppMsg clsWhatsAppMsg, ILogger<MainController> _logger)
        {
            try
            {
                DataTable tblSetupTable = ExecSp(new TamaRequest() { spName = "sp_Get_Table", paras = [new Para() { Name = "TableName", DataType = SqlDbType.NVarChar, Value = "SetupTable" }] });
                decimal msgAmount = (tblSetupTable.Rows[0]["WhatsappMsgPrice"] == DBNull.Value) ? 0 : (decimal)tblSetupTable.Rows[0]["WhatsappMsgPrice"];

                DataTable tblClinic = ExecSp(new TamaRequest() { spName = "sp_Get_Clinic", paras = [new Para() { Name = "ClinicNo", DataType = SqlDbType.Int, Value = clsWhatsAppMsg.clinicNo.ToString() }] });
                if (tblClinic.Rows.Count == 0) return "Can't find a clinic";
                string ClinicName = (string)tblClinic.Rows[0]["ClinicName"];

                decimal WhatsAppBalance = (tblClinic.Rows[0]["WhatsAppBalance"] == DBNull.Value) ? 0 : (decimal)tblClinic.Rows[0]["WhatsAppBalance"];
                
                if (WhatsAppBalance < msgAmount) return new
                {
                    MsgId = 2,
                    MsgAr = "There is not enough balance.",
                    MsgEn = "There is not enough balance.",
                    Data= clsWhatsAppMsg.customerPhoneNo
                };

                string apiUrl = "https://graph.facebook.com/v20.0/435425779648819/messages";
                string token = "EAARBrWpzZCQ8BO4WGlKBB7ZBNUs3sr4bGfZBBzAQZAZCpcsGV8ZCxLHczauW0tRqSn0Gpscsns7bHURqUOTidiQucQG13B7ISyjzdqmHY04JZAVGR8sbRApnXEjbtl5U4VS2ksvx67iJjDxyZBGE9QFQovQw1aavxLxqwbZAAmexW0lekiwPZCCswZC5IZBZA7OxzMLfK6VmAq7sIaDJNZCKVj";

                List<Component> components = new List<Component>(
                [
                    new Component{ type = "header", 
                                    parameters = new List<Parameter>([new Parameter { type= "text", text=ClinicName }])
                                    },
                    new Component{ type = "body",
                                    parameters = new List<Parameter>([
                                            new Parameter { type= "text", text= clsWhatsAppMsg.budy },
                                            new Parameter { type= "text", text= clsWhatsAppMsg.OtherInfo??"" }
                                            ])
                                }
                ]);

                clsWhatsAppMsg.customerPhoneNo = CheckPoneNoForWhatsApp(clsWhatsAppMsg.customerPhoneNo);

                if (clsWhatsAppMsg.customerPhoneNo == "") return new
                {
                    MsgId = 2,
                    MsgAr = "The phone number not correct",
                    MsgEn = "The phone number not correct",
                    Data = clsWhatsAppMsg.customerPhoneNo
                };

                Template template = new Template(name: "msg", language: new Language { code = "ar" }, components: components);
                ClsModWhatsAppMessage clsModWhatsAppMessage = new ClsModWhatsAppMessage(messaging_product: "whatsapp", to: clsWhatsAppMsg.customerPhoneNo, type: "template", template: template);

                string json = JsonConvert.SerializeObject(clsModWhatsAppMessage);

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();

                        logger.LogInformation($"XXX_Whatsapp Result===> {JsonConvert.SerializeObject(result)}");

                        ExecCmd(new TamaRequestCmd() { Cmd = $"Update Clinic Set WhatsAppBalance=WhatsAppBalance-{msgAmount} Where ClinicNo={clsWhatsAppMsg.clinicNo}" });
                        return new
                        {
                            MsgId = 1,
                            MsgAr = "Ok",
                            MsgEn = "Ok",
                            Data = clsWhatsAppMsg.customerPhoneNo
                        };
                    }
                    else
                    {
                        //Console.WriteLine("حدث خطأ: " + response.StatusCode);
                        return new
                        {
                            MsgId = 2,
                            MsgAr = response.ReasonPhrase!,
                            MsgEn = response.ReasonPhrase!,
                            Data = clsWhatsAppMsg.customerPhoneNo

                        };
                    }
                }
            }
            catch (Exception ex) { return ex.Message; }
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

            ExecSp(new TamaRequest() { spName = "sp_Ins_Notification", paras = paras });

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
            DataTable tblNamaAdmin = ExecSp(new TamaRequest() {spName= "sp_Get_Table",paras=paras});
            return tblNamaAdmin;
        }
        
        [Obsolete]
        DataTable getClinicAdminsTokens(int ClinicNo) {
            List<Para> paras = [
                new(){ Name="PermissionId",Value="1",DataType= SqlDbType.Int},
                new(){ Name="ClinicNo",Value=ClinicNo.ToString(),DataType= SqlDbType.Int}
            ];
            DataTable tblNamaAdmin = ExecSp(new TamaRequest() { spName = "sp_Get_ClinicUserPermission", paras = paras });
            return tblNamaAdmin;
        }

        [Obsolete]
        DataTable getTamaPharmAppTokens() {
            DataTable tblPharmacyUser = ExecSp(new TamaRequest() { spName = "sp_Get_PharmacyUser", paras = [] });
            return tblPharmacyUser;
        }

        [Obsolete]
        private static string GetPatientFireBaseToken(int RemoteCustomerId,int clinicNo, out int UserId,out int clinicId,out int PatientId) {
            DataTable dtData= ExecSp(new TamaRequest() { spName = "sp_Get_ClinicPatient", paras = [
                new Para(){Name="RemoteCustomerId",DataType=SqlDbType.Int,Value=RemoteCustomerId.ToString()},
                new Para(){Name="ClinicNo",DataType=SqlDbType.Int,Value=clinicNo.ToString()}
                ] });
            UserId = 0;
            clinicId = 0;
            PatientId = 0;
            if (dtData.Rows.Count != 0) {
                clinicId= getValueInt(dtData.Rows[0], "ClinicId");
                UserId = getValueInt(dtData.Rows[0], "UserId");
                PatientId = getValueInt(dtData.Rows[0], "PatientId");
                return getValueString(dtData.Rows[0], "firebaseTokenPatient");
            }
            else
                return "";
        }
        
        [Obsolete]
        private static string GetDoctorFireBaseToken(int RemoteCustomerId, int clinicNo, out int UserId)
        {
            string cmd = $"SELECT        dbo.Doctor.firebaseToken,dbo.Person.UserId\r\nFROM            dbo.ClinicUser INNER JOIN\r\n                         dbo.Clinic ON dbo.ClinicUser.ClinicId = dbo.Clinic.ID INNER JOIN\r\n                         dbo.Doctor INNER JOIN\r\n                         dbo.Person ON dbo.Doctor.PersonId = dbo.Person.ID ON dbo.ClinicUser.UserId = dbo.Person.UserId\r\nWHERE        (dbo.Clinic.ClinicNo = {clinicNo}) AND (dbo.ClinicUser.RemoteClinicUserId = {RemoteCustomerId}) AND (dbo.ClinicUser.DeletionDate IS NULL) AND (dbo.Clinic.DeletionDate IS NULL)";
            DataTable dtData = ExecCommand(new TamaRequestCmd()
            {
                Cmd = cmd
            });

            UserId = 0;
            if (dtData.Rows.Count != 0) {
                UserId = getValueInt(dtData.Rows[0], "UserId");
                return getValueString(dtData.Rows[0], "firebaseToken");
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
            try { 
                string cmd = $"SELECT TOP (1) * FROM dbo.Notification WHERE (UserId = {UserId}) AND (NotificationTypeId = 3) ORDER BY ID DESC";
                DataTable dtData = ExecCommand(new TamaRequestCmd(){Cmd = cmd});
                if (dtData.Rows.Count == 0)
                    return false;
                else {
                    string SavedData = (string)dtData.Rows[0]["Data"]!;
                    //dynamic SavedDataClass = JsonConvert.DeserializeObject<dynamic>(SavedData)!;
                    using (JsonDocument doc = JsonDocument.Parse(SavedData))
                    {
                        JsonElement root = doc.RootElement;
                        if (root.GetProperty("ClinicNo").GetString()! == Data!["ClinicNo"])
                            if (root.GetProperty("PatientId").GetString()! == Data["PatientId"])
                                if (root.GetProperty("OpdScheduleId").GetString()! == Data["OpdScheduleId"])
                                    if (root.GetProperty("AfterHours").GetString()! == Data["AfterHours"]) 
                                        return true;

                    }
                    return false;
                }
            }
            catch (Exception Ex) {
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
                        apiNotification.Token = getValueString(row, "firebaseToken");
                        message = new Message()
                        {
                            Notification = apiNotification.Notification,
                            Data = apiNotification.Data,
                            Token = apiNotification.Token
                        };
                        msgRes=await SendNotification(message, EnumNotificationType.contactUs, getValueInt(row, "UserId"), _logger);
                    }
                    break;
                
                case EnumNotificationType.searchMedecine:
                    apiNotification.Data!.Add($"NotificationType", EnumNotificationType.searchMedecine.ToString()!);
                    DataTable tamaPharmAppTokens = getTamaPharmAppTokens();
                    foreach (DataRow row in tamaPharmAppTokens.Rows)
                    {
                        apiNotification.Token = getValueString(row, "firebaseTokenPharm");
                        message = new Message()
                        {
                            Notification = apiNotification.Notification,
                            Data = apiNotification.Data,
                            Token = apiNotification.Token
                        };
                        msgRes = await SendNotification(message, EnumNotificationType.searchMedecine, getValueInt(row, "UserId"), _logger);
                    }
                    break;
                
                case EnumNotificationType.clinicAdmins:
                    int ClinicNo = int.Parse(apiNotification.Data!["ClinicNo"]);
                    apiNotification.Data!.Add($"NotificationType", EnumNotificationType.clinicAdmins.ToString()!);

                    DataTable tamaClinicAdmins = getClinicAdminsTokens(ClinicNo);
                    foreach (DataRow row in tamaClinicAdmins.Rows)
                    {
                        apiNotification.Token = getValueString(row, "firebaseToken");
                        message = new Message()
                        {
                            Notification = apiNotification.Notification,
                            Data = apiNotification.Data,
                            Token = apiNotification.Token
                        };
                        msgRes = await SendNotification(message, EnumNotificationType.clinicAdmins, getValueInt(row, "UserId"), _logger);
                    }
                    break;
            }
            return msgRes;
        }
    }
}
