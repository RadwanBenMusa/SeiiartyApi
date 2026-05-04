using Newtonsoft.Json;
using System.Data;
using System.Dynamic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static Seiiarty.Services.db.DbService;

namespace Seiiarty
{
    // ══════════════════════════════════════════════════════════════════════════════
    //  General — shared utility functions used across the whole backend
    //
    //  Sections:
    //    1. Para Helpers       — read parameters from the request
    //    2. DataRow Helpers    — safely read values from a DB row
    //    3. Date Helpers       — format and compare dates
    //    4. String Helpers     — trim, clean, truncate text
    //    5. Password Helpers   — hash and verify passwords
    //    6. Phone Helpers      — format, validate, convert Libyan numbers
    //    7. Number Helpers     — decimal rounding, range checks
    //    8. Type / Null Helpers— null checks and safe casting
    // ══════════════════════════════════════════════════════════════════════════════

    public class General
    {
        // ┌─────────────────────────────────────────────────────────────────────┐
        // │  1. PARA HELPERS                                                    │
        // │  Read values from the List<Para> that comes in with every request.  │
        // └─────────────────────────────────────────────────────────────────────┘

        /// <summary>
        /// Returns the raw string value of a parameter by name (case-insensitive).
        /// Returns <paramref name="fallback"/> if the parameter is missing or blank.
        /// </summary>
        public static dynamic? GetPara(List<Para> paras, string name, dynamic? fallback = null)
        {
            string? raw = paras
                .FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase))
                ?.Value;

            if (raw is null || string.IsNullOrWhiteSpace(raw.ToString()))
                return fallback;

            return raw;
        }

        /// <summary>
        /// Returns the parameter value as <see cref="int"/>.
        /// Returns <c>null</c> if the parameter is missing or cannot be parsed.
        /// </summary>
        public static int? GetParaInt(List<Para> paras, string name)
        {
            dynamic? val = GetPara(paras, name);
            if (val is null) return null;
            return int.TryParse(val.ToString(), out int parsed) ? parsed : null;
        }

        /// <summary>
        /// Returns the parameter value as <see cref="bool"/>.
        /// Returns <c>null</c> if the parameter is missing or cannot be parsed.
        /// Accepts "true"/"false" (case-insensitive).
        /// </summary>
        public static bool? GetParaBool(List<Para> paras, string name)
        {
            dynamic? val = GetPara(paras, name);
            if (val is null) return null;
            return bool.TryParse(val.ToString(), out bool parsed) ? parsed : null;
        }

        /// <summary>
        /// Returns the parameter value as <see cref="DateTime"/>.
        /// Returns <c>null</c> if the parameter is missing or cannot be parsed.
        /// </summary>
        public static DateTime? GetParaDate(List<Para> paras, string name)
        {
            dynamic? val = GetPara(paras, name);
            if (val is null) return null;
            return DateTime.TryParse(val.ToString(), out DateTime parsed) ? parsed : null;
        }

        /// <summary>
        /// Returns the parameter value as <see cref="decimal"/>.
        /// Returns <c>null</c> if the parameter is missing or cannot be parsed.
        /// </summary>
        public static decimal? GetParaDec(List<Para> paras, string name)
        {
            dynamic? val = GetPara(paras, name);
            if (val is null) return null;
            return decimal.TryParse(val.ToString(), out decimal parsed) ? parsed : null;
        }

        /// <summary>
        /// Returns the parameter value as <see cref="double"/>.
        /// Returns <c>null</c> if the parameter is missing or cannot be parsed.
        /// </summary>
        public static double? GetParaDouble(List<Para> paras, string name)
        {
            dynamic? val = GetPara(paras, name);
            if (val is null) return null;
            return double.TryParse(val.ToString(), out double parsed) ? parsed : null;
        }

        /// <summary>
        /// Returns the parameter value as <see cref="long"/>.
        /// Useful for IDs or large numbers that exceed int range.
        /// Returns <c>null</c> if the parameter is missing or cannot be parsed.
        /// </summary>
        public static long? GetParaLong(List<Para> paras, string name)
        {
            dynamic? val = GetPara(paras, name);
            if (val is null) return null;
            return long.TryParse(val.ToString(), out long parsed) ? parsed : null;
        }

        // ┌─────────────────────────────────────────────────────────────────────┐
        // │  2. DATAROW HELPERS                                                 │
        // │  Safely read typed values from a DataRow without null exceptions.   │
        // └─────────────────────────────────────────────────────────────────────┘

        /// <summary>
        /// Returns a <see cref="bool"/> from a DataRow field.
        /// Returns <paramref name="defaultValue"/> if the field is null or DBNull.
        /// </summary>
        public static bool GetValueBool(DataRow dr, string fieldName, bool defaultValue = false)
        {
            if (dr[fieldName] == null || dr[fieldName] == DBNull.Value) return defaultValue;
            return (bool)dr[fieldName];
        }

        /// <summary>
        /// Returns 1 if the DataRow boolean field is true, 0 if false.
        /// Returns 0 (or 1 if defaultValue is true) when the field is null or DBNull.
        /// </summary>
        public static int GetValueBoolZeroOne(DataRow dr, string fieldName, bool defaultValue = false)
        {
            if (dr[fieldName] == null || dr[fieldName] == DBNull.Value) return defaultValue ? 1 : 0;
            return ((bool)dr[fieldName]) ? 1 : 0;
        }

        /// <summary>
        /// Returns a <see cref="decimal"/> from a DataRow field.
        /// Returns <paramref name="defaultValue"/> if the field is null or DBNull.
        /// </summary>
        public static decimal GetValueDecimal(DataRow dr, string fieldName, decimal defaultValue = 0)
        {
            if (dr[fieldName] == null || dr[fieldName] == DBNull.Value) return defaultValue;
            return (decimal)dr[fieldName];
        }

        /// <summary>
        /// Returns a trimmed <see cref="string"/> from a DataRow field.
        /// Returns <paramref name="defaultValue"/> if the field is null or DBNull.
        /// </summary>
        public static string GetValueString(DataRow dr, string fieldName, string defaultValue = "")
        {
            if (dr[fieldName] == null || dr[fieldName] == DBNull.Value) return defaultValue;
            return dr[fieldName].ToString()!.Trim();
        }

        /// <summary>
        /// Returns an <see cref="int"/> from a DataRow field.
        /// Returns <paramref name="defaultValue"/> if the field is null or DBNull.
        /// </summary>
        public static int GetValueInt(DataRow dr, string fieldName, int defaultValue = 0)
        {
            if (dr[fieldName] == null || dr[fieldName] == DBNull.Value) return defaultValue;
            return (int)dr[fieldName];
        }

        /// <summary>
        /// Returns a <see cref="DateTime"/> from a DataRow field.
        /// Returns <c>null</c> if the field is null or DBNull.
        /// </summary>
        public static DateTime? GetValueDate(DataRow dr, string fieldName)
        {
            if (dr[fieldName] == null || dr[fieldName] == DBNull.Value) return null;
            return (DateTime)dr[fieldName];
        }

        /// <summary>
        /// Returns a <see cref="long"/> from a DataRow field.
        /// Returns <paramref name="defaultValue"/> if the field is null or DBNull.
        /// </summary>
        public static long GetValueLong(DataRow dr, string fieldName, long defaultValue = 0)
        {
            if (dr[fieldName] == null || dr[fieldName] == DBNull.Value) return defaultValue;
            return Convert.ToInt64(dr[fieldName]);
        }

        // ┌─────────────────────────────────────────────────────────────────────┐
        // │  3. DATE HELPERS                                                    │
        // └─────────────────────────────────────────────────────────────────────┘

        /// <summary>
        /// Formats a <see cref="DateTime"/> to a SQL-safe string: <c>yyyy-MM-dd HH:mm:ss</c>.
        /// Always use this before inserting or comparing dates in raw SQL commands.
        /// </summary>
        public static string ToSqlDate(DateTime dt)
            => dt.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// Formats a <see cref="DateTime"/> for display to the user: <c>dd/MM/yyyy HH:mm</c>.
        /// Example output: 25/06/2024 14:30
        /// </summary>
        public static string ToDisplayDate(DateTime dt)
            => dt.ToString("dd/MM/yyyy HH:mm");

        /// <summary>
        /// Returns only the date part as a string for display: <c>dd/MM/yyyy</c>.
        /// Example output: 25/06/2024
        /// </summary>
        public static string ToDisplayDateOnly(DateTime dt)
            => dt.ToString("dd/MM/yyyy");

        /// <summary>
        /// Returns the date formatted for display in Arabic-friendly order: <c>yyyy/MM/dd</c>.
        /// </summary>
        public static string ToArabicDate(DateTime dt)
            => dt.ToString("yyyy/MM/dd");

        /// <summary>
        /// Returns <c>true</c> if the given date has already passed (is before now).
        /// Useful for checking subscription expiry, deletion dates, etc.
        /// </summary>
        public static bool IsExpired(DateTime? date)
            => date.HasValue && date.Value < DateTime.Now;

        /// <summary>
        /// Returns the number of days between today and a future date.
        /// Returns 0 if the date has already passed or is null.
        /// </summary>
        public static int DaysUntil(DateTime? date)
        {
            if (!date.HasValue || date.Value <= DateTime.Now) return 0;
            return (int)(date.Value - DateTime.Now).TotalDays;
        }

        /// <summary>
        /// Returns <c>true</c> if two dates fall on the same calendar day,
        /// ignoring the time portion.
        /// </summary>
        public static bool IsSameDay(DateTime a, DateTime b)
            => a.Date == b.Date;

        /// <summary>
        /// Returns the start of a given day (midnight: 00:00:00).
        /// Useful for date-range filters in SQL queries.
        /// </summary>
        public static DateTime StartOfDay(DateTime dt)
            => dt.Date;

        /// <summary>
        /// Returns the end of a given day (23:59:59).
        /// Useful for date-range filters in SQL queries.
        /// </summary>
        public static DateTime EndOfDay(DateTime dt)
            => dt.Date.AddDays(1).AddSeconds(-1);

        // ┌─────────────────────────────────────────────────────────────────────┐
        // │  4. STRING HELPERS                                                  │
        // └─────────────────────────────────────────────────────────────────────┘

        /// <summary>
        /// Returns <c>true</c> if the string is null, empty, or contains only whitespace.
        /// Shortcut for <see cref="string.IsNullOrWhiteSpace"/>.
        /// </summary>
        public static bool IsBlank(string? value)
            => string.IsNullOrWhiteSpace(value);

        /// <summary>
        /// Returns <paramref name="fallback"/> if the string is null or blank,
        /// otherwise returns the trimmed string.
        /// </summary>
        public static string OrDefault(string? value, string fallback = "")
            => string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();

        /// <summary>
        /// Truncates a string to a maximum length and appends "..." if it was cut.
        /// Useful for notification previews or short summaries.
        /// Example: Truncate("Hello World", 5) → "Hello..."
        /// </summary>
        public static string Truncate(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Length <= maxLength) return value;
            return value[..maxLength] + "...";
        }

        /// <summary>
        /// Removes all special characters from a string, keeping only
        /// letters, digits, and spaces. Useful for sanitizing user input
        /// before storing it in the DB.
        /// </summary>
        public static string RemoveSpecialChars(string input)
            => Regex.Replace(input, @"[^a-zA-Z0-9\u0600-\u06FF\s]", "");

        /// <summary>
        /// Capitalizes the first letter of a string and lowercases the rest.
        /// Example: "hELLO wORLD" → "Hello world"
        /// </summary>
        public static string Capitalize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            return char.ToUpper(value[0]) + value[1..].ToLower();
        }

        /// <summary>
        /// Escapes single quotes in a string to prevent SQL injection in raw string commands.
        /// Example: "O'Brien" → "O''Brien"
        /// Note: prefer parameterized queries when possible.
        /// </summary>
        public static string EscapeSql(string value)
            => value.Replace("'", "''");

        // ┌─────────────────────────────────────────────────────────────────────┐
        // │  5. PASSWORD HELPERS                                                │
        // └─────────────────────────────────────────────────────────────────────┘

        /// <summary>
        /// Hashes a plain-text password using SHA-256.
        /// Returns a lowercase hex string that matches Dart's implementation on Flutter.
        /// Always hash before storing or comparing passwords.
        /// </summary>
        public static string HashPassword(string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash).ToLower();
        }

        /// <summary>
        /// Returns <c>true</c> if the plain-text password matches a stored hash.
        /// Hashes the input and compares it to the stored value.
        /// </summary>
        public static bool VerifyPassword(string plainText, string storedHash)
            => HashPassword(plainText) == storedHash.ToLower();

        /// <summary>
        /// Generates a random numeric OTP of the given length (default: 4 digits).
        /// Example output: "7392"
        /// </summary>
        public static string GenerateOtp(int length = 4)
        {
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            int number = Math.Abs(BitConverter.ToInt32(bytes, 0));
            return (number % (int)Math.Pow(10, length))
                   .ToString()
                   .PadLeft(length, '0');
        }

        // ┌─────────────────────────────────────────────────────────────────────┐
        // │  6. PHONE HELPERS                                                   │
        // │  All functions work with Libyan numbers (country code: +218).       │
        // └─────────────────────────────────────────────────────────────────────┘

        /// <summary>
        /// Converts any Libyan phone format to international format with 00218 prefix.
        /// Accepted inputs: 0919005626 / 919005626 / 218919005626 / 00218919005626
        /// Output: 00218919005626
        /// Returns the original string if the format is unrecognized.
        /// </summary>
        public static string FormatLibyanPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return phone;

            string digits = Regex.Replace(phone, @"\D", "");
            string normalized;

            if (digits.StartsWith("00218")) normalized = digits;
            else if (digits.StartsWith("218")) normalized = "00" + digits;
            else if (digits.StartsWith("0") && digits.Length == 10) normalized = "00218" + digits[1..];
            else if (digits.Length == 9) normalized = "00218" + digits;
            else return phone; // unrecognized

            if (normalized.Length != 14 || !normalized.StartsWith("00218"))
                return phone;

            return normalized;
        }

        /// <summary>
        /// Converts an international Libyan number (00218919005626) to local format (0919005626).
        /// Also handles 218xxxxxxxxx and bare 9-digit numbers.
        /// Returns the original string if the format is unrecognized.
        /// </summary>
        public static string ToLocalFormat(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return phone;

            string digits = Regex.Replace(phone, @"\D", "");

            if (digits.StartsWith("00218")) return "0" + digits[5..];
            if (digits.StartsWith("218")) return "0" + digits[3..];
            if (digits.StartsWith("0") && digits.Length == 10) return digits;
            if (digits.Length == 9) return "0" + digits;

            return phone;
        }

        /// <summary>
        /// Returns the number stripped of all non-digit characters,
        /// in international format: 00218919005626
        /// </summary>
        public static string GetDigitsOnly(string phone)
        {
            string digits = Regex.Replace(phone, @"\D", "");

            if (digits.StartsWith("00218")) return digits;
            if (digits.StartsWith("218")) return "00" + digits;
            if (digits.StartsWith("0") && digits.Length == 10) return "00218" + digits[1..];
            if (digits.Length == 9) return "00218" + digits;

            return digits;
        }

        /// <summary>
        /// Returns <c>true</c> if the phone number is a valid Libyan mobile number.
        /// Valid operator prefixes: 91, 92, 93, 94, 95, 21.
        /// Works with any accepted input format.
        /// </summary>
        public static bool IsValidLibyanPhone(string phone)
        {
            string digits = Regex.Replace(phone, @"\D", "");

            // Strip country code to reach the 9-digit local number
            if (digits.StartsWith("00218")) digits = digits[5..];
            else if (digits.StartsWith("218")) digits = digits[3..];
            else if (digits.StartsWith("0")) digits = digits[1..];

            if (digits.Length != 9) return false;

            string[] validPrefixes = ["91", "92", "93", "94", "95", "21"];
            return validPrefixes.Contains(digits[..2]);
        }

        /// <summary>
        /// Formats a Libyan phone number for display: +218 91-9005626
        /// Returns the original string if the format is unrecognized.
        /// </summary>
        public static string FormatForDisplay(string phone)
        {
            string international = FormatLibyanPhone(phone);

            if (international.Length != 14 || !international.StartsWith("00218"))
                return phone;

            string operatorCode = international[5..7]; // e.g. 91
            string subscriberNumber = international[7..];  // e.g. 9005626

            return $"+218 {operatorCode}-{subscriberNumber}";
        }

        /// <summary>
        /// Sanitizes a phone number by removing all non-digit and non-plus characters.
        /// Use before passing to FormatLibyanPhone when the input may contain dashes or spaces.
        /// </summary>
        public static string SanitizePhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return phone;
            // Keep only digits; FormatLibyanPhone handles the rest
            return Regex.Replace(phone, @"[^\d+]", "");
        }

        // ┌─────────────────────────────────────────────────────────────────────┐
        // │  7. NUMBER HELPERS                                                  │
        // └─────────────────────────────────────────────────────────────────────┘

        /// <summary>
        /// Rounds a decimal value to the given number of decimal places.
        /// Default is 2 places (e.g. prices).
        /// Example: Round(12.3456m) → 12.35
        /// </summary>
        public static decimal Round(decimal value, int decimals = 2)
            => Math.Round(value, decimals, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Clamps a value between a minimum and maximum.
        /// Returns <paramref name="min"/> if value is below it,
        /// <paramref name="max"/> if above it, or the value itself if within range.
        /// Example: Clamp(150, 0, 100) → 100
        /// </summary>
        public static int Clamp(int value, int min, int max)
            => Math.Max(min, Math.Min(max, value));

        /// <summary>
        /// Returns <c>true</c> if a value is within the given range (inclusive on both ends).
        /// Example: InRange(5, 1, 10) → true
        /// </summary>
        public static bool InRange(int value, int min, int max)
            => value >= min && value <= max;

        /// <summary>
        /// Formats a decimal as a currency string with 2 decimal places and "LYD" suffix.
        /// Example: FormatCurrency(1250.5m) → "1,250.50 LYD"
        /// </summary>
        public static string FormatCurrency(decimal amount)
            => $"{amount:N2} LYD";

        // ┌─────────────────────────────────────────────────────────────────────┐
        // │  8. TYPE / NULL HELPERS                                             │
        // └─────────────────────────────────────────────────────────────────────┘

        /// <summary>
        /// Returns <paramref name="whenEmpty"/> if the value is <c>null</c>
        /// or an empty/error-like Dictionary (the shape Flutter sends for empty maps).
        /// Otherwise returns the value as-is.
        /// Used to safely check API responses that may be "No rows found" strings.
        /// </summary>
        public static dynamic IfEmptyOrNull(dynamic value, dynamic? whenEmpty = null)
        {
            whenEmpty ??= "";
            if (value is null) return whenEmpty;
            return value;
        }

        /// <summary>
        /// Returns <c>true</c> if the value is null, an empty string,
        /// or the "No * found" placeholder string that SP classes return when
        /// the DB query comes back with zero rows.
        /// </summary>
        public static bool IsEmptyResult(dynamic? value)
        {
            if (value is null) return true;
            string? str = value.ToString();
            if (string.IsNullOrWhiteSpace(str)) return true;
            if (str.StartsWith("No ") && str.EndsWith("found")) return true;
            return false;
        }

        /// <summary>
        /// Safely casts a dynamic value to <see cref="int"/>.
        /// Returns <paramref name="fallback"/> if the cast or parse fails.
        /// </summary>
        public static int ToInt(dynamic? value, int fallback = 0)
        {
            if (value is null) return fallback;
            return int.TryParse(value.ToString(), out int parsed) ? parsed : fallback;
        }

        /// <summary>
        /// Safely casts a dynamic value to <see cref="decimal"/>.
        /// Returns <paramref name="fallback"/> if the cast or parse fails.
        /// </summary>
        public static decimal ToDecimal(dynamic? value, decimal fallback = 0)
        {
            if (value is null) return fallback;
            return decimal.TryParse(value.ToString(), out decimal parsed) ? parsed : fallback;
        }

        /// <summary>
        /// Safely casts a dynamic value to <see cref="bool"/>.
        /// Also treats "1" as true and "0" as false.
        /// Returns <paramref name="fallback"/> if the cast or parse fails.
        /// </summary>
        public static bool ToBool(dynamic? value, bool fallback = false)
        {
            if (value is null) return fallback;
            string str = value.ToString()!;
            if (str == "1") return true;
            if (str == "0") return false;
            return bool.TryParse(str, out bool parsed) ? parsed : fallback;
        }
    }

    // ══════════════════════════════════════════════════════════════════════════════
    //  DataTable Extensions
    //  Adds .ToDynamic() and .ToJson() to every DataTable in the project.
    // ══════════════════════════════════════════════════════════════════════════════

    public static class DataTableExtensions
    {
        /// <summary>
        /// Converts a <see cref="DataTable"/> to a list of dynamic objects
        /// where each row becomes an <see cref="ExpandoObject"/> with property names
        /// matching the column names. Used before returning data to the Flutter client.
        /// </summary>
        public static List<dynamic> ToDynamic(this DataTable dt)
        {
            var result = new List<dynamic>();

            foreach (DataRow row in dt.Rows)
            {
                dynamic obj = new ExpandoObject();
                var dict = (IDictionary<string, object>)obj;

                foreach (DataColumn col in dt.Columns)
                    dict[col.ColumnName] = row[col];

                result.Add(obj);
            }

            return result;
        }

        /// <summary>
        /// Serializes a <see cref="DataTable"/> to a JSON string using Newtonsoft.Json.
        /// Useful for logging or passing raw JSON in API responses.
        /// </summary>
        public static string ToJson(this DataTable dt)
            => JsonConvert.SerializeObject(dt);
    }
}