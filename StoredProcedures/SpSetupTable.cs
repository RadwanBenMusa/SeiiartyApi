using System.Data;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.StoredProcedures
{
    // ── REQUEST CLASSES ───────────────────────────────────────────

    public class GetSetupTable
    {
        // no filters needed for now
    }

    // ── SP CLASS ──────────────────────────────────────────────────

    public class SpSetupTable
    {
        // ── GET ──────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Get(GetSetupTable get)
        {
            string cmd = "SELECT * FROM SetupTable";

            DataTable result = ExecCommand(new RequestCmd { Cmd = cmd });

            if (result.Rows.Count == 0) return "No setup table found";
            return result;
        }
    }
}