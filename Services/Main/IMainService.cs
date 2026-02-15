using FirebaseAdmin.Messaging;
using TamaApi.clsMod;
using TamaApi.Controllers;
using static TamaApi.Controllers.MainController;
using static TamaApi.Services.db.DbService;

namespace TamaApi.Services.Main
{
    public interface IMainService
    {
        public dynamic GetSetupTable();
        public dynamic GetTable(GetTableCP getTableCP);
        public dynamic ExecStoredProcedure(TamaRequest tamaRequest);
        public dynamic ExecCmd(TamaRequestCmd tamaRequestCmd);
        public dynamic DoSomeThings(TamaRequest tamaRequest);
        public int GetRemoteContractCoId(string PhoneNumber,int clinicId);
        public Task<dynamic> GetUrlPay(ClsUrlPay clsUrlPay, ILogger<MainController> _logger);
        public Task<dynamic> Notification(ApiNotification apiNotification, ILogger<MainController> _logger);
        public Task<dynamic> SmsLinkAnalysisRes(ClsSmsLinkAnalysisRes clsSmsLinkAnalysisRes, ILogger<MainController> logger);
        public Task<dynamic> WhatsAppMsgAnalysisRes(int ClinicNo, string FileName, string? OtherInfo, string customerPhoneNo, ILogger<MainController> logger);
        public Task<dynamic> WhatsAppMsg(ClsWhatsAppMsg clsWhatsAppMsg, ILogger<MainController> _logger);

    }
}
