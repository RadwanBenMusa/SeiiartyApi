using Newtonsoft.Json;
using System.Data;
using System.Dynamic;

namespace TamaApi
{
    public class General
    {
        public static string checkPhoneNumberOk(string phoneNo)
        {
            if (string.IsNullOrEmpty(phoneNo)) return "";
            phoneNo = phoneNo.Replace("-", "");
            phoneNo = phoneNo.Replace("_", "");
            phoneNo = phoneNo.Replace("\\", "");
            phoneNo = phoneNo.Replace("/", "");
            phoneNo = phoneNo.Replace(".", "");
            phoneNo = phoneNo.Replace(",", "");
            phoneNo = phoneNo.Replace(" ", "");
            phoneNo = phoneNo.Replace(" ", "");
            phoneNo = phoneNo.Replace(" ", "");
            phoneNo = phoneNo.Replace("(", "");
            phoneNo = phoneNo.Replace(")", "");

            foreach (char s in phoneNo.Substring(1))
                if (!char.IsDigit(s)) return "";

            if (!phoneNo.StartsWith("+2189")) return "";
            if (phoneNo.Length != 13) return "";
            return phoneNo;
        }
        public static string getDateTimeFromat(DateTime dateTime)
        {
            return $"{dateTime.Year}-{dateTime.Month}-{dateTime.Day} {dateTime.Hour}:{dateTime.Minute}:{dateTime.Second}";
        }
        public static bool getValueBool(DataRow dr, string fieldName, bool? defaultValue=false) {
            if (dr[fieldName] == null || dr[fieldName] == DBNull.Value) return (bool)defaultValue!;

            bool res = (bool)dr[fieldName];
            return res;
        }
        public static int getValueBoolZeroOne(DataRow dr, string fieldName, bool? defaultValue = false)
        {
            if (dr[fieldName] == null || dr[fieldName] == DBNull.Value) return ((bool)defaultValue!)?1:0;
            return ((bool)dr[fieldName]) ? 1 : 0; ;
        }
        public static decimal getValueDecimal(DataRow dr, string fieldName, decimal? defaultValue = 0)
        {
            if (dr[fieldName] == null || dr[fieldName] == DBNull.Value) return (decimal)defaultValue!;

            decimal res = (decimal)dr[fieldName];
            return res;
        }
        public static string getValueString(DataRow dr, string fieldName, string? defaultValue = "")
        {
            if (dr[fieldName] == null || dr[fieldName] == DBNull.Value) return defaultValue!;

            string res =(string)dr[fieldName];
            return res;
        }
        public static int getValueInt(DataRow dr, string fieldName, int? defaultValue = 0)
        {
            if (dr[fieldName] == null || dr[fieldName] == DBNull.Value) return (int)defaultValue!;

            int res = (int)dr[fieldName];
            return res;
        }
    }
    public static class DataTableExtensions
    {
        public static List<dynamic> ToDynamic(this DataTable dt)
        {
            var dynamicDt = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic dyn = new ExpandoObject();
                dynamicDt.Add(dyn);
                foreach (DataColumn column in dt.Columns)
                {
                    var dic = (IDictionary<string, object>)dyn;
                    dic[column.ColumnName] = row[column];
                }
            }
            return dynamicDt;
        }
        public static string ToJson(this DataTable dataTable)
        {
            return JsonConvert.SerializeObject(dataTable);
        }
    }
}
