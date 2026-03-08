
using System.Data;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.StoredProcedures
{
    // ── REQUEST CLASSES ───────────────────────────────────────────

    public class GetItem
    {
        public int? Id { get; set; }
        public int? CategoryId { get; set; }
        public string? Name { get; set; }
        public bool? IsFrozen { get; set; }
        public bool? IsDeleted { get; set; }
        public int? CatTypeId { get; set; }
    }

    public class InsItem
    {
        public required int ItemNo { get; set; }
        public required string Name { get; set; }
        public required string EName { get; set; }
        public required string Descrip { get; set; }
        public required int CategoryId { get; set; }
    }

    public class UpdateItem
    {
        public required int Id { get; set; }
        public int? ItemNo { get; set; }
        public string? Name { get; set; }
        public string? EName { get; set; }
        public string? Descrip { get; set; }
        public int? CategoryId { get; set; }
        public bool? Freeze { get; set; }
        public DateTime? DeletionDate { get; set; }
        public bool RemoveDeletionDate { get; set; } = false;
    }

    public class DeleteItem
    {
        public required int Id { get; set; }
    }

    // ── SP CLASS ──────────────────────────────────────────────────

    public class SpItem
    {
        // ── GET ──────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Get(GetItem get)
        {
            const string baseSelect =
                "SELECT Item.*, Category.CatNo " +
                "FROM Item " +
                "INNER JOIN Category ON Item.CategoryId = Category.ID";

            var conditions = new List<string>();

            if (get.Id != null) conditions.Add($"Item.ID         = {get.Id}");
            if (get.CategoryId != null) conditions.Add($"Item.CategoryId = {get.CategoryId}");
            if (get.Name != null) conditions.Add($"(Item.Name LIKE '%{get.Name}%' OR Item.EName LIKE '%{get.Name}%')");
            if (get.IsFrozen != null) conditions.Add($"Item.Freeze     = {(get.IsFrozen.Value ? 1 : 0)}");
            if (get.CatTypeId != null) conditions.Add($"Category.CatTypeId = {get.CatTypeId}");

            if (get.IsDeleted != null)
                conditions.Add(get.IsDeleted.Value
                    ? "Item.DeletionDate IS NOT NULL"
                    : "Item.DeletionDate IS NULL");

            string where = conditions.Count == 0
                ? ""
                : $"WHERE {string.Join(" AND ", conditions)}";

            string cmd = $"{baseSelect} {where}";

            DataTable result = ExecCommand(new RequestCmd { Cmd = cmd });

            if (result.Rows.Count == 0) return "No items found";
            return result;
        }

        // ── GET LAST ITEM NO ──────────────────────────────────────
        [Obsolete]
        public static int? GetLastItemNo(int categoryId)
        {
            string cmd = $"SELECT MAX(ItemNo) AS LastNo FROM [Item] WHERE CategoryId = {categoryId}";

            DataTable result = ExecCommand(new RequestCmd { Cmd = cmd });

            if (result.Rows.Count == 0 || result.Rows[0]["LastNo"] == DBNull.Value)
                return null;

            return int.TryParse(result.Rows[0]["LastNo"].ToString(), out int parsed) ? parsed : null;
        }

        // ── GET NEXT ITEM NO ──────────────────────────────────────
        [Obsolete]
        public static int GetNextItemNo(int categoryId, int catNo)
        {
            int? lastItemNo = GetLastItemNo(categoryId);

            if (lastItemNo == null)
                return int.Parse($"{catNo}001");

            string catNoStr = catNo.ToString();
            string lastStr = lastItemNo.Value.ToString();
            int suffix = int.Parse(lastStr.Substring(catNoStr.Length));

            return int.Parse($"{catNo}{suffix + 1}");
        }

        // ── INSERT ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Insert(InsItem ins)
        {
            string cmd =
                $"INSERT INTO [Item] (ItemNo, Name, EName, Descrip, CategoryId, Freeze) " +
                $"VALUES ({ins.ItemNo}, N'{ins.Name}', '{ins.EName}', N'{ins.Descrip}', {ins.CategoryId}, 0)";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── UPDATE ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Update(UpdateItem upd)
        {
            var setParts = new List<string>();

            if (upd.ItemNo != null) setParts.Add($"ItemNo       = {upd.ItemNo}");
            if (upd.Name != null) setParts.Add($"Name         = N'{upd.Name.Trim()}'");
            if (upd.EName != null) setParts.Add($"EName        = '{upd.EName.Trim()}'");
            if (upd.Descrip != null) setParts.Add($"Descrip      = N'{upd.Descrip.Trim()}'");
            if (upd.CategoryId != null) setParts.Add($"CategoryId   = {upd.CategoryId}");
            if (upd.Freeze != null) setParts.Add($"Freeze       = {(upd.Freeze.Value ? 1 : 0)}");

            if (upd.DeletionDate != null) setParts.Add($"DeletionDate = '{General.ToSqlDate(upd.DeletionDate.Value)}'");
            if (upd.RemoveDeletionDate) setParts.Add("DeletionDate = NULL");

            if (setParts.Count == 0) return null;

            string cmd = $"UPDATE [Item] SET {string.Join(", ", setParts)} WHERE ID = {upd.Id}";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── DELETE ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic Delete(DeleteItem q)
        {
            string cmd = $"DELETE FROM [Item] WHERE ID = {q.Id}";

            return ExecCommand(new RequestCmd { Cmd = cmd });
        }

        // ── SOFT DELETE ───────────────────────────────────────────
        [Obsolete]
        public static dynamic? SoftDelete(int id) =>
            Update(new UpdateItem { Id = id, DeletionDate = DateTime.Now });

        // ── RESTORE ───────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Restore(int id) =>
            Update(new UpdateItem { Id = id, RemoveDeletionDate = true });

        // ── FREEZE ────────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Freeze(int id) =>
            Update(new UpdateItem { Id = id, Freeze = true });

        // ── UNFREEZE ──────────────────────────────────────────────
        [Obsolete]
        public static dynamic? Unfreeze(int id) =>
            Update(new UpdateItem { Id = id, Freeze = false });
    }
}