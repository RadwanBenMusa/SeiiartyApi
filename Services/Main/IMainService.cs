using FirebaseAdmin.Messaging;
using TamaApi.clsMod;
using TamaApi.Controllers;
using static TamaApi.Controllers.MainController;
using static TamaApi.Services.db.DbService;

namespace TamaApi.Services.Main
{
    public interface IMainService
    {
        public dynamic ExecStoredProcedure(Request request);
        public dynamic ExecCmd(RequestCmd requestCmd);
        public dynamic DoSomeThings(Request request);
        public Task<dynamic> Notification(ApiNotification apiNotification, ILogger<MainController> _logger);

    }
}
