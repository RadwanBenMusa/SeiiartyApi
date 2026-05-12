using Seiiarty.Services.Main;
using Seiiarty.StoredProcedures;
using SeiiartyApi.Services.Firebase;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty.Services.Sp
{
    public class SpService() : ISpService
    {
        [Obsolete]
        public dynamic DoSpBasket(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];

            switch (requestSp.Method)
            {
                // ── Get baskets (by Id or UserId)
                case "Get":
                    result = SpBasket.Get(new GetBasket
                    {
                        Id = General.GetParaInt(p, "Id"),
                        UserId = General.GetParaInt(p, "UserId"),
                        WithDeleted = General.GetParaBool(p, "WithDeleted") ?? false,
                    });
                    break;

                // ── Create a new basket for a user — returns the new basket ID
                case "Insert":
                    result = SpBasket.Insert(new InsBasket
                    {
                        UserId = General.GetParaInt(p, "UserId") ?? 0,
                    });
                    break;

                // ── Soft delete or restore a basket
                case "SoftDelete":
                    result = SpBasket.SoftDelete(General.GetParaInt(p, "Id") ?? 0);
                    break;

                case "Restore":
                    result = SpBasket.Restore(General.GetParaInt(p, "Id") ?? 0);
                    break;

                // ── Hard delete
                case "Delete":
                    result = SpBasket.Delete(new DeleteBasket
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                    });
                    break;
            }

            return result;
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
                        Id = General.GetParaInt(p, "Id"),
                        CatTypeId = General.GetParaInt(p, "CatTypeId"),
                    });
                    break;
                case "Insert":
                    result = SpCategory.Insert(new InsCategory
                    {
                        CatNo = General.GetParaInt(p, "CatNo") ?? 0,
                        Descrip = General.GetPara(p, "Descrip") ?? "",
                        CatTypeId = General.GetParaInt(p, "CatTypeId") ?? 0,
                    });
                    break;
                case "Update":
                    result = SpCategory.Update(new UpdateCategory
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
                    result = SpCategory.Delete(new DeleteCategory
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                    });
                    break;
                case "SoftDelete":
                    result = SpCategory.SoftDelete(General.GetParaInt(p, "Id") ?? 0);
                    break;
                case "Restore":
                    result = SpCategory.Restore(General.GetParaInt(p, "Id") ?? 0);
                    break;
                case "Freeze":
                    result = SpCategory.Freeze(General.GetParaInt(p, "Id") ?? 0);
                    break;
                case "Unfreeze":
                    result = SpCategory.Unfreeze(General.GetParaInt(p, "Id") ?? 0);
                    break;
            }
            return Task.FromResult(result);
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
                    result = SpItem.Insert(new InsItem
                    {
                        ItemNo = General.GetParaInt(p, "ItemNo") ?? 0,
                        Name = General.GetPara(p, "Name") ?? "",
                        EName = General.GetPara(p, "EName") ?? "",
                        Descrip = General.GetPara(p, "Descrip") ?? "",
                        CategoryId = General.GetParaInt(p, "CategoryId") ?? 0,
                    });
                    break;
                case "Update":
                    result = SpItem.Update(new UpdateItem
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
            return Task.FromResult(result);
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
                        Id = General.GetParaInt(p, "Id"),
                        NotificationTypeId = General.GetParaInt(p, "NotificationTypeId"),
                        UserId = General.GetParaInt(p, "UserId"),
                        Readed = General.GetParaBool(p, "Readed"),
                    });
                    break;
                case "Insert":
                    result = SpNotification.Insert(new InsNotification
                    {
                        NotificationTypeId = General.GetPara(p, "NotificationTypeId") ?? 0,
                        UserId = General.GetParaInt(p, "UserId"),
                        Title = General.GetPara(p, "Title"),
                        Body = General.GetPara(p, "Body"),
                        Data = General.GetPara(p, "Data"),
                    });
                    break;
                case "Update":
                    result = SpNotification.Update(new UpdateNotification
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
                    result = SpNotification.Update(new UpdateNotification
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        Readed = true,
                    });
                    break;
            }
            return Task.FromResult(result);
        }

        [Obsolete]
        public async Task<dynamic> DoSpOrder(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];

            switch (requestSp.Method)
            {
                case "Get":
                    result = SpOrder.Get(new GetOrder
                    {
                        Id = General.GetParaInt(p, "Id"),
                        CreationUserId = General.GetParaInt(p, "CreationUserId"),
                        Status = (OrderStatus?)General.GetParaInt(p, "StatusId"),
                        WithDeleted = General.GetParaBool(p, "WithDeleted") ?? false,
                        FromDate = General.GetParaDate(p, "FromDate"),
                        ToDate = General.GetParaDate(p, "ToDate")
                    });
                    break;

                case "OSR": // One Shot Request
                    var orderResult = SpOrder.Insert(new InsOrder
                    {
                        CreationUserId = General.GetParaInt(p, "CreationUserId") ?? 0,
                        StatusId = General.GetParaInt(p, "StatusId") ?? (int)OrderStatus.Pending,
                    });

                    int orderId = Convert.ToInt32(orderResult);

                    result = SpOrderDet.Insert(new InsOrderDet
                    {
                        ItemId = General.GetParaInt(p, "ItemId") ?? 0,
                        OrderId = orderId,
                        Quantity = General.GetParaInt(p, "Quantity") ?? 0,
                    });

                    NotificationService notificationService = new NotificationService();
                    // Awaiting here prevents the 'int' does not contain a definition for 'GetAwaiter' error
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
                    result = SpOrder.Update(new UpdateOrder
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        StatusId = General.GetParaInt(p, "StatusId"),
                        DeletionDate = General.GetParaDate(p, "DeletionDate"),
                        RemoveDeletionDate = General.GetParaBool(p, "RemoveDeletionDate") ?? false,
                    });
                    break;

                case "Delete":
                    result = SpOrder.Delete(new DeleteOrder
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                    });
                    break;

                case "Confirm":
                    result = SpOrder.ChangeStatus(General.GetParaInt(p, "Id") ?? 0, OrderStatus.Confirmed);
                    break;

                case "MarkReady":
                    result = SpOrder.ChangeStatus(General.GetParaInt(p, "Id") ?? 0, OrderStatus.Ready);
                    break;

                case "MarkDelivered":
                    result = SpOrder.ChangeStatus(General.GetParaInt(p, "Id") ?? 0, OrderStatus.Delivered);
                    break;

                case "Cancel":
                    result = SpOrder.ChangeStatus(General.GetParaInt(p, "Id") ?? 0, OrderStatus.Cancelled);
                    break;

                case "SoftDelete":
                    result = SpOrder.SoftDelete(General.GetParaInt(p, "Id") ?? 0);
                    break;

                case "Restore":
                    result = SpOrder.Restore(General.GetParaInt(p, "Id") ?? 0);
                    break;
            }

            return result;
        }

        [Obsolete]
        public dynamic DoSpOrderDet(RequestSp requestSp)
        {
            dynamic result = "";
            var p = requestSp.Paras ?? [];

            switch (requestSp.Method)
            {
                case "Get":
                    result = SpOrderDet.Get(new GetOrderDet
                    {
                        Id = General.GetParaInt(p, "Id"),
                        ItemId = General.GetParaInt(p, "ItemId"),
                        OrderId = General.GetParaInt(p, "OrderId"),
                        BasketId = General.GetParaInt(p, "BasketId"),
                    });
                    break;

                case "Insert":
                    result = SpOrderDet.Insert(new InsOrderDet
                    {
                        ItemId = General.GetParaInt(p, "ItemId") ?? 0,
                        OrderId = General.GetParaInt(p, "OrderId") ?? 0,
                        Quantity = General.GetParaInt(p, "Quantity") ?? 0,
                        BasketId = General.GetParaInt(p, "BasketId")?? 0,
                    });
                    break;

                case "Update":
                    result = SpOrderDet.Update(new UpdateOrderDet
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        ItemId = General.GetParaInt(p, "ItemId"),
                        Quantity = General.GetParaInt(p, "Quantity"),
                    });
                    break;

                case "Delete":
                    result = SpOrderDet.Delete(new DeleteOrderDet
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                    });
                    break;
            }

            return Task.FromResult(result);
        }

        [Obsolete]
        public dynamic DoSpSetupTable(RequestSp requestSp)
        {
            dynamic result = "";
            switch (requestSp.Method)
            {
                case "Get":
                    result = SpSetupTable.Get();
                    break;
            }
            return Task.FromResult(result);
        }

        [Obsolete]
        public dynamic DoSpStore(RequestSp requestSp)
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

                    result = SpStore.Get(new GetStore
                    {
                        Id = General.GetParaInt(p, "Id"),
                        Name = General.GetPara(p, "Name"),
                        StoreTypeId = General.GetParaInt(p, "StoreTypeId"),
                        UserRequestId = General.GetParaInt(p, "UserRequestId"),
                        Status = status,
                    });
                    break;

                case "Insert":
                    result = SpStore.Insert(new InsStore
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

                    result = SpStore.Update(new UpdateStore
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
                    result = SpStore.Delete(new DeleteStore
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                    });
                    break;

                case "Approve":
                    result = SpStore.Approve(General.GetParaInt(p, "Id") ?? 0);
                    break;

                case "RevokeApproval":
                    result = SpStore.RevokeApproval(General.GetParaInt(p, "Id") ?? 0);
                    break;

                case "SoftDelete":
                    result = SpStore.SoftDelete(General.GetParaInt(p, "Id") ?? 0);
                    break;

                case "Restore":
                    result = SpStore.Restore(General.GetParaInt(p, "Id") ?? 0);
                    break;
            }

            return Task.FromResult(result);
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
                        Id = General.GetParaInt(p, "Id"),
                    });
                    break;
                case "Insert":
                    result = SpStoreType.Insert(new InsStoreType
                    {
                        Descrip = General.GetPara(p, "Descrip") ?? "",
                    });
                    break;
                case "Update":
                    result = SpStoreType.Update(new UpdateStoreType
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        Descrip = General.GetPara(p, "Descrip"),
                    });
                    break;
                case "Delete":
                    result = SpStoreType.Delete(new DeleteStoreType
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                    });
                    break;
            }
            return Task.FromResult(result);
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
                        Id = General.GetParaInt(p, "Id"),
                        StoreId = General.GetParaInt(p, "StoreId"),
                        UserId = General.GetParaInt(p, "UserId"),
                        WithDeleted = General.GetParaBool(p, "WithDeleted") ?? false,
                    });
                    break;
                case "Insert":
                    result = SpStoreUser.Insert(new InsStoreUser
                    {
                        StoreId = General.GetParaInt(p, "StoreId") ?? 0,
                        UserId = General.GetParaInt(p, "UserId") ?? 0,
                    });
                    break;
                case "Update":
                    result = SpStoreUser.Update(new UpdateStoreUser
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        DeletionDate = General.GetParaDate(p, "DeletionDate"),
                        RemoveDeletionDate = General.GetParaBool(p, "RemoveDeletionDate") ?? false,
                    });
                    break;
                case "Delete":
                    result = SpStoreUser.Delete(new DeleteStoreUser
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                    });
                    break;
                case "SoftDelete":
                    result = SpStoreUser.SoftDelete(General.GetParaInt(p, "Id") ?? 0);
                    break;
                case "Restore":
                    result = SpStoreUser.Restore(General.GetParaInt(p, "Id") ?? 0);
                    break;
            }
            return Task.FromResult(result);
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
                        Id = General.GetParaInt(p, "Id"),
                        PhoneNo = General.GetPara(p, "PhoneNo"),
                        ExcludeStoreId = General.GetParaInt(p, "ExcludeStoreId"),
                        HasStore = General.GetParaBool(p, "HasStore") ?? false,
                        WithDeletionDate = General.GetParaBool(p, "WithDeletionDate") ?? false,
                    });
                    break;
                case "Update":
                    result = SpUser.Update(new UpdateUser
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
                    result = SpUser.Delete(new DeleteUser
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
                    result = SpUser.Update(new UpdateUser
                    {
                        Id = General.GetParaInt(p, "Id") ?? 0,
                        RemoveDeletionDate = true,
                    });
                    break;
            }
            return Task.FromResult(result);
        }
    }
}