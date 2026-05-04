using System.Data;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.StoredProcedures
{
    // ── ENUMS ─────────────────────────────────────────────────────

    public enum StoreStatus
    {
        UnderReview, // ExpiredDate IS NULL  + DeletionDate IS NULL
        Active,      // ExpiredDate IS NOT NULL + DeletionDate IS NULL
        Deleted,     // DeletionDate IS NOT NULL
    }

    // ── REQUEST CLASSES ───────────────────────────────────────────

    public class GetStore
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public int? StoreTypeId { get; set; }
        public StoreStatus? Status { get; set; }
        public int? UserRequestId { get; set; }
    }

    public class InsStore
    {
        public required string Name { get; set; }
        public required string Descrip { get; set; }
        public required string Location { get; set; }
        public required int StoreTypeId { get; set; }
        public required int UserRequestId { get; set; }
    }

    public class UpdateStore
    {
        public required int Id { get; set; }
        public string? Name { get; set; }
        public string? Descrip { get; set; }
        public string? Location { get; set; }
        public int? StoreTypeId { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public bool RemoveExpiredDate { get; set; } = false;
        public DateTime? DeletionDate { get; set; }
        public bool RemoveDeletionDate { get; set; } = false;
    }

    public class DeleteStore
    {
        public required int Id { get; set; }
    }

    // ── SP CLASS ──────────────────────────────────────────────────

    public class SpStore
    {
        // ── GET ──────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Get(GetStore get)
        {
            const string baseSelect =
                "SELECT Store.*, [User].FullName, [User].PhoneNumber " +
                "FROM Store " +
                "INNER JOIN [User] ON Store.UserRequestId = [User].ID";

            var conditions = new List<string>();

            if (get.Id != null) conditions.Add($"Store.ID            = {get.Id}");
            if (get.Name != null) conditions.Add($"Store.Name          LIKE N'%{get.Name}%'");
            if (get.StoreTypeId != null) conditions.Add($"Store.StoreTypeId   = {get.StoreTypeId}");
            if (get.UserRequestId != null) conditions.Add($"Store.UserRequestId = {get.UserRequestId}");

            if (get.Status != null)
            {
                switch (get.Status)
                {
                    case StoreStatus.UnderReview:
                        conditions.Add("Store.ExpiredDate  IS NULL");
                        conditions.Add("Store.DeletionDate IS NULL");
                        break;
                    case StoreStatus.Active:
                        conditions.Add("Store.ExpiredDate  IS NOT NULL");
                        conditions.Add("Store.DeletionDate IS NULL");
                        break;
                    case StoreStatus.Deleted:
                        conditions.Add("Store.DeletionDate IS NOT NULL");
                        break;
                }
            }

            string where = conditions.Count == 0
                ? ""
                : $"WHERE {string.Join(" AND ", conditions)}";

            string cmd = $"{baseSelect} {where}";

            DataTable result = ExecCommand(new RequestCmd { Cmd = cmd });

            if (result.Rows.Count == 0) return "No stores found";
            return result;
        }

        // ── INSERT ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Insert(InsStore ins)
        {
            string now = General.ToSqlDate(DateTime.Now);

            string cmd =
                $"INSERT INTO Store (Name, Descrip, Location, UserRequestId, StoreTypeId, CreationDate) " +
                $"VALUES (" +
                $"N'{ins.Name.Trim()}', " +
                $"N'{ins.Descrip.Trim()}', " +
                $"N'{ins.Location.Trim()}', " +
                $"{ins.UserRequestId}, " +
                $"{ins.StoreTypeId}, " +
                $"'{now}'" +
                $")";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── UPDATE ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Update(UpdateStore upd)
        {
            var setParts = new List<string>();

            if (upd.Name != null) setParts.Add($"Name        = N'{upd.Name.Trim()}'");
            if (upd.Descrip != null) setParts.Add($"Descrip     = N'{upd.Descrip.Trim()}'");
            if (upd.Location != null) setParts.Add($"Location    = N'{upd.Location.Trim()}'");
            if (upd.StoreTypeId != null) setParts.Add($"StoreTypeId = {upd.StoreTypeId}");

            if (upd.ExpiredDate != null) setParts.Add($"ExpiredDate  = '{General.ToSqlDate(upd.ExpiredDate.Value)}'");
            if (upd.RemoveExpiredDate) setParts.Add("ExpiredDate  = NULL");

            if (upd.DeletionDate != null) setParts.Add($"DeletionDate = '{General.ToSqlDate(upd.DeletionDate.Value)}'");
            if (upd.RemoveDeletionDate) setParts.Add("DeletionDate = NULL");

            if (setParts.Count == 0) return null;

            string cmd = $"UPDATE Store SET {string.Join(", ", setParts)} WHERE ID = {upd.Id}";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── DELETE ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Delete(DeleteStore q)
        {
            string cmd = $"DELETE FROM Store WHERE ID = {q.Id}";
            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── APPROVE ───────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Approve(int id) =>
            Update(new UpdateStore { Id = id, ExpiredDate = DateTime.Now });

        // ── REVOKE APPROVAL ───────────────────────────────────────
        [Obsolete]
        public static dynamic? RevokeApproval(int id) =>
            Update(new UpdateStore { Id = id, RemoveExpiredDate = true });

        // ── SOFT DELETE ───────────────────────────────────────────
        [Obsolete]
        public static dynamic? SoftDelete(int id) =>
            Update(new UpdateStore { Id = id, DeletionDate = DateTime.Now });

        // ── RESTORE ───────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Restore(int id) =>
            Update(new UpdateStore { Id = id, RemoveDeletionDate = true });
    }
}