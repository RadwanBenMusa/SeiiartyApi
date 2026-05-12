
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
        [HttpPost("Basket")]
#else
        [HttpPost("Basket"), Authorize]
#endif

        public IActionResult Basket(RequestSp requestSp)
        {
            try
            {
                dynamic msgRes = _spService.DoSpBasket(requestSp);
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

#if DEBUG
        [HttpPost("Category")]
#else
        [HttpPost("Category"), Authorize]
#endif

        public async Task<IActionResult> Category(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = await _spService.DoSpCategory(requestSp);
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

        public async Task<IActionResult> Item(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = await _spService.DoSpItem(requestSp);
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

        public async Task<IActionResult> Notification(RequestSp requestSp)
        {
            try
            {
                dynamic msgRes = await _spService.DoSpNotification(requestSp);
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

#if DEBUG
        [HttpPost("Order")]
#else
        [HttpPost("Notification"), Authorize]
#endif       
        public async Task<IActionResult> Order(RequestSp requestSp)
        {
            try
            {
                // Await the asynchronous service call
                dynamic msgRes = await _spService.DoSpOrder(requestSp);
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

#if DEBUG
        [HttpPost("OrderDet")]
#else
        [HttpPost("OrderDet"), Authorize]
#endif
        public async Task<IActionResult> OrderDet(RequestSp requestSp)
        {
            try
            {
                dynamic msgRes = await _spService.DoSpOrderDet(requestSp);
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }


        [HttpPost("SetupTable")]
        public async Task<IActionResult> SetupTable(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = await _spService.DoSpSetupTable(requestSp);
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

        public async Task<IActionResult> Store(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = await _spService.DoSpStore(requestSp);
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

        public async Task<IActionResult> StoreType(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = await _spService.DoSpStoreType(requestSp);
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

        public async Task<IActionResult> StoreUser(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = await _spService.DoSpStoreUser(requestSp);
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

        public async Task<IActionResult> User(RequestSp requestSp)
        {
            try
            {

                dynamic msgRes = await _spService.DoSpUser(requestSp);
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