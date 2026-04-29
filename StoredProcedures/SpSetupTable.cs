using System.Data;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.StoredProcedures
{

    public class SpSetupTable
    {
        // ── GET ──────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Get()
        {
            string cmd = "SELECT * FROM SetupTable";

            DataTable result = ExecCommand(new RequestCmd { Cmd = cmd });

            if (result.Rows.Count == 0) return "No setup table found";
            return result;
        }
    }
}