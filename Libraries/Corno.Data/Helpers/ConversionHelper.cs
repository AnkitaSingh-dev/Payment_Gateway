using System;

namespace Corno.Data.Helpers
{
    public static class ConversionHelper
    {
        #region -- Methods --

        /// <summary>
        ///  Convert object To Unsigned Int
        /// </summary>
        /// <param name="obj">string</param>
        /// <returns>int</returns>
        public static int ToUInt(object obj)
        {
            var intValue = 0;
            if (obj != null)
                int.TryParse(obj.ToString(), out intValue);

            return intValue;
        }

        public static ushort ToUShort(object obj)
        {
            ushort intValue = 0;
            if (obj != null)
                ushort.TryParse(obj.ToString(), out intValue);

            return intValue;
        }

        /// <summary>
        /// Convert object To int
        /// </summary>
        /// <param name="obj">string</param>
        /// <returns>int</returns>
        public static int ToInt(object obj)
        {
            var intValue = 0;
            if (obj != null)
                int.TryParse(obj.ToString(), out intValue);

            return intValue;
        }

        /// <summary>
        ///  Convert object To double for 2 decimal Places
        /// </summary>
        /// <param name="obj">string</param>
        /// <returns>double</returns>
        public static double ToDouble(object obj)
        {
            double doubleValue = 0;
            if (obj == null) return doubleValue;

            double.TryParse(obj.ToString(), out doubleValue);
            var strValue = doubleValue.ToString("F3");
            return Convert.ToDouble(strValue);
        }

        /// <summary>
        ///  Convert object To double with decimal points specified.
        /// </summary>
        /// <param name="obj">string</param>
        /// <param name="decimalPoints">Decimal Points to convert</param>
        /// <returns>double</returns>
        public static double ToDouble(object obj, int decimalPoints)
        {
            double doubleValue = 0;
            if (obj == null) return doubleValue;

            double.TryParse(obj.ToString(), out doubleValue);
            var strValue = doubleValue.ToString("F" + decimalPoints);
            return Convert.ToDouble(strValue);
        }

        /// <summary>
        ///  Converts object to string
        /// </summary>
        /// <param>double</param>
        /// <param name="obj"></param>
        /// <returns>string</returns>
        public static string ToString(object obj)
        {
            var value = "";
            if (obj != null)
                value = Convert.ToString(obj);

            return value;
        }

        /// <summary>
        ///  Converts object to Boolean
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool ToBoolean(object obj)
        {
            var value = false;
            if (obj != null)
                value = Convert.ToBoolean(obj);

            return value;
        }

        /// <summary>
        ///  Format date in dd/MM/yyyy Formats
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string FormatDate(DateTime dateTime)
        {
            var date = dateTime.Day + //dd
                       "/" + dateTime.Month + //mm
                       "/" + dateTime.Year; //yyyy

            return date;
        }

        #endregion
    }
}