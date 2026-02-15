using TamaApi.Controllers;

namespace TamaApi.Services.PaymentCallBack
{
    public interface IPaymentCallBackService
    {
        public Task<dynamic> PaymentCallBack(ClsPaymentCallBack callBack, ILogger<MainController> _logger);
    }
}
