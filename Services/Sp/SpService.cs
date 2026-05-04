using Seiiarty.Services.Main;
using Seiiarty.StoredProcedures;
using SeiiartyApi.Services.Firebase;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.Services.Sp
{
    public class SpService() : ISpService
    {
        [Obsolete]
        public async Task<dynamic> DoSpCategory(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];
            switch (requestSp.Method)
            {
                case "Get":
                    result = await SpCategory.Get(new GetCategory
                    {
                        Id = General.GetParaInt(p, "Id"),
                        CatTypeId = General.GetParaInt(p, "CatTypeId"),
                    });
                    break;
                case "Insert":
                    result = await SpCategory.Insert(new InsCategory
                    {
                        CatNo = General.GetParaInt(p, "CatNo") ?? 0,
                        Descrip = General.GetPara(p, "Descrip") ?? "",
                        CatTypeId = General.GetParaInt(p, "CatTypeId") ?? 0,
                    });
                    break;
                case "Update":
                    result = await SpCategory.Update(new UpdateCategory
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        CatNo = General.GetParaInt(p, "CatNo"),
                        Descrip = General.GetPara(p, "Descrip"),
                        CatTypeId = General.GetParaInt(p, "CatTypeId"),
                        Freeze = General.GetParaBool(p, "Freeze"),
                        DeletionDate = General.GetParaDate(p, "DeletionDate"),
                        RemoveDeletionDate = General.GetParaBool(p, "RemoveDeletionDate") ?? false,
                    });
                    break;
                case "Delete":
                    result = await SpCategory.Delete(new DeleteCategory
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                    });
                    break;
                case "SoftDelete":
                    result = await SpCategory.SoftDelete(General.GetParaInt(p, "Id") ?? 0);
                    break;
                case "Restore":
                    result = await SpCategory.Restore(General.GetParaInt(p, "Id") ?? 0);
                    break;
                case "Freeze":
                    result = await SpCategory.Freeze(General.GetParaInt(p, "Id") ?? 0);
                    break;
                case "Unfreeze":
                    result = await SpCategory.Unfreeze(General.GetParaInt(p, "Id") ?? 0);
                    break;
            }
            return result;
        }

        [Obsolete]
        public async Task<dynamic> DoSpItem(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];
            switch (requestSp.Method)
            {
                case "Get":
                    result = await SpItem.Get(new GetItem
                    {
                        Id = General.GetParaInt(p, "Id"),
                        CategoryId = General.GetParaInt(p, "CategoryId"),
                        Name = General.GetPara(p, "Name"),
                        IsFrozen = General.GetParaBool(p, "IsFrozen"),
                        IsDeleted = General.GetParaBool(p, "IsDeleted"),
                        CatTypeId = General.GetParaInt(p, "CatTypeId"),
                    });
                    break;
                case "GetNextItemNo":
                    result = SpItem.GetNextItemNo(
                        categoryId: General.GetParaInt(p, "CategoryId") ?? 0,
                        catNo: General.GetParaInt(p, "CatNo") ?? 0
                    );
                    break;
                case "GetLastItemNo":
                    result = SpItem.GetLastItemNo(
                        categoryId: General.GetParaInt(p, "CategoryId") ?? 0
                    );
                    break;
                case "Insert":
                    result = await SpItem.Insert(new InsItem
                    {
                        ItemNo = General.GetParaInt(p, "ItemNo") ?? 0,
                        Name = General.GetPara(p, "Name") ?? "",
                        EName = General.GetPara(p, "EName") ?? "",
                        Descrip = General.GetPara(p, "Descrip") ?? "",
                        CategoryId = General.GetParaInt(p, "CategoryId") ?? 0,
                    });
                    break;
                case "Update":
                    result = await SpItem.Update(new UpdateItem
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        ItemNo = General.GetParaInt(p, "ItemNo"),
                        Name = General.GetPara(p, "Name"),
                        EName = General.GetPara(p, "EName"),
                        Descrip = General.GetPara(p, "Descrip"),
                        CategoryId = General.GetParaInt(p, "CategoryId"),
                        Freeze = General.GetParaBool(p, "Freeze"),
                        DeletionDate = General.GetParaDate(p, "DeletionDate"),
                        RemoveDeletionDate = General.GetParaBool(p, "RemoveDeletionDate") ?? false,
                    });
                    break;
                case "Delete":
                    result = SpItem.Delete(new DeleteItem
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                    });
                    break;
                case "SoftDelete":
                    result = SpItem.SoftDelete(General.GetParaInt(p, "Id") ?? 0);
                    break;
                case "Restore":
                    result = SpItem.Restore(General.GetParaInt(p, "Id") ?? 0);
                    break;
                case "Freeze":
                    result = SpItem.Freeze(General.GetParaInt(p, "Id") ?? 0);
                    break;
                case "Unfreeze":
                    result = SpItem.Unfreeze(General.GetParaInt(p, "Id") ?? 0);
                    break;
            }
            return result;
        }

        [Obsolete]
        public async Task<dynamic> DoSpNotification(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];

            switch (requestSp.Method)
            {
                case "Get":
                    result = await SpNotification.Get(new GetNotification
                    {
                        Id = General.GetParaInt(p, "Id"),
                        NotificationTypeId = General.GetParaInt(p, "NotificationTypeId"),
                        UserId = General.GetParaInt(p, "UserId"),
                        Readed = General.GetParaBool(p, "Readed"),
                    });
                    break;
                case "Insert":
                    result = await SpNotification.Insert(new InsNotification
                    {
                        NotificationTypeId = General.GetPara(p, "NotificationTypeId") ?? 0,
                        UserId = General.GetParaInt(p, "UserId"),
                        Title = General.GetPara(p, "Title"),
                        Body = General.GetPara(p, "Body"),
                        Data = General.GetPara(p, "Data"),
                    });
                    break;
                case "Update":
                    result = await SpNotification.Update(new UpdateNotification
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        NotificationTypeId = General.GetParaInt(p, "NotificationTypeId"),
                        UserId = General.GetParaInt(p, "UserId"),
                        Title = General.GetPara(p, "Title"),
                        Body = General.GetPara(p, "Body"),
                        Data = General.GetPara(p, "Data"),
                        Readed = General.GetParaBool(p, "Readed"),
                    });
                    break;
                case "MarkAsRead":
                    result = await SpNotification.Update(new UpdateNotification
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        Readed = true,
                    });
                    break;
            }
            return result;
        }

        [Obsolete]
        public async Task<dynamic> DoSpOrder(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];

            switch (requestSp.Method)
            {
                case "OSR": // One Shot Request: create order + order detail + send notification in one call
                    // 1. Insert the order (sync, returns int)
                    var orderResult = SpOrder.Insert(new InsOrder
                    {
                        CreationUserId = General.GetParaInt(p, "CreationUserId") ?? 0,
                        StatusId = General.GetParaInt(p, "StatusId") ?? (int)OrderStatus.Pending,
                    });

                    // 2. Parse the OrderId
                    int orderId = Convert.ToInt32(orderResult);

                    // 3. Insert the order detail (sync, returns int)
                    result = SpOrderDet.Insert(new InsOrderDet
                    {
                        ItemId = General.GetParaInt(p, "ItemId") ?? 0,
                        OrderId = orderId,
                        Quantity = General.GetParaInt(p, "Quantity") ?? 0,
                    });

                    // 4. Send notification (async)
                    NotificationService notificationService = new NotificationService();
                    await notificationService.SendAndSaveNotificationAsync(
                        title: General.GetPara(p, "Title") ?? "",
                        body: General.GetPara(p, "Body") ?? "",
                        notificationTypeId: 1,
                        userId: General.GetParaInt(p, "CreationUserId") ?? 0
                    );
                    break;

                case "Insert":
                    result = SpOrder.Insert(new InsOrder
                    {
                        CreationUserId = General.GetParaInt(p, "CreationUserId") ?? 0,
                        StatusId = General.GetParaInt(p, "StatusId") ?? (int)OrderStatus.Pending,
                    });
                    break;

                case "Update":
                    result = await SpOrder.Update(new UpdateOrder
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        StatusId = General.GetParaInt(p, "StatusId"),
                        DeletionDate = General.GetParaDate(p, "DeletionDate"),
                        RemoveDeletionDate = General.GetParaBool(p, "RemoveDeletionDate") ?? false,
                    });
                    break;

                case "Delete":
                    result = await SpOrder.Delete(new DeleteOrder
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                    });
                    break;

                case "Confirm":
                    result = await SpOrder.ChangeStatus(General.GetParaInt(p, "Id") ?? 0, OrderStatus.Confirmed);
                    break;

                case "MarkReady":
                    result = await SpOrder.ChangeStatus(General.GetParaInt(p, "Id") ?? 0, OrderStatus.Ready);
                    break;

                case "MarkDelivered":
                    result = await SpOrder.ChangeStatus(General.GetParaInt(p, "Id") ?? 0, OrderStatus.Delivered);
                    break;

                case "Cancel":
                    result = await SpOrder.ChangeStatus(General.GetParaInt(p, "Id") ?? 0, OrderStatus.Cancelled);
                    break;

                case "SoftDelete":
                    result = await SpOrder.SoftDelete(General.GetParaInt(p, "Id") ?? 0);
                    break;

                case "Restore":
                    result = await SpOrder.Restore(General.GetParaInt(p, "Id") ?? 0);
                    break;
            }

            return result;
        }

        [Obsolete]
        public async Task<dynamic> DoSpOrderDet(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];

            switch (requestSp.Method)
            {
                case "Get":
                    result = await SpOrderDet.Get(new GetOrderDet
                    {
                        Id = General.GetParaInt(p, "Id"),
                        ItemId = General.GetParaInt(p, "ItemId"),
                        OrderId = General.GetParaInt(p, "OrderId"),
                    });
                    break;

                case "Insert":
                    result = SpOrderDet.Insert(new InsOrderDet
                    {
                        ItemId = General.GetParaInt(p, "ItemId") ?? 0,
                        OrderId = General.GetParaInt(p, "OrderId") ?? 0,
                        Quantity = General.GetParaInt(p, "Quantity") ?? 0,
                    });
                    break;

                case "Update":
                    result = await SpOrderDet.Update(new UpdateOrderDet
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        ItemId = General.GetParaInt(p, "ItemId"),
                        Quantity = General.GetParaInt(p, "Quantity"),
                    });
                    break;

                case "Delete":
                    result = await SpOrderDet.Delete(new DeleteOrderDet
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                    });
                    break;
            }

            return result;
        }

        [Obsolete]
        public async Task<dynamic> DoSpSetupTable(RequestSp requestSp)
        {
            dynamic result = "";
            switch (requestSp.Method)
            {
                case "Get":
                    result = await SpSetupTable.Get();
                    break;
            }
            return result;
        }

        [Obsolete]
        public async Task<dynamic> DoSpStore(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];

            switch (requestSp.Method)
            {
                case "Get":
                    StoreStatus? status = null;
                    string? statusStr = General.GetPara(p, "Status");
                    if (statusStr != null && Enum.TryParse<StoreStatus>(statusStr, ignoreCase: true, out var parsedStatus))
                        status = parsedStatus;

                    result = await SpStore.Get(new GetStore
                    {
                        Id = General.GetParaInt(p, "Id"),
                        Name = General.GetPara(p, "Name"),
                        StoreTypeId = General.GetParaInt(p, "StoreTypeId"),
                        UserRequestId = General.GetParaInt(p, "UserRequestId"),
                        Status = status,
                    });
                    break;

                case "Insert":
                    result = await SpStore.Insert(new InsStore
                    {
                        Name = General.GetPara(p, "Name") ?? "",
                        Descrip = General.GetPara(p, "Descrip") ?? "",
                        Location = General.GetPara(p, "Location") ?? "",
                        StoreTypeId = General.GetParaInt(p, "StoreTypeId") ?? 0,
                        UserRequestId = General.GetParaInt(p, "UserRequestId") ?? 0,
                    });
                    break;

                case "Update":
                    bool removeExpired = General.GetParaBool(p, "RemoveExpiredDate") ?? false;
                    bool removeDeleted = General.GetParaBool(p, "RemoveDeletionDate") ?? false;

                    result = await SpStore.Update(new UpdateStore
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        Name = General.GetPara(p, "Name"),
                        Descrip = General.GetPara(p, "Descrip"),
                        Location = General.GetPara(p, "Location"),
                        StoreTypeId = General.GetParaInt(p, "StoreTypeId"),
                        ExpiredDate = removeExpired ? null : General.GetParaDate(p, "ExpiredDate"),
                        RemoveExpiredDate = removeExpired,
                        DeletionDate = removeDeleted ? null : General.GetParaDate(p, "DeletionDate"),
                        RemoveDeletionDate = removeDeleted,
                    });
                    break;

                case "Delete":
                    result = await SpStore.Delete(new DeleteStore
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                    });
                    break;

                case "Approve":
                    result = await SpStore.Approve(General.GetParaInt(p, "Id") ?? 0);
                    break;

                case "RevokeApproval":
                    result = await SpStore.RevokeApproval(General.GetParaInt(p, "Id") ?? 0);
                    break;

                case "SoftDelete":
                    result = await SpStore.SoftDelete(General.GetParaInt(p, "Id") ?? 0);
                    break;

                case "Restore":
                    result = await SpStore.Restore(General.GetParaInt(p, "Id") ?? 0);
                    break;
            }

            return result;
        }

        [Obsolete]
        public async Task<dynamic> DoSpStoreType(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];
            switch (requestSp.Method)
            {
                case "Get":
                    result = await SpStoreType.Get(new GetStoreType
                    {
                        Id = General.GetParaInt(p, "Id"),
                    });
                    break;
                case "Insert":
                    result = await SpStoreType.Insert(new InsStoreType
                    {
                        Descrip = General.GetPara(p, "Descrip") ?? "",
                    });
                    break;
                case "Update":
                    result = await SpStoreType.Update(new UpdateStoreType
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        Descrip = General.GetPara(p, "Descrip"),
                    });
                    break;
                case "Delete":
                    result = await SpStoreType.Delete(new DeleteStoreType
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                    });
                    break;
            }
            return result;
        }

        [Obsolete]
        public async Task<dynamic> DoSpStoreUser(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];
            switch (requestSp.Method)
            {
                case "Get":
                    result = await SpStoreUser.Get(new GetStoreUser
                    {
                        Id = General.GetParaInt(p, "Id"),
                        StoreId = General.GetParaInt(p, "StoreId"),
                        UserId = General.GetParaInt(p, "UserId"),
                        WithDeleted = General.GetParaBool(p, "WithDeleted") ?? false,
                    });
                    break;
                case "Insert":
                    result = await SpStoreUser.Insert(new InsStoreUser
                    {
                        StoreId = General.GetParaInt(p, "StoreId") ?? 0,
                        UserId = General.GetParaInt(p, "UserId") ?? 0,
                    });
                    break;
                case "Update":
                    result = await SpStoreUser.Update(new UpdateStoreUser
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        DeletionDate = General.GetParaDate(p, "DeletionDate"),
                        RemoveDeletionDate = General.GetParaBool(p, "RemoveDeletionDate") ?? false,
                    });
                    break;
                case "Delete":
                    result = await SpStoreUser.Delete(new DeleteStoreUser
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                    });
                    break;
                case "SoftDelete":
                    result = await SpStoreUser.SoftDelete(General.GetParaInt(p, "Id") ?? 0);
                    break;
                case "Restore":
                    result = await SpStoreUser.Restore(General.GetParaInt(p, "Id") ?? 0);
                    break;
            }
            return result;
        }

        [Obsolete]
        public async Task<dynamic> DoSpUser(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];
            switch (requestSp.Method)
            {
                case "Get":
                    result = await SpUser.Get(new GetUser
                    {
                        Id = General.GetParaInt(p, "Id"),
                        PhoneNo = General.GetPara(p, "PhoneNo"),
                        ExcludeStoreId = General.GetParaInt(p, "ExcludeStoreId"),
                        HasStore = General.GetParaBool(p, "HasStore") ?? false,
                        WithDeletionDate = General.GetParaBool(p, "WithDeletionDate") ?? false,
                    });
                    break;
                case "Update":
                    result = await SpUser.Update(new UpdateUser
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        FullName = General.GetPara(p, "FullName"),
                        PhoneNumber = General.GetPara(p, "PhoneNumber"),
                        Password = General.GetPara(p, "Password"),
                        FirebaseToken = General.GetPara(p, "FirebaseToken"),
                        PhoneToken = General.GetPara(p, "PhoneToken"),
                        LastLogin = General.GetParaDate(p, "LastLogin"),
                        Admin = General.GetParaBool(p, "Admin") ?? false,
                        DeletionDate = General.GetParaDate(p, "DeletionDate"),
                        RemoveDeletionDate = General.GetParaBool(p, "RemoveDeletionDate") ?? false,
                    });
                    break;
                case "Delete":
                    result = await SpUser.Delete(new DeleteUser
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                    });
                    break;
                case "SoftDelete":
                    result = SpUser.Update(new UpdateUser
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        DeletionDate = DateTime.Now,
                    });
                    break;
                case "Restore":
                    result = await SpUser.Update(new UpdateUser
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        RemoveDeletionDate = true,
                    });
                    break;
            }
            return result;
        }
    }
}