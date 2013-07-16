#region Header Information
/*********************************************************************
 * File Name     : Extensions.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 19th March 2013
 * Modified Date : 19th March 2013
 * Description   : This file contains extensions methods for null checks
 *                 and tryparse
 * *******************************************************************/
#endregion

#region Namespace Used
using System;
using System.Globalization;
#endregion

namespace CurrentDesk.Common
{
    /// <summary>
    /// This class contains extensions methods for null checks and tryparse
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Check whether the input is null or Dbnull
        /// </summary>
        /// <param name="obj">input</param>
        /// <returns>Boolean output</returns>
        public static bool IsNotNull(this object obj)
        {
            return obj != null && obj != DBNull.Value;
        }

        /// <summary>
        /// Trypase the input db value to string value
        /// </summary>
        /// <param name="input">input object</param>
        /// <returns>output string</returns>
        public static string StringTryParse(this object input)
        {
            string output = string.Empty;

            //Check whether the input is null or not
            if (input.IsNotNull())
            {
                output = input.ToString();
            }

            //Return the output string
            return output;
        }

        /// <summary>
        /// Try parse the input db value to integer value
        /// </summary>
        /// <param name="input">input object</param>
        /// <returns>output integer</returns>
        public static int Int32TryParse(this object input)
        {
            int output = 0;

            //Check whether the input is null or not
            if (input.IsNotNull())
            {
                Int32.TryParse(input.ToString(), out output);
            }

            //Return the output int
            return output;
        }

        /// <summary>
        /// Try parse the input db value to datetime value
        /// </summary>
        /// <param name="input">input object</param>
        /// <returns>output datetime</returns>
        public static DateTime DateTimeTryParse(this object input)
        {
            DateTime output = DateTime.Now;

            //Check whether the input is null or not
            if (input.IsNotNull())
            {
                DateTime.TryParse(input.ToString(), out output);
            }

            //Return the output datetime
            return output;
        }

        /// <summary>
        /// Try parse the input db value to decimal value
        /// </summary>
        /// <param name="input">input object</param>
        /// <returns>output decimal</returns>
        public static decimal DecimalTryParse(this object input)
        {
            Decimal output = Decimal.Zero;

            //Check whether the input is null or not
            if (input.IsNotNull())
            {
                Decimal.TryParse(input.ToString(), out output);
            }

            //Return the decimal output
            return output;
        }

        /// <summary>
        /// Convert to float 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static float FloatTryParse(this object input)
        {
            float output = 0F;

            //Check whether the input is null or not
            if (input.IsNotNull())
            {
                float.TryParse(input.ToString(), out output);
            }

            //Return the decimal output
            return output;
        }

        /// <summary>
        /// Try parse the input db value to boolean value
        /// </summary>
        /// <param name="input">input object</param>
        /// <returns>boolean output</returns>
        public static bool BooleanTryParse(this object input)
        {
            bool output = false;

            //Check whether the input is null or not
            if (input.IsNotNull())
            {
                Boolean.TryParse(input.ToString(), out output);
            }

            //Return the boolean output
            return output;
        }

        public static DateTime TimeStampToDateTime(this long objTimeStamp)
        {
            DateTime dt = DateTime.Now;
            if (objTimeStamp != null)
            {
                // First make a System.DateTime equivalent to the UNIX Epoch.
                var dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

                // Add the number of seconds in UNIX timestamp to be converted.
                dt = dateTime.AddSeconds( Convert.ToDouble(objTimeStamp));
            }

            return dt;
        }


        /// <summary>
        /// Convert array of double to comma separated string
        /// </summary>
        /// <param name="arrNumber"></param>
        /// <returns></returns>
        public static string DoubleArrayToString(this double[]  arrNumber)
        {
            var s = string.Empty;

            if (arrNumber != null && arrNumber.Length > 0)
            {
                s = string.Join(",", arrNumber);
            }
            return s;
        }

        /// <summary>
        /// Concert array of integer to comma separated string
        /// </summary>
        /// <param name="arrNumber"></param>
        /// <returns></returns>
        public static string  IntArrayToString(this int[]  arrNumber)
        {
            var s = string.Empty;
            if (arrNumber != null && arrNumber.Length > 0)
            {
                s = string.Join(",", arrNumber);
            }

            return s;
        }

        /// <summary>
        /// Try parse the input db value to decimal value
        /// </summary>
        /// <param name="input">input object</param>
        /// <returns>output decimal</returns>
        public static double DoubleTryParse(this object input)
        {
            Double output = 0.0D;

            //Check whether the input is null or not
            if (input.IsNotNull())
            {
                Double.TryParse(input.ToString(), out output);
            }

            //Return the decimal output
            return output;
        }

        /// <summary>
        /// Cuurency format
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CurrencyFormat(this object input)
        {
            if (input != null)
            {
                System.Globalization.NumberFormatInfo nfi;
                nfi = new NumberFormatInfo();
                nfi.CurrencySymbol = "";
                return String.Format(nfi, "{0:C}", input);
            }
            else
            {
                return "0";
            }
        }
    }
}
