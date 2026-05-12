using System.Data;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.StoredProcedures
{
    // ══════════════════════════════════════════════════════════════════════════════
    // ORDER STATUS ENUM
    // Matches the Status table in the DB
    // ══════════════════════════════════════════════════════════════════════════════

    public enum OrderStatus
    {
        Pending = 1,  // Waiting for store to confirm
        Confirmed = 2,  // Store confirmed the order
        Ready = 3,  // Order is ready for pickup / delivery
        Delivered = 4,  // Order delivered to customer
        Cancelled = 5,  // Order was cancelled
    }

    // ══════════════════════════════════════════════════════════════════════════════
    // REQUEST CLASSES — Order
    // ══════════════════════════════════════════════════════════════════════════════

    public class GetOrder
    {
        public int? Id { get; set; }
        public int? CreationUserId { get; set; }   // filter by customer
        public OrderStatus? Status { get; set; }   // filter by status
        public bool WithDeleted { get; set; } = false;
        public DateTime? FromDate { get; set; }   // filter by date range
        public DateTime? ToDate { get; set; }
    }

    public class InsOrder
    {
        public required int CreationUserId { get; set; }
        public required int StatusId { get; set; } = (int)OrderStatus.Pending;
    }

    public class UpdateOrder
    {
        public required int Id { get; set; }
        public int? StatusId { get; set; }   // change order status
        public DateTime? DeletionDate { get; set; }
        public bool RemoveDeletionDate { get; set; } = false;
    }

    public class DeleteOrder
    {
        public required int Id { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════════════════
    // REQUEST CLASSES — OrderDet (order detail / line items)
    // ══════════════════════════════════════════════════════════════════════════════

    public class GetOrderDet
    {
        public int? Id { get; set; }
        public int? ItemId { get; set; }
        public int? OrderId { get; set; }
        public int? BasketId { get; set; }
    }   

    public class InsOrderDet
    {
        public  int? ItemId { get; set; }
        public  int? OrderId { get; set; }
        public  int? Quantity { get; set; }
        public int? BasketId { get; set; }

    }

    public class UpdateOrderDet
    {
        public required int Id { get; set; }
        public int? ItemId { get; set; }
        public int? Quantity { get; set; }
    }

    public class DeleteOrderDet
    {
        public required int Id { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════════════════
    // SpOrder — handles the Order table
    // ══════════════════════════════════════════════════════════════════════════════

    public class SpOrder
    {
        // ── GET ───────────────────────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Get(GetOrder get)
        {
            // Join with User so we get customer name, and Status for the label
            const string baseSelect =
                "SELECT [Order].*, " +
                "       [User].FullName, [User].PhoneNumber, " +
                "       [Status].Description AS StatusDescription " +
                "FROM   [Order] " +
                "INNER  JOIN [User]   ON [Order].CreationUserId = [User].ID " +
                "INNER  JOIN [Status] ON [Order].StatusId       = [Status].ID";

            var conditions = new List<string>();

            // Only show non-deleted orders by default
            if (!get.WithDeleted)
                conditions.Add("[Order].DeletionDate IS NULL");

            if (get.Id != null)
                conditions.Add($"[Order].ID = {get.Id}");

            if (get.CreationUserId != null)
                conditions.Add($"[Order].CreationUserId = {get.CreationUserId}");

            if (get.Status != null)
                conditions.Add($"[Order].StatusId = {(int)get.Status}");

            // Date range filter (useful for reports / admin screens)
            if (get.FromDate != null)
                conditions.Add($"[Order].CreationDate >= '{General.ToSqlDate(get.FromDate.Value)}'");

            if (get.ToDate != null)
                conditions.Add($"[Order].CreationDate <= '{General.ToSqlDate(get.ToDate.Value)}'");

            string where = conditions.Count == 0
                ? ""
                : $"WHERE {string.Join(" AND ", conditions)}";

            string cmd = $"{baseSelect} {where} ORDER BY [Order].CreationDate DESC";

            DataTable result = ExecCommand(new RequestCmd { Cmd = cmd });

            if (result.Rows.Count == 0) return "No orders found";
            return result;
        }

        // ── INSERT ────────────────────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Insert(InsOrder ins)
        {
            string now = General.ToSqlDate(DateTime.Now);

            string cmd =
                $"INSERT INTO [Order] (CreationUserId, StatusId, CreationDate) " +
                $"VALUES ({ins.CreationUserId}, {ins.StatusId}, '{now}'); " +
                $"SELECT SCOPE_IDENTITY() AS ID;";

            DataTable result = ExecCommand(new RequestCmd { Cmd = cmd });

            return int.Parse(result.Rows[0]["ID"].ToString()!);
        }

        // ── UPDATE ────────────────────────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Update(UpdateOrder upd)
        {
            var setParts = new List<string>();

            if (upd.StatusId != null) setParts.Add($"StatusId     = {upd.StatusId}");
            if (upd.DeletionDate != null) setParts.Add($"DeletionDate = '{General.ToSqlDate(upd.DeletionDate.Value)}'");
            if (upd.RemoveDeletionDate) setParts.Add("DeletionDate = NULL");

            if (setParts.Count == 0) return null;

            string cmd = $"UPDATE [Order] SET {string.Join(", ", setParts)} WHERE ID = {upd.Id}";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── DELETE ────────────────────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Delete(DeleteOrder q)
        {
            string cmd = $"DELETE FROM [Order] WHERE ID = {q.Id}";
            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── SHORTCUT HELPERS (same pattern as SpStore.Approve / SoftDelete) ───────

        [Obsolete]
        public static dynamic? ChangeStatus(int id, OrderStatus status) =>
            Update(new UpdateOrder { Id = id, StatusId = (int)status });

        [Obsolete]
        public static dynamic? SoftDelete(int id) =>
            Update(new UpdateOrder { Id = id, DeletionDate = DateTime.Now });

        [Obsolete]
        public static dynamic? Restore(int id) =>
            Update(new UpdateOrder { Id = id, RemoveDeletionDate = true });
    }

    // ══════════════════════════════════════════════════════════════════════════════
    // SpOrderDet — handles the OrderDet table (the line items of an order)
    // ══════════════════════════════════════════════════════════════════════════════

    public class SpOrderDet
    {
        // ── GET ───────────────────────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Get(GetOrderDet get)
        {
            // Join with Item so the caller gets the item name too
            const string baseSelect =
                "SELECT OrderDet.*, " +
                "       Item.Name, Item.EName " +
                "FROM   OrderDet " +
                "INNER  JOIN Item ON OrderDet.ItemId = Item.ID";

            var conditions = new List<string>();

            if (get.Id != null) conditions.Add($"OrderDet.ID     = {get.Id}");
            if (get.ItemId != null) conditions.Add($"OrderDet.ItemId = {get.ItemId}");
            if (get.OrderId != null) conditions.Add($"OrderDet.OrderId = {get.OrderId}");

            string where = conditions.Count == 0
                ? ""
                : $"WHERE {string.Join(" AND ", conditions)}";

            string cmd = $"{baseSelect} {where}";

            DataTable result = ExecCommand(new RequestCmd { Cmd = cmd });

            if (result.Rows.Count == 0) return "No order details found";
            return result;
        }

        // ── INSERT ────────────────────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Insert(InsOrderDet ins)
        {
            string cmd =
                $"INSERT INTO OrderDet (ItemId, OrderId, Quantity) " +
                $"VALUES ({ins.ItemId}, {ins.OrderId}, {ins.Quantity}); " +
                $"SELECT SCOPE_IDENTITY() AS ID;";

            DataTable result = ExecCommand(new RequestCmd { Cmd = cmd });

            return int.Parse(result.Rows[0]["ID"].ToString()!);
        }

        // ── UPDATE ────────────────────────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Update(UpdateOrderDet upd)
        {
            var setParts = new List<string>();

            if (upd.ItemId != null) setParts.Add($"ItemId   = {upd.ItemId}");
            if (upd.Quantity != null) setParts.Add($"Quantity = {upd.Quantity}");

            if (setParts.Count == 0) return null;

            string cmd = $"UPDATE OrderDet SET {string.Join(", ", setParts)} WHERE ID = {upd.Id}";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── DELETE ────────────────────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Delete(DeleteOrderDet q)
        {
            string cmd = $"DELETE FROM OrderDet WHERE ID = {q.Id}";
            return ExecCommand(new RequestCmd { Cmd = cmd });
        }
    }
}