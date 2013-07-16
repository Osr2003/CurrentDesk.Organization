#region Header Information
/***********************************************************************
 * File Name     : FeeStructureModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 21st March 2013
 * Modified Date : 21st March 2013
 * Description   : This file contains view model for FeeStructure view
 * ********************************************************************/
#endregion

#region Namespace Used
using System.Collections.Generic;
#endregion

namespace CurrentDesk.BackOffice.Areas.IntroducingBroker.Models
{
    /// <summary>
    /// This class represents view model for FeeStructure view
    /// </summary>
    public class FeeStructureModel
    {
        public int WidenSpread { get; set; }
        public int IncreasedCommission { get; set; }
        public int AccountCurrency { get; set; }
    }

    public class FeeStructure
    {
        public int PK_FeeID { get; set; }
        public string StructureName { get; set; }
        public string SpreadMarkUp { get; set; }
        public string CommissionMarkUp { get; set; }
        public string Currency { get; set; }
        public string WidenSpreadOther { get; set; }
        public string CommissionSpreadOther { get; set; }
        public string Action { get; set; }
    }
}