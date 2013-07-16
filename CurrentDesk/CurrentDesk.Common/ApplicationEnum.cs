#region Header Information
/***************************************************************************
 * File Name     : ApplicationEnum.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 14th Jan 2013
 * Modified Date : 14th Jan 2013
 * Description   : This file contains different enums used in different layers of app
 * *************************************************************************/
#endregion

#region Namespace Used
#endregion

namespace CurrentDesk.Common
{
    public class ApplicationEnum
    {

    }

    /// <summary>
    /// Account Type Indicating different type of account
    /// </summary>
    public enum LoginAccountType
    {
        /// <summary>
        /// Indicates Live Account
        /// </summary>
        LiveAccount,
        /// <summary>
        /// Indicates Partner Account
        /// </summary>
        PartnerAccount,
        /// <summary>
        /// Indicates Partner Account
        /// </summary>
        AdminAccount
    }

    /// <summary>
    /// Indicates position of account number creation sub parts
    /// </summary>
    public enum AccountCreationPosition
    {
        Currency = 1,
        AccountNumberBelongingToThatCurrency = 2,
        ClientAccountNumber = 3
    }

    /// <summary>
    /// Indicates types of Activity by users
    /// </summary>
    public enum ActivityType
    {
        ProfileActivity = 1,
        DocumentActivity = 2,
        AccountActivity = 3,
        TransferActivity = 4,
        ConversionActivity = 5,
        DepositOrWithdrawActivity = 6
    }

    /// <summary>
    /// Indicates types of Account activity of users
    /// </summary>
    public enum AccountActivityType
    {
        NewAccountCreation = 1
    }

    /// <summary>
    /// Periods
    /// </summary>
    public enum Periods
    {
        Monthly = 1,
        Quaterly = 2,
        Annualy = 3
    }

    /// <summary>
    /// Admin Transaction Type
    /// </summary>
    public enum AdminTransactionType
    {
        IncomingFunds = 1,
        OutgoingFunds = 2,
        InternalTransfers = 3,
        ConversionsRequests = 4
    }

    /// <summary>
    /// Transfer Approval Options
    /// </summary>
    public enum TransferApprovalOptions
    {
        Immediate = 1,
        Administrator = 2,
        Limited = 3
    }

    /// <summary>
    /// MT4 commands
    /// </summary>
    public enum TradeCommands : short { OP_BUY = 0, OP_SELL, OP_BUY_LIMIT, OP_SELL_LIMIT, OP_BUY_STOP, OP_SELL_STOP, OP_BALANCE, OP_CREDIT };

    /// <summary>
    /// Trade transaction types
    /// </summary>
    public enum TradeTransTypes : short
    {
        //---
        TT_PRICES_GET,                      // prices requests
        TT_PRICES_REQUOTE,                  // re-quote

        //--- client trade transaction
        TT_ORDER_IE_OPEN = 64,              // open order (Instant Execution)
        TT_ORDER_REQ_OPEN,                  // open order (Request Execution)
        TT_ORDER_MK_OPEN,                   // open order (Market Execution)
        TT_ORDER_PENDING_OPEN,              // open pending order

        //---
        TT_ORDER_IE_CLOSE,                  // close order (Instant Execution)
        TT_ORDER_REQ_CLOSE,                 // close order (Request Execution)
        TT_ORDER_MK_CLOSE,                  // close order (Market Execution)

        //---
        TT_ORDER_MODIFY,                    // modify pending order
        TT_ORDER_DELETE,                    // delete pending order
        TT_ORDER_CLOSE_BY,                  // close order by order
        TT_ORDER_CLOSE_ALL,                 // close all orders by symbol

        //--- broker trade transactions
        TT_BR_ORDER_OPEN,                   // open order
        TT_BR_ORDER_CLOSE,                  // close order
        TT_BR_ORDER_DELETE,                 // delete order (ANY OPEN ORDER!!!)
        TT_BR_ORDER_CLOSE_BY,               // close order by order
        TT_BR_ORDER_CLOSE_ALL,              // close all orders by symbol
        TT_BR_ORDER_MODIFY,                 // modify open price, stop loss, take profit etc. of order
        TT_BR_ORDER_ACTIVATE,               // activate pending order
        TT_BR_ORDER_COMMENT,                // modify comment of order
        TT_BR_BALANCE                       // balance/credit
    };

    /// <summary>
    /// Conversion Markup Type
    /// </summary>
    public enum ConversionMarkupType
    {
        Points = 1,
        Percentage = 2
    }
}
