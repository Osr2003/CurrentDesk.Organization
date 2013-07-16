#region Header Information
/***************************************************************
 * File Name     : CurrencyAccountModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 8th Feb 2013
 * Modified Date : 8th Feb 2013
 * Description   : This file contains model for data displayed in 
 *                 different currencies grid in MyAccounts page
 * *************************************************************/
#endregion

#region Namespace Used
#endregion

namespace CurrentDesk.BackOffice.Models.MyAccount
{
    /// <summary>
    /// This class represents model for data displayed in 
    /// different currencies grid in MyAccounts page
    /// </summary>
    public class CurrencyAccountModel
    {
        public string Type { get; set; }
        public string Account {get; set;}
		public string Balance {get; set;}	
		public string Floating {get; set;}
		public string Equity {get; set;}
        public string Change { get; set; }
        public bool? IsTradingAccount { get; set; }
        public string FeeStructure { get; set; }
        public int PlatFormLogin { get; set; }
    }
}