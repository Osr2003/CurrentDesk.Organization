#region Header Information
/*****************************************************************************
 * File Name     : MetaTraderWrapperManager.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 20th Feb 2013
 * Modified Date : 20th Feb 2013
 * Description   : This file contains metohds that interact with Meta trader
 *                 api and returns result
 * ***************************************************************************/
#endregion

#region Namespace Used
using System.Collections.Generic;
using System.Linq;
using MT4ManLibraryNETv03;
#endregion

namespace MT4Wrapper
{
    public class MetaTraderWrapperManager
    {


        #region "Credentials"

        const string ServerName = "mtdem01.primexm.com:443";
        const string Account = "900";
        const string PassWord = "!FQS123!!";


        #endregion

        private MT4ManLibraryNETv03.CMTManager _manager;
        

        public MetaTraderWrapperManager(string server, int account, string pass)
        {
            _manager = new MT4ManLibraryNETv03.CMTManager();
            int iRet = _manager.ManagerFactory();
            iRet = _manager.Connect(server);
            iRet = _manager.Login(account, pass);
            //MT4ManLibraryNETv03.PumpFuncDelegate myPumpFunc = new MT4ManLibraryNETv03.PumpFuncDelegate(callback);
            //_manager.PassMeADelegate(myPumpFunc);
            //_manager.PumpingSwitch();

        }

       //public MetaTraderWrapperManager()
       //{
       //     _manager = new MT4ManLibraryNETv03.CMTManager();
       //     int iRet = _manager.ManagerFactory();
       //     iRet = _manager.Connect(ServerName);
       //     iRet = _manager.Login(Account, PassWord);
       //}

        /// <summary>
        /// Check Whether User Is Conneceted or Not 
        /// </summary>
        /// <returns>Return 1 if connected or 0 If Not</returns>
        public int IsConnected()
        {
            return _manager.IsConnected();
        }

        /// <summary>
        /// This Function Will Create New Recored
        /// </summary>
        /// <param name="newUser">new User</param>
        /// <returns>integer 0, 1 depending Upon the Result of Operation</returns>
        public int CreateNewUser(MT4ManLibraryNETv03.UserRecordNET user)
        {
            return _manager.UserRecordNew(user);
        }

        /// <summary>
        /// This Function will Update the data For the Selected User
        /// </summary>
        /// <param name="selecetdUser">selecetdUser</param>
        /// <returns></returns>
        public int UpdateUser(MT4ManLibraryNETv03.UserRecordNET selecetdUser)
        {
            return _manager.UserRecordUpdate(selecetdUser);
        }

        /// <summary>
        /// This Function Will Get The Trade History 
        /// </summary>
        /// <param name="login">login</param>
        /// <param name="from">from</param>
        /// <param name="to">to</param>
        /// <param name="total">total</param>
        /// <returns>Array of MT4ManLibraryNETv03.TradeRecordNET[] </returns>
        public dynamic GetUsersTradeHistory(int login, long from, long to, ref int total)
        {
            //MT4ManLibraryNETv03.TradeRecordNET[]
            return _manager.TradesUserHistory(login, from, to, ref total);
        }

        /// <summary>
        /// This Function will return all the open trades of the users.
        /// </summary>
        /// <param name="total">Total of open Trdes</param>
        /// <returns>Array of MT4ManLibraryNETv03.TradeRecordNET[]</returns>
        public MT4ManLibraryNETv03.TradeRecordNET[] GetUsersOpenTrades(ref int total)
        {
            //MT4ManLibraryNETv03.TradeRecordNET[]
            return _manager.TradesGet(ref total);
        }

        /// <summary>
        /// This Function will return all the closed trades of the users.
        /// </summary>
        /// <param name="total">Total of open Trdes</param>
        /// <returns>Array of MT4ManLibraryNETv03.TradeRecordNET[]</returns>
        public dynamic GetUsersCloseTrades(ref int total)
        {
            //MT4ManLibraryNETv03.TradeRecordNET[]
            return _manager.TradesRequest(ref total);
        }



        /// <summary>
        /// This Function Will Get Values
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, double> GetValues()
        {
            MT4ManLibraryNETv03.SymbolSummaryNET[] summarys;
            int iRef = 0;
            summarys = _manager.SummaryGetAll(ref iRef);
            Dictionary<string, double> values = new Dictionary<string, double>();
            summarys.ToList().ForEach(sum =>
            {
                values.Add(sum.strSymbol, sum.buylots - sum.selllots);
            });
            return values;
        }

        protected void callback(int code)
        {

        }

        public void Disconnect()
        {
            if (_manager != null)
            {
                _manager.Disconnect();
            }
        }

        public string ErrorDescription(int status)
        {
            return _manager.ErrorDescription(status);
        }

        ~MetaTraderWrapperManager()
        {
            if (_manager != null)
            {
                _manager.Disconnect();
            }
        }

        public int UserRecordGet(int code)
        {
            //PumpFuncDelegate pumping = new PumpFuncDelegate(ClbkUserRecordGet);
            //iRet = manager.PassMeADelegate(pumping);
            //manager.PumpingSwitch();
            return 0;
        }


        public void ClbkUserRecordGet(int code)
        {

        }

        /// <summary>
        /// This method credits or debits from MT4 login account,
        /// negative for withdrawal and positive for deposit
        /// </summary>
        /// <param name="newTrade"></param>
        /// <returns></returns>
        public int TradeTransaction(TradeTransInfoNET newTransaction)
        {
            return _manager.TradeTransaction(ref newTransaction);
        }
    }
}
