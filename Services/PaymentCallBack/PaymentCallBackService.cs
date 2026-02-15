using System.Data;
using TamaApi.Controllers;
using static TamaApi.Services.db.DoSomeThings;
using static TamaApi.Services.db.DbService;
using static TamaApi.Services.Main.MainService;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using TamaApi.clsMod;
using Newtonsoft.Json;
using TamaApi.Services.Main;
using System.Collections.Generic;
using System;
using System.Data.SqlClient;
using System.Transactions;

namespace TamaApi.Services.PaymentCallBack
{
    public class PaymentCallBackService : IPaymentCallBackService
    {

        static bool checkEnvironmentToStop(DataTable dtPaymentGate)
        {
            bool realEnvironment=true;
            if (dtPaymentGate.Rows[0]["realEnvironment"] != null)
                realEnvironment = (bool)dtPaymentGate.Rows[0]["realEnvironment"];

            bool TestClinic=false;
            if (dtPaymentGate.Rows[0]["TestClinic"] == null || dtPaymentGate.Rows[0]["TestClinic"]==DBNull.Value)
            {
                TestClinic = false;
            }
            else
                TestClinic = (bool)dtPaymentGate.Rows[0]["TestClinic"];
            
            //لم يدفع حقيقتا ويريد ان يشتغل على مصحة حقيقية
            if (realEnvironment == false && TestClinic == false) 
                return true;
            else
                return false;
        }

        [Obsolete]
        static int getTreasury_TamamTreasuryId(DataTable dtPaymentGate, DataTable? dtSetupTable=null) {
            
            int clinicId = (int)dtPaymentGate.Rows[0]["ClinicId"];
            int Treasury_TamamTreasuryId;

            dtSetupTable ??= ExecSp(new TamaRequest(){spName = "sp_Get_Table",paras = [new Para() { Name = "TableName", DataType = SqlDbType.NVarChar, Value = "SetupTable" }],clinicId= clinicId});

            if (dtSetupTable.Rows[0]["Treasury_TamamTreasuryId"] == null || dtSetupTable.Rows[0]["Treasury_TamamTreasuryId"] == DBNull.Value)
            {
                DataTable dtTreasury = ExecSp(new TamaRequest()
                {
                    spName = "sp_Ins_Treasury",
                    clinicId = clinicId,
                    paras = [
                        new Para() { Name = "TreasuryDescription", DataType = SqlDbType.NVarChar, Value = "خزينة التمام" },
                        new Para() { Name = "CurrencyId", DataType = SqlDbType.Int, Value = "1" },
                        new Para() { Name = "CompanyAccId", DataType = SqlDbType.Int, Value = "1" }
                        ]
                });

                if ((int)dtTreasury.Rows[0]["MessageId"] != 0) 
                    return 0;
                else{
                    Treasury_TamamTreasuryId = (int)General.getValueDecimal(dtTreasury.Rows[0],"ID");
                    string cmd = $"Update SetupTable Set Treasury_TamamTreasuryId = {Treasury_TamamTreasuryId}";
                    ExecCommand(new TamaRequestCmd() { Cmd = cmd, ClinicId = clinicId });
                }
            }
            else {
                Treasury_TamamTreasuryId = (int)dtSetupTable.Rows[0]["Treasury_TamamTreasuryId"];
            }

            return Treasury_TamamTreasuryId;
        }
        
        [Obsolete]
        static int getContractCo_TamamID(DataTable dtPaymentGate, DataTable? dtSetupTable = null)
        {
            int clinicId = (int)dtPaymentGate.Rows[0]["ClinicId"];
            int ContractCo_TamamID;

            dtSetupTable ??= ExecSp(new TamaRequest() { spName = "sp_Get_Table", paras = [new Para() { Name = "TableName", DataType = SqlDbType.NVarChar, Value = "SetupTable" }], clinicId = clinicId });

            if (dtSetupTable.Rows[0]["ContractCo_TamamID"] == null || dtSetupTable.Rows[0]["ContractCo_TamamID"] == DBNull.Value)
            {
                DataTable dtContractCo = ExecSp(new TamaRequest()
                {
                    spName = "sp_Ins_ContractCo",
                    clinicId = clinicId,
                    paras = [
                        new Para() { Name = "ContractCoDescription", DataType = SqlDbType.NVarChar, Value = "شركة النماء" },
                        new Para() { Name = "ContractCoEnDesc", DataType = SqlDbType.NVarChar, Value = "Nama Co" },
                        new Para() { Name = "ContractCoParentId", DataType = SqlDbType.Int, Value = "1" },
                        new Para() { Name = "DigitsUnderIt", DataType = SqlDbType.Int, Value = "1" },
                        new Para() { Name = "CurrencyId", DataType = SqlDbType.Int, Value = "1" },
                        new Para() { Name = "ForceIns", DataType = SqlDbType.Int, Value = "1" },
                        ]
                });

                if ((int)dtContractCo.Rows[0]["MessageId"] != 0)
                    return 0;
                else
                {
                    ContractCo_TamamID = General.getValueInt(dtContractCo.Rows[0], "ID");
                    string cmd = $"Update SetupTable Set ContractCo_TamamID = {ContractCo_TamamID}";
                    ExecCommand(new TamaRequestCmd() { Cmd = cmd, ClinicId = clinicId });
                }
            }
            else
            {
                ContractCo_TamamID = (int)dtSetupTable.Rows[0]["ContractCo_TamamID"];
            }

            return ContractCo_TamamID;
        }
        
        [Obsolete]
        public async Task<dynamic> PaymentCallBack(ClsPaymentCallBack callBack, ILogger<MainController> _logger)
        {
            if (callBack.custom_ref == null) return new
            {
                MsgId = 2,
                MsgAr = "There is no custom_ref",
                MsgEn = "There is no custom_ref"
            };

            _logger.LogInformation($"callBack.custom_ref == {callBack.custom_ref}");

            callBack.custom_ref = callBack.custom_ref.Replace("@!@!@Tamam@!@!@_", "");
            callBack.custom_ref = MainService.DecryptInvoiceNo(callBack.custom_ref, "TamamKeyEncrypt");

            int PaymentGateId;
            PaymentGateId = int.Parse(callBack.custom_ref);
            _logger.LogInformation($"PaymentGateId  == {PaymentGateId}");

           

            DataTable dtPaymentGate = ExecCommand(new TamaRequestCmd() {
                Cmd = $"SELECT PaymentGate.*, Clinic.TestClinic, [User].PhoneNumber,Clinic.MoneyToNamaCo FROM PaymentGate INNER JOIN Clinic ON PaymentGate.ClinicId = Clinic.ID INNER JOIN [User] ON PaymentGate.CreationUserId = [User].ID WHERE (PaymentGate.ID = {PaymentGateId})"
            });
            int clinicId = (int)dtPaymentGate.Rows[0]["ClinicId"];
            string Cmd = $"SELECT * FROM Clinic WHERE (Id = {clinicId})";
            DataTable clinic = ExecCommand(new TamaRequestCmd() { Cmd = Cmd });

            if (dtPaymentGate.Rows.Count == 0) return new
            {
                MsgId = 2,
                MsgAr = "There is no custom_ref",
                MsgEn = "There is no custom_ref"
            };

            _logger.LogInformation($"callBack.PhoneNumber == {dtPaymentGate.Rows[0]["PhoneNumber"]}");

            bool Finished = General.getValueBool(dtPaymentGate.Rows[0], "Finished");// (dtPaymentGate.Rows[0]["Finished"] == DBNull.Value) ? false : (bool)dtPaymentGate.Rows[0]["Finished"];
            
            if (Finished) {
                _logger.LogError($"XXXXXXXXXXXX Finished Record XXXXXXXXXXXXXX");
                return new
                {
                    MsgId = 2,
                    MsgAr = "It Already Finished",
                    MsgEn = "It Already Finished"
                };
            }

            //التأكد من وضع البيئة التجربية
            if (checkEnvironmentToStop(dtPaymentGate))
            {
                _logger.LogError($"XXXXXXXXXXXX Paid testEnviroment Appointment To Real Clinic XXXXXXXXXXXXXX");
                return new
                {
                    MsgId = 2,
                    MsgAr = "Paid testEnviroment Appointment To Real Clinic",
                    MsgEn = "Paid testEnviroment Appointment To Real Clinic"
                };
            }

            bool moneyToNamaCo = General.getValueBool(dtPaymentGate.Rows[0], "MoneyToNamaCo");


            switch (dtPaymentGate.Rows[0]["PaymentGateTypeId"]) {
                case (int)EnumPaymentGateType.walletRecharge + 1:
                    ExecSp(new TamaRequest()
                    {
                        spName = "sp_Tran_PayingCardXXX",
                        paras = [new Para() { Name = "PaymentGateId", Value = PaymentGateId.ToString(), DataType = SqlDbType.Int }]
                    });

                    break;

                case (int)EnumPaymentGateType.patientAppointment + 1:
                    //تم الدفع
                    ExecCommand(new TamaRequestCmd() { Cmd = $"Update PaymentGate Set Paid=1 Where Id={PaymentGateId}"});

                    DataTable dtPayGateAppointment = ExecSp(new TamaRequest()
                    {
                        spName = "sp_Get_PayGateAppointment",
                        paras = [new Para() { Name = "PaymentGateId", DataType = SqlDbType.Int, Value = PaymentGateId.ToString() }]
                    });

                    int patientId = (int)dtPayGateAppointment.Rows[0]["PatientId"];

                    //الحجز في قاعدة بيانات المصحة
                    DataTable dtClinicPatient = ExecCommand(new TamaRequestCmd() { Cmd = $"Select * From ClinicPatient Where PatientId={patientId} And ClinicId={clinicId}" });
                    int CustomerId = 0;
                    if (dtClinicPatient.Rows.Count > 0) {
                        CustomerId = (dtClinicPatient.Rows[0]["RemoteCustomerId"] == DBNull.Value)?0:(int)dtClinicPatient.Rows[0]["RemoteCustomerId"];
                    }

                    List<Para> paras = [
                        new Para() { Name = "OpdScheduleId", DataType = SqlDbType.Int, Value =((int)dtPayGateAppointment.Rows[0]["RemoteOpdScheduleId"]).ToString() },
                        new Para() { Name = "AppointmentDate",DataType= SqlDbType.DateTime,Value= ((DateTime)dtPayGateAppointment.Rows[0]["AppointmentDate"]).ToString()},
                        new Para() { Name = "Frst_Name", DataType = SqlDbType.NVarChar, Value = (string)dtPayGateAppointment.Rows[0]["FrstName"] },
                        new Para() { Name = "F_Name", DataType = SqlDbType.NVarChar, Value = (string)dtPayGateAppointment.Rows[0]["FName"]},
                        new Para() { Name = "S_Name", DataType = SqlDbType.NVarChar, Value = (string)dtPayGateAppointment.Rows[0]["SName"]},
                        new Para() { Name = "PaidAmount", DataType = SqlDbType.NVarChar, Value = (Convert.ToDouble( dtPayGateAppointment.Rows[0]["Amount"])).ToString()},
                        new Para() { Name = "VersionNo", DataType = SqlDbType.Int, Value = (-1).ToString() },
                        new Para() { Name = "TamamPatientId", DataType = SqlDbType.Int, Value = patientId.ToString() },
                        new Para() { Name = "MoneyToNamaCo", DataType = SqlDbType.Bit, Value = moneyToNamaCo.ToString() },
                    ];

                    if(CustomerId!=0) paras.Add(new Para() {Name="CustomerId", DataType=SqlDbType.Int, Value=CustomerId.ToString()});

                    DataTable dtInsOpdAppointment=ExecSp(new TamaRequest() 
                    {spName="sp_Ins_OpdAppointment",clinicId= clinicId, paras= paras});
                    
                    if ((int)dtInsOpdAppointment.Rows[0]["MessageId"] != 0)
                    {
                        _logger.LogError($"XXXXXXXXXXXX Error sp_Ins_OpdAppointment XXXXXXXXXXXXXX");
                        _logger.LogError($"{JsonConvert.SerializeObject(dtInsOpdAppointment)}");
                        return new
                        {
                            MsgId = 2,
                            MsgAr = "Error sp_Ins_OpdAppointment",
                            MsgEn = "Error sp_Ins_OpdAppointment"
                        };
                    }

                    //تم انهاء المطلوب وهو الحجز
                    ExecCommand(new TamaRequestCmd() { Cmd = $"Update PaymentGate Set Finished=1 Where Id={PaymentGateId}"});
                    break;

                case (int)EnumPaymentGateType.orderPayment + 1:
                    //تم الدفع
                    ExecCommand(new TamaRequestCmd() { Cmd = $"Update PaymentGate Set Paid=1 Where Id={PaymentGateId}" });
                    
                    DataTable dtSetupTable = ExecSp(new TamaRequest() { spName = "sp_Get_SetupTable", clinicId = clinicId });
                    //التأكد من ان هناك خزينة للتمام
                    //والا سيتم فتحها
                    int Treasury_TamamTreasuryId = getTreasury_TamamTreasuryId(dtPaymentGate, dtSetupTable);
                    //التأكد من ان هناك شركة للنماء
                    //والا سيتم فتحها
                    int ContractCo_TamamID = getContractCo_TamamID(dtPaymentGate, dtSetupTable);

                    //get OrderDetIds
                    string cmd = $"Select * From PayGateOrderDetIds Where PaymentGateId={PaymentGateId}";
                    DataTable dtPayGateOrderDetIds = ExecCommand(new TamaRequestCmd() { Cmd = cmd});
                    
                    //getData From Order To Make VoucherIn
                    cmd = $"SELECT dbo.OrdersDet.ID,dbo.OrdersDet.OrdersId,dbo.Item.EItemName,dbo.Item.ItemName,dbo.Item.DefaultSellingPrice,ISNULL(dbo.Orders.AppointNumber, 0) AS AppointNumber,dbo.Orders.OpdScheduleId,dbo.Orders.OrderDate, dbo.Orders.OrdersTypeId, dbo.Orders.CustomerId, dbo.OrdersType.OrdersTypeDescription, dbo.Item.PercentageOfferer, ISNULL(dbo.Item.FixedOfferer, 0)  AS FixedOfferer, dbo.Employee.Reception_NumberingWhen, dbo.Employee.UserId, dbo.Employee.ProvideServiceWhithPay,dbo.Employee.EmpFirstName, dbo.Employee.EmpFatherName, dbo.Employee.EmpSurName FROM dbo.Employee INNER JOIN dbo.OpdSchedule ON dbo.Employee.ID = dbo.OpdSchedule.EmployeeId RIGHT OUTER JOIN dbo.OrdersDet INNER JOIN dbo.Orders ON dbo.OrdersDet.OrdersId = dbo.Orders.ID INNER JOIN dbo.Customer ON dbo.Orders.CustomerId = dbo.Customer.ID INNER JOIN dbo.OrdersType ON dbo.Orders.OrdersTypeId = dbo.OrdersType.ID INNER JOIN dbo.Item ON dbo.OrdersDet.ItemId = dbo.Item.ID ON dbo.OpdSchedule.ID = dbo.Orders.OpdScheduleId";
                    string where = " WHERE ";
                    bool first = true;
                    foreach (DataRow dr in dtPayGateOrderDetIds.Rows) {
                        if (first)
                            first = false;
                        else
                            where = where + " Or ";

                        where = where + $"(dbo.OrdersDet.ID ={(int)dr["RemoteOrderDetId"]})";
                    }
                    cmd = cmd + where;
                    DataTable dtOrdersDet = ExecCommand(new TamaRequestCmd() { Cmd=cmd,ClinicId = clinicId });

                    //وصف الايصال
                    string VoucherDescription= General.getValueString(dtOrdersDet.Rows[0],"OrdersTypeDescription");
                    if (dtOrdersDet.Rows.Count == 1) { 
                        VoucherDescription = General.getValueString(dtOrdersDet.Rows[0],"ItemName");
                        if(VoucherDescription=="") VoucherDescription = General.getValueString(dtOrdersDet.Rows[0], "EItemName");
                    }
                    else if (dtOrdersDet.Rows.Count == 2) {
                        VoucherDescription = General.getValueString(dtOrdersDet.Rows[0],"ItemName");
                        if (VoucherDescription == "") VoucherDescription = General.getValueString(dtOrdersDet.Rows[0], "EItemName");
                        string x= General.getValueString(dtOrdersDet.Rows[1], "ItemName");
                        if (x == "") x = General.getValueString(dtOrdersDet.Rows[1], "EItemName");
                        VoucherDescription = VoucherDescription + " == " + x;
                    }
                    //getVoucherNum
                    DataTable dtVoucherNum = ExecSp(new TamaRequest() { spName = "sp_Get_FreeVoucherInNumber", paras = [new Para() { Name = "TreasuryId", DataType = SqlDbType.Int, Value = Treasury_TamamTreasuryId.ToString() }], clinicId = clinicId });
                    int VoucherInNumer = (int)dtVoucherNum.Rows[0]["FreeNumber"];
                    decimal Amount = (decimal)dtPaymentGate.Rows[0]["Amount"];

                    //get VoucherInTypeId
                    int VoucherInTypeId=0;
                    switch (General.getValueInt(dtOrdersDet.Rows[0],"OrdersTypeId")) {
                        case 1://تحاليل
                            VoucherInTypeId =General.getValueInt(dtSetupTable.Rows[0],"Treasury_RevenueLABVoucherInTypeId");
                            break;
                        case 2://اشعة
                            VoucherInTypeId = General.getValueInt(dtSetupTable.Rows[0],"Treasury_RevenueRADVoucherInTypeId");
                            break;
                        case 4://خدمات اخرى
                            VoucherInTypeId = General.getValueInt(dtSetupTable.Rows[0], "Treasury_RevenuePubServceVoucherInTypeId");
                            break;
                        case 5://كشوفات
                            VoucherInTypeId = General.getValueInt(dtSetupTable.Rows[0], "Treasury_RevenueOPDVoucherInTypeId");
                            VoucherDescription += " == " + General.getValueString(dtOrdersDet.Rows[0], "EmpFirstName") + " " + General.getValueString(dtOrdersDet.Rows[0], "EmpSurName") ;
                            break;
                        //case 8://عمليات جراحية
                        //    VoucherInTypeId = (int)dtSetupTable.Rows[0]["Treasury_RevenueLABVoucherInTypeId"];
                        //    break;
                        //case 9://تحاليل انسجة
                        //    VoucherInTypeId = (int)dtSetupTable.Rows[0]["Treasury_RevenueLABVoucherInTypeId"];
                        //    break;
                    }
                    //Insert Voucher
                    string dateTimeNow = General.getDateTimeFromat(DateTime.Now);
                    cmd = $"Insert Into VoucherIn(TreasuryId,VoucherInTypeId,VoucherNumber,VoucherDate,VoucherAmount,TotalServiceAmount,CustomerId,VoucherDescription,Confirmed,ContractCoId,ClinicTreasury,CreationDate)Values(";
                    cmd += $"{Treasury_TamamTreasuryId},";//TreasuryId
                    cmd += $"{VoucherInTypeId},";//VoucherInTypeId
                    cmd += $"{VoucherInNumer},";//VoucherNumber
                    cmd += $"'{dateTimeNow}',";//VoucherDate
                    cmd += $"{0},";//VoucherAmount
                    cmd += $"{Amount},";//TotalServiceAmount
                    cmd += $"{dtOrdersDet.Rows[0]["CustomerId"]},";//CustomerId
                    cmd += $"'{VoucherDescription}',";//VoucherDescription
                    cmd += $"{1},";//Confirmed
                    cmd += $"{ContractCo_TamamID},";//ContractCoId
                    cmd += $"{1},"; //ClinicTreasury
                    cmd += $"'{dateTimeNow}')";//CreationDate

                    SqlConnection sqlConn = GetSqlConnection(clinicId: clinicId)!;
                    sqlConn.Open();
                    //SqlTransaction transaction;//= sqlConn.BeginTransaction();

                    int voucherInId = ExecCommandIns(new TamaRequestCmd() { Cmd = cmd, ClinicId = clinicId }, sqlConn);
                    //transaction.Commit();

                    int OrdersId = General.getValueInt(dtOrdersDet.Rows[0], "OrdersId");
                    
                    //Update OrderDet
                    string cmdAdd;
                    foreach (DataRow dr in dtOrdersDet.Rows) {
                        cmdAdd = "";
                        //تقديم الخدمات اليا في حالة تقديم الي للطبيب
                        if (General.getValueBool(dr,"ProvideServiceWhithPay")){
                            int userId = General.getValueInt(dr, "UserId");
                            if(userId!=0)cmdAdd = $",OffereDate=GetDate(),UserOffererId={userId}";
                        }
                        cmd = $"Update OrdersDet Set " +
                                    $"PriceWhenPay={General.getValueDecimal(dr, "DefaultSellingPrice")}," +
                                    $"PercentWhenPay={General.getValueDecimal(dr,"PercentageOfferer")}," +
                                    $"FixedOfferWhenPay={General.getValueBoolZeroOne(dr,"FixedOfferer")}," +
                                    $"VoucherInId={voucherInId} " + cmdAdd +
                            $"Where ID={General.getValueInt(dr, "ID")}";
                        ExecCommand(new TamaRequestCmd() { Cmd = cmd, ClinicId = clinicId },sqlConn);

                        ////في حالة دفع كشف لازم التأكد من الترقيم
                        if (General.getValueInt(dr,"Reception_NumberingWhen") == 3 && General.getValueInt(dr, "AppointNumber") == 0){
                            int AppointNumber = ExecCommandFunction(new TamaRequest(){
                                clinicId = clinicId,
                                spName = "Select dbo.Fun_AppointmentNo(@OpdScheduleId,@AppointmentDate)",
                                paras = [
                                    new Para() { Name= "OpdScheduleId", Value = General.getValueInt(dr,"OpdScheduleId").ToString(), DataType = SqlDbType.Int },
                                    new Para() { Name= "AppointmentDate", Value =General.getDateTimeFromat((DateTime)dr["OrderDate"]), DataType = SqlDbType.DateTime},
                                ]
                                });

                            if (AppointNumber != 0) { 
                                cmd = $"Update Orders Set AppointNumber={AppointNumber} Where Id={OrdersId}";
                                ExecCommand(new TamaRequestCmd() { Cmd = cmd, ClinicId = clinicId }, sqlConn);
                            }

                        }
                    }

                    //التأكد من سداد جميع الطلبات
                    cmd = $"Select * From OrdersDet Where OrdersId ={OrdersId} And VoucherInId is null";
                    DataTable dtAllOrderDet = ExecCommand(new TamaRequestCmd() { Cmd= cmd, ClinicId = clinicId }, sqlConn);
                    if (dtAllOrderDet.Rows.Count == 0) {
                        cmd = $"Update Orders Set Paid=1 Where Id={OrdersId}";
                        ExecCommand(new TamaRequestCmd() { Cmd = cmd, ClinicId = clinicId }, sqlConn);
                    }
                    //transaction.Commit();
                    sqlConn.Close();

                    //تم انهاء المطلوب 
                    ExecCommand(new TamaRequestCmd() { Cmd = $"Update PaymentGate Set Finished=1 Where Id={PaymentGateId}" });
                    break;
                
                case (int)EnumPaymentGateType.activeClinic + 1: 
                    //تم الدفع
                    ExecCommand(new TamaRequestCmd() { Cmd = $"Update PaymentGate Set Paid=1 Where Id={PaymentGateId}" });
                    //تنفيذ المطلوب
                    DataTable dtPayGateActiveClinic = ExecSp(new TamaRequest()
                    {
                        spName = "sp_Get_PayGateActiveClinic",
                        paras = [new Para() { Name = "PaymentGateId", DataType = SqlDbType.Int, Value = PaymentGateId.ToString() }]
                    });

                    if (dtPayGateActiveClinic != null && dtPayGateActiveClinic.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtPayGateActiveClinic.Rows)
                        {
                            ActivateClinic(
                                (int)row["ClinicId"],
                                (int)row["YearNo"],
                                (int)row["MonthNo"],
                                _logger
                            );
                        }
                    }
                    //تم انهاء المطلوب وهو تفعيل الشهر 
                    ExecCommand(new TamaRequestCmd() { Cmd = $"Update PaymentGate Set Finished=1 Where Id={PaymentGateId}" });
                    break;

                case (int)EnumPaymentGateType.subscribeMachine + 1:
                    //تم الدفع
                    ExecCommand(new TamaRequestCmd() { Cmd = $"Update PaymentGate Set Paid=1 Where Id={PaymentGateId}" });
                    //تنفيذ المطلوب
                    DataTable dtPayGateMachine = ExecSp(new TamaRequest()
                    {
                        spName = "sp_Get_PayGateMachine",
                        paras = [new Para() { Name = "PaymentGateId", DataType = SqlDbType.Int, Value = PaymentGateId.ToString() }]
                    });
                    string ExDate = Convert.ToString(dtPayGateMachine.Rows[0]["ExDate"])!;
                    int MachineId = Convert.ToInt32(dtPayGateMachine.Rows[0]["MachineId"]);
                    bool Com = Convert.ToBoolean(dtPayGateMachine.Rows[0]["Com"]);
                    SubscribeMachine(clinicId, ExDate, MachineId,Com);
                    //تم انهاء المطلوب وهو تمديد مدة اشتراك الاجهزة 
                    ExecCommand(new TamaRequestCmd() { Cmd = $"Update PaymentGate Set Finished=1 Where Id={PaymentGateId}" });
                    break;

                case (int)EnumPaymentGateType.subscribeWhatsApp + 1:
                    subscribeMessage(true);
                    break;
                case (int)EnumPaymentGateType.subscribeSms + 1:
                    subscribeMessage(false);
                    break;
            }
            
            return new
            {
                MsgId = 1,
                MsgAr = "Ok",
                MsgEn = "Ok"
            };
            void subscribeMessage(bool whatsApp)
            {
                ExecCommand(new TamaRequestCmd() { Cmd = $"Update PaymentGate Set Paid=1 Where Id={PaymentGateId}" });


                //تنفيذ المطلوب
                dtPaymentGate = ExecCommand(new TamaRequestCmd()
                {
                    Cmd = $"SELECT * From PaymentGate WHERE (PaymentGate.ID = {PaymentGateId})"
                });
                double balance = whatsApp
                    ? (clinic.Rows[0]["WhatsAppBalance"] == DBNull.Value ? 0 : Convert.ToDouble(clinic.Rows[0]["WhatsAppBalance"]))
                    : (clinic.Rows[0]["SmsBalance"] == DBNull.Value ? 0 : Convert.ToDouble(clinic.Rows[0]["SmsBalance"]));
                balance += Convert.ToDouble(dtPaymentGate.Rows[0]["Amount"]);
                ExecCommand(new TamaRequestCmd() { 
                    Cmd = whatsApp ? $"Update Clinic Set WhatsAppBalance={balance} Where Id={clinicId}" 
                    :$"Update Clinic Set SmsBalance={balance} Where Id={clinicId}" 
                });
                //تم انهاء المطلوب وهو تعبئة رصيد الواتساب 
                ExecCommand(new TamaRequestCmd() { Cmd = $"Update PaymentGate Set Finished=1 Where Id={PaymentGateId}" });
            }
        }
        

    }
}
