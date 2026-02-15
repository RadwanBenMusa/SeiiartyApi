using System.Data;
using System.Data.SqlClient;
using System.Management;
using TamaApi.clsMod;
using TamaApi.Controllers;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace TamaApi.Services.db
{
    public class DbService
    {
        const string mainDbName = "Tamam";

#if DEBUG
        const string dbInstance = "tamam.ly\\Sql2008";
#else
        const string dbInstance = ".\\Sql2008";
#endif
        const string connStr = "user id=sa;password=195205085;Max Pool Size=20000;Pooling=true";


     
        public class ClsWhatsAppMsgAnalysisRes
        {
            public required int ClinicNo { get; set; }
            public required string FileName { get; set; }
            public string? OtherInfo { get; set; }
            public required string customerPhoneNo { get; set; }
        }

        public class ClsSmsLinkAnalysisRes
        {
            public required int ClinicNo { get; set; }
            public required string FileName { get; set; }
            public string? OtherInfo { get; set; }
            public required string customerPhoneNo { get; set; }
        }

        public class ClsWhatsAppMsg
        {
            public required int clinicNo { get; set; }
            public required string budy { get; set; }
            public string? OtherInfo { get; set; }
            public required string customerPhoneNo { get; set; }
        }

      
        public class TamaRequest
        {
            public required string spName { get; set; }
            public int? clinicId {  set; get; }
            public List<Para>? paras { get; set; }
        }


        public class TamaRequestCmd
        {
            public required string Cmd { get; set; }
            public int? ClinicId { get; set; }
        }
        public class Para
        {
            public required string Name { get; set; }
            public SqlDbType DataType { get; set; }
            public required string Value { get; set; }
        }

        [Obsolete]
        private static string GetDbInstanceOfClinic(int? clinicId) 
        {
            dynamic clinic = GetTable(tableName:"Clinic",IntFN:"ID",IntFV:clinicId).ToDynamic();
            string dbInstance = "";
            if (clinic[0].IpAddress is DBNull)
                throw new NotImplementedException("No Ip For This Clinic");

            dbInstance = clinic[0].IpAddress;
            if (clinic[0].DbInstance != DBNull.Value) dbInstance += "\\" + clinic[0].DbInstance;
            return dbInstance;
        }

        [Obsolete]
        public static SqlConnection? GetSqlConnection(TamaRequest? tamaRequest=null,int? clinicId = null)
        {
            if (tamaRequest != null)
                clinicId = tamaRequest.clinicId;

            string _dbName = (clinicId==null) ? mainDbName:"Erp";
            string _dbInstance = (clinicId == null) ? dbInstance : GetDbInstanceOfClinic(clinicId);


            string strSqlConn = $"server={_dbInstance};";
            strSqlConn += $"database={_dbName};";
            strSqlConn += connStr;

            return new SqlConnection(strSqlConn);
        }


        private static object getValue(Para para) 
        {
            object data;
            if (para.DataType == SqlDbType.DateTime)
                data = Convert.ToDateTime(para.Value);
            else if (para.DataType == SqlDbType.Bit)
                if (para.Value == "1" || para.Value == "true" || para.Value == "True" || para.Value == "-1")
                    data = true;
                else
                    data = false;
            else
                data = para.Value;
            
            return data;
        }

        [Obsolete]
        private static SqlParameter CreatePara(Para para)
        {
            SqlParameter workParam = new SqlParameter(para.Name, para.DataType);
            workParam.Direction = ParameterDirection.Input;
            workParam.Value = getValue(para);
            //if (para.DataType == SqlDbType.DateTime)
            //    workParam.Value = Convert.ToDateTime(para.Value);
            //else if (para.DataType == SqlDbType.Bit)
            //    if (para.Value == "1" || para.Value == "true" || para.Value == "True" || para.Value == "-1")
            //        workParam.Value = true;
            //    else
            //        workParam.Value = false;
            //else
            //    workParam.Value = para.Value;
            return (workParam);
        }

        [Obsolete]
        public static DataTable ExecSp(TamaRequest tamaRequest)
        {
            SqlConnection sqlConn = GetSqlConnection(tamaRequest)!;

            sqlConn.Open();

            SqlDataAdapter da;
            SqlParameter workParam;
            DataSet ds;
            DataTable dt;

            da = new SqlDataAdapter(tamaRequest.spName, sqlConn);
            //if (itsTransaction) da.SelectCommand.Transaction = Trans;
            da.SelectCommand.CommandType = CommandType.StoredProcedure;

            if (tamaRequest.paras != null)
            {
                for (int i = 0; i <= tamaRequest.paras.Count - 1; i++)
                {
                    workParam = CreatePara(tamaRequest.paras[i]);
                    da.SelectCommand.Parameters.Add(workParam);
                }
            }
            ds = new DataSet();
            da.Fill(ds, "TableData");
            dt = ds.Tables["TableData"]!;

            return dt;
        }

        [Obsolete]
        public static dynamic ExecDoSomeThings(TamaRequest tamaRequest, ILogger<MainController> logger)
        {
            dynamic result = "";
            switch (tamaRequest.spName)
            {
                case "AddAdminToClinic":
                    return DoSomeThings.AddAdminToClinic(tamaRequest, logger);
                case "GetAvailableAppointments":
                    return DoSomeThings.GetAvailableAppointments(tamaRequest);
                case "ActivateClinic":
                    return DoSomeThings.ActivateClinic(0,0,0, logger);
                case "OrderForAnalysis":
                    return DoSomeThings.OrderForAnalysis(tamaRequest, logger);
                case "OrderForOtherService":
                    return DoSomeThings.OrderForOtherService(tamaRequest, logger);
                case "OpdAppointmentStatistic":
                    return DoSomeThings.OpdAppointmentStatistic(tamaRequest, logger);
                case "GetItemsForCompanyCo":
                    return DoSomeThings.GetItemsForCompanyCo(tamaRequest, logger);
                case "GetVoucherCompanyCo":
                    return DoSomeThings.GetVoucherCompanyCo(tamaRequest, logger);
                default:
                    break;
            }
            return result;
        }

        [Obsolete]
        public static DataTable ExecCommand(TamaRequestCmd tamaRequestCmd, SqlConnection? sqlConn = null, SqlTransaction? transaction = null)
        {
            SqlCommand command;
            bool sqlConnClose = true;
            if (sqlConn == null)
            {
                sqlConnClose = false;
                sqlConn = GetSqlConnection(clinicId: tamaRequestCmd.ClinicId)!;
                sqlConn.Open();
                command = new SqlCommand(tamaRequestCmd.Cmd, sqlConn) { CommandType = CommandType.Text };
            }
            else
                command = new SqlCommand(tamaRequestCmd.Cmd, sqlConn, transaction) { CommandType = CommandType.Text };


            //SqlCommand command = new SqlCommand(tamaRequestCmd.Cmd, sqlConn) { CommandType = CommandType.Text };

            SqlDataAdapter adapter = new SqlDataAdapter(command);

            DataTable dt=new DataTable();
            adapter.Fill(dt);
            
            if (sqlConnClose) sqlConn.Close();
            
            return dt;
        }

        [Obsolete]
        public static int ExecCommandIns(TamaRequestCmd tamaRequestCmd, SqlConnection? sqlConn=null, SqlTransaction? transaction=null)
        {
            SqlCommand command;
            bool sqlConnClose=true;
            if (sqlConn == null) { 
                sqlConnClose = false;
                sqlConn = GetSqlConnection(clinicId: tamaRequestCmd.ClinicId)!;
                sqlConn.Open();
                command = new SqlCommand(tamaRequestCmd.Cmd + " SELECT SCOPE_IDENTITY();", sqlConn) { CommandType = CommandType.Text };
            }else
                command = new SqlCommand(tamaRequestCmd.Cmd + " SELECT SCOPE_IDENTITY();", sqlConn) { CommandType = CommandType.Text };


            int newId = 0;

            object result;
            result = command.ExecuteScalar();

            newId = Convert.ToInt32(result);

            if(sqlConnClose) sqlConn.Close();

            return newId; 
        }

        [Obsolete]
        public static int ExecCommandFunction(TamaRequest tamaRequest)
        {
            SqlConnection sqlConn = GetSqlConnection(clinicId: tamaRequest.clinicId)!;

            sqlConn.Open();

            using (SqlCommand command = new SqlCommand(tamaRequest.spName, sqlConn)){ 
                command.CommandType = CommandType.Text ;
                // إضافة المعاملات للدالة
                if (tamaRequest.paras != null) 
                    foreach(Para para in tamaRequest.paras)
                        command.Parameters.AddWithValue($"@{para.Name}", para.Value);
                object scalarResult = command.ExecuteScalar();
                if (scalarResult != null && scalarResult != DBNull.Value)
                {
                    return Convert.ToInt32(scalarResult);
                }
            }
            return 0;
        }

        [Obsolete]
        public static DataTable GetTable(
            string? tableName = "",
            string? IntFN = "",
            int? IntFV = 0,
            string? StrFN = "",
            string? StrFV = "",
            bool? DeletionDateIsNull = null,
            string? OtherT1 = "",
            string? OtherT1F1 = "",
            GetTableCP? getTableCP = null)
        {
            TamaRequest tamaRequest = new TamaRequest() { spName= "sp_Get_Table", paras = [] };

            if (getTableCP != null) 
            {
                tableName= getTableCP.TableName;
                IntFN = getTableCP.IntFN;
                IntFV = getTableCP.IntFV;
                StrFN = getTableCP.StrFN;
                StrFV = getTableCP.StrFV;
                DeletionDateIsNull= getTableCP.DeletionDateIsNull;
                OtherT1 = getTableCP.OtherT1;
                OtherT1F1= getTableCP.OtherT1F1;
            }

            tamaRequest.paras!.Add(new Para() { Name = "TableName", DataType = SqlDbType.NVarChar, Value = tableName! });
            if (IntFV != 0 && IntFV !=null) 
            {
                tamaRequest.paras.Add(new Para() { Name = "IntFN", DataType = SqlDbType.NVarChar, Value = IntFN! });
                tamaRequest.paras.Add(new Para() { Name = "IntFV", DataType = SqlDbType.Int, Value = IntFV!.ToString()! });
            }
            if (StrFV != "" && StrFV != null)
            {
                tamaRequest.paras.Add(new Para() { Name = "StrFN", DataType = SqlDbType.NVarChar, Value = StrFN! });
                tamaRequest.paras.Add(new Para() { Name = "StrFV", DataType = SqlDbType.NVarChar, Value = StrFV });
            }
            
            if (DeletionDateIsNull!=null) 
            {
                tamaRequest.paras.Add(new Para() { Name = "DeletionDateIsNull", DataType = SqlDbType.Bit, Value = (bool)DeletionDateIsNull ? "1" : "0" });
            }

            if (OtherT1 != "" && OtherT1 != null)
                tamaRequest.paras.Add(new Para() { Name = "OtherT1", DataType = SqlDbType.NVarChar, Value = OtherT1 });

            if (OtherT1F1 != "" && OtherT1F1 != null)
                tamaRequest.paras.Add(new Para() { Name = "OtherT1F1", DataType = SqlDbType.NVarChar, Value = OtherT1F1 });

            return ExecSp(tamaRequest);
        }

        [Obsolete]
        public static DataTable RegisterUser(string PhoneNumber, string Password )
        {
            TamaRequest tamaRequest = new TamaRequest() { spName = "sp_Ins_User", paras = [] };

            tamaRequest.paras!.Add(new Para() { Name = "PhoneNumber", DataType = SqlDbType.NVarChar, Value = PhoneNumber });
            tamaRequest.paras.Add(new Para() { Name = "Password", DataType = SqlDbType.NVarChar, Value = Password });

            return ExecSp(tamaRequest);
        }

        [Obsolete]
        public static DataTable DeleteUser(string PhoneNumber)
        {
            TamaRequest tamaRequest = new TamaRequest() { spName = "sp_Ins_User", paras = [] };

            tamaRequest.paras!.Add(new Para() { Name = "PhoneNumber", DataType = SqlDbType.NVarChar, Value = PhoneNumber });
            tamaRequest.paras.Add(new Para() { Name = "Delete", DataType = SqlDbType.Bit, Value = "1" });

            return ExecSp(tamaRequest);
        }
        
        [Obsolete]
        public static DataTable ResetPasswordUser(string PhoneNumber, string Password)
        {
            TamaRequest tamaRequest = new TamaRequest() { spName = "sp_Ins_User", paras = [] };

            tamaRequest.paras!.Add(new Para() { Name = "PhoneNumber", DataType = SqlDbType.NVarChar, Value = PhoneNumber });
            tamaRequest.paras.Add(new Para() { Name = "Password", DataType = SqlDbType.NVarChar, Value = Password });
            tamaRequest.paras.Add(new Para() { Name = "ResetPassword", DataType = SqlDbType.Bit, Value = "1" });

            return ExecSp(tamaRequest);
        }

        private static dynamic ExecRestartService(string serverIP, string serviceName, string username, string password)
        {
            try
            {
                // Include domain if needed: "DOMAIN\\username" or ".\\username" for local
                string formattedUsername = username.Contains("\\") ? username : $".\\{username}";

                Console.WriteLine($"Connecting to {serverIP} as {formattedUsername}...");

                ConnectionOptions options = new ConnectionOptions
                {
                    Username = formattedUsername,
                    Password = password,
                    EnablePrivileges = true,
                    Authentication = AuthenticationLevel.PacketPrivacy,
                    Impersonation = ImpersonationLevel.Impersonate,
                    Timeout = TimeSpan.FromSeconds(30)
                };

                ManagementScope scope = new ManagementScope($"\\\\{serverIP}\\root\\cimv2", options);

                Console.WriteLine("Attempting to connect...");
                scope.Connect();
                Console.WriteLine("Connected successfully!");

                ObjectQuery query = new ObjectQuery($"SELECT * FROM Win32_Service WHERE Name = '{serviceName}'");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                foreach (ManagementObject service in searcher.Get())
                {
                    Console.WriteLine($"Found service: {service["DisplayName"]}");
                    Console.WriteLine($"Current state: {service["State"]}");

                    // Stop the service
                    Console.WriteLine("Stopping service...");
                    var stopResult = service.InvokeMethod("StopService", null);
                    Console.WriteLine($"Stop result: {stopResult}");

                    if (stopResult != null && (uint)stopResult == 0)
                    {
                        // Wait for service to stop
                        System.Threading.Thread.Sleep(3000);

                        // Start the service
                        Console.WriteLine("Starting service...");
                        var startResult = service.InvokeMethod("StartService", null);
                        Console.WriteLine($"Start result: {startResult}");

                        if (startResult != null && (uint)startResult == 0)
                        {
                            Console.WriteLine("Service restarted successfully!");
                            return new
                            {
                                MsgId = 1,
                                MsgAr = "Sccess",
                                MsgEn = "Sccess",
                            };
                        }
                    }
                }

                Console.WriteLine("Service not found or operation failed.");
                return new
                {
                    MsgId = 2,
                    MsgAr = "Service not found or operation failed.",
                    MsgEn = "Service not found or operation failed.",
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access Denied: {ex.Message}");
                Console.WriteLine("Check: Username/Password, Admin rights, DCOM permissions, UAC settings");
                return new
                {
                    MsgId = 2,
                    MsgAr = $"الرجاء الاتصال بنا",
                    MsgEn = $"Contact us",
                };
            }
            catch (ManagementException ex)
            {
                Console.WriteLine($"Management Error: {ex.Message}");
                Console.WriteLine($"Error Code: {ex.ErrorCode}");
                return new
                {
                    MsgId = 2,
                    MsgAr = $"Management Error: {ex.Message}",
                    MsgEn = $"Management Error: {ex.Message}",
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                return new
                {
                    MsgId = 2,
                    MsgAr = $"Error: {ex.Message}",
                    MsgEn = $"Error: {ex.Message}",
                };
            }
        }
        private static string ClinicPassword(int ClinicNo)
        {
            string clinicPassword;
            if (ClinicNo == 800 || ClinicNo == 1)
            {
                clinicPassword = "1663444697";
            }
            else
            {
                ClinicNo = ClinicNo + 100;
                clinicPassword = "1663440" + ClinicNo.ToString() + "4697";
            }

            return clinicPassword;
        }
        private static string cleanDate(string ExDate)
        {
            DateTime parsedDate = DateTime.Parse(ExDate);
            string cleanDate = parsedDate.ToString("MM/dd/yyyy HH:mm:ss");
            return cleanDate;
        }
        

        [Obsolete]
        public static dynamic SubscribeMachine(int clinicId,string ExDate,int MachineId,bool Com)
        {
            ExDate = cleanDate(ExDate);
            string cmd = $"Update ConnMachineIp Set endLicense='{ExDate}' Where ID={MachineId}";
            if (Com)
            {
                cmd = $"Update ConnMachineCom Set endLicense='{ExDate}' Where ID={MachineId}";
            }
            dynamic res = ExecCommand(new TamaRequestCmd() { Cmd = cmd, ClinicId = clinicId });
            DataTable Clinic = ExecSp(new TamaRequest()
            {
                spName = "sp_Get_Clinic",
                paras = [
                    new Para() { Name = "ID", DataType = SqlDbType.Int, Value = clinicId.ToString() },
                ]
            });
            string ServerIP = "", serviceName = "NetworkConn", username = "Dev\\Administrator", password = "";
            if (Clinic != null)
            {
                var ClinicDet = Clinic.Rows[0];
                ServerIP = ClinicDet["IpAddress"].ToString()!;
                password = ClinicPassword((int)ClinicDet["ClinicNo"]!);
            }
            return ExecRestartService(ServerIP, serviceName, username, password);
        }
    }
}
