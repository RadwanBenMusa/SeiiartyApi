
using System.Data;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.StoredProcedures
{
    // ── REQUEST CLASSES ───────────────────────────────────────────

    public class GetStoreType
    {
        public int? Id { get; set; }
    }

    public class InsStoreType
    {
        public required string Descrip { get; set; }
    }

    public class UpdateStoreType
    {
        public required int Id { get; set; }
        public string? Descrip { get; set; }
    }

    public class DeleteStoreType
    {
        public required int Id { get; set; }
    }

    // ── SP CLASS ──────────────────────────────────────────────────

    public class SpStoreType
    {
        // ── GET ──────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Get(GetStoreType get)
        {
            string cmd = "SELECT * FROM StoreType WHERE Active = 1";

            if (get.Id != null) cmd += $" AND ID = {get.Id}";

            DataTable result = ExecCommand(new RequestCmd { Cmd = cmd });

            if (result.Rows.Count == 0) return "No store types found";
            return result;
        }

        // ── INSERT ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Insert(InsStoreType ins)
        {
            string cmd = $"INSERT INTO StoreType (Descrip) VALUES (N'{ins.Descrip.Trim()}')";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── UPDATE ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Update(UpdateStoreType upd)
        {
            var setParts = new List<string>();

            if (upd.Descrip != null) setParts.Add($"Descrip = N'{upd.Descrip.Trim()}'");

            if (setParts.Count == 0) return null;

            string cmd = $"UPDATE StoreType SET {string.Join(", ", setParts)} WHERE ID = {upd.Id}";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── DELETE ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Delete(DeleteStoreType q)
        {
            string cmd = $"DELETE FROM StoreType WHERE ID = {q.Id}";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }
    }
}