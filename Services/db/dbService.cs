using Seiiarty.StoredProcedures;
using System.Data;
using System.Data.SqlClient;
namespace Seiiarty.Services.db
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

        public class Request
        {
            public required string SpName { get; set; }
            public required string Method{ get; set; }
            public List<Para>? Paras { get; set; }
        }


        public class RequestSp
        {
            public required string Method { get; set; }
            public List<Para>? Paras { get; set; }
        }


        public class RequestCmd
        {
            public required string Cmd { get; set; }
        }


        public class Para
        {
            public required string Name { get; set; }
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
      

        [Obsolete]
        public static dynamic ExecDoSomeThings(Request request)
        {
            dynamic result = "";
            var p = request.Paras ?? [];
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

            SqlDataAdapter adapter = new(command);

            DataTable dt = new();
            adapter.Fill(dt);

            if (sqlConnClose) sqlConn.Close();

            return dt;
        }


        private static string CleanDate(string ExDate)
        {
            DateTime parsedDate = DateTime.Parse(ExDate);
            string cleanDate = parsedDate.ToString("MM/dd/yyyy HH:mm:ss");
            return cleanDate;
        }

    }
}
