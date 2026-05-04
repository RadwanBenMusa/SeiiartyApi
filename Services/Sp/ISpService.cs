using static Seiiarty.Services.db.DbService;

namespace Seiiarty.Services.Main
{
    public interface ISpService
    {
        public Task<dynamic> DoSpCategory(RequestSp requestSp);
        public Task<dynamic> DoSpItem(RequestSp requestSp);
        public Task<dynamic> DoSpNotification(RequestSp requestSp);
        public Task<dynamic> DoSpOrder(RequestSp requestSp);  // ← NEW
        public Task<dynamic> DoSpOrderDet(RequestSp requestSp);  // ← NEW
        public Task<dynamic> DoSpSetupTable(RequestSp requestSp);
        public Task<dynamic> DoSpStore(RequestSp requestSp);
        public Task<dynamic> DoSpStoreType(RequestSp requestSp);
        public Task<dynamic> DoSpStoreUser(RequestSp requestSp);
        public Task<dynamic> DoSpUser(RequestSp requestSp);
    }
}