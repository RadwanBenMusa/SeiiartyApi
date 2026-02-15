 using static TamaApi.Services.db.DbService;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System;
using TamaApi.clsMod;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TamaApi.Controllers;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace TamaApi.Services.db
{
    public static class DoSomeThings
    {
        static public string GetFromat(DateTime dateTime) 
        {
            return $"{dateTime.Year}-{dateTime.Month}-{dateTime.Day} 00:00:00";
        }
        static string getValueOfField(string fName, List<Para> paras)
        {
            foreach (var item in paras)
            {
                if (item.Name == fName)
                { return item.Value; }
            }
            return "";
        }
        static DateTime GetFirstDateForDayOfWeek(int dayOfWeekNumber)
        {
            if (dayOfWeekNumber < 1 || dayOfWeekNumber > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(dayOfWeekNumber), "رقم اليوم يجب أن يكون بين 1 و 7 (الأحد=1، الاثنين=2، ...، السبت=7).");
            }

            // الحصول على تاريخ اليوم الحالي
            DateTime today = DateTime.Now;

            // تحديد اليوم الحالي من الأسبوع (الأحد = 0، الاثنين = 1، ...)
            int currentDayOfWeek = (int)today.DayOfWeek + 1;

            // تعديل قيمة اليوم الحالي لجعل الأحد = 1، الاثنين = 2، ...، السبت = 7
            int adjustedCurrentDayOfWeek = currentDayOfWeek == 8 ? 1 : currentDayOfWeek;

            // حساب الفرق في الأيام بين اليوم المطلوب واليوم الحالي
            int daysDifference = dayOfWeekNumber - adjustedCurrentDayOfWeek;

            if (daysDifference < 0) daysDifference = 7 + daysDifference;
            // إضافة الفرق في الأيام إلى تاريخ اليوم للحصول على أقرب تاريخ
            DateTime closestDate = today.AddDays(daysDifference);

            return closestDate;
        }
        static int getEncryptionType()
        {
            Random rng = new Random();

            int encryptionType;
            do
            {
                encryptionType = rng.Next(1,6);
            }
            while (encryptionType <= 0 || encryptionType >= 6);

            return encryptionType;
        }
        static List<int> fillSeq(int encryptionType)
        {
            List<int> digitsSeq = [0, 0, 0, 0, 0, 0, 0, 0, 0];
            switch (encryptionType)
            {
                case 1:
                    {
                        digitsSeq[1] = 6;
                        digitsSeq[2] = 1;
                        digitsSeq[3] = 5;
                        digitsSeq[4] = 8;
                        digitsSeq[5] = 2;
                        digitsSeq[6] = 4;
                        digitsSeq[7] = 7;
                        digitsSeq[8] = 3;
                    }
                    break;

                case 2:
                    {
                        digitsSeq[1] = 3;
                        digitsSeq[2] = 7;
                        digitsSeq[3] = 8;
                        digitsSeq[4] = 4;
                        digitsSeq[5] = 1;
                        digitsSeq[6] = 5;
                        digitsSeq[7] = 2;
                        digitsSeq[8] = 6;
                    }
                    break;

                case 3:
                    {
                        digitsSeq[1] = 4;
                        digitsSeq[2] = 7;
                        digitsSeq[3] = 2;
                        digitsSeq[4] = 5;
                        digitsSeq[5] = 3;
                        digitsSeq[6] = 1;
                        digitsSeq[7] = 8;
                        digitsSeq[8] = 6;
                    }
                    break;

                case 4:
                    {
                        digitsSeq[1] = 8;
                        digitsSeq[2] = 1;
                        digitsSeq[3] = 3;
                        digitsSeq[4] = 6;
                        digitsSeq[5] = 7;
                        digitsSeq[6] = 2;
                        digitsSeq[7] = 5;
                        digitsSeq[8] = 4;
                    }
                    break;

                case 5:
                    {
                        digitsSeq[1] = 3;
                        digitsSeq[2] = 5;
                        digitsSeq[3] = 8;
                        digitsSeq[4] = 1;
                        digitsSeq[5] = 6;
                        digitsSeq[6] = 4;
                        digitsSeq[7] = 7;
                        digitsSeq[8] = 2;
                    }
                    break;
            }
            return digitsSeq;
        }
        static int getWitchNoToAdd(int encryptionType)
        {
            int x = 0;
            switch (encryptionType)
            {
                case 1:
                    x = 7;
                    break;
                case 2:
                    x = 6;
                    break;
                case 3:
                    x = 9;
                    break;
                case 4:
                    x = 8;
                    break;
                case 5:
                    x = 5;
                    break;
            }
            return x;
        }
        static string GeneratedCode(string clinicNo,int yearNo, int monthNo) 
        {
            int encryptionType = getEncryptionType();

            List<string> EncrypeCode = [encryptionType.ToString(), "", "", "", "", "", "", "", ""];
            List<int> digitsSeq= fillSeq(encryptionType);

            //Month
            string strMonth = monthNo.ToString();
            if (strMonth.Length == 1) strMonth = "0" + strMonth;

            EncrypeCode[digitsSeq[1]] = ((int.Parse(strMonth[0].ToString()) + getWitchNoToAdd(encryptionType))%16).ToString("x").ToUpper();//.remainder(16).toRadixString(16).toString().toUpperCase();
            EncrypeCode[digitsSeq[2]] = ((int.Parse(strMonth[1].ToString()) + getWitchNoToAdd(encryptionType))%16).ToString("x").ToUpper();//.remainder(16).toRadixString(16).toString().toUpperCase();

            //Year
            string strYear = yearNo.ToString();
            EncrypeCode[digitsSeq[3]] = ((int.Parse(strYear[2].ToString()) + getWitchNoToAdd(encryptionType))%16).ToString("x").ToUpper(); //.remainder(16).toRadixString(16).toString().toUpperCase();
            EncrypeCode[digitsSeq[4]] = ((int.Parse(strYear[3].ToString()) + getWitchNoToAdd(encryptionType))%16).ToString("x").ToUpper();//.remainder(16).toRadixString(16).toString().toUpperCase();


            if (clinicNo.Length < 4)
            {
                while (clinicNo.Length < 4)
                {
                    clinicNo = "0" +clinicNo;
                }
            }

            EncrypeCode[digitsSeq[5]] = ((int.Parse(clinicNo[0].ToString()) + getWitchNoToAdd(encryptionType))%16).ToString("x").ToUpper();//.remainder(16).toRadixString(16).toString().toUpperCase();
            EncrypeCode[digitsSeq[6]] = ((int.Parse(clinicNo[1].ToString()) + getWitchNoToAdd(encryptionType))%16).ToString("x").ToUpper();//.remainder(16).toRadixString(16).toString().toUpperCase();
            EncrypeCode[digitsSeq[7]] = ((int.Parse(clinicNo[2].ToString()) + getWitchNoToAdd(encryptionType))%16).ToString("x").ToUpper();//.remainder(16).toRadixString(16).toString().toUpperCase();
            EncrypeCode[digitsSeq[8]] = ((int.Parse(clinicNo[3].ToString()) + getWitchNoToAdd(encryptionType))%16).ToString("x");//.remainder(16).toRadixString(16).toString().toUpperCase();

            string SortCode = string.Join("", EncrypeCode);


            //Convert To LoargCode
            Random random = new Random();
            string largeCode = "";
            //'1-2 Rnd
            largeCode = (random.Next() % 16).ToString("x").ToUpper();
            largeCode += (random.Next() % 16).ToString("x").ToUpper();
            // '3-4-5
            largeCode += SortCode.Substring(0, 3);
            // '6
            largeCode += SortCode.Substring(3, 1);

            // '7 rnd
            largeCode += (random.Next() % 16).ToString("x").ToUpper();
            // '8
            largeCode += SortCode.Substring(4, 1);
            // '9 rnd
            largeCode += (random.Next() % 16).ToString("x").ToUpper();
            // '10
            largeCode += SortCode.Substring(5, 1);
            // '11 rnd
            largeCode += (random.Next() % 16).ToString("x").ToUpper();
            // '12-13
            largeCode += SortCode.Substring(6, 2);
            // '14 rnd
            largeCode += (random.Next() % 16).ToString("x").ToUpper();
            // '15
            largeCode += SortCode.Substring(8, 1);

            return largeCode;
        }
        
        [Obsolete]
        public static dynamic GetVoucherCompanyCo(TamaRequest tamaRequest, ILogger<MainController> _logger)
        {
            _logger.LogInformation($"XXX_GetItemsForCompanyCo ===> tamaRequest = {JsonConvert.SerializeObject(tamaRequest)}");

            int CompanyCoId = int.Parse(tamaRequest.paras![0].Value);
            DateTime DateFrom = DateTime.Parse(tamaRequest.paras![1].Value);
            DateTime DateTo = DateTime.Parse(tamaRequest.paras![2].Value);

            string cmd = "SELECT dbo.VoucherIn.VoucherDate, dbo.VoucherIn.TotalServiceAmount, dbo.VoucherIn.VoucherAmount, dbo.OrdersDet.ItemId, dbo.OrdersDet.PriceWhenPay, dbo.OrdersDet.Qty, dbo.Item.ItemName, dbo.Item.EItemName, ";
            cmd += "dbo.VoucherIn.VoucherNumber, dbo.OrdersDet.VoucherInId, dbo.Customer.CustomerNumber, dbo.Customer.Frst_Name, dbo.Customer.F_Name, dbo.Customer.S_Name, dbo.Customer.BirthDate, dbo.Customer.Male, ";
            cmd += "dbo.Customer.IDNo ";
            cmd += "FROM dbo.VoucherIn INNER JOIN ";
            cmd += "dbo.OrdersDet ON dbo.VoucherIn.ID = dbo.OrdersDet.VoucherInId INNER JOIN ";
            cmd += "dbo.Item ON dbo.OrdersDet.ItemId = dbo.Item.ID INNER JOIN ";
            cmd += "dbo.Orders ON dbo.OrdersDet.OrdersId = dbo.Orders.ID INNER JOIN ";
            cmd += "dbo.Customer ON dbo.Orders.CustomerId = dbo.Customer.ID ";
            cmd += $"WHERE(dbo.VoucherIn.DeletionDate IS NULL) AND(dbo.VoucherIn.ContractCoId ={CompanyCoId}) ";
            //cmd += $"AND(dbo.VoucherIn.VoucherDate >= CONVERT(DATETIME, '{2023-06-03 00:00:00}', 102)) AND ";
            string dateFrom = $"{DateFrom.Year}-{DateFrom.Month}-{DateFrom.Day} {DateFrom.Hour}:{DateFrom.Minute}:{DateFrom.Second}";
            cmd += $"AND(dbo.VoucherIn.VoucherDate >= CONVERT(DATETIME, '{dateFrom}', 102)) AND ";
            string dateTo = $"{DateTo.Year}-{DateTo.Month}-{DateTo.Day} {DateTo.Hour}:{DateTo.Minute}:{DateTo.Second}";
            cmd += $"(dbo.VoucherIn.VoucherDate <= CONVERT(DATETIME, '{dateTo}', 102))";

            DataTable dtVoucherCompanyCo = ExecCommand(new TamaRequestCmd()
            {
                ClinicId = tamaRequest.clinicId,
                Cmd = cmd,
            });

            return new
            {
                MsgId = 1,
                MsgAr = "تمت العملية بنجاح",
                MsgEn = "Success",
                Data = dtVoucherCompanyCo.ToDynamic(),
            };
        }

        [Obsolete]
        public static dynamic GetItemsForCompanyCo(TamaRequest tamaRequest, ILogger<MainController> _logger)
        {
            _logger.LogInformation($"XXX_GetItemsForCompanyCo ===> tamaRequest = {JsonConvert.SerializeObject(tamaRequest)}");

            int CompanyCoId = int.Parse(tamaRequest.paras![0].Value);
            string cmd = $"Select * From ContractCo Where ID={CompanyCoId}";
            DataTable dtCompanyCo = ExecCommand(new TamaRequestCmd()
            {
                ClinicId = tamaRequest.clinicId,
                Cmd = cmd,
            });

            if(dtCompanyCo.Rows.Count==0)
            return new
            {
                MsgId = 2,
                MsgAr = "لم نتمكن من وجود الشركة",
                MsgEn = "لم نتمكن من وجود الشركة",
            };

            int ListPriceNO = General.getValueInt(dtCompanyCo.Rows[0], "ListPriceNO");
            int idFrom =  int.Parse(tamaRequest.paras![1].Value);
            int idTo = int.Parse(tamaRequest.paras![2].Value);

            cmd = "SELECT  dbo.Item.ID, dbo.Item.ItemName, dbo.Item.EItemName, dbo.ItemCategory.ItemCategoryParentId, ";
            cmd += "dbo.ItemCategory.ItemCategoryName, ItemCategory_1.ItemCategoryParentId AS ItemCategoryParentId1, ";
            cmd += "ItemCategory_1.ItemCategoryName AS ItemCategoryName1, ItemCategory_2.ItemCategoryParentId AS ItemCategoryParentId2, ";
            cmd += "ItemCategory_2.ItemCategoryName AS ItemCategoryName2, ItemCategory_3.ItemCategoryParentId AS ItemCategoryParentId3, ";
            cmd += "ItemCategory_3.ItemCategoryName AS ItemCategoryName3, ";
            cmd += "CASE IsNull(dbo.Item.ServiceItem ,0) WHEN 0 THEN dbo.Item.WholeWholeSellingPrice ";
            cmd += $"WHEN 1 THEN CASE {ListPriceNO} WHEN 0 THEN dbo.Item.DefaultSellingPrice ";
            cmd += "WHEN 1 THEN dbo.Item.Price1 WHEN 2 THEN dbo.Item.Price2 WHEN 3 THEN dbo.Item.Price3 ";
            cmd += "WHEN 4 THEN dbo.Item.Price4 WHEN 5 THEN dbo.Item.Price5 WHEN 6 THEN dbo.Item.Price6 ";
            cmd += "WHEN 7 THEN dbo.Item.Price7 WHEN 8 THEN dbo.Item.Price8 WHEN 9 THEN dbo.Item.Price9 ";
            cmd += "END END AS ItemPrice FROM dbo.Item INNER JOIN dbo.ItemCategory ON ";
            cmd += "dbo.Item.ItemCategoryId = dbo.ItemCategory.ID INNER JOIN dbo.ItemCategory AS ItemCategory_1 ON ";
            cmd += "dbo.ItemCategory.ItemCategoryParentId = ItemCategory_1.ID INNER JOIN dbo.ItemCategory AS ItemCategory_2 ON ";
            cmd += "ItemCategory_1.ItemCategoryParentId = ItemCategory_2.ID INNER JOIN dbo.ItemCategory AS ItemCategory_3 ON ";
            cmd += "ItemCategory_2.ItemCategoryParentId = ItemCategory_3.ID WHERE (dbo.Item.DeletionDate IS NULL) ";
            cmd += $"And(dbo.Item.ID>={idFrom}) And(dbo.Item.ID<={idTo})";

            DataTable dtItems = ExecCommand(new TamaRequestCmd()
            {
                ClinicId = tamaRequest.clinicId,
                Cmd = cmd,
            });

            return new
            {
                MsgId = 1,
                MsgAr = "تمت العملية بنجاح",
                MsgEn = "Success",
                Data = dtItems.ToDynamic(),
            };
        }

        [Obsolete]
        public static dynamic OpdAppointmentStatistic(TamaRequest tamaRequest, ILogger<MainController> _logger)
        {
            _logger.LogInformation($"XXX_OpdAppointmentStatistic ===> tamaRequest = {JsonConvert.SerializeObject(tamaRequest)}");

            int OpdScheduleId = int.Parse(tamaRequest.paras![0].Value);
            DateTime AppointmentDate = DateTime.Parse(tamaRequest.paras![1].Value).Date;
            if (tamaRequest.clinicId == 0)
            {
                int ClinicNo= int.Parse(tamaRequest.paras![2].Value);
                DataTable dtClinics = ExecSp(new TamaRequest() { spName= "sp_Get_Clinic", paras = [new Para() { Name= "ClinicNo",Value= ClinicNo.ToString(),DataType= SqlDbType.Int }] });
                if (dtClinics.Rows.Count == 1) tamaRequest.clinicId = General.getValueInt(dtClinics.Rows[0],"ID");
            }

            DateTime dateTime = AppointmentDate;
            string cmd = $"SELECT dbo.OrdersDet.OffereDate, dbo.Orders.ComingTime, dbo.Employee.ProvideServiceWhithPay as AutoProvide FROM dbo.OrdersDet INNER JOIN dbo.Orders ON dbo.OrdersDet.OrdersId = dbo.Orders.ID INNER JOIN dbo.OpdSchedule ON dbo.Orders.OpdScheduleId = dbo.OpdSchedule.ID INNER JOIN dbo.Employee ON dbo.OpdSchedule.EmployeeId = dbo.Employee.ID WHERE (dbo.Orders.OpdScheduleId = {OpdScheduleId}) AND CAST(dbo.Orders.OrderDate AS DATE) = '{dateTime.Year}-{dateTime.Month}-{dateTime.Day}'";
            DataTable dtOpdAppointment = ExecCommand(new TamaRequestCmd() { Cmd = cmd,ClinicId= tamaRequest.clinicId });

            float FinishedCount = 0;
            float WaitingCount = 0;
            float NotCommingCount = 0;

            float Waiting = 0;
            float Finished = 0;
            float NotComming = 0;
            bool AutoProvide=false;
            if (dtOpdAppointment.Rows.Count > 0) {
                foreach (DataRow dr in dtOpdAppointment.Rows) {
                    AutoProvide = General.getValueBool(dr, "AutoProvide");
                    if (DBNull.Value != dr["OffereDate"]){
                        FinishedCount++;
                    }
                    else {
                        if (DBNull.Value != dr["ComingTime"])
                            WaitingCount++;
                        else
                            NotCommingCount++;

                    }
                }
                Finished = FinishedCount / dtOpdAppointment.Rows.Count * 100;
                Waiting = WaitingCount / dtOpdAppointment.Rows.Count * 100;
                NotComming = NotCommingCount / dtOpdAppointment.Rows.Count * 100;
            }

            return new
            {
                MsgId = 1,
                MsgAr = "تمت العملية بنجاح",
                MsgEn = "Success",
                Data =new { 
                     Waiting,
                     Finished,
                     NotComming,
                    AutoProvide
                }
            };

        }

        [Obsolete]

        public static int GetUserIdByPhone(string PhoneNo)
        {
            return 1;
        }

        [Obsolete]
        public static dynamic AddAdminToClinic(TamaRequest tamaRequest, ILogger<MainController> _logger)
        {
            _logger.LogInformation($"XXX_AddAdminToClinic ===> tamaRequest = {JsonConvert.SerializeObject(tamaRequest)}");

            string LoginName = tamaRequest.paras![0].Value;

            //جلب بيانات المستخدم من منظومة المصحة
            DataTable dtRemoteClinicUser = ExecCommand(new TamaRequestCmd() { Cmd = $"Select * From Users Where (DeletionDate Is Null) And (LoginName='{LoginName}')", ClinicId = tamaRequest.clinicId});
            if (dtRemoteClinicUser.Rows.Count == 0)
            {
                return new
                {
                    MsgId = 2,
                    MsgAr = "اسم الدخول غير موجود في المصحة",
                    MsgEn = "اسم الدخول غير موجود في المصحة",
                };
            }

            string NameInClinic = General.getValueString(dtRemoteClinicUser.Rows[0],"UserName");
            int RemoteClinicUserId = General.getValueInt(dtRemoteClinicUser.Rows[0], "ID");

            //لدينا رقم الهاتف ونريد
            //UserId
            //من المنظومة الرئيسية
            string PhoneNo = General.checkPhoneNumberOk(tamaRequest.paras![1].Value);
            if(PhoneNo=="")
                return new
                {
                    MsgId = 2,
                    MsgAr = "رقم الهاتف غير صحيح",
                    MsgEn = "رقم الهاتف غير صحيح",
                };

            DataTable dtTamamUser = ExecCommand(new TamaRequestCmd() { Cmd = $"Select * From [User] Where (DeletionDate Is Null) And (PhoneNumber='{PhoneNo}')" });
            if (dtTamamUser.Rows.Count == 0)
            {
                return new
                {
                    MsgId = 2,
                    MsgAr = "رقم الهاتف غير مدرج بالمنظومة",
                    MsgEn = "رقم الهاتف غير مدرج بالمنظومة",
                };
            }

            int UserId = (int)dtTamamUser.Rows[0]["ID"];

            DataTable dtClinicUser = ExecSp(new TamaRequest() { 
                spName= "sp_Ins_ClinicUser",
                paras = [
                    new Para(){Name="UserId", Value=UserId.ToString(),DataType=SqlDbType.Int },
                    new Para(){Name="ClinicId", Value=tamaRequest.clinicId.ToString()!,DataType=SqlDbType.Int  },
                    new Para(){Name="RemoteClinicUserId", Value=RemoteClinicUserId.ToString(),DataType=SqlDbType.Int  },
                    new Para(){Name="NameInClinic", Value=NameInClinic,DataType=SqlDbType.NVarChar },
                    ]
            });
            if (General.getValueInt(dtClinicUser.Rows[0], "MsgId") != 1) return dtClinicUser;

            DataTable dtClinicUserPermission = ExecSp(new TamaRequest()
            {
                spName = "sp_Ins_ClinicUserPermission",
                paras = [
                    new Para(){Name="ClinicUserId", Value=dtClinicUser.Rows[0]["ID"].ToString()!,DataType=SqlDbType.Int },
                    new Para(){Name="PermissionId", Value=1.ToString()!,DataType=SqlDbType.Int  },
                    ]
            });

            //return ;
            return new
            {
                MsgId = 1,
                MsgAr = "تمت العملية بنجاح",
                MsgEn = "Success",
                Data = dtClinicUserPermission.ToDynamic(),
            };
        }

        [Obsolete]
        public static dynamic OrderForAnalysis(TamaRequest tamaRequest, ILogger<MainController> _logger) {
            _logger.LogInformation($"XXX_OrderForAnalysis ===> tamaRequest = {JsonConvert.SerializeObject(tamaRequest)}");

            int customerId = int.Parse(tamaRequest.paras![0].Value);
            int userId = int.Parse(tamaRequest.paras![1].Value);

            DataTable dtSetupTable = ExecSp(new TamaRequest() { spName = "sp_Get_SetupTable", clinicId = tamaRequest.clinicId });

            DataTable dtInPatient = ExecSp(new TamaRequest() { spName = "sp_Get_InPatient", clinicId = tamaRequest.clinicId, paras = [
                new Para() { Name="CustomerId",DataType= SqlDbType.Int,Value= customerId.ToString() },
                ]
            });

            dtInPatient.DefaultView.Sort = "AdmissionDate Desc";
            DataTable sortedDtInPatient = dtInPatient.DefaultView.ToTable();

            int inPatientId = 0;
            if (sortedDtInPatient.Rows.Count!=0) {
                if (sortedDtInPatient.Rows[0].IsNull("DischargeDate"))
                {
                    bool goOut = false;
                    if (!sortedDtInPatient.Rows[0].IsNull("GoOut")) goOut = bool.Parse(sortedDtInPatient.Rows[0]["GoOut"].ToString()!);
                    if (!goOut) inPatientId = int.Parse(sortedDtInPatient.Rows[0]["ID"].ToString()!);
                }else{
                    if (sortedDtInPatient.Rows[0].IsNull("VoucherInId")) {
                        bool InPatient_AddOrderAfterDischarge=false;
                        if (!dtSetupTable.Rows[0].IsNull("InPatient_AddOrderAfterDischarge")) InPatient_AddOrderAfterDischarge = bool.Parse(dtSetupTable.Rows[0]["InPatient_AddOrderAfterDischarge"].ToString()!);
                        if (InPatient_AddOrderAfterDischarge) inPatientId= int.Parse(sortedDtInPatient.Rows[0]["ID"].ToString()!);
                    }
                }
            }

            List<Para> paras = [];
            paras.Add(new Para() { Name = "OrdersTypeId", DataType = SqlDbType.Int, Value = 1.ToString() });
            paras.Add(new Para() { Name = "CustomerId", DataType = SqlDbType.Int, Value = customerId.ToString() });
            if(inPatientId != 0)paras.Add(new Para() { Name = "InPatientId", DataType = SqlDbType.Int, Value = inPatientId.ToString() });
            paras.Add(new Para() { Name = "VersionNo", DataType = SqlDbType.Int, Value = (-1).ToString() });
            paras.Add(new Para() { Name = "UserId", DataType = SqlDbType.Int, Value = userId.ToString() });
            paras.Add(new Para() { Name = "OrderDate", DataType = SqlDbType.DateTime, Value = DateTime.Now.ToString() });

            DataTable dtOrders = ExecSp(new TamaRequest() { spName = "sp_Ins_Orders", clinicId = tamaRequest.clinicId, paras = paras});

            if (int.Parse(dtOrders.Rows[0]["MessageId"].ToString()!) != 0) {
                return new
                {
                    MsgId = 2,
                    MsgAr = dtOrders.Rows[0]["MessageDescription"],
                    MsgEn = dtOrders.Rows[0]["MessageDescription"]
                };
            }

            DataTable dtOrdersDet;

            for (int loopCount = 2; loopCount < tamaRequest.paras.Count; loopCount++) {
                dtOrdersDet = ExecSp(new TamaRequest()
                {
                    spName = "sp_Ins_OrdersDet",
                    clinicId = tamaRequest.clinicId,
                    paras = [
                    new Para(){Name="OrdersId",DataType= SqlDbType.Int,Value= dtOrders.Rows[0]["Id"].ToString()! },
                    new Para(){Name="ItemId",DataType= SqlDbType.Int,Value= tamaRequest.paras[loopCount].Value.ToString() },
                    new Para() { Name="UserId",DataType= SqlDbType.Int,Value= userId.ToString() }
                    ]
                });

                if (int.Parse(dtOrders.Rows[0]["MessageId"].ToString()!) != 0)
                {
                    return new
                    {
                        MsgId = 2,
                        MsgAr = dtOrders.Rows[0]["MessageDescription"],
                        MsgEn = dtOrders.Rows[0]["MessageDescription"]
                    };
                }
            }

            return new
            {
                MsgId = 1,
                MsgAr = "تمت العملية بنجاح",
                MsgEn = "Success"
            };
        }

        [Obsolete]
        public static dynamic OrderForOtherService(TamaRequest tamaRequest, ILogger<MainController> _logger)
        {
            _logger.LogInformation($"XXX_OrderForOtherService ===> tamaRequest = {JsonConvert.SerializeObject(tamaRequest)}");

            int customerId = int.Parse(tamaRequest.paras![0].Value);
            int userId = int.Parse(tamaRequest.paras![1].Value);

            DataTable dtSetupTable = ExecSp(new TamaRequest() { spName = "sp_Get_SetupTable", clinicId = tamaRequest.clinicId });

            DataTable dtInPatient = ExecSp(new TamaRequest()
            {
                spName = "sp_Get_InPatient",
                clinicId = tamaRequest.clinicId,
                paras = [
                new Para() { Name="CustomerId",DataType= SqlDbType.Int,Value= customerId.ToString() },
                ]
            });

            dtInPatient.DefaultView.Sort = "AdmissionDate Desc";
            DataTable sortedDtInPatient = dtInPatient.DefaultView.ToTable();

            int inPatientId = 0;
            if (sortedDtInPatient.Rows.Count != 0)
            {
                if (sortedDtInPatient.Rows[0].IsNull("DischargeDate"))
                {
                    bool goOut = false;
                    if (!sortedDtInPatient.Rows[0].IsNull("GoOut")) goOut = bool.Parse(sortedDtInPatient.Rows[0]["GoOut"].ToString()!);
                    if (!goOut) inPatientId = int.Parse(sortedDtInPatient.Rows[0]["ID"].ToString()!);
                }
                else
                {
                    if (sortedDtInPatient.Rows[0].IsNull("VoucherInId"))
                    {
                        bool InPatient_AddOrderAfterDischarge = false;
                        if (!dtSetupTable.Rows[0].IsNull("InPatient_AddOrderAfterDischarge")) 
                            InPatient_AddOrderAfterDischarge = bool.Parse(dtSetupTable.Rows[0]["InPatient_AddOrderAfterDischarge"].ToString()!);
                        
                        if (InPatient_AddOrderAfterDischarge) 
                            inPatientId = int.Parse(sortedDtInPatient.Rows[0]["ID"].ToString()!);
                    }
                }
            }

            List<Para> paras = [];
            paras.Add(new Para() { Name = "OrdersTypeId", DataType = SqlDbType.Int, Value = 4.ToString() });
            paras.Add(new Para() { Name = "CustomerId", DataType = SqlDbType.Int, Value = customerId.ToString() });
            if (inPatientId != 0) paras.Add(new Para() { Name = "InPatientId", DataType = SqlDbType.Int, Value = inPatientId.ToString() });
            paras.Add(new Para() { Name = "VersionNo", DataType = SqlDbType.Int, Value = (-1).ToString() });
            paras.Add(new Para() { Name = "UserId", DataType = SqlDbType.Int, Value = userId.ToString() });
            paras.Add(new Para() { Name = "OrderDate", DataType = SqlDbType.DateTime, Value = DateTime.Now.ToString() });
            paras.Add(new Para() { Name = "ProvideByUserId", DataType = SqlDbType.Int, Value = userId.ToString() });

            DataTable dtOrders = ExecSp(new TamaRequest() { spName = "sp_Ins_Orders", clinicId = tamaRequest.clinicId, paras = paras });

            if (int.Parse(dtOrders.Rows[0]["MessageId"].ToString()!) != 0)
            {
                return new
                {
                    MsgId = 2,
                    MsgAr = dtOrders.Rows[0]["MessageDescription"],
                    MsgEn = dtOrders.Rows[0]["MessageDescription"]
                };
            }

            DataTable dtOrdersDet;

            for (int loopCount = 2; loopCount < tamaRequest.paras.Count; loopCount++)
            {
                dtOrdersDet = ExecSp(new TamaRequest()
                {
                    spName = "sp_Ins_OrdersDet",
                    clinicId = tamaRequest.clinicId,
                    paras = [
                    new Para(){Name="OrdersId",DataType= SqlDbType.Int,Value= dtOrders.Rows[0]["Id"].ToString()! },
                    new Para(){Name="ItemId",DataType= SqlDbType.Int,Value= tamaRequest.paras[loopCount].Value.ToString() },
                    new Para(){Name="UserId",DataType= SqlDbType.Int,Value= userId.ToString() }
                    ]
                });

                if (int.Parse(dtOrders.Rows[0]["MessageId"].ToString()!) != 0)
                {
                    return new
                    {
                        MsgId = 2,
                        MsgAr = dtOrders.Rows[0]["MessageDescription"],
                        MsgEn = dtOrders.Rows[0]["MessageDescription"]
                    };
                }
            }

            return new
            {
                MsgId = 1,
                MsgAr = "تمت العملية بنجاح",
                MsgEn = "Success"
            };
        }
        [Obsolete]
        public static dynamic ActivateClinic(int clinicId , int yearNo, int monthNo, ILogger<MainController> _logger)
        {

            //_logger.LogInformation($"XXX_ActivateClinic ===> tamaRequest = {JsonConvert.SerializeObject(tamaRequest)}");
            SqlConnection sqlConn = GetSqlConnection()!;
            sqlConn.Open();
            string cmd = $"SELECT * FROM Clinic WHERE (Id = {clinicId})";
            SqlCommand command = new SqlCommand(cmd, sqlConn) { CommandType = CommandType.Text };
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dtClinic = new DataTable();
            adapter.Fill(dtClinic);

            List<Para> paraList;

            

            //التأكد من انه لم يتم تفعيل هذا الشهر مسبقا
            paraList = new List<Para>() {
                 new Para() { Name= "YearNo",DataType= SqlDbType.Int,Value= yearNo.ToString()! },
                 new Para() { Name= "MonthNo",DataType= SqlDbType.Int,Value= monthNo.ToString()! },
            };

            DataTable dtMonthly = ExecSp(new TamaRequest() { spName = "sp_Get_Monthly",clinicId= clinicId, paras = paraList });
            if (dtMonthly.Rows.Count != 0)
            {
                return new
                {
                    MsgId = 2,
                    MsgAr = "تم ترخيص هذا الشهر مسبقا!",
                    MsgEn = "This month is pre-licensed!"
                };
            }

            //التأكد من رصيد المصحة كافي
            //paraList = new List<Para>() {
            //     new Para() { Name= "ClinicId",DataType= SqlDbType.Int,Value= tamaRequest.clinicId.ToString()! },
            //};

            //DataTable dtClinicAccStatement = ExecSp(new TamaRequest() { spName = "sp_Get_ClinicAccStatement", paras = paraList });
            //double Balance = 0;
            //if (dtClinicAccStatement.Rows.Count != 0)
            //{
            //    if (!dtClinicAccStatement.Rows[0].IsNull("Balance"))
            //        Balance = double.Parse(dtClinicAccStatement.Rows[0]["Balance"].ToString()!);
            //}

            //double Value = 0;
            //if (!dtClinic.Rows[0].IsNull("Value"))
            //    Value = double.Parse(dtClinic.Rows[0]["Value"].ToString()!);

            //if (Balance < Value)
            //{
            //    return new
            //    {
            //        MsgId = 2,
            //        MsgAr = "رصيد المحفظة غير كافي!",
            //        MsgEn = "Insufficient wallet balance!"
            //    };
            //}


            string largeCode = GeneratedCode(dtClinic.Rows[0]["ClinicNo"].ToString()!, yearNo, monthNo);


            //Now I have a code to Active Clinic
            paraList = new List<Para>() {
                 new Para() { Name= "MonthNo",DataType= SqlDbType.Int,Value= monthNo.ToString() },
                 new Para() { Name= "YearNo",DataType= SqlDbType.Int,Value= yearNo.ToString() },
                 new Para() { Name= "Code",DataType= SqlDbType.NVarChar,Value= largeCode.ToString() },
                 //new Para() { Name= "UserId",DataType= SqlDbType.Int,Value= getValueOfField("ClinicUserId", tamaRequest.paras!).ToString() },
            };
            
            DataTable dtInsMonthly = ExecSp(new TamaRequest() { spName = "sp_Ins_Monthly" ,clinicId= clinicId,paras= paraList });

            //WalletDebit خصم من المحفظة
            //paraList = new List<Para>() {
            //     new Para() { Name= "ClinicId",DataType= SqlDbType.Int,Value= tamaRequest.clinicId.ToString()! },
            //     new Para() { Name= "AmountD",DataType= SqlDbType.Money,Value=  dtClinic.Rows[0]["Value"].ToString()! },
            //     new Para() { Name= "WalletDebitTypeId",DataType= SqlDbType.Int,Value= "1" },
            //     new Para() { Name= "CreationUserId",DataType= SqlDbType.Int,Value= getValueOfField("UserId", tamaRequest.paras!).ToString() },
            //};

            //DataTable dtInsWalletDebit = ExecSp(new TamaRequest() { spName = "sp_Ins_WalletDebit", paras = paraList });

            //if ((int)dtInsWalletDebit.Rows[0]["MsgId"] != 1)
            //{
            //    _logger.LogInformation($"XXXXXXXXXXX === Denger === XXXXXXXXXXX");
            //    _logger.LogInformation($"تم تفعيل المنظومة ولم يتم الخصم");
            //}

            bool succes = (int)dtInsMonthly.Rows[0]["MessageId"] == 0;
            return new
            {
                MsgId = succes ? 1:2,
                MsgAr = succes ? "succes" : dtInsMonthly.Rows[0]["MessageDescription"],
                MsgEn = succes ? "succes" : dtInsMonthly.Rows[0]["MessageDescription"]
            };

        }

        [Obsolete]
        public static List<AvailableAppointments> GetAvailableAppointments(TamaRequest tamaRequest)
        {
            int EmployeeId = 0;
            if (tamaRequest.paras != null) int.TryParse(getValueOfField("EmployeeId", tamaRequest.paras), out EmployeeId);
            if(EmployeeId == 0) return new List<AvailableAppointments>();

            int MaxMonths = 4;
            SqlConnection sqlConn = GetSqlConnection(clinicId: tamaRequest.clinicId)!;

            sqlConn.Open();

            string cmd = $"SELECT Id,DayId,StartTime FROM OpdSchedule WHERE (EmployeeId = {EmployeeId}) AND (DeletionDate IS NULL)";
            SqlCommand command = new SqlCommand(cmd, sqlConn) { CommandType = CommandType.Text };
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            List<AvailableAppointments> availableAppointmentsList = new List<AvailableAppointments>();
            foreach (DataRow dr in dt.Rows)
            {
                DateTime FirstDateForDayOfWeek = GetFirstDateForDayOfWeek((int)dr["DayId"]);
                availableAppointmentsList.Add(new AvailableAppointments() {
                    appointmentsDate = FirstDateForDayOfWeek, 
                    deleted = false ,
                    startTime =(DateTime)dr["StartTime"],
                    OpdScheduleId= (int)dr["Id"] 
                });
                for(int i=1;i< MaxMonths*4; i++) 
                {
                    availableAppointmentsList.Add(new AvailableAppointments() { 
                        appointmentsDate = FirstDateForDayOfWeek.AddDays(i * 7),
                        deleted = false,
                        startTime = (DateTime)dr["StartTime"],
                        OpdScheduleId = (int)dr["Id"] 
                    });
                } 
            }

            cmd = $"SELECT StopFrom, StopTo FROM StopAppointment WHERE (DeletionDate IS NULL) AND (EmployeeId = {EmployeeId})";
            command = new SqlCommand(cmd, sqlConn) { CommandType = CommandType.Text };
            adapter = new SqlDataAdapter(command);
            dt = new DataTable();
            adapter.Fill(dt);
            foreach (DataRow dr in dt.Rows) 
            {
                DateTime StopFrom = (DateTime)dr["StopFrom"];
                DateTime StopTo = (DateTime)dr["StopTo"];
                foreach (AvailableAppointments availableAppointments in availableAppointmentsList) 
                {
                    if(availableAppointments.appointmentsDate>= StopFrom && availableAppointments.appointmentsDate <= StopTo)
                        availableAppointments.deleted = true;
                }
            }


            DateTime FromNow = DateTime.Now;
            DateTime To2Months = FromNow.AddMonths(MaxMonths);
            cmd = $"SELECT OpdSchedule.MaxPatient, COUNT(Orders.ID) AS AppoinmentCount, Orders.OrderDate ";
            cmd += $"FROM Orders INNER JOIN OpdSchedule ON Orders.OpdScheduleId = OpdSchedule.ID ";
            cmd += $"WHERE(OpdSchedule.EmployeeId = {EmployeeId}) AND(Orders.DeletionDate IS NULL) ";
            cmd += $"AND(Orders.OrderDate >= CONVERT(DATETIME, '{GetFromat(FromNow)}', 102)) ";
            cmd += $"AND(Orders.OrderDate <= CONVERT(DATETIME,'{GetFromat(To2Months)}', 102)) ";
            cmd += $"GROUP BY OpdSchedule.MaxPatient, Orders.OrderDate";

            command = new SqlCommand(cmd, sqlConn) { CommandType = CommandType.Text };
            adapter = new SqlDataAdapter(command);
            dt = new DataTable();
            adapter.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                if ((int)dr["AppoinmentCount"]>=(int)dr["MaxPatient"] ) 
                {
                    DateTime appoinmentDate = (DateTime)dr["OrderDate"];
                    foreach (AvailableAppointments availableAppointments in availableAppointmentsList)
                    {
                        if (availableAppointments.appointmentsDate.Year == appoinmentDate.Year && availableAppointments.appointmentsDate.Month == appoinmentDate.Month && availableAppointments.appointmentsDate.Day == appoinmentDate.Day)
                            availableAppointments.deleted = true;
                    }
                }
            }

            availableAppointmentsList.RemoveAll(item => item.deleted);
            availableAppointmentsList.Sort((item1, item2) => item1.appointmentsDate.CompareTo(item2.appointmentsDate));
            return availableAppointmentsList;
        }
    }
}
