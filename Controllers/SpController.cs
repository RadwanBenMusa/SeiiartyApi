
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

    public class SpController(IConfiguration config, ISpService spService) : ControllerBase
    {
        public IConfiguration Configuration { get; } = config;

        private readonly ISpService _spService = spService;

        [HttpPost("Category")]
        public IActionResult Category(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = _spService.DoSpCategory(requestSp);
                //_logger.LogInformation($"XXX_ExecCmd===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {

                return Error(Ex);
            }
        }

        [HttpPost("Item")]
        public IActionResult Item(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = _spService.DoSpItem(requestSp);
                //_logger.LogInformation($"XXX_ExecCmd===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {

                return Error(Ex);
            }
        }

        [HttpPost("Notification")]
        public IActionResult Notification(RequestSp requestSp)
        {
            try
            {
                dynamic msgRes = _spService.DoSpNotification(requestSp);
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPost("SetupTable")]
        public IActionResult SetupTable(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = _spService.DoSpSetupTable(requestSp);
                //_logger.LogInformation($"XXX_ExecCmd===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {

                return Error(Ex);
            }
        }

        [HttpPost("Store")]
        public IActionResult Store(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = _spService.DoSpStore(requestSp);
                //_logger.LogInformation($"XXX_ExecCmd===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {

                return Error(Ex);
            }
        }

        [HttpPost("StoreType")]
        public IActionResult StoreType(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = _spService.DoSpStoreType(requestSp);
                //_logger.LogInformation($"XXX_ExecCmd===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {

                return Error(Ex);
            }
        }

        [HttpPost("StoreUser")]
        public IActionResult StoreUser(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = _spService.DoSpStoreUser(requestSp);
                //_logger.LogInformation($"XXX_ExecCmd===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {

                return Error(Ex);
            }
        }

        [HttpPost("User")]
        public IActionResult User(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = _spService.DoSpUser(requestSp);
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
