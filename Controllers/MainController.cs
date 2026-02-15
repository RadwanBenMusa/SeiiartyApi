using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TamaApi.clsMod;
using TamaApi.Services.Main;
using static TamaApi.Services.db.DbService;

namespace TamaApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MainController(IConfiguration config, ILogger<MainController> logger, IMainService mainService) : ControllerBase
    {
        public IConfiguration Configuration { get; } = config;
        private readonly ILogger<MainController> _logger = logger;
        private readonly IMainService _mainService = mainService;

        [HttpGet("Ping")]
        public IActionResult Ping(string? test = "")
        {
            //_logger.LogInformation($"XXX_MainController (Ping)");
            return Ok($"Pinging == Ok\n{test}");
        }


        [HttpPost("ExecStoredProcedure"), Authorize]

        public IActionResult ExecStoredProcedure(Request request)
        {
            try
            {
                _logger.LogInformation($"XXX_ExecStoredProcedure ===> request = {JsonConvert.SerializeObject(request)}");
                dynamic msgRes = _mainService.ExecStoredProcedure(request);
                //_logger.LogInformation($"XXX_ExecStoredProcedure ===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_ExecStoredProcedure ===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }




        [HttpPost("ExecCmd")]


        public IActionResult ExecCmd(RequestCmd requestCmd)
        {
            try
            {
                _logger.LogInformation($"XXX_ExecCmd===> requestCmd = {JsonConvert.SerializeObject(requestCmd)}");
                dynamic msgRes = _mainService.ExecCmd(requestCmd);
                //_logger.LogInformation($"XXX_ExecCmd===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_ExecStoredProcedure ===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }


        [HttpPost("DoSomeThings")]

        public IActionResult DoSomeThings(Request request)
        {
            try
            {
                _logger.LogInformation($"XXX_DoSomeThings ===> request = {JsonConvert.SerializeObject(request)}");
                dynamic msgRes = _mainService.DoSomeThings(request);
                //_logger.LogInformation($"XXX_ExecStoredProcedure ===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_ExecStoredProcedure ===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }


        [HttpPost("Notification")]

        public async Task<IActionResult> Notification(ApiNotification apiNotification)
        {
            try
            {
                _logger.LogInformation($"Notification ===> Message = {JsonConvert.SerializeObject(apiNotification)}");
                dynamic msgRes = await _mainService.Notification(apiNotification, _logger);
                //_logger.LogInformation($"Notification ===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_Notification===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }

        private IActionResult Error(Exception Ex)
        {
            if (Ex.Source == "TamaApi")
                return StatusCode(StatusCodes.Status423Locked, Ex.Message);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, Ex.Message);
        }

    }
}
