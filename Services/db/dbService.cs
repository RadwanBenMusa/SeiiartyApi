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
        const string mainDbName = "Seiiarty";

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
            public required string CustomerPhoneNo { get; set; }
        }

        public class ClsSmsLinkAnalysisRes
        {
            public required int ClinicNo { get; set; }
            public required string FileName { get; set; }
            public string? OtherInfo { get; set; }
            public required string CustomerPhoneNo { get; set; }
        }

        public class ClsWhatsAppMsg
        {
            public required int ClinicNo { get; set; }
            public required string Budy { get; set; }
            public string? OtherInfo { get; set; }
            public required string CustomerPhoneNo { get; set; }
        }
        public class Request
        {
            public required string SpName { get; set; }
            public int? ClinicId {  set; get; }
            public List<Para>? Paras { get; set; }
        }
        public class RequestCmd
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
        public static SqlConnection? GetSqlConnection()
        {
            string _dbName =  mainDbName;
            string _dbInstance = dbInstance ;


            string strSqlConn = $"server={_dbInstance};";
            strSqlConn += $"database={_dbName};";
            strSqlConn += connStr;

            return new SqlConnection(strSqlConn);
        }


        private static object GetValue(Para para) 
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
            SqlParameter workParam = new(para.Name, para.DataType)
            {
                Direction = ParameterDirection.Input,
                Value = GetValue(para)
            };
            return (workParam);
        }

        [Obsolete]
        public static DataTable ExecSp(Request request)
        {
            SqlConnection sqlConn = GetSqlConnection()!;

            sqlConn.Open();

            SqlDataAdapter da;
            SqlParameter workParam;
            DataSet ds;
            DataTable dt;

            da = new SqlDataAdapter(request.SpName, sqlConn);
            //if (itsTransaction) da.SelectCommand.Transaction = Trans;
            da.SelectCommand.CommandType = CommandType.StoredProcedure;

            if (request.Paras != null)
            {
                for (int i = 0; i <= request.Paras.Count - 1; i++)
                {
                    workParam = CreatePara(request.Paras[i]);
                    da.SelectCommand.Parameters.Add(workParam);
                }
            }
            ds = new DataSet();
            da.Fill(ds, "TableData");
            dt = ds.Tables["TableData"]!;

            return dt;
        }

        [Obsolete]
        public static dynamic ExecDoSomeThings(Request request, ILogger<MainController> logger)
        {
            dynamic result = "";
            switch (request.SpName)
            {
                case "AddAdminToClinic":
                    return ""; //DoSomeThings.AddAdminToClinic(request, logger);
                default:
                    break;
            }
            return result;
        }

        [Obsolete]
        public static DataTable ExecCommand(RequestCmd requestCmd, SqlConnection? sqlConn = null, SqlTransaction? transaction = null)
        {
            SqlCommand command;
            bool sqlConnClose = true;
            if (sqlConn == null)
            {
                sqlConnClose = false;
                sqlConn = GetSqlConnection()!;
                sqlConn.Open();
                command = new SqlCommand(requestCmd.Cmd, sqlConn) { CommandType = CommandType.Text };
            }
            else
                command = new SqlCommand(requestCmd.Cmd, sqlConn, transaction) { CommandType = CommandType.Text };


            //SqlCommand command = new SqlCommand(requestCmd.Cmd, sqlConn) { CommandType = CommandType.Text };

            SqlDataAdapter adapter = new(command);

            DataTable dt=new();
            adapter.Fill(dt);
            
            if (sqlConnClose) sqlConn.Close();
            
            return dt;
        }

        [Obsolete]
        public static int ExecCommandIns(RequestCmd requestCmd, SqlConnection? sqlConn=null, SqlTransaction? transaction=null)
        {
            SqlCommand command;
            bool sqlConnClose=true;
            if (sqlConn == null) { 
                sqlConnClose = false;
                sqlConn = GetSqlConnection()!;
                sqlConn.Open();
                command = new SqlCommand(requestCmd.Cmd + " SELECT SCOPE_IDENTITY();", sqlConn) { CommandType = CommandType.Text };
            }else
                command = new SqlCommand(requestCmd.Cmd + " SELECT SCOPE_IDENTITY();", sqlConn) { CommandType = CommandType.Text };


            int newId;

            object result;
            result = command.ExecuteScalar();

            newId = Convert.ToInt32(result);

            if(sqlConnClose) sqlConn.Close();

            return newId; 
        }

        [Obsolete]
        public static int ExecCommandFunction(Request request)
        {
            SqlConnection sqlConn = GetSqlConnection()!;

            sqlConn.Open();

            using SqlCommand command = new(request.SpName, sqlConn);
            command.CommandType = CommandType.Text;
            // إضافة المعاملات للدالة
            if (request.Paras != null)
                foreach (Para para in request.Paras)
                    command.Parameters.AddWithValue($"@{para.Name}", para.Value);
            object scalarResult = command.ExecuteScalar();
            if (scalarResult != null && scalarResult != DBNull.Value)
            {
                return Convert.ToInt32(scalarResult);
            }
            return 0;
        }
    
        [Obsolete]
        public static DataTable RegisterUser(string PhoneNumber, string Password )
        {
            Request request = new() { SpName = "sp_Ins_User", Paras = [] };

            request.Paras!.Add(new Para() { Name = "PhoneNumber", DataType = SqlDbType.NVarChar, Value = PhoneNumber });
            request.Paras.Add(new Para() { Name = "Password", DataType = SqlDbType.NVarChar, Value = Password });

            return ExecSp(request);
        }

        [Obsolete]
        public static DataTable DeleteUser(string PhoneNumber)
        {
            Request request = new() { SpName = "sp_Ins_User", Paras = [] };

            request.Paras!.Add(new Para() { Name = "PhoneNumber", DataType = SqlDbType.NVarChar, Value = PhoneNumber });
            request.Paras.Add(new Para() { Name = "Delete", DataType = SqlDbType.Bit, Value = "1" });

            return ExecSp(request);
        }
        
        [Obsolete]
        public static DataTable ResetPasswordUser(string PhoneNumber, string Password)
        {
            Request request = new() { SpName = "sp_Ins_User", Paras = [] };

            request.Paras!.Add(new Para() { Name = "PhoneNumber", DataType = SqlDbType.NVarChar, Value = PhoneNumber });
            request.Paras.Add(new Para() { Name = "Password", DataType = SqlDbType.NVarChar, Value = Password });
            request.Paras.Add(new Para() { Name = "ResetPassword", DataType = SqlDbType.Bit, Value = "1" });

            return ExecSp(request);
        }
        
        private static string CleanDate(string ExDate)
        {
            DateTime parsedDate = DateTime.Parse(ExDate);
            string cleanDate = parsedDate.ToString("MM/dd/yyyy HH:mm:ss");
            return cleanDate;
        }

        [Obsolete]
        public static dynamic GetSetupTable()
        {
           dynamic res = ExecCommand(new RequestCmd() { Cmd = $"Select * from SetupTable" });
            return res;
        }
    }
}
