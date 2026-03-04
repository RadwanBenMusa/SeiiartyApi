using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TamaApi.Controllers;
using static TamaApi.General;

namespace TamaApi.Services.Auth
{
    public interface IAuthService
    {
        public int User(UserData user, ILogger<AuthController> _logger);
        public bool CheckPhoneToken(int userId,string? PhoneToken, ILogger<AuthController> _logger); 
        public dynamic SendOTP(UserToSendOTP userToSendOTP);
        public dynamic VerifyOTP(UserToVerifyOTP userToVerifyOTP);
        public dynamic GetSetupTable();
        public JwtSecurityToken GetToken(List<Claim> authClaims, bool? forResetOrRegister = false ,int? ExpiredTokenMinutes=0);
    }
}
