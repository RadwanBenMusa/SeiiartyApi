using System.Data;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.StoredProcedures
{
    // ══════════════════════════════════════════════════════════════════════════════
    // REQUEST CLASSES — Basket
    // ══════════════════════════════════════════════════════════════════════════════

    public class GetBasket
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public bool WithDeleted { get; set; } = false;
    }

    public class InsBasket
    {
        public required int UserId { get; set; }
    }

    public class UpdateBasket
    {
        public required int Id { get; set; }
        public DateTime? DeletionDate { get; set; }
        public bool RemoveDeletionDate { get; set; } = false;
    }

    public class DeleteBasket
    {
        public required int Id { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════════════════
    // SpBasket — handles the Basket table
    // ══════════════════════════════════════════════════════════════════════════════

    public class SpBasket
    {
        // ── GET ───────────────────────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Get(GetBasket get)
        {
            // Join User so we get the customer name alongside the basket
            const string baseSelect =
                "SELECT Basket.*, [User].FullName, [User].PhoneNumber " +
                "FROM   Basket " +
                "INNER  JOIN [User] ON Basket.UserId = [User].ID";

            var conditions = new List<string>();

            if (!get.WithDeleted)
                conditions.Add("Basket.DeletionDate IS NULL");

            if (get.Id != null) conditions.Add($"Basket.ID     = {get.Id}");
            if (get.UserId != null) conditions.Add($"Basket.UserId = {get.UserId}");

            string where = conditions.Count == 0
                ? ""
                : $"WHERE {string.Join(" AND ", conditions)}";

            string cmd = $"{baseSelect} {where}";

            DataTable result = ExecCommand(new RequestCmd { Cmd = cmd });

            if (result.Rows.Count == 0) return "No baskets found";
            return result;
        }

        // ── INSERT ────────────────────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Insert(InsBasket ins)
        {
            string now = General.ToSqlDate(DateTime.Now);

            string cmd =
                $"INSERT INTO Basket (UserId, CreationDate) " +
                $"VALUES ({ins.UserId}, '{now}'); " +
                $"SELECT SCOPE_IDENTITY() AS ID;";

            DataTable result = ExecCommand(new RequestCmd { Cmd = cmd });

            return int.Parse(result.Rows[0]["ID"].ToString()!);
        }

        // ── UPDATE ────────────────────────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Update(UpdateBasket upd)
        {
            var setParts = new List<string>();

            if (upd.DeletionDate != null) setParts.Add($"DeletionDate = '{General.ToSqlDate(upd.DeletionDate.Value)}'");
            if (upd.RemoveDeletionDate) setParts.Add("DeletionDate = NULL");

            if (setParts.Count == 0) return null;

            string cmd = $"UPDATE Basket SET {string.Join(", ", setParts)} WHERE ID = {upd.Id}";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── DELETE ────────────────────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Delete(DeleteBasket q)
        {
            string cmd = $"DELETE FROM Basket WHERE ID = {q.Id}";
            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── SOFT DELETE ───────────────────────────────────────────────────────────
        [Obsolete]
        public static dynamic? SoftDelete(int id) =>
            Update(new UpdateBasket { Id = id, DeletionDate = DateTime.Now });

        // ── RESTORE ───────────────────────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Restore(int id) =>
            Update(new UpdateBasket { Id = id, RemoveDeletionDate = true });
    }
}