using System;
using System.Collections.Generic;
using System.Linq;

namespace Corno.Data.Helpers
{
    public static class SubstringExtensions
    {
        ///// <summary>
        ///// Get string value between [first] a and [last] b.
        ///// </summary>
        //public static string Between(this string value, string a, string b)
        //{
        //    var posA = value.IndexOf(a, StringComparison.Ordinal);
        //    var posB = value.LastIndexOf(b, StringComparison.Ordinal);
        //    if (posA == -1)
        //    {
        //        return "";
        //    }
        //    if (posB == -1)
        //    {
        //        return "";
        //    }
        //    var adjustedPosA = posA + a.Length;
        //    if (adjustedPosA >= posB)
        //    {
        //        return "";
        //    }
        //    return value.Substring(adjustedPosA, posB - adjustedPosA);
        //}

        /// <summary>
        /// Get string value between [first] a and [last] b.
        /// </summary>
        public static string Between(this string value, string a, string b)
        {
            var posA = value.IndexOf(a, StringComparison.Ordinal);
            if (posA == -1)
            {
                return "";
            }
            var posB = value.IndexOf(b, posA, StringComparison.Ordinal);
            if (posB == -1)
            {
                return "";
            }
            var adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        /// <summary>
        /// Get string value after [first] a.
        /// </summary>
        public static string Before(this string value, string a)
        {
            var posA = value.IndexOf(a, StringComparison.Ordinal);
            if (posA == -1)
            {
                return "";
            }
            return value.Substring(0, posA);
        }

        /// <summary>
        /// Get string value after [last] a.
        /// </summary>
        public static string After(this string value, string a)
        {
            var posA = value.LastIndexOf(a, StringComparison.Ordinal);
            if (posA == -1)
            {
                return "";
            }
            var adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length)
            {
                return "";
            }
            return value.Substring(adjustedPosA);
        }

        public static string ToHtmlTable<T>(this List<T> listOfClassObjects)
        {
            var ret = string.Empty;

            return listOfClassObjects == null || !listOfClassObjects.Any()
                ? ret
                : "<table width='100%'>" +
                  listOfClassObjects.First().GetType().GetProperties().Select(p => p.Name).ToList().ToColumnHeaders() +
                  listOfClassObjects.Aggregate(ret, (current, t) => current + t.ToHtmlTableRow()) +
                  "</table>";
        }

        public static string ToColumnHeaders<T>(this List<T> listOfProperties)
        {
            var ret = string.Empty;

            return listOfProperties == null || !listOfProperties.Any()
                ? ret
                : "<tr>" +
                  listOfProperties.Aggregate(ret,
                      (current, propValue) =>
                          current + "<th style='font-size: 11pt; font-weight: bold;'>" + (Convert.ToString(propValue).Length <= 100
                              ? Convert.ToString(propValue)
                              : Convert.ToString(propValue).Substring(0, 100)) + "</th>") +
                  "</tr>";
        }

        public static string ToHtmlTableRow<T>(this T classObject)
        {
            var ret = string.Empty;

            return classObject == null
                ? ret
                : "<tr>" +
                  classObject.GetType()
                      .GetProperties()
                      .Aggregate(ret,
                          (current, prop) =>
                              current + "<td style='font-size: 11pt; font-weight: normal;'>" + (Convert.ToString(prop.GetValue(classObject, null)).Length <= 100
                                  ? Convert.ToString(prop.GetValue(classObject, null))
                                  : Convert.ToString(prop.GetValue(classObject, null)).Substring(0, 100) +
                                    "...") + "</td>") + "</tr>";
        }
    }
}
