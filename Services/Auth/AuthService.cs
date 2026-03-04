using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using System.Data;
using TamaApi.Controllers;
using static TamaApi.Services.db.DbService;
using System.Text;
using System.Security.Cryptography;
using static TamaApi.General;
using RestSharp;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using TamaApi.Services.db;
using System;

namespace TamaApi.Services.Auth
{
    public class AuthService(IConfiguration config) : IAuthService
    {
        public IConfiguration Configuration { get; } = config;

        public const string key = "HiSaiidMusa195$$";

        public static string Encrypt(string password)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = new byte[16];
            ICryptoTransform cryptoTransform = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream memoryStream = new();
            using (CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write))
            {
                using StreamWriter streamWriter = new(cryptoStream);
                streamWriter.Write(password);
            }
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public static string Decrypt(string encryptedTxt)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = new byte[16];
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new(Convert.FromBase64String(encryptedTxt));
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            return srDecrypt.ReadToEnd();
        }

        [Obsolete]
        public bool CheckPhoneToken(int userId,string? PhoneToken, ILogger<AuthController> _logger)
        {
            if (PhoneToken == null) return true;

            _logger.LogInformation("XXX_CheckPhoneToken");
            DataTable dtUser = ExecCommand(new RequestCmd() { Cmd = $"Select * from [User] where ID = {userId} And DeletionDate = null" }); 

            if (dtUser.Rows.Count > 0)
            {
                string dbPhoneToken = General.GetValueString(dtUser.Rows[0], "phoneToken");
                if (string.IsNullOrEmpty(dbPhoneToken))
                {
                    string cmd = $"Update [User] Set phoneToken='{PhoneToken}' Where ID={userId}";
                    ExecCommand(new RequestCmd() { Cmd = cmd });
                    return true;
                }
                else
                {
                    return (dbPhoneToken == PhoneToken);
                }
            }
            else 
                return true;

        }

        [Obsolete]
        public int User(UserData user, ILogger<AuthController> _logger)
        {
            _logger.LogInformation("XXX_AuthService_Login");
            DataTable dtUser = ExecCommand(new RequestCmd() { Cmd = $"Select * from [User] where PhoneNumber = {user.PhoneNumber} And DeletionDate = null" });
            //DataTable dtUser = JsonConvert.DeserializeObject<DataTable>(msgRes.Data!)!;
            if (dtUser.Rows.Count > 0)
            {
                string dbPasssword = "";
                if (dtUser.Rows[0]["Password"] != DBNull.Value)
                    dbPasssword = (string)dtUser.Rows[0]["Password"];
                if (user.Password == Decrypt(dbPasssword)) {
                    int userId = (int)dtUser.Rows[0]["Id"];
                    string lastLogin= $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day} {DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}";
                    string cmd = $"Update [User] Set lastLogin='{lastLogin}' Where ID={userId}";
                    ExecCommand(new RequestCmd() { Cmd = cmd });
                    return userId;
                }
            }

            return 0;
            //throw new NotImplementedException();
        }

        static readonly string ApiKey = "78512214deafed6a";
        static readonly string SecretKet = "bce5825b";
        static readonly string callerID = "Tamam";
        static readonly string length = "4";
        static readonly string BaseUrl = "https://otp.libyasms.com/";

        [Obsolete]
        private dynamic CheckOtpForWhat(UserToSendOTP userToSendOTP)
        {
            if (userToSendOTP.OtpForWhat == null) return new { MsgId = 1 };

            DataTable dtUser = ExecCommand(new RequestCmd() { Cmd = $"Select * from [User] where PhoneNumber = {userToSendOTP.PhoneNumber} And DeletionDate = null" });

            switch (userToSendOTP.OtpForWhat)
            {
                case OtpForWhat.CreateNewUser:
                    if (dtUser.Rows.Count > 0)
                        return new { MsgId = 2, MsgAr = "الرقم موجود مسبقا", MsgEn = "The number already exists" };
                    else
                        return new { MsgId = 1 };

                case OtpForWhat.ResetPassword:
                    if (dtUser.Rows.Count > 0)
                        return new { MsgId = 1 };
                    else
                        return new { MsgId = 2, MsgAr = "هذا الرقم غير موجود", MsgEn = "This number does not exist" };

            }

            return new { MsgId = 2, MsgAr = "رفض", MsgEn = "Reject" };
        }

        class DataRes()
        {
            public string? Status { get; set; }
            public string? Text { get; set; }
            public string? Otp_ID { get; set; }
        }

        [Obsolete]
        public dynamic SendOTP(UserToSendOTP userToSendOTP)
        {
            string PhoneNumber = CheckPhoneNumberOk(userToSendOTP.PhoneNumber);
            if (PhoneNumber == "")
            {
                return new
                {
                    MsgId = 2,
                    MsgAr = "رقم الهاتف غير صحيح!",
                    MsgEn = "Invalid phone number!"
                };
            }

            dynamic msgRes = CheckOtpForWhat(userToSendOTP);
            if (msgRes.MsgId != 1)
                return msgRes;

            DataTable tdSetupTable = DbService.GetSetupTable();
            if ((bool)tdSetupTable.DefaultView[0]["OtpDebugMode"] == true)
            {
                DataRes xxx = new()
                {
                    Status = "0",
                    Text = "ACCEPTD",
                    Otp_ID = "4c075ca2-ecc1-43cc-8f7a-750607ffeac2"
                };

                return new
                {
                    MsgId = xxx.Status == "0" ? 1 : 2,
                    MsgAr = xxx.Status == "0" ? "تمت بنجاح" : "لم يتم الارسال",
                    MsgEn = xxx.Status == "0" ? "Sending Successfully" : "Sending Fail",
                    Data = xxx
                };


            }
            else
            {
                string URL = BaseUrl + "sendotp?";
                URL += $"apikey={ApiKey}";
                URL += $"&secretkey={SecretKet}";
                URL += $"&callerID={callerID}";
                URL += $"&toUser={userToSendOTP.PhoneNumber}";
                URL += $"&length={length}";
                URL += $"&appSignature={userToSendOTP.AppSignature}";
                URL += $"&validityInSeconds={userToSendOTP.ValidityInSeconds}";

                var client = new RestClient(BaseUrl);
                var request = new RestRequest(URL, Method.Get);
                var response = client.Execute(request);
                string xxx = response.Content!;
                dynamic res = JsonConvert.DeserializeObject(xxx)!;
                return new
                {
                    MsgId = res.Status == "0" ? 1 : 2,
                    MsgAr = res.Status == "0" ? "تمت بنجاح" : "لم يتم الارسال",
                    MsgEn = res.Status == "0" ? "Sending Successfully" : "Sending Fail",
                    Data = new DataRes()
                    {
                        Status = res.Status,
                        Text = res.Text,
                        Otp_ID = res.Otp_ID
                    }
                };
            }
        }

        [Obsolete]
        public dynamic VerifyOTP(UserToVerifyOTP userToVerifyOTP)
        {
            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, "+218910001122"),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Role,"+218910001122"),
            };
            var token = GetToken(authClaims, forResetOrRegister:true);

            DataTable tdSetupTable = DbService.GetSetupTable();
            if ((bool)tdSetupTable.DefaultView[0]["OtpDebugMode"] == true)
            {
                return new
                {
                    MsgId = 1,
                    MsgAr = "الرمز مطابق",
                    MsgEn = "الرمز مطابق",
                    Data = new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    }
                };
            }
            else
            {
                string URL = BaseUrl + "verify?";
                URL += $"otpId={userToVerifyOTP.OtpId}";
                URL += $"&otpCode={userToVerifyOTP.OtpCode}";

                var client = new RestClient(BaseUrl);
                var request = new RestRequest(URL, Method.Get);
                var response = client.Execute(request);

                if (response.Content == "true") {
                    return new
                    {
                        MsgId = 1,
                        MsgAr = "الرمز مطابق",
                        MsgEn = "الرمز مطابق",
                        Data = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        }
                    };
                }
                else
                    return new
                    {
                        MsgId = 2,
                        MsgAr = "الرمز غير مطابق",
                        MsgEn = "الرمز غير مطابق"//,
                                                 //Data = response.Content
                    };
            }
        }

        [Obsolete]
        public dynamic GetSetupTable() 
        {
            return DbService.GetSetupTable();
        }

        [Obsolete]
        public JwtSecurityToken GetToken(List<Claim> authClaims, bool? forResetOrRegister = false,int? ExpiredTokenMinutes=0)
        {
            double expiredTokenMinutes = 0;
            ExpiredTokenMinutes ??= 0;
            if (ExpiredTokenMinutes != 0)
                expiredTokenMinutes = int.Parse(ExpiredTokenMinutes.ToString()!);

            bool forResetOrRegister1;
            forResetOrRegister1 = forResetOrRegister ?? false;

            double expirCount;
            if (forResetOrRegister1)
                expirCount = 2;
            else
            {
                DataTable dtSetuTable = DbService.GetSetupTable();
                expirCount = double.Parse(dtSetuTable.DefaultView[0]["TokenExpiredHours"].ToString()!);
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]!));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            DateTime xxx = (expiredTokenMinutes != 0) ? DateTime.Now.AddMinutes(expiredTokenMinutes) : forResetOrRegister1 ? DateTime.Now.AddMinutes(expirCount) : DateTime.Now.AddHours(expirCount);
            xxx.ToLocalTime();

            var token = new JwtSecurityToken(
                issuer: Configuration["JWT:ValidIssuer"],
                audience: Configuration["JWT:ValidAudience"],
                claims: authClaims,
                expires: (expiredTokenMinutes != 0) ? DateTime.Now.AddMinutes(expiredTokenMinutes) : forResetOrRegister1 ? DateTime.Now.AddMinutes(expirCount) : DateTime.Now.AddHours(expirCount),
                signingCredentials: signinCredentials
            );

            return token;
        }


    }
}
