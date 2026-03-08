
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Seiiarty.Services.Auth;

namespace Seiiarty.Controllers
{
    public class UserData
    {
        public required string PhoneNumber { get; set; }
        public string? Password { get; set; } = "";
        public string? PhoneToken { get; set; }
    }

    public class UserToSendOTP
    {
        public required string PhoneNumber { get; set; }
        public string? AppSignature { get; set; }
        public int? ValidityInSeconds { get; set; }
        public OtpForWhat? OtpForWhat { get; set; }
    }

    public enum OtpForWhat
    {
        CreateNewUser,
        ResetPassword
    }

    public class UserToVerifyOTP
    {
        public required string OtpId { get; set; }
        public required string? OtpCode { get; set; }
    }

    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController(IConfiguration config, IAuthService authService) : ControllerBase
    {
        public IConfiguration Configuration { get; } = config;
        private readonly IAuthService _authService = authService;

        [HttpPost("User")]
        
        public IActionResult User(UserData user)
        {
            try
            {
                int userId = _authService.User(user);
                if (userId == 0)
                {
                    return StatusCode(StatusCodes.Status200OK,
                        new
                        {
                            MsgId = 2,
                            MsgAr = "رقم الهاتف او كلمة المرور غير صحيحة!",
                            MsgEn = "Phone Number Or Password Not Correct!"

                        });
                }

                var authClaims = new List<Claim>
                    {
                        new(ClaimTypes.Name, user.PhoneNumber),
                        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new(ClaimTypes.Role,user.PhoneNumber),
                    };

                var token = _authService.GetToken(authClaims);

                var resData = new{
                    userId,
                    phoneNumber = user.PhoneNumber,
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo.ToLocalTime()
                };

                if (!_authService.CheckPhoneToken(userId, user.PhoneToken)){
                    return StatusCode(StatusCodes.Status200OK,
                        new
                        {
                            MsgId = 3,
                            MsgAr = "لا يمكن تسجيل الدخول من هاتف آخر!",
                            MsgEn = "Cannot log in from another phone!",
                            Data = resData
                        });
                }
                else{ 
                    return StatusCode(StatusCodes.Status200OK,
                        new
                        {
                            MsgId = 1,
                            MsgAr = "دخول صحيح",
                            MsgEn = "Valid entry",
                            Data = resData
                        }
                    );
                }
            }
            catch (Exception Ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, Ex.Message);
            }
        }

        [HttpGet("Ping")]
        public IActionResult Ping(string? test = "")
        {
            //_logger.LogInformation($"XXX_MainController (Ping)");
            return Ok($"Pinging == Ok\n{test}");
        }
    }
}
