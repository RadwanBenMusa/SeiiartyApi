
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Seiiarty.Services.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.Controllers
{
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

        [HttpPost("Auth")]
        
        public IActionResult Auth(RequestSp requestSp)
        {
            try
            {
                dynamic msgRes = _authService.Auth(requestSp);
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, Ex.Message);
            }
        }

        [HttpGet("Ping")]
        public IActionResult Ping(string? test = "")
        {
            return Ok($"Pinging == Ok\n{test}");
        }

    }
}
