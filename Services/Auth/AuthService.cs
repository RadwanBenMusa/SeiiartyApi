using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp;
using Seiiarty.Controllers;
using Seiiarty.Services.db;
using Seiiarty.StoredProcedures;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Seiiarty.General;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.Services.Auth
{
    public class AuthService(IConfiguration config) : IAuthService
    {
        public IConfiguration Configuration { get; } = config;

        [Obsolete]
        public bool CheckPhoneToken(int userId,string? PhoneToken)
        {
            if (PhoneToken == null) return true;

            
            DataTable dtUser = ExecCommand(new RequestCmd() { Cmd = $"Select * from [User] where ID = {userId} And DeletionDate = null" }); 

            if (dtUser.Rows.Count > 0)
            {
                string dbPhoneToken = General.GetValue<string>(dtUser.Rows[0], "phoneToken");
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
        public dynamic Auth(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];

            switch (requestSp.Method)
            {
                case "Login":
                    result = Login(
                        General.GetPara(p, "PhoneNo"),
                        General.GetPara(p, "Password"),
                        General.GetPara(p, "PhoneToken")
                    );
                    break;

                case "Register":
                    result = Register(
                        General.GetPara(p, "FullName"),
                        General.GetPara(p, "PhoneNumber"),
                        General.GetPara(p, "Password"),
                        General.GetPara(p, "PhoneToken")
                    );
                    break;

                case "CheckPhone":
                    result = CheckPhone(
                        General.GetPara(p, "PhoneNumber")
                    );
                    break;

                case "ResetPassword":
                    result = ResetPassword(
                        General.GetPara(p, "PhoneNumber"),
                        General.GetPara(p, "Password")
                    );
                    break;
                case "NewDevice":
                    SpUser.Update(new UpdateUser
                    {
                        Id = General.GetParaInt(p, "ID")!.Value,
                        PhoneToken = General.GetPara(p, "PhoneToken"),
                    });
                    result = new { Success = true, Message = "DEVICE_CHANGED" };
                    break;
            }
            return result;
        }

        // ── Login ────────────────────────────────────────────────────────────────────
        [Obsolete]
        private dynamic Login(string phoneNo, string password, string phoneToken)
        {
            dynamic userResult = SpUser.Get(new GetUser { PhoneNo = phoneNo });
            if (userResult is string)
                return new { Success = false, Message = "USER_NOT_FOUND" };

            DataTable dt = (DataTable)userResult;
            DataRow user = dt.Rows[0];

            if (user["DeletionDate"] != DBNull.Value)
                return new { Success = false, Message = "USER_DELETED" };

            if (user["Password"].ToString() != password)
                return new { Success = false, Message = "WRONG_PASSWORD",Data = user };

            string dbPhoneToken = user["PhoneToken"]?.ToString() ?? "";
            bool isNewDevice = !string.IsNullOrEmpty(dbPhoneToken)
                                  && dbPhoneToken != phoneToken;


            if (isNewDevice) {
                return new { Success = false, Message = "DIFFERENT_DEVICE",Data = user };
            }

                

            // First login on this device — save phone token
            if (string.IsNullOrEmpty(dbPhoneToken))
                SpUser.Update(new UpdateUser
                {
                    Id = int.Parse(user["ID"].ToString()!),
                    PhoneToken = phoneToken,
                });

            SpUser.Update(new UpdateUser
            {
                Id = int.Parse(user["ID"].ToString()!),
                LastLogin = DateTime.Now,
            });

            return BuildTokenResponse(user, phoneToken,true);
        }

        // ── Register ─────────────────────────────────────────────────────────────────
        [Obsolete]
        private dynamic Register(string fullName, string phoneNumber, string password, string phoneToken)
        {
            string formatted = General.FormatLibyanPhone(phoneNumber);

            // Check if phone already exists (including soft-deleted)
            dynamic existing = SpUser.Get(new GetUser
            {
                PhoneNo = formatted,
                WithDeletionDate = true,
            });

            int userId;

            if (existing is DataTable dt && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                // Phone exists and is active — cannot register again
                if (row["DeletionDate"] == DBNull.Value)
                    return new { Success = false, Message = "PHONE_ALREADY_EXISTS" };

                // Soft-deleted — restore the account
                SpUser.Update(new UpdateUser
                {
                    Id = int.Parse(row["ID"].ToString()!),
                    FullName = fullName,
                    PhoneNumber = formatted,
                    Password = password,
                    PhoneToken = phoneToken,
                    RemoveDeletionDate = true,
                });
                userId = int.Parse(row["ID"].ToString()!);
            }
            else
            {
                // Brand new user
                userId = SpUser.Insert(new InsUser
                {
                    FullName = fullName,
                    PhoneNumber = formatted,
                    Password = password,
                    PhoneToken = phoneToken,
                });
            }

            // Get the fresh user row to build token
            dynamic newUser = SpUser.Get(new GetUser { Id = userId });
            DataRow user = ((DataTable)newUser).Rows[0];

            return BuildTokenResponse(user, phoneToken,false);
        }

        // ── CheckPhone ───────────────────────────────────────────────────────────────
        [Obsolete]
        private dynamic CheckPhone(string phoneNumber)
        {
            string formatted = General.FormatLibyanPhone(phoneNumber);
            dynamic result = SpUser.Get(new GetUser { PhoneNo = formatted });

            if (result is string)
                return new { Exists = false };

            return new { Exists = true };
        }

        // ── ResetPassword ─────────────────────────────────────────────────────────────
        [Obsolete]
        private dynamic ResetPassword(string phoneNumber, string password)
        {
            string formatted = General.FormatLibyanPhone(phoneNumber);
            dynamic userResult = SpUser.Get(new GetUser { PhoneNo = formatted });

            if (userResult is string)
                return new { Success = false, Message = "USER_NOT_FOUND" };

            DataTable dt = (DataTable)userResult;
            DataRow user = dt.Rows[0];

            SpUser.Update(new UpdateUser
            {
                Id = int.Parse(user["ID"].ToString()!),
                Password = password,
            });

            return new { Success = true, Message = "PASSWORD_RESET" };
        }

        // ── Shared: Build JWT response ────────────────────────────────────────────────
        [Obsolete]
        private dynamic BuildTokenResponse(DataRow user, string phoneToken,bool Login)
        {
            var claims = new List<Claim>
            {
                new ("ID",          user["ID"].ToString()!),
                new ("PhoneNumber", user["PhoneNumber"].ToString()!),
                new ("FullName",    user["FullName"].ToString()!),
                new ("Admin",       user["Admin"].ToString()!),
                new ("PhoneToken",  phoneToken),
            };

            JwtSecurityToken token = GetToken(claims);
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new
            {
                Success = true,
                Message = (Login)? "LOGIN_SUCCESS" : "REGISTER_SUCCESS",
                Token = tokenString,
                Expires = token.ValidTo,
                Data = user
            };
        }

        static readonly string ApiKey = "78512214deafed6a";
        static readonly string SecretKet = "bce5825b";
        static readonly string callerID = "Tamam";
        static readonly string length = "4";
        static readonly string BaseUrl = "https://otp.libyasms.com/";

        

        class DataRes()
        {
            public string? Status { get; set; }
            public string? Text { get; set; }
            public string? Otp_ID { get; set; }
        }

        



        [Obsolete]
        public JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            
            DataTable dtSetuTable = SpSetupTable.Get();
            double expirCount = double.Parse(dtSetuTable.DefaultView[0]["TokenExpiredHours"].ToString()!);
            

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]!));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: Configuration["JWT:ValidIssuer"],
                audience: Configuration["JWT:ValidAudience"],
                claims: authClaims,
                expires: DateTime.Now.AddHours(expirCount),
                signingCredentials: signinCredentials
            );

            return token;
        }


    }
}
