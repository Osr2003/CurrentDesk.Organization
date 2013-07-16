#region Header Information
/*********************************************************************************
 * File Name     : DocumentModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 5th April 2013
 * Modified Date : 5th April 2013
 * Description   : This file contains view model  for Documents page
 * ******************************************************************************/
#endregion

#region Namespace Used
using System.Collections.Generic;
#endregion

namespace CurrentDesk.BackOffice.Models.Document
{
    /// <summary>
    /// This class represents view model for Documents page
    /// </summary>
    public class DocumentModel
    {
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public string Status { get; set; }
    }
}