using Seiiarty.Controllers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.Services.Auth
{
    public interface IAuthService
    {
        public dynamic Auth(RequestSp requestSp);
        public bool CheckPhoneToken(int userId,string? PhoneToken); 
        public JwtSecurityToken GetToken(List<Claim> authClaims);
    }
}
