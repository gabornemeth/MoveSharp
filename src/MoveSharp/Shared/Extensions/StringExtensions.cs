using System;
using System.Globalization;
using System.IO;

namespace MoveSharp
{
    public static class StringExtensions
    {
        public static bool TryParseAsDateTime(this string fileName, out DateTime result)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);
            if (TryParseAsDateTime(fileName, "yyyy-MM-dd-HH-mm-ss", out result))
                return true;
            if (TryParseAsDateTime(fileName, "yyyy-MM-dd HH:mm:ss", out result))
                return true;
            if (TryParseAsDateTime(fileName, "yyyy/MM/dd HH:mm:ss", out result))
                return true;

            return false;

        }

        private static bool TryParseAsDateTime(string fileName, string format, out DateTime result) =>
            DateTime.TryParseExact(fileName, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
