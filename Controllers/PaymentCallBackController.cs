using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TamaApi.Services.PaymentCallBack;
using static TamaApi.Services.db.DbService;


namespace TamaApi.Controllers
{
    public class TDSPPaymentCallBack
    {
        public string? result { get; set; }
        public string? amount { get; set; }
        public string? store_id { get; set; }
        public string? our_ref { get; set; }
        public string? payment_method { get; set; }
        public string? customer_phone { get; set; }
        public string? custom_ref { get; set; }
        public string? message { get; set; }
        public List<object>? errors { get; set; }
    }

    public class ClsPaymentCallBack
    {
        public string? result { get; set; }
        public string? amount { get; set; }
        public string? store_id { get; set; }
        public string? our_ref { get; set; }
        public string? payment_method { get; set; }
        public string? customer_phone { get; set; }
        public string? custom_ref { get; set; }
        public string? message { get; set; }
        public List<object>? errors { get; set; }
    }

    [Route("[controller]")]
    [ApiController]
    public class PaymentCallBackController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        private readonly ILogger<MainController> _logger;

        private readonly IPaymentCallBackService _paymentCallBackService;

        public PaymentCallBackController(IConfiguration config, ILogger<MainController> logger, IPaymentCallBackService paymentCallBackService)
        {
            _logger = logger;
            Configuration = config;
            _paymentCallBackService = paymentCallBackService;
        }

        [HttpPost("TDSPPaymentCallBack")]
        public IActionResult TDSPPaymentCallBack(ClsPaymentCallBack callBack)
        {
            try
            {
                _logger.LogInformation($"XXX_TDSPPaymentCallBack ()()Tadawil()() ===> callBack = {JsonConvert.SerializeObject(callBack)}");
                ////التأكد من ان ليس هناك مشاكل من تداول
                //if (callBack == null)
                //    return;

                //if (callBack.errors != null)
                //    if (callBack.errors.Count > 0)
                //        return;
                //if (callBack.custom_ref == null)
                //    return;
                dynamic msgRes;
                if (callBack.result == "success") { 
                    msgRes = _paymentCallBackService.PaymentCallBack(callBack, _logger);

                    //_logger.LogInformation($"XXX_TDSPPaymentCallBack===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                    return StatusCode(StatusCodes.Status200OK, msgRes.Result);
                }
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_TDSPPaymentCallBack ===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }

#if DEBUG
    [HttpPost("PaymentCallBack")]
#else
        [HttpPost("PaymentCallBack"), Authorize]
#endif
        public IActionResult PaymentCallBack(ClsPaymentCallBack callBack)
        {
            try
            {
                _logger.LogInformation($"XXX_PaymentCallBack ()()()() ===> callBack = {JsonConvert.SerializeObject(callBack)}");
                dynamic msgRes = _paymentCallBackService.PaymentCallBack(callBack, _logger);

                //_logger.LogInformation($"XXX_PaymentCallBack===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes.Result);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_PaymentCallBack ===> Error = {Ex.Message}");
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
