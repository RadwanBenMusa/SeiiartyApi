
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


    public class SpController(IConfiguration config, ISpService spService) : ControllerBase
    {
        public IConfiguration Configuration { get; } = config;

        private readonly ISpService _spService = spService;

#if DEBUG
    [HttpPost("Category")]
#else
    [HttpPost("Category"), Authorize]
#endif

        public IActionResult Category(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = _spService.DoSpCategory(requestSp);
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {

                return Error(Ex);
            }
        }
#if DEBUG
        [HttpPost("Item")]
#else
        [HttpPost("Item"), Authorize]
#endif
        
        public IActionResult Item(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = _spService.DoSpItem(requestSp);
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {

                return Error(Ex);
            }
        }
#if DEBUG
        [HttpPost("Notification")]
#else
        [HttpPost("Notification"), Authorize]
#endif
        
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
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {

                return Error(Ex);
            }
        }
#if DEBUG
        [HttpPost("Store")]
#else
        [HttpPost("Store"), Authorize]
#endif
        
        public IActionResult Store(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = _spService.DoSpStore(requestSp);
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {

                return Error(Ex);
            }
        }
#if DEBUG
        [HttpPost("StoreType")]
#else
        [HttpPost("StoreType"), Authorize]
#endif
        
        public IActionResult StoreType(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = _spService.DoSpStoreType(requestSp);
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {

                return Error(Ex);
            }
        }
#if DEBUG
        [HttpPost("StoreUser")]
#else
        [HttpPost("StoreUser"), Authorize]
#endif

        public IActionResult StoreUser(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = _spService.DoSpStoreUser(requestSp);
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {

                return Error(Ex);
            }
        }
#if DEBUG
        [HttpPost("User")]
#else
        [HttpPost("User"), Authorize]
#endif

        public IActionResult User(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = _spService.DoSpUser(requestSp);
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
