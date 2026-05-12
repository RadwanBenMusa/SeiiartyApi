using FirebaseAdmin.Messaging;
using Seiiarty.clsMod;
using Seiiarty.Controllers;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.Services.Main
{
    public interface IMainService
    {
        public dynamic ExecCmd(RequestCmd requestCmd);
        public dynamic DoSomeThings(Request request);

    }
}
