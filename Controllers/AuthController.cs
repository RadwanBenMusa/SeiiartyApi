using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TamaApi.Services.Auth;

namespace TamaApi.Controllers
{
    public class UserData
    {
        public required string PhoneNumber { get; set; }
        public string? Password { get; set; } = "";
        public int? ExpiredTokenMinutes { get; set; }
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

    public class AuthController : ControllerBase
    {
        public IConfiguration _Configuration { get; }
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration config, ILogger<AuthController> logger, IAuthService authService)
        {
            _Configuration = config;
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("Login")]
        public IActionResult Login(UserData user)
        {
            try
            {
                _logger.LogInformation($"XXX_AuthController_Login ===> user = {JsonConvert.SerializeObject(user)}");
                int userId = _authService.Login(user, _logger);
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

                var token = _authService.GetToken(authClaims, ExpiredTokenMinutes: user.ExpiredTokenMinutes);

                var resData = new{
                    userId,
                    phoneNumber = user.PhoneNumber,
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo.ToLocalTime()
                };

                if (!_authService.CheckPhoneToken(userId, user.PhoneToken, _logger)){
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
                _logger.LogError($"XXX_AuthController_Login ===> Error = {Ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, Ex.Message);
            }
        }

        [HttpPost("VerifyOTP")]
        public IActionResult VerifyOTP(UserToVerifyOTP userToVerifyOTP)
        {
            try
            {
                _logger.LogInformation($"XXX_AuthController_VerifyOTP ===> userToVerifyOTP = {JsonConvert.SerializeObject(userToVerifyOTP)}");
                dynamic msgRes = _authService.VerifyOTP(userToVerifyOTP);
                _logger.LogInformation($"XXX_AuthController_VerifyOTP ===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_AuthController_VerifyOTP ===> Error = {Ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, Ex.Message);
            }
        }

        [HttpPost("SendOTP")]
        public IActionResult SendOTP(UserToSendOTP userToSendOTP)
        {
            try
            {
                _logger.LogInformation($"XXX_AuthController_SendOTP ===> userToSendOTP = {JsonConvert.SerializeObject(userToSendOTP)}");
                dynamic msgRes = _authService.SendOTP(userToSendOTP);
                _logger.LogInformation($"XXX_AuthController_SendOTP ===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_AuthController_SendOTP ===> Error = {Ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, Ex.Message);
            }
        }

#if DEBUG
        [HttpPost("Register")]
#else
        [HttpPost("Register"), Authorize]
#endif
        public IActionResult Register(UserData user)
        {
            try { 
            _logger.LogInformation($"XXX_AuthController_Register ===> user = {JsonConvert.SerializeObject(user)}");
            dynamic msgRes = _authService.Register(user);
            _logger.LogInformation($"XXX_AuthController_Register ===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
            return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex) {
                _logger.LogError($"XXX_AuthController_Register ===> Error = {Ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, Ex.Message);
            }
        }

        [HttpPost("ResetPassword"), Authorize]
        public IActionResult ResetPassword(UserData user)
        {
            try
            {
                _logger.LogInformation($"XXX_AuthController_Register ===> user = {JsonConvert.SerializeObject(user)}");
                dynamic msgRes = _authService.ResetPassword(user);
                _logger.LogInformation($"XXX_AuthController_Register ===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_AuthController_Register ===> Error = {Ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, Ex.Message);
            }
        }


        [HttpPost("Delete"), Authorize]
        public IActionResult Delete(UserData user)
        {
            try
            {
                _logger.LogInformation($"XXX_AuthController_DetletUser ===> user = {JsonConvert.SerializeObject(user)}");
                dynamic msgRes = _authService.Delete(user);
                _logger.LogInformation($"XXX_AuthController_DetletUser ===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_AuthController_Register ===> Error = {Ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, Ex.Message);
            }
        }

    }
}
