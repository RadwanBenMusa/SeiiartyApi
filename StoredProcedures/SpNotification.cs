using System.Data;
using Seiiarty;
using Seiiarty.clsMod;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.StoredProcedures
{
    // ── REQUEST CLASSES ───────────────────────────────────────────

    public class InsNotification
    {
        public required int NotificationTypeId { get; set; }
        public int? UserId { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public string? Data { get; set; }
    }

    public class GetNotification
    {
        public int? Id { get; set; }
        public int? NotificationTypeId { get; set; }
        public int? UserId { get; set; }
        public bool? Readed { get; set; }
    }

    public class UpdateNotification
    {
        public required int Id { get; set; }
        public int? NotificationTypeId { get; set; }
        public int? UserId { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public string? Data { get; set; }
        public bool? Readed { get; set; }
    }

    // ── SP CLASS ──────────────────────────────────────────────────

    public class SpNotification
    {
        // ── GET ──────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Get(GetNotification get)
        {
            var conditions = new List<string>();

            if (get.Id != null)
                conditions.Add($"[Notification].ID = {get.Id}");

            if (get.NotificationTypeId != null)
                conditions.Add($"[Notification].NotificationTypeId = {get.NotificationTypeId}");

            if (get.UserId != null)
                conditions.Add($"[Notification].UserId = {get.UserId}");

            if (get.Readed != null)
                conditions.Add($"[Notification].Readed = {(get.Readed.Value ? 1 : 0)}");

            string where = conditions.Count == 0
                ? ""
                : $"WHERE {string.Join(" AND ", conditions)}";

            string cmd = $"SELECT * FROM [Notification] {where}";

            DataTable result = ExecCommand(new RequestCmd { Cmd = cmd });

            if (result.Rows.Count == 0) return "No notifications found";
            return result;
        }

        // ── INSERT ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Insert(InsNotification ins)
        {
            string now = General.ToSqlDate(DateTime.Now);

            string cmd =
                $"INSERT INTO [Notification] (NotificationTypeId, UserId, Title, Body, Data, Readed, CreationDate) " +
                $"VALUES ({ins.NotificationTypeId}, {(ins.UserId.HasValue ? ins.UserId.ToString() : "NULL")}, " +
                $"'{ins.Title}', '{ins.Body}', '{ins.Data}', 0, '{now}')";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── UPDATE ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Update(UpdateNotification upd)
        {
            var setParts = new List<string>();

            if (upd.NotificationTypeId != null) setParts.Add($"NotificationTypeId = {upd.NotificationTypeId}");
            if (upd.UserId != null) setParts.Add($"UserId             = {upd.UserId}");
            if (upd.Title != null) setParts.Add($"Title              = '{upd.Title}'");
            if (upd.Body != null) setParts.Add($"Body                = '{upd.Body}'");
            if (upd.Data != null) setParts.Add($"Data                = '{upd.Data}'");
            if (upd.Readed != null) setParts.Add($"Readed             = {(upd.Readed.Value ? 1 : 0)}");

            if (setParts.Count == 0) return null;

            string cmd = $"UPDATE [Notification] SET {string.Join(", ", setParts)} WHERE ID = {upd.Id}";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }
    }
}