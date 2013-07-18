#region Header Information
/*********************************************************************
 * File Name     : Utility.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 18th July 2013
 * Modified Date : 18th July 2013
 * Description   : This file contains all utility methods used in application
 * *******************************************************************/
#endregion

#region Namespace Used

using System;
using System.Globalization;
#endregion

namespace CurrentDesk.BackOffice.Utilities
{
    /// <summary>
    /// This class contains all utility methods used in application
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// This method formats currency value as per symbol passed
        /// </summary>
        /// <param name="amount">amount</param>
        /// <param name="currSymbol">currSymbol</param>
        /// <returns></returns>
        public static string FormatCurrencyValue(decimal amount, string currSymbol)
        {
            var nfi = new NumberFormatInfo {CurrencySymbol = currSymbol};

            return String.Format(nfi, "{0:C}", amount);
        }
    }
}