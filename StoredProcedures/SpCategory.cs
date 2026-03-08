
using System.Data;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.StoredProcedures
{
    // ── REQUEST CLASSES ───────────────────────────────────────────

    public class GetCategory
    {
        public int? Id { get; set; }
        public int? CatTypeId { get; set; }
    }

    public class InsCategory
    {
        public required int CatNo { get; set; }
        public required string Descrip { get; set; }
        public required int CatTypeId { get; set; }
    }

    public class UpdateCategory
    {
        public required int Id { get; set; }
        public int? CatNo { get; set; }
        public string? Descrip { get; set; }
        public int? CatTypeId { get; set; }
        public bool? Freeze { get; set; }
        public DateTime? DeletionDate { get; set; }
        public bool RemoveDeletionDate { get; set; } = false;
    }

    public class DeleteCategory
    {
        public required int Id { get; set; }
    }

    // ── SP CLASS ──────────────────────────────────────────────────

    public class SpCategory
    {
        // ── GET ──────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Get(GetCategory get)
        {
            string cmd = "SELECT * FROM [Category]";

            var conditions = new List<string>();

            if (get.CatTypeId != null) conditions.Add($"CatTypeId = {get.CatTypeId}");
            if (get.Id != null) conditions.Add($"ID        = {get.Id}");

            if (conditions.Count > 0)
                cmd += $" WHERE {string.Join(" AND ", conditions)}";

            DataTable result = ExecCommand(new RequestCmd { Cmd = cmd });

            if (result.Rows.Count == 0) return "No categories found";
            return result;
        }

        // ── INSERT ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Insert(InsCategory ins)
        {
            string cmd =
                $"INSERT INTO [Category] (CatNo, Descrip, CatTypeId, Freeze) " +
                $"VALUES ({ins.CatNo}, N'{ins.Descrip.Trim()}', {ins.CatTypeId}, 0)";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── UPDATE ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Update(UpdateCategory upd)
        {
            var setParts = new List<string>();

            if (upd.CatNo != null) setParts.Add($"CatNo      = {upd.CatNo}");
            if (upd.Descrip != null) setParts.Add($"Descrip    = N'{upd.Descrip.Trim()}'");
            if (upd.CatTypeId != null) setParts.Add($"CatTypeId  = {upd.CatTypeId}");
            if (upd.Freeze != null) setParts.Add($"Freeze     = {(upd.Freeze.Value ? 1 : 0)}");

            if (upd.DeletionDate != null) setParts.Add($"DeletionDate = '{General.ToSqlDate(upd.DeletionDate.Value)}'");
            if (upd.RemoveDeletionDate) setParts.Add("DeletionDate = NULL");

            if (setParts.Count == 0) return null;

            string cmd = $"UPDATE [Category] SET {string.Join(", ", setParts)} WHERE ID = {upd.Id}";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── DELETE ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Delete(DeleteCategory q)
        {
            string cmd = $"DELETE FROM [Category] WHERE ID = {q.Id}";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── SOFT DELETE ───────────────────────────────────────────
        [Obsolete]
        public static dynamic? SoftDelete(int id) =>
            Update(new UpdateCategory { Id = id, DeletionDate = DateTime.Now });

        // ── RESTORE ───────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Restore(int id) =>
            Update(new UpdateCategory { Id = id, RemoveDeletionDate = true });

        // ── FREEZE ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Freeze(int id) =>
            Update(new UpdateCategory { Id = id, Freeze = true });

        // ── UNFREEZE ──────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Unfreeze(int id) =>
            Update(new UpdateCategory { Id = id, Freeze = false });
    }
}