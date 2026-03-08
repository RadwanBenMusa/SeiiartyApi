
using System.Data;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.StoredProcedures
{
    // ── REQUEST CLASSES ───────────────────────────────────────────

    public class GetStoreUser
    {
        public int? Id { get; set; }
        public int? StoreId { get; set; }
        public int? UserId { get; set; }
        public bool WithDeleted { get; set; } = false;
    }

    public class InsStoreUser
    {
        public required int StoreId { get; set; }
        public required int UserId { get; set; }
    }

    public class UpdateStoreUser
    {
        public required int Id { get; set; }
        public DateTime? DeletionDate { get; set; }
        public bool RemoveDeletionDate { get; set; } = false;
    }

    public class DeleteStoreUser
    {
        public required int Id { get; set; }
    }

    // ── SP CLASS ──────────────────────────────────────────────────

    public class SpStoreUser
    {
        // ── GET ──────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Get(GetStoreUser get)
        {
            const string baseSelect =
                "SELECT StoreUser.*, " +
                "[User].FullName, [User].PhoneNumber, " +
                "Store.Name AS StoreName " +
                "FROM StoreUser " +
                "INNER JOIN [User] ON StoreUser.UserId  = [User].ID " +
                "INNER JOIN Store  ON StoreUser.StoreId = Store.ID";

            var conditions = new List<string>();

            if (get.Id != null) conditions.Add($"StoreUser.ID      = {get.Id}");
            if (get.StoreId != null) conditions.Add($"StoreUser.StoreId = {get.StoreId}");
            if (get.UserId != null) conditions.Add($"StoreUser.UserId  = {get.UserId}");

            if (!get.WithDeleted) conditions.Add("StoreUser.DeletionDate IS NULL");

            string where = conditions.Count == 0
                ? ""
                : $"WHERE {string.Join(" AND ", conditions)}";

            string cmd = $"{baseSelect} {where}";

            DataTable result = ExecCommand(new RequestCmd { Cmd = cmd });

            if (result.Rows.Count == 0) return "No store users found";
            return result;
        }

        // ── INSERT ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Insert(InsStoreUser ins)
        {
            string now = General.ToSqlDate(DateTime.Now);

            string cmd =
                $"INSERT INTO StoreUser (StoreId, UserId, CreationDate) " +
                $"VALUES ({ins.StoreId}, {ins.UserId}, '{now}')";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── UPDATE ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Update(UpdateStoreUser upd)
        {
            var setParts = new List<string>();

            if (upd.DeletionDate != null) setParts.Add($"DeletionDate = '{General.ToSqlDate(upd.DeletionDate.Value)}'");
            if (upd.RemoveDeletionDate) setParts.Add("DeletionDate = NULL");

            if (setParts.Count == 0) return null;

            string cmd = $"UPDATE StoreUser SET {string.Join(", ", setParts)} WHERE ID = {upd.Id}";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── DELETE ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Delete(DeleteStoreUser q)
        {
            string cmd = $"DELETE FROM StoreUser WHERE ID = {q.Id}";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── SOFT DELETE ───────────────────────────────────────────
        [Obsolete]
        public static dynamic? SoftDelete(int id) =>
            Update(new UpdateStoreUser { Id = id, DeletionDate = DateTime.Now });

        // ── RESTORE ───────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Restore(int id) =>
            Update(new UpdateStoreUser { Id = id, RemoveDeletionDate = true });
    }
}