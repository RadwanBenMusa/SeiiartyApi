using Newtonsoft.Json;
using System.Data;
using System.Dynamic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Seiiarty
{
    public class General
    {
        public static string CheckPhoneNumberOk(string phoneNo)
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

            foreach (char s in phoneNo[1..])
                if (!char.IsDigit(s)) return "";

            if (!phoneNo.StartsWith("+2189")) return "";
            if (phoneNo.Length != 13) return "";
            return phoneNo;
        }
        public static string GetDateTimeFromat(DateTime dateTime)
        {
            return $"{dateTime.Year}-{dateTime.Month}-{dateTime.Day} {dateTime.Hour}:{dateTime.Minute}:{dateTime.Second}";
        }
        public static bool GetValueBool(DataRow dr, string fieldName, bool? defaultValue=false) {
            if (dr[fieldName] == null || dr[fieldName] == DBNull.Value) return (bool)defaultValue!;

            bool res = (bool)dr[fieldName];
            return res;
        }
        public static int GetValueBoolZeroOne(DataRow dr, string fieldName, bool? defaultValue = false)
        {
            if (dr[fieldName] == null || dr[fieldName] == DBNull.Value) return ((bool)defaultValue!)?1:0;
            return ((bool)dr[fieldName]) ? 1 : 0; ;
        }
        public static decimal GetValueDecimal(DataRow dr, string fieldName, decimal? defaultValue = 0)
        {
            if (dr[fieldName] == null || dr[fieldName] == DBNull.Value) return (decimal)defaultValue!;

            decimal res = (decimal)dr[fieldName];
            return res;
        }
        public static string GetValueString(DataRow dr, string fieldName, string? defaultValue = "")
        {
            if (dr[fieldName] == null || dr[fieldName] == DBNull.Value) return defaultValue!;

            string res =(string)dr[fieldName];
            return res;
        }
        public static int GetValueInt(DataRow dr, string fieldName, int? defaultValue = 0)
        {
            if (dr[fieldName] == null || dr[fieldName] == DBNull.Value) return (int)defaultValue!;

            int res = (int)dr[fieldName];
            return res;
        }
        // ── DATE ─────────────────────────────────────────────────

        /// <summary>Formats a DateTime to SQL-safe string: yyyy-MM-dd HH:mm:ss</summary>
        public static string ToSqlDate(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // ── TYPE HELPERS ─────────────────────────────────────────

        /// <summary>
        /// Returns whenEmpty if value is null or a Dictionary (Map in Flutter).
        /// Otherwise returns the value as-is.
        /// </summary>
        public static dynamic IfEmptyOrNull(dynamic value, dynamic? whenEmpty = null)
        {
            whenEmpty ??= "";

            if (value is null)
                return whenEmpty;

            return value;
        }

        // ── PASSWORD ─────────────────────────────────────────────

        /// <summary>Hashes a password using SHA-256.</summary>
        public static string HashPassword(string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = SHA256.HashData(bytes);

            // Convert to hex string (matches Dart's hash.toString())
            return Convert.ToHexString(hash).ToLower();
        }

        // ── PHONE HELPERS ─────────────────────────────────────────

        /// <summary>
        /// Converts various Libyan phone number formats to international format.
        /// Output: 00218919005626
        /// </summary>
        public static string FormatLibyanPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return phone;

            // Remove all non-digit characters
            string digitsOnly = Regex.Replace(phone, @"\D", "");

            string normalized;

            if (digitsOnly.StartsWith("00218"))
                // 00218919005626 -> already correct
                normalized = digitsOnly;

            else if (digitsOnly.StartsWith("218"))
                // 218919005626 -> 00218919005626
                normalized = "00" + digitsOnly;

            else if (digitsOnly.StartsWith("0") && digitsOnly.Length == 10)
                // 0919005626 -> 00218919005626
                normalized = "00218" + digitsOnly[1..];

            else if (digitsOnly.Length == 9)
                // 919005626 -> 00218919005626
                normalized = "00218" + digitsOnly;

            else
                return phone; // Unrecognized format

            // Validate: must be 14 digits starting with 00218
            if (normalized.Length != 14 || !normalized.StartsWith("00218"))
                return phone;

            return normalized;
        }

        /// <summary>
        /// Converts international format (00218919005626) to local format (0919005626).
        /// </summary>
        public static string ToLocalFormat(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return phone;

            string digitsOnly = Regex.Replace(phone, @"\D", "");

            if (digitsOnly.StartsWith("00218"))
                // 00218919005626 -> 0919005626
                return "0" + digitsOnly[5..];

            if (digitsOnly.StartsWith("218"))
                // 218919005626 -> 0919005626
                return "0" + digitsOnly[3..];

            if (digitsOnly.StartsWith("0") && digitsOnly.Length == 10)
                // Already local format
                return digitsOnly;

            if (digitsOnly.Length == 9)
                // 919005626 -> 0919005626
                return "0" + digitsOnly;

            return phone; // Unrecognized format
        }

        /// <summary>
        /// Validates if the phone number is a valid Libyan number.
        /// Valid operator codes: 91, 92, 93, 94, 95, 21
        /// </summary>
        public static bool IsValidLibyanPhone(string phone)
        {
            string digitsOnly = Regex.Replace(phone, @"\D", "");

            // Strip country code to get 9-digit local number
            if (digitsOnly.StartsWith("00218"))
                digitsOnly = digitsOnly[5..];
            else if (digitsOnly.StartsWith("218"))
                digitsOnly = digitsOnly[3..];
            else if (digitsOnly.StartsWith("0"))
                digitsOnly = digitsOnly[1..];

            if (digitsOnly.Length != 9) return false;

            string[] validOperators = ["91", "92", "93", "94", "95", "21"];
            string operatorCode = digitsOnly[..2];

            return validOperators.Contains(operatorCode);
        }

        /// <summary>
        /// Returns only digits in international format: 00218919005626
        /// </summary>
        public static string GetDigitsOnly(string phone)
        {
            string digitsOnly = Regex.Replace(phone, @"\D", "");

            if (digitsOnly.StartsWith("00218"))
                return digitsOnly;

            if (digitsOnly.StartsWith("218"))
                return "00" + digitsOnly;

            if (digitsOnly.StartsWith("0") && digitsOnly.Length == 10)
                return "00218" + digitsOnly[1..];

            if (digitsOnly.Length == 9)
                return "00218" + digitsOnly;

            return digitsOnly;
        }

        /// <summary>
        /// Formats phone for display: +218 91-9005626
        /// </summary>
        public static string FormatForDisplay(string phone)
        {
            string international = FormatLibyanPhone(phone);

            if (international.Length != 14 || !international.StartsWith("00218"))
                return phone;

            string operatorCode = international[5..7]; // 91, 92, etc.
            string subscriberNumber = international[7..];  // 9005626

            return $"+218 {operatorCode}-{subscriberNumber}";
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
