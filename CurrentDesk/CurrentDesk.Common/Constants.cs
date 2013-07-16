/* **************************************************************
* File Name     :- Constants.cs
* Author        :- Mukesh Nayak
* Copyright     :- Mindfire Solutions 
* Date          :- 10th Jan 2013
* Modified Date :- 10th Jan 2013
* Description   :- This file contains all the constants.
****************************************************************/

#region Namespace
#endregion


namespace CurrentDesk.Common
{
    /// <summary>
    /// Constant class with all the constant
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// LIVE INDIVIDUAL ACCOUNT
        /// </summary>
        public static int K_LIVE_INDIVIDUAL = 4;

        /// <summary>
        /// LIVE JOINT ACCOUNT
        /// </summary>
        public static int K_LIVE_JOINT = 5;

        /// <summary>
        ///LIVE CORPORATE ACCOUNT
        /// </summary>
        public static int K_LIVE_CORPORATE = 6;

        /// <summary>
        /// LIVE TRUST ACCOUNT
        /// </summary>
        public static int K_LIVE_TRUST = 7;


        /// <summary>
        /// INDIVIDUAL ACCOUNT
        /// </summary>
        public static int K_ACCT_INDIVIDUAL = 1;

        /// <summary>
        /// JOINT ACCOUNT
        /// </summary>
        public static int K_ACCT_JOINT = 2;

        /// <summary>
        /// CORPORATE ACCOUNT
        /// </summary>
        public static int K_ACCT_CORPORATE = 3;

        /// <summary>
        /// TRUST ACCOUNT
        /// </summary>
        public static int K_ACCT_TRUST = 4;

        /// <summary>
        /// PARTNER INDIVIDUAL ACCOUNT
        /// </summary>
        public static int K_PARTNER_INDIVIDUAL = 8;

        /// <summary>
        /// PARTNER JOINT ACCOUNT
        /// </summary>
        public static int K_PARTNER_JOINT = 9;

        /// <summary>
        /// PARTNER CORPORATE ACCOUNT
        /// </summary>
        public static int K_PARTNER_CORPORATE = 10;

        /// <summary>
        /// PARTNER TRUST ACCOUNT
        /// </summary>
        public static int K_PARTNER_TRUST = 11;

        /// <summary>
        /// TRUSTEE TYPE INDIVIDUAL
        /// </summary>
        public static int K_TRUSTEETYPE_INDIVIDUAL = 2;

        /// <summary>
        /// TRUSTEE TYPE COMPANY
        /// </summary>
        public static int K_TRUSTEETYPE_COMPANY = 1;

        /// <summary>
        /// INTRODUCING BROKER CODE
        /// </summary>
        public const int K_ACCTCODE_IB = 4;

        /// <summary>
        /// ASSET MANAGER CODE
        /// </summary>
        public const int K_ACCTCODE_AM = 5;

        /// <summary>
        /// SUPER ADMIN CODE
        /// </summary>
        public const int  K_ACCTCODE_SUPERADMIN = 7;


        public static string K_ENC_KEY = "backoffice";
        public static string K_ENC_SALT = "CURRENTDESKBACKOFFICE";

        /// <summary>
        /// BROKER DEMO ACCOUNT
        /// </summary>
        public static int K_BROKER_DEMO = 1;

        /// <summary>
        /// BROKER  LIVE ACCOUNT
        /// </summary>
        public static int K_BROKER_LIVE = 2;

        /// <summary>
        /// BROKER PARTNER ACCOUNT
        /// </summary>
        public static int K_BROKER_PARTNER = 3;

        /// <summary>
        /// BROKER ADMIN ACCOUNT
        /// </summary>
        public static int K_BROKER_ADMIN = 4;

        /// <summary>
        /// TRADING ACCOUNT
        /// </summary>
        public const int K_TRADING_ACCOUNT = 2;

        /// <summary>
        /// MANAGED ACCOUNT
        /// </summary>
        public const int K_MANAGED_ACCOUNT = 3;

        /// <summary>
        /// META TRADER PLATFORM
        /// </summary>
        public const int K_META_TRADER = 3;

        /// <summary>
        /// AM META TRADER PLATFORM
        /// </summary>
        public const int K_AM_META_TRADER = 4;

        /// <summary>
        /// META TRADER ID
        /// </summary>
        public const int K_META_TRADER_ID = 1;

        public static string K_DEPOSIT = "deposit";
        public static string K_WITHDRAW = "withdraw";
        public static string K_STATUS_PENDING = "pending";
        public static string K_STATUS_TRANSFERRED = "transferred";

    }
}
