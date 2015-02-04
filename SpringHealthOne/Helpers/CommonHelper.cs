using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpringHealthOne.Helpers
{
    public class CommonHelper
    {
        /// <summary>
        /// Gets query string value by name
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <returns>Query string value</returns>
        public static string QueryString(string name)
        {
            string result = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Request.QueryString[name] != null)
                result = HttpContext.Current.Request.QueryString[name].ToString();
            return result;
        }

        /// <summary>
        /// Gets boolean value from query string 
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <returns>Query string value</returns>
        public static bool QueryStringBool(string name)
        {
            string resultStr = QueryString(name).ToUpperInvariant();
            return (resultStr == "YES" || resultStr == "TRUE" || resultStr == "1");
        }

        /// <summary>
        /// Gets integer value from query string 
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <returns>Query string value</returns>
        public static int QueryStringInt(string name)
        {
            string resultStr = QueryString(name).ToUpperInvariant();
            int result;
            Int32.TryParse(resultStr, out result);
            return result;
        }
    }
}