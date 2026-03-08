using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Seiiarty.Controllers;

namespace Seiiarty.Services.Auth
{
    public interface IAuthService
    {
        public int User(UserData user);
        public bool CheckPhoneToken(int userId,string? PhoneToken); 
        public dynamic SendOTP(UserToSendOTP userToSendOTP);
        public dynamic VerifyOTP(UserToVerifyOTP userToVerifyOTP);
        public dynamic GetSetupTable();
        public JwtSecurityToken GetToken(List<Claim> authClaims, bool? forResetOrRegister = false ,int? ExpiredTokenMinutes=0);
    }
}
