using static Seiiarty.Services.db.DbService;

namespace Seiiarty.Services.Main
{
    public interface ISpService
    {
        public dynamic DoSpCategory    (RequestSp requestSp);
        public dynamic DoSpItem        (RequestSp requestSp);
        public dynamic DoSpNotification(RequestSp requestSp);
        public dynamic DoSpSetupTable  (RequestSp requestSp);
        public dynamic DoSpStore       (RequestSp requestSp);
        public dynamic DoSpStoreType   (RequestSp requestSp);
        public dynamic DoSpStoreUser   (RequestSp requestSp);
        public dynamic DoSpUser        (RequestSp requestSp);


    }
}
