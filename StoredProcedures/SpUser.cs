using System.Data;
using Seiiarty;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.StoredProcedures
{
    // ── REQUEST CLASSES ───────────────────────────────────────────

    public class InsUser
    {
        public required string FullName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
        public string? PhoneToken { get; set; }
    }

    public class GetUser
    {
        public int? Id { get; set; }
        public string? PhoneNo { get; set; }
        public bool? Admin { get; set; }
        public int? ExcludeStoreId { get; set; }
        public bool? HasStore { get; set; }
        public bool WithDeletionDate { get; set; } = false;
    }

    public class UpdateUser
    {
        public required int Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public string? Password { get; set; }
        public string? FirebaseToken { get; set; }
        public string? PhoneToken { get; set; }
        public DateTime? LastLogin { get; set; }
        // ── FIX: nullable so callers that don't pass Admin leave the column untouched.
        // Previously bool (default false) meant Update() always skipped the column
        // and could never set Admin = 0 intentionally.
        public bool? Admin { get; set; }
        public DateTime? DeletionDate { get; set; }
        public bool RemoveDeletionDate { get; set; } = false;
    }

    public class DeleteUser
    {
        public required int Id { get; set; }
    }

    // ── SP CLASS ──────────────────────────────────────────────────

    public class SpUser
    {
        // ── GET ──────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Get(GetUser get)
        {
            var conditions = new List<string>();

            if (!get.WithDeletionDate)
            {
                conditions.Add("[User].DeletionDate IS NULL");
            }

            if (get.Id != null)
            {
                conditions.Add($"[User].ID = {get.Id}");
            }

            if (get.PhoneNo != null)
            {
                string formatted = General.FormatLibyanPhone(get.PhoneNo);
                conditions.Add($"[User].PhoneNumber = '{formatted}'");
            }

            if (get.Admin != null)
            {
                conditions.Add($"[User].Admin = {(get.Admin.Value ? 1 : 0)}");
            }
            if (get.HasStore == true)
            {
                conditions.Add($"EXISTS (SELECT 1 FROM StoreUser INNER JOIN Store ON StoreUser.StoreId = Store.ID WHERE StoreUser.UserId = [User].ID AND StoreUser.DeletionDate IS NULL AND Store.DeletionDate IS NULL AND (Store.ExpiredDate IS NULL OR Store.ExpiredDate > GETDATE()))");
            }
            if (get.ExcludeStoreId != null)
            {
                conditions.Add($"[User].ID NOT IN (SELECT UserId FROM StoreUser WHERE StoreId = {get.ExcludeStoreId} AND DeletionDate IS NULL)");
            }

            string where = conditions.Count == 0
                ? ""
                : $"WHERE {string.Join(" AND ", conditions)}";

            string cmd = $"SELECT * FROM [User] {where}";

            DataTable result = ExecCommand(new RequestCmd { Cmd = cmd });

            if (result.Rows.Count == 0) return "No users found";
            return result;
        }

        // ── INSERT ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Insert(InsUser ins)
        {
            string now = General.ToSqlDate(DateTime.Now);

            string cmd =
                $"INSERT INTO [User] (FullName, PhoneNumber, Password, PhoneToken, CreationDate) " +
                $"VALUES ('{ins.FullName}', '{ins.PhoneNumber}', '{ins.Password}', '{ins.PhoneToken}', '{now}'); " +
                $"SELECT SCOPE_IDENTITY() AS ID;";
            DataTable result = ExecCommand(new RequestCmd { Cmd = cmd });

            return int.Parse(result.Rows[0]["ID"].ToString()!);
        }

        // ── UPDATE ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Update(UpdateUser upd)
        {
            var setParts = new List<string>();

            if (upd.PhoneNumber != null) setParts.Add($"PhoneNumber   = '{upd.PhoneNumber}'");
            if (upd.FullName != null) setParts.Add($"FullName      = '{upd.FullName.Trim()}'");
            if (upd.Password != null) setParts.Add($"Password      = '{upd.Password}'");
            if (upd.FirebaseToken != null) setParts.Add($"FirebaseToken = '{upd.FirebaseToken}'");
            if (upd.PhoneToken != null) setParts.Add($"PhoneToken    = '{upd.PhoneToken}'");
            if (upd.LastLogin != null) setParts.Add($"LastLogin     = '{General.ToSqlDate(upd.LastLogin.Value)}'");
            // ── FIX: only touch the Admin column when the caller explicitly passes a value.
            // HasValue = true means the caller sent true or false on purpose.
            if (upd.Admin.HasValue) setParts.Add($"Admin         = {(upd.Admin.Value ? 1 : 0)}");
            if (upd.DeletionDate != null) setParts.Add($"DeletionDate  = '{General.ToSqlDate(upd.DeletionDate.Value)}'");
            if (upd.RemoveDeletionDate) setParts.Add("DeletionDate  = NULL");

            if (setParts.Count == 0) return null;

            string cmd = $"UPDATE [User] SET {string.Join(", ", setParts)} WHERE ID = {upd.Id}";

            dynamic res = ExecCommand(new RequestCmd { Cmd = cmd });

            return res;
        }

        // ── DELETE ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Delete(DeleteUser q)
        {
            string cmd = $"DELETE FROM [User] WHERE ID = {q.Id}";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }
    }
}