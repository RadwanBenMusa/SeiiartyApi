using Seiiarty.Controllers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.Services.Auth
{
    public interface IAuthService
    {
        public int Login(RequestSp requestSp);
        public bool CheckPhoneToken(int userId,string? PhoneToken); 
        public dynamic SendOTP(UserToSendOTP userToSendOTP);
        public dynamic VerifyOTP(UserToVerifyOTP userToVerifyOTP);
        public dynamic GetSetupTable();
        public JwtSecurityToken GetToken(List<Claim> authClaims, bool? forResetOrRegister = false ,int? ExpiredTokenMinutes=0);
    }
}
