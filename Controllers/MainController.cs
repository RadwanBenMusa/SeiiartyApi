
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Seiiarty.clsMod;
using Seiiarty.Services.Main;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.Controllers
{
    [Route("[controller]")]
    [ApiController]
#if !DEBUG
    [Authorize]
#endif
    public class MainController(IConfiguration config,  IMainService mainService) : ControllerBase
    {
        public IConfiguration Configuration { get; } = config;
        
        private readonly IMainService _mainService = mainService;


        [HttpPost("ExecCmd")]
        


        public IActionResult ExecCmd(RequestCmd requestCmd)
        {
            try
            {
                
                dynamic msgRes = _mainService.ExecCmd(requestCmd);
                //_logger.LogInformation($"XXX_ExecCmd===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
               
                return Error(Ex);
            }
        }


        [HttpPost("DoSomeThings")]
        
        public IActionResult DoSomeThings(Request request)
        {
            try
            {
               
                dynamic msgRes = _mainService.DoSomeThings(request);
                //_logger.LogInformation($"XXX_ExecStoredProcedure ===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                
                return Error(Ex);
            }
        }


        [HttpPost("Notification")]
        
        public async Task<IActionResult> Notification(ApiNotification apiNotification)
        {
            try
            {
                
                dynamic msgRes = await _mainService.Notification(apiNotification);
                //_logger.LogInformation($"Notification ===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                
                return Error(Ex);
            }
        }
       
        [HttpPost("TestAuth"),Authorize]
        public IActionResult TestAuth()
        {
            try
            {

                string msgRes = "Tamam Mya Mya";
                //_logger.LogInformation($"XXX_ExecCmd===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {

                return Error(Ex);
            }
        }
        private IActionResult Error(Exception Ex)
        {
            if (Ex.Source == "Seiiarty")
                return StatusCode(StatusCodes.Status423Locked, Ex.Message);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, Ex.Message);
        }

    }
}
