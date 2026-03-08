using Seiiarty.Services.Main;
using Seiiarty.StoredProcedures;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.Services.Sp
{
    public class SpService() : ISpService
    {
        private static dynamic? GetPara(List<Para> paras, string name, dynamic? fallback = null)
        {
            string? raw = paras.FirstOrDefault(p =>
                string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase))?.Value;

            if (raw is null || string.IsNullOrWhiteSpace(raw.ToString()))
                return fallback;

            return raw;
        }

        /// <summary>Returns int? — null if param is missing or unparseable.</summary>
        private static int? GetParaInt(List<Para> paras, string name)
        {
            dynamic? val = GetPara(paras, name);
            if (val is null) return null;
            return int.TryParse(val.ToString(), out int parsed) ? parsed : null;
        }


        private static bool? GetParaBool(List<Para> paras, string name)
        {
            dynamic? val = GetPara(paras, name);
            if (val is null) return null;
            return bool.TryParse(val.ToString(), out bool parsed) ? parsed : null;
        }

        /// <summary>Returns DateTime? — null if param is missing or unparseable.</summary>
        private static DateTime? GetParaDate(List<Para> paras, string name)
        {
            dynamic? val = GetPara(paras, name);
            if (val is null) return null;
            return DateTime.TryParse(val.ToString(), out DateTime parsed) ? parsed : null;
        }

        
        [Obsolete]
        public dynamic DoSpCategory(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];
            switch (requestSp.Method)
            {
                case "Get":
                    result = SpCategory.Get(new GetCategory
                    {
                        Id = GetParaInt(p, "Id"),
                        CatTypeId = GetParaInt(p, "CatTypeId"),
                    });
                    break;
                case "Insert":
                    result = SpCategory.Insert(new InsCategory
                    {
                        CatNo = GetParaInt(p, "CatNo") ?? 0,
                        Descrip = GetPara(p, "Descrip") ?? "",
                        CatTypeId = GetParaInt(p, "CatTypeId") ?? 0,
                    });
                    break;
                case "Update":
                    result = SpCategory.Update(new UpdateCategory
                    {
                        Id = GetParaInt(p, "Id") ?? 0,
                        CatNo = GetParaInt(p, "CatNo"),
                        Descrip = GetPara(p, "Descrip"),
                        CatTypeId = GetParaInt(p, "CatTypeId"),
                        Freeze = GetParaBool(p, "Freeze"),
                        DeletionDate = GetParaDate(p, "DeletionDate"),
                        RemoveDeletionDate = GetParaBool(p, "RemoveDeletionDate") ?? false,
                    });
                    break;
                case "Delete":
                    result = SpCategory.Delete(new DeleteCategory
                    {
                        Id = GetParaInt(p, "Id") ?? 0,
                    });
                    break;
                case "SoftDelete":
                    result = SpCategory.SoftDelete(GetParaInt(p, "Id") ?? 0);
                    break;
                case "Restore":
                    result = SpCategory.Restore(GetParaInt(p, "Id") ?? 0);
                    break;
                case "Freeze":
                    result = SpCategory.Freeze(GetParaInt(p, "Id") ?? 0);
                    break;
                case "Unfreeze":
                    result = SpCategory.Unfreeze(GetParaInt(p, "Id") ?? 0);
                    break;
            }
            return result;
        }


        [Obsolete]
        public dynamic DoSpItem(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];
            switch (requestSp.Method)
            {
                case "Get":
                    result = SpItem.Get(new GetItem
                    {
                        Id = GetParaInt(p, "Id"),
                        CategoryId = GetParaInt(p, "CategoryId"),
                        Name = GetPara(p, "Name"),
                        IsFrozen = GetParaBool(p, "IsFrozen"),
                        IsDeleted = GetParaBool(p, "IsDeleted"),
                        CatTypeId = GetParaInt(p, "CatTypeId"),
                    });
                    break;
                case "GetNextItemNo":
                    result = SpItem.GetNextItemNo(
                        categoryId: GetParaInt(p, "CategoryId") ?? 0,
                        catNo: GetParaInt(p, "CatNo") ?? 0
                    );
                    break;
                case "GetLastItemNo":
                    result = SpItem.GetLastItemNo(
                        categoryId: GetParaInt(p, "CategoryId") ?? 0
                    );
                    break;
                case "Insert":
                    result = SpItem.Insert(new InsItem
                    {
                        ItemNo = GetParaInt(p, "ItemNo") ?? 0,
                        Name = GetPara(p, "Name") ?? "",
                        EName = GetPara(p, "EName") ?? "",
                        Descrip = GetPara(p, "Descrip") ?? "",
                        CategoryId = GetParaInt(p, "CategoryId") ?? 0,
                    });
                    break;
                case "Update":
                    result = SpItem.Update(new UpdateItem
                    {
                        Id = GetParaInt(p, "Id") ?? 0,
                        ItemNo = GetParaInt(p, "ItemNo"),
                        Name = GetPara(p, "Name"),
                        EName = GetPara(p, "EName"),
                        Descrip = GetPara(p, "Descrip"),
                        CategoryId = GetParaInt(p, "CategoryId"),
                        Freeze = GetParaBool(p, "Freeze"),
                        DeletionDate = GetParaDate(p, "DeletionDate"),
                        RemoveDeletionDate = GetParaBool(p, "RemoveDeletionDate") ?? false,
                    });
                    break;
                case "Delete":
                    result = SpItem.Delete(new DeleteItem
                    {
                        Id = GetParaInt(p, "Id") ?? 0,
                    });
                    break;
                case "SoftDelete":
                    result = SpItem.SoftDelete(GetParaInt(p, "Id") ?? 0);
                    break;
                case "Restore":
                    result = SpItem.Restore(GetParaInt(p, "Id") ?? 0);
                    break;
                case "Freeze":
                    result = SpItem.Freeze(GetParaInt(p, "Id") ?? 0);
                    break;
                case "Unfreeze":
                    result = SpItem.Unfreeze(GetParaInt(p, "Id") ?? 0);
                    break;
            }
            return result;
        }

        [Obsolete]
        public dynamic DoSpNotification(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];

            switch (requestSp.Method)
            {
                case "Get":
                    result = SpNotification.Get(new GetNotification
                    {
                        Id = GetParaInt(p, "Id"),
                        NotificationTypeId = GetParaInt(p, "NotificationTypeId"),
                        UserId = GetParaInt(p, "UserId"),
                        Readed = GetParaBool(p, "Readed"),
                    });
                    break;
                case "Insert":
                    result = SpNotification.Insert(new InsNotification
                    {
                        NotificationTypeId = GetPara(p, "NotificationTypeId") ?? 0,
                        UserId = GetParaInt(p, "UserId"),
                        Title = GetPara(p, "Title"),
                        Body = GetPara(p, "Body"),
                        Data = GetPara(p, "Data"),
                    });
                    break;
                case "Update":
                    result = SpNotification.Update(new UpdateNotification
                    {
                        Id = GetParaInt(p, "Id") ?? 0,
                        NotificationTypeId = GetParaInt(p, "NotificationTypeId"),
                        UserId = GetParaInt(p, "UserId"),
                        Title = GetPara(p, "Title"),
                        Body = GetPara(p, "Body"),
                        Data = GetPara(p, "Data"),
                        Readed = GetParaBool(p, "Readed"),
                    });
                    break;
                case "MarkAsRead":
                    result = SpNotification.Update(new UpdateNotification
                    {
                        Id = GetParaInt(p, "Id") ?? 0,
                        Readed = true,
                    });
                    break;
            }
            return result;
        }


        [Obsolete]
        public dynamic DoSpSetupTable(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];
            switch (requestSp.Method)
            {
                case "Get":
                    result = SpSetupTable.Get(new GetSetupTable());
                    break;
            }
            return result;
        }


        [Obsolete]
        public dynamic DoSpStore(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];
            switch (requestSp.Method)
            {
                case "Get":
                    result = SpStore.Get(new GetStore
                    {
                        Id = GetParaInt(p, "Id"),
                        Name = GetPara(p, "Name"),
                        StoreTypeId = GetParaInt(p, "StoreTypeId"),
                        UserRequestId = GetParaInt(p, "UserRequestId"),
                        Status = GetPara(p, "Status") is string s
                                        ? Enum.TryParse<StoreStatus>(s, out var st) ? st : null
                                        : null,
                    });
                    break;
                case "Insert":
                    result = SpStore.Insert(new InsStore
                    {
                        Name = GetPara(p, "Name") ?? "",
                        Descrip = GetPara(p, "Descrip") ?? "",
                        Location = GetPara(p, "Location") ?? "",
                        StoreTypeId = GetParaInt(p, "StoreTypeId") ?? 0,
                        UserRequestId = GetParaInt(p, "UserRequestId") ?? 0,
                    });
                    break;
                case "Update":
                    result = SpStore.Update(new UpdateStore
                    {
                        Id = GetParaInt(p, "Id") ?? 0,
                        Name = GetPara(p, "Name"),
                        Descrip = GetPara(p, "Descrip"),
                        Location = GetPara(p, "Location"),
                        StoreTypeId = GetParaInt(p, "StoreTypeId"),
                        ExpiredDate = GetParaDate(p, "ExpiredDate"),
                        RemoveExpiredDate = GetParaBool(p, "RemoveExpiredDate") ?? false,
                        DeletionDate = GetParaDate(p, "DeletionDate"),
                        RemoveDeletionDate = GetParaBool(p, "RemoveDeletionDate") ?? false,
                    });
                    break;
                case "Delete":
                    result = SpStore.Delete(new DeleteStore
                    {
                        Id = GetParaInt(p, "Id") ?? 0,
                    });
                    break;
                case "Approve":
                    result = SpStore.Approve(GetParaInt(p, "Id") ?? 0);
                    break;
                case "RevokeApproval":
                    result = SpStore.RevokeApproval(GetParaInt(p, "Id") ?? 0);
                    break;
                case "SoftDelete":
                    result = SpStore.SoftDelete(GetParaInt(p, "Id") ?? 0);
                    break;
                case "Restore":
                    result = SpStore.Restore(GetParaInt(p, "Id") ?? 0);
                    break;
            }
            return result;
        }


        [Obsolete]
        public dynamic DoSpStoreType(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];
            switch (requestSp.Method)
            {
                case "Get":
                    result = SpStoreType.Get(new GetStoreType
                    {
                        Id = GetParaInt(p, "Id"),
                    });
                    break;
                case "Insert":
                    result = SpStoreType.Insert(new InsStoreType
                    {
                        Descrip = GetPara(p, "Descrip") ?? "",
                    });
                    break;
                case "Update":
                    result = SpStoreType.Update(new UpdateStoreType
                    {
                        Id = GetParaInt(p, "Id") ?? 0,
                        Descrip = GetPara(p, "Descrip"),
                    });
                    break;
                case "Delete":
                    result = SpStoreType.Delete(new DeleteStoreType
                    {
                        Id = GetParaInt(p, "Id") ?? 0,
                    });
                    break;
            }
            return result;
        }


        [Obsolete]
        public dynamic DoSpStoreUser(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];
            switch (requestSp.Method)
            {
                case "Get":
                    result = SpStoreUser.Get(new GetStoreUser
                    {
                        Id = GetParaInt(p, "Id"),
                        StoreId = GetParaInt(p, "StoreId"),
                        UserId = GetParaInt(p, "UserId"),
                        WithDeleted = GetParaBool(p, "WithDeleted") ?? false,
                    });
                    break;
                case "Insert":
                    result = SpStoreUser.Insert(new InsStoreUser
                    {
                        StoreId = GetParaInt(p, "StoreId") ?? 0,
                        UserId = GetParaInt(p, "UserId") ?? 0,
                    });
                    break;
                case "Update":
                    result = SpStoreUser.Update(new UpdateStoreUser
                    {
                        Id = GetParaInt(p, "Id") ?? 0,
                        DeletionDate = GetParaDate(p, "DeletionDate"),
                        RemoveDeletionDate = GetParaBool(p, "RemoveDeletionDate") ?? false,
                    });
                    break;
                case "Delete":
                    result = SpStoreUser.Delete(new DeleteStoreUser
                    {
                        Id = GetParaInt(p, "Id") ?? 0,
                    });
                    break;
                case "SoftDelete":
                    result = SpStoreUser.SoftDelete(GetParaInt(p, "Id") ?? 0);
                    break;
                case "Restore":
                    result = SpStoreUser.Restore(GetParaInt(p, "Id") ?? 0);
                    break;
            }
            return result;
        }

        [Obsolete]
        public dynamic DoSpUser(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];
            switch (requestSp.Method)
            {
                case "Get":
                    result = SpUser.Get(new GetUser
                    {
                        Id = GetParaInt(p, "Id"),
                        PhoneNo = GetPara(p, "PhoneNo"),
                        ExcludeStoreId = GetParaInt(p, "ExcludeStoreId"),
                        WithDeletionDate = GetParaBool(p, "WithDeletionDate") ?? false,
                    });
                    break;
                case "Insert":
                    result = SpUser.Insert(new InsUser
                    {
                        Name = GetPara(p, "Name") ?? "",
                        PhoneNumber = GetPara(p, "PhoneNumber") ?? "",
                        Password = GetPara(p, "Password") ?? "",
                        FirebaseToken = GetPara(p, "FirebaseToken"),
                    });
                    break;
                case "Update":
                    result = SpUser.Update(new UpdateUser
                    {
                        Id = GetParaInt(p, "Id") ?? 0,
                        FullName = GetPara(p, "FullName"),
                        PhoneNumber = GetPara(p, "PhoneNumber"),
                        Password = GetPara(p, "Password"),
                        FirebaseToken = GetPara(p, "FirebaseToken"),
                        LastLogin = GetParaDate(p, "LastLogin"),
                        Admin = GetParaBool(p, "Admin") ?? false,
                        DeletionDate = GetParaDate(p, "DeletionDate"),
                        RemoveDeletionDate = GetParaBool(p, "RemoveDeletionDate") ?? false,
                    });
                    break;
                case "Delete":
                    result = SpUser.Delete(new DeleteUser
                    {
                        Id = GetParaInt(p, "Id") ?? 0,
                    });
                    break;
                case "SoftDelete":
                    result = SpUser.Update(new UpdateUser
                    {
                        Id = GetParaInt(p, "Id") ?? 0,
                        DeletionDate = DateTime.Now,
                    });
                    break;
                case "Restore":
                    result = SpUser.Update(new UpdateUser
                    {
                        Id = GetParaInt(p, "Id") ?? 0,
                        RemoveDeletionDate = true,
                    });
                    break;
            }
            return result;
        }

    }
}
