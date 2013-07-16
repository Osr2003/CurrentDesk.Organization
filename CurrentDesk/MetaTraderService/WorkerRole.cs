#region Header Information
/*****************************************************************************
 * File Name     : WorkerRole.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 11th March 2013
 * Modified Date : 11th March 2013
 * Description   : This file represents azure worker role(service) and contains code that
 *                 is called repeatedly after fixed interval of time
 * ***************************************************************************/
#endregion

#region Namespace Used
using System.Diagnostics;
using System.Net;
using System.Threading;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure;
using Microsoft.ApplicationServer.Caching;

using CurrentDesk.Models;
using CurrentDesk.Common;
using CurrentDesk.Logging;
using CurrentDesk.Repository;
using CurrentDesk.Repository.CurrentDesk;

using MT4Wrapper;
using MT4ManLibraryNETv03;
#endregion

namespace MetaTraderService
{
    /// <summary>
    /// Synch BackOffice tables using MT4 API
    /// </summary>
    public class WorkerRole : RoleEntryPoint
    {


        #region "Constants"

        const int PUMP_START_PUMPING = 0;
        const int PUMP_UPDATE_SYMBOLS = 1;
        const int TEST_LOCAL = 777;
        const int PUMP_UPDATE_GROUPS = 2;
        const int PUMP_UPDATE_USERS = 3;
        const int PUMP_UPDATE_ONLINE = 4;
        const int PUMP_UPDATE_BIDASK = 5;
        const int PUMP_UPDATE_NEWS = 6;
        const int PUMP_UPDATE_NEWS_BODY = 7;
        const int PUMP_UPDATE_MAIL = 8;
        const int PUMP_UPDATE_TRADES = 9;
        const int PUMP_UPDATE_REQUESTS = 10;
        const int PUMP_UPDATE_PLUGINS = 11;
        const int PUMP_UPDATE_ACTIVATION = 12;
        const int PUMP_UPDATE_MARGINCALL = 13;
        const int PUMP_STOP_PUMPING = 14;
        const int PUMP_PING = 15;

        const int BATCHSIZE = 1000;

        enum Transaction : int { TRANS_ADD = 0, TRANS_DELETE, TRANS_UPDATE, TRANS_CHANGE_GRP };


        /// <summary>
        /// Pumping mode flags    
        /// </summary>
        enum PumpingFlags : int
        {
            //--- user flags
            CLIENT_FLAGS_HIDETICKS = 1,     // do not send ticks
            CLIENT_FLAGS_HIDENEWS = 2,     // do not send news
            CLIENT_FLAGS_HIDEMAIL = 4,     // do not send mails
            CLIENT_FLAGS_SENDFULLNEWS = 8,     // send news body with news header in pumping mode
            CLIENT_FLAGS_RESERVED = 16,    // reserved
            //--- manager flags
            CLIENT_FLAGS_HIDEONLINE = 32,    // do not send online users table
            CLIENT_FLAGS_HIDEUSERS = 64     // do not send users table
        };

        #endregion

        #region "Broker Constants"

        const string BrokerSymbol = ".fq";

        #endregion

        #region "Variable"

        public MT4ManLibraryNETv03.CMTManager manager = new MT4ManLibraryNETv03.CMTManager();

        //Require for pumping without ticks
        public MT4ManLibraryNETv03.CMTManager manager2 = new MT4ManLibraryNETv03.CMTManager();
        public MT4ManLibraryNETv03.CMTManager managerTrades = new MT4ManLibraryNETv03.CMTManager();

        public MT4ManLibraryNETv03.CMTManager managerA1 = new MT4ManLibraryNETv03.CMTManager();
        public MT4ManLibraryNETv03.CMTManager managerTradesA1 = new MT4ManLibraryNETv03.CMTManager();

        public MT4ManLibraryNETv03.CMTManager managerA2 = new MT4ManLibraryNETv03.CMTManager();
        public MT4ManLibraryNETv03.CMTManager managerTradesA2 = new MT4ManLibraryNETv03.CMTManager();

        public MT4ManLibraryNETv03.CMTManager managerA3 = new MT4ManLibraryNETv03.CMTManager();
        public MT4ManLibraryNETv03.CMTManager managerTradesA3 = new MT4ManLibraryNETv03.CMTManager();

        public MT4ManLibraryNETv03.CMTManager managerA4 = new MT4ManLibraryNETv03.CMTManager();
        public MT4ManLibraryNETv03.CMTManager managerTradesA4 = new MT4ManLibraryNETv03.CMTManager();

        public MT4ManLibraryNETv03.CMTManager managerA5 = new MT4ManLibraryNETv03.CMTManager();
        public MT4ManLibraryNETv03.CMTManager managerTradesA5 = new MT4ManLibraryNETv03.CMTManager();


        public MT4ManLibraryNETv03.CMTManager managerA6 = new MT4ManLibraryNETv03.CMTManager();
        public MT4ManLibraryNETv03.CMTManager managerTradesA6 = new MT4ManLibraryNETv03.CMTManager();

        public Task t;

        MetaTraderTrades metaTrader;

        //Task for TickInfoLast        
        List<string> lstSymbol = new List<string>();

        Task taskOpenTrades;
        Task taskMargin;
        Task taskPrice;
        Task taskUsers;
        Task taskSymbols;

        Task taskRealtimeMargin;

        Task taskRealtimeMargin1;
        Task taskRealtimeMargin2;
        Task taskRealtimeMargin3;
        Task taskRealtimeMargin4;

        Task taskPrice1;
        Task taskPrice2;
        Task taskPrice3;
        Task taskPrice4;


        bool IsMarginSynchFirsttime = false;

        public MT4ManLibraryNETv03.MarginLevelNET[] arrMarginLevel;
        public MT4ManLibraryNETv03.MarginLevelNET[] arrMarginLevel1;
        public MT4ManLibraryNETv03.MarginLevelNET[] arrMarginLevel2;
        public MT4ManLibraryNETv03.MarginLevelNET[] arrMarginLevel3;


        public List<MarginLevelNET> lstTempMargins = new List<MarginLevelNET>();
        public List<MarginLevelNET> lstTempMargins1 = new List<MarginLevelNET>();
        public List<MarginLevelNET> lstTempMargins2 = new List<MarginLevelNET>();
        public List<MarginLevelNET> lstTempMargins3 = new List<MarginLevelNET>();

        public List<SymbolMarginMode> lstSymbMarginMode = new List<SymbolMarginMode>();

        bool isBulkSymbolColpied = false;
        #endregion

        #region "Cache Variable"

        public DataCache marginCache = null;
        public DataCache tradesCache = null;
        public DataCache priceCache = null;

        public DataCache marginCache1 = null;
        public DataCache tradesCache1 = null;
        public DataCache priceCache1 = null;

        public DataCache marginCache2 = null;
        public DataCache tradesCache2 = null;
        public DataCache priceCache2 = null;

        public DataCache marginCache3 = null;
        public DataCache tradesCache3 = null;
        public DataCache priceCache3 = null;

        public DataCache marginCache4 = null;
        public DataCache tradesCache4 = null;
        public DataCache priceCache4 = null;

        public DataCache marginCache5 = null;
        public DataCache tradesCache5 = null;
        public DataCache priceCache5 = null;

        public DataCache marginCache6 = null;
        public DataCache tradesCache6 = null;
        public DataCache priceCache6 = null;

        
        #endregion

        #region "Server Credentials"

        string ServerName = "mtdem01.primexm.com:443";
        int ManagerId = 900;
        string ManagerPassword =  "!FQS123!!";

        string ServerNameL = "mtpro01.primexm.com:443";
        int ManagerIdL = 707;
        string ManagerPasswordL = "3$MtM#$2013";


        #endregion

        #region "Azure Events"

        /// <summary>
        /// Connect to metatrader in pumping mode and synch Trades,Price,Margin  tables
        /// </summary>
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("MetaTraderService entry point called", "MetaTrader Service");
            CurrentDeskLog.Info("Started Run");

            //Run new thread to update last trading date
            //UpdateLastTradingDateThread();
            bool isCacheCreated =  false;
            while (!isCacheCreated)
            {
                try
                {
                    marginCache = new DataCache("MarginCache");
                    if (marginCache != null)
                    {
                        isCacheCreated = true;
                    }

                    marginCache = new DataCache("MarginCache");
                    tradesCache = new DataCache("TradesCache");
                    priceCache = new DataCache("PriceCache");

                    CurrentDeskLog.Info("All Cache Created 1st");

                    marginCache1 = new DataCache("MarginCache1");
                    tradesCache1 = new DataCache("TradesCache1");
                    priceCache1 = new DataCache("PriceCache1");

                    CurrentDeskLog.Info("All Cache Created 2nd");

                    marginCache2 = new DataCache("MarginCache2");
                    tradesCache2 = new DataCache("TradesCache2");
                    priceCache2 = new DataCache("PriceCache2");

                    CurrentDeskLog.Info("All Cache Created 3rd");

                    marginCache3 = new DataCache("MarginCache3");
                    tradesCache3 = new DataCache("TradesCache3");
                    priceCache3 = new DataCache("PriceCache3");

                    CurrentDeskLog.Info("All Cache Created 4th");

                    CurrentDeskLog.Info("All Cache Created");


                }
                catch (Exception ex)
                {
                    isCacheCreated = false;
                    CurrentDeskLog.Error(ex.Message, ex);

                }

                Thread.Sleep(5000);
            }
            //Create Caches
            

            GetMT4Crdential(true);
          
            /*
            SynchBackOfficeTables();
            
            */

            #region "Set Pumping"

            try
            {
                CurrentDeskLog.Info("Initialize Pumping");

                ConnectToManager(manager, ServerName, ManagerId, ManagerPassword);
                int flags = ((int)PumpingFlags.CLIENT_FLAGS_HIDEMAIL) | ((int)PumpingFlags.CLIENT_FLAGS_HIDENEWS) | ((int)PumpingFlags.CLIENT_FLAGS_HIDEONLINE);

                PumpFuncExDelegate pumpEx = new PumpFuncExDelegate(CallBackMethodEx);

                var iPumpRet = manager.PassMeAnExDelegate(pumpEx);
                iPumpRet = manager.PumpingSwitchEx(flags, null);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message,ex);
            }


          

            try
            {
                ConnectToManager(managerA1, ServerName, ManagerId, ManagerPassword);

                CurrentDeskLog.Info("Initialize Pumping 1");

                int flags1= ((int)PumpingFlags.CLIENT_FLAGS_HIDEMAIL) | ((int)PumpingFlags.CLIENT_FLAGS_HIDENEWS) | ((int)PumpingFlags.CLIENT_FLAGS_HIDEONLINE);

                PumpFuncExDelegate pumpEx1 = new PumpFuncExDelegate(CallBackMethodEx1);

                var iPumpRet = managerA1.PassMeAnExDelegate(pumpEx1);
                iPumpRet = managerA1.PumpingSwitchEx(flags1, null);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }


            try
            {
                ConnectToManager(managerA2, ServerName, ManagerId, ManagerPassword);

                CurrentDeskLog.Info("Initialize Pumping 2");

                int flags2 = ((int)PumpingFlags.CLIENT_FLAGS_HIDEMAIL) | ((int)PumpingFlags.CLIENT_FLAGS_HIDENEWS) | ((int)PumpingFlags.CLIENT_FLAGS_HIDEONLINE);

                PumpFuncExDelegate pumpEx2 = new PumpFuncExDelegate(CallBackMethodEx2);

                var iPumpRet2 = managerA2.PassMeAnExDelegate(pumpEx2);
                iPumpRet2 = managerA2.PumpingSwitchEx(flags2, null);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }


            try
            {
                CurrentDeskLog.Info("Initialize Pumping 3");

                ConnectToManager(managerA3, ServerNameL, ManagerIdL, ManagerPasswordL);

                int flags3 = ((int)PumpingFlags.CLIENT_FLAGS_HIDEMAIL) | ((int)PumpingFlags.CLIENT_FLAGS_HIDENEWS) | ((int)PumpingFlags.CLIENT_FLAGS_HIDEONLINE);

                PumpFuncExDelegate pumpEx3 = new PumpFuncExDelegate(CallBackMethodEx3);

                var iPumpRet3 = managerA3.PassMeAnExDelegate(pumpEx3);
                iPumpRet3 = managerA3.PumpingSwitchEx(flags3, null);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }


            #endregion

            //try
            //{
            //    //New thread to Synch HistoryTables
            //    Thread t = new Thread(new ThreadStart(SynchHistory));
            //    t.Start();

            //}
            //catch (Exception ex)
            //{
            //    CurrentDeskLog.Error(ex.Message, ex);
            //}

            try
            {
                Thread threadSynchOpenTrades = new Thread(new ThreadStart(SynchTradesInCache));
                threadSynchOpenTrades.Start();
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
            
            try
            {
                Thread threadSynchOpenTrades1 = new Thread(new ThreadStart(SynchTradesInCacheMa1));
                threadSynchOpenTrades1.Start();
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }


            try
            {
                Thread threadSynchOpenTrades2 = new Thread(new ThreadStart(SynchTradesInCacheMa2));
                threadSynchOpenTrades2.Start();
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }

            try
            {
                Thread threadSynchOpenTrades3 = new Thread(new ThreadStart(SynchTradesInCacheMa3));
                threadSynchOpenTrades3.Start();
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }

            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }

        #endregion

        #region "Convert MT4 type to BO type"

        /// <summary>
        /// Convert MT4 margin type to BO Margin
        /// </summary>
        /// <param name="margin"></param>
        /// <returns></returns>
        public Margin GetMargin(MarginLevelNET margin)
        {
            var marginLevel = new Margin
            {
                Group = margin.@group,
                Login = margin.login,
                Leverage = margin.leverage,
                Updated = margin.updated,
                Balance = Convert.ToDecimal(margin.balance),
                Equity = Convert.ToDecimal(margin.equity),
                Volume = margin.volume,
                Margin1 = Convert.ToDecimal(margin.margin),
                MarginFree = Convert.ToDecimal(margin.margin_free),
                MarginLevel = Convert.ToDecimal(margin.margin_level),
                MarginType = margin.margin_type,
                LevelType = margin.level_type

            };

            return marginLevel;
        }

        /// <summary>
        /// Convert MT4 TradeRecord to BO Trade
        /// </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
        public Trade GetTrade(TradeRecordNET tr)
        {
            var trade = new Trade
            {

                Activation = tr.activation,
                ClosePrice = tr.close_price,
                CloseTime = tr.close_time,
                Cmd = tr.cmd,
                Comment = tr.comment,
                Commission = tr.commission,
                CommissionAgent = tr.commission_agent,
                ConvRates = tr.conv_rates[0],
                ConvReserv = tr.conv_reserv,
                Digits = tr.digits,
                Expiration = tr.expiration,
                InternalID = tr.internal_id,
                Login = tr.login,
                Magic = tr.magic,
                MarginRate = tr.margin_rate,
                OpenPrice = tr.open_price,
                OpenTime = tr.open_time,
                OrderID = tr.order,
                Profit = tr.profit,
                Sl = tr.sl,
                Spread = tr.spread,
                State = tr.state,
                Storage = tr.storage,
                Symbol = tr.symbol,
                Taxes = tr.taxes,
                Timestamp = tr.timestamp,
                Tp = tr.tp,
                ValueDate = tr.value_date,
                Volume = tr.volume,
                TradeDateTime = tr.timestamp.TimeStampToDateTime()
            };

            return trade;
        }

        #endregion

        #region "Synch functions"

        /// <summary>
        /// Synch Backoffice table with Meta Trades API
        /// 1. Trades
        /// 2. UserTables
        /// </summary>
        public void SynchBackOfficeTables()
        {
            try
            {
                var allTasks = new List<Task>();
                metaTrader = new MetaTraderTrades();

                #region "Get OpenTrades from MetaTrader Server and Save to Trade table"

                //MT4 server request to Metatrader for OpenTrades
                var total = 0;
                MT4ManLibraryNETv03.TradeRecordNET[] tradeInfo;
                tradeInfo = manager.TradesRequest(ref total);


                DataTable dtTrades = new DataTable("OpenTrades");
                #region "Create DataTable for OpenTrades"

                dtTrades.Columns.Add("PK_TradeID", typeof(int));
                dtTrades.Columns.Add("Activation", typeof(int));
                dtTrades.Columns.Add("ClosePrice", typeof(float));
                dtTrades.Columns.Add("CloseTime", typeof(Int64));
                dtTrades.Columns.Add("Cmd", typeof(int));
                dtTrades.Columns.Add("Comment", typeof(string));
                dtTrades.Columns.Add("Commission", typeof(float));
                dtTrades.Columns.Add("CommissionAgent", typeof(float));
                dtTrades.Columns.Add("ConvRates", typeof(float));
                dtTrades.Columns.Add("ConvReserv", typeof(int));
                dtTrades.Columns.Add("Digits", typeof(int));
                dtTrades.Columns.Add("Expiration", typeof(Int64));
                dtTrades.Columns.Add("InternalID", typeof(int));
                dtTrades.Columns.Add("Login", typeof(int));
                dtTrades.Columns.Add("Magic", typeof(int));
                dtTrades.Columns.Add("MarginRate", typeof(float));
                dtTrades.Columns.Add("OpenPrice", typeof(float));
                dtTrades.Columns.Add("OpenTime", typeof(Int64));
                dtTrades.Columns.Add("OrderID", typeof(int));
                dtTrades.Columns.Add("Profit", typeof(float));
                dtTrades.Columns.Add("Sl", typeof(float));
                dtTrades.Columns.Add("Spread", typeof(int));
                dtTrades.Columns.Add("State", typeof(int));
                dtTrades.Columns.Add("Storage", typeof(float));
                dtTrades.Columns.Add("Symbol", typeof(string));
                dtTrades.Columns.Add("Taxes", typeof(float));
                dtTrades.Columns.Add("Timestamp", typeof(Int64));
                dtTrades.Columns.Add("Tp", typeof(float));
                dtTrades.Columns.Add("ValueDate", typeof(Int64));
                dtTrades.Columns.Add("Volume", typeof(Int32));
             
                  foreach(var item in tradeInfo)
                  {
                      dtTrades.Rows.Add(0, item.activation, item.close_price,
                                                        item.close_time, item.cmd, item.comment,
                                                        item.commission, item.commission_agent,
                                                        item.conv_rates[0], item.conv_reserv,
                                                        item.digits, item.expiration,
                                                        item.internal_id,
                                                        item.login, item.magic, item.margin_rate,
                                                        item.open_price, item.open_time, item.order,
                                                        item.profit, item.sl, item.spread,
                                                        item.state,
                                                        item.storage, item.symbol, item.taxes,
                                                        item.timestamp, item.tp, item.value_date, item.volume);
                                                    }

                  try
                  {


                      TradesHistoryBO tradesHistory = new TradesHistoryBO();
                      string connectionString = tradesHistory.GetConnectionString();

                      bool isClear = false;

                      while (!isClear)
                      {
                          TradeBO tbo = new TradeBO();
                          isClear = tbo.ClearTrades();

                          if (!isClear)
                          {
                              CurrentDeskLog.Info("Truncate fail for Trades");
                              Thread.Sleep(2000);
                          }
                      }

                      if (isClear)
                      {
                          using (SqlConnection connection = new SqlConnection(connectionString))
                          {
                              connection.Open();
                              using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                              {
                                  bulkCopy.BatchSize = BATCHSIZE;
                                  bulkCopy.DestinationTableName =
                                      "dbo.Trades";

                                  try
                                  {
                                      bulkCopy.WriteToServer(dtTrades);
                                  }
                                  catch (Exception ex)
                                  {
                                      CurrentDeskLog.Error(ex.Message, ex);
                                  }
                              }

                              connection.Close();
                          }
                      }
                  }
                  catch (Exception exception)
                  {
                      CurrentDeskLog.Error(exception.Message, exception);
                  }
                #endregion

                #endregion

                #region "Get All Users from MetaTrader Server and Save to UserRecord table"


                //UserRequest to MT4
                int totalUser = 0;
                UserRecordNET[] userRecordInfo;
                userRecordInfo = manager.UsersRequest(ref totalUser);

                //New task to save UserRecord in BO
                taskUsers = new Task(() =>
                {
                    try
                    {
                        var lstUser = (from u in userRecordInfo
                                       select new UserRecord
                                       {
                                           Login = u.login,
                                           Group = u.@group,
                                           Address = u.address,
                                           AgentAccount = u.agent_account,
                                           ApiData = u.api_data,
                                           Balance = u.balance,
                                           BalanceStatus = u.balance_status,
                                           City = u.city,
                                           Comment = u.comment,
                                           Country = u.country,
                                           Credit = u.credit,
                                           Email = u.email,

                                           Enable = u.enable,
                                           EnableChangePassword = u.enable_change_password,
                                           EnableReadOnly = u.enable_read_only,

                                           ID = u.id,
                                           InterestRate = u.interestrate,
                                           LastDate = u.lastdate.TimeStampToDateTime(),

                                           Leverage = u.leverage,
                                           Name = u.password,
                                           PasswordInvestor = u.password_investor,
                                           PasswordPhone = u.password_phone,

                                           Phone = u.phone,
                                           PrevBalance = u.prevbalance,

                                           PrevEquity = u.prevequity,
                                           PublicKey = u.publickey,

                                           RegDate = u.regdate.TimeStampToDateTime(),
                                           Reserved = u.reserved.IntArrayToString(),
                                           Reserved2 = u.reserved2.DoubleArrayToString(),

                                           SendReports = u.send_reports,
                                           State = u.state,
                                           Status = u.status,
                                           Taxes = u.taxes,
                                           Timestamp = u.timestamp.TimeStampToDateTime(),
                                           Unused = u.unused,
                                           UserColor = u.user_color,
                                           ZipCode = u.zipcode

                                       }).ToList();

                        //UserRecordBO userRecordBO = new UserRecordBO();
                        //userRecordBO.SaveUserRecord(lstUser);
                    }
                    catch (Exception ex)
                    {
                        CurrentDeskLog.Error(ex.Message, ex);
                    }

                });


                #endregion

                //Save tables asynchronously
                bool isSynchedSucessfully = false;
                try
                {
                    //allTasks.Add(taskOpenTrades);
                    ////allTasks.Add(taskUsers);

                    //taskOpenTrades.Start();
                    ////taskUsers.Start();

                    //Task.WaitAll(allTasks.ToArray());

                    //isSynchedSucessfully = true;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message + "\n" + ex.ToString(), "MetaTrader SynchBackOfficeTables");
                    CurrentDeskLog.Error(ex.Message, ex);
                }

                if (isSynchedSucessfully)
                {
                    //Synch Users
                    //UserRecordBO ubo = new UserRecordBO();
                    //ubo.AddClientRecord();

                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + "\n" + ex.ToString(), "MetaTrader SynchBackOfficeTables");
                CurrentDeskLog.Info("Cache object created");
            }
            //Synch equity
            //SynchClientEquity();
        }

        /// <summary>
        /// This method runs new thread to update
        /// last trading date once a day
        /// </summary>
        private void UpdateLastTradingDateThread()
        {
            Thread t = new Thread(new ThreadStart(UpdateTradingDate));
            t.Start();
        }

        /// <summary>
        /// This method update last trading date 
        /// </summary>
        private void UpdateTradingDate()
        {
            var lastDateTime = DateTime.UtcNow;
            while (true)
            {

                MetaTraderTrades metaTrader = new MetaTraderTrades();
                var allLoginTradedToday = metaTrader.GetAllLoginsTradedToday(lastDateTime);
                lastDateTime = DateTime.UtcNow;

                metaTrader.UpdateClientTradeDate(allLoginTradedToday);

                Thread.Sleep(60 * 60 * 24 * 1000);
            }
        }

        /// <summary>
        /// Thread for synch Equity
        /// </summary>
        public void SynchClientEquity()
        {
            Thread t = new Thread(new ThreadStart(UpdateClientEquity));
            t.Start();
        }

        /// <summary>
        /// Sunch Equity and Balance
        /// </summary>
        public void UpdateClientEquity()
        {
            while (true)
            {
                try
                {
                    Client_AccountBO cbo = new Client_AccountBO();
                    cbo.SynchClientAccount();
                }
                catch (Exception ex)
                {
                    CurrentDeskLog.Error(ex.Message, ex);
                }
            }
        }

        
        /// <summary>
        /// Get last close traded time and get the trades between closetime and current time
        /// </summary>
        public void SynchHistory()
        {

            TradesHistoryBO tradesHistory = new TradesHistoryBO();
            string connectionString = tradesHistory.GetConnectionString();

            //Start Time
            Int64 from = 0;
            Int64 to = 0;
            TimeZone tZone = TimeZone.CurrentTimeZone;
            DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            //Connect to meta trader server
            MT4ManLibraryNETv03.CMTManager mgr = new MT4ManLibraryNETv03.CMTManager();


            while (true)
            {
                try
                {

                    //Get Symbole Margin mode
                    //SymbolBO sbo = new SymbolBO();

                    // var lstSymbolMarginMode = sbo.GetSymolMarginMode();                   
                    //if (lstSymbMarginMode.Count == 0)
                    //{
                    //    continue;
                    //}

                    TradesHistoryBO thbo = new TradesHistoryBO();
                    long? closeTime = 0; // thbo.GetMaxCloseTime();
                    if (closeTime == -1)
                    {
                        continue;
                    }

                    if (closeTime > 0)
                    {
                        TimeSpan fromSpan = baseTime - startTime + tZone.GetUtcOffset(baseTime);
                        DateTime endTime = DateTime.Now;
                        TimeSpan toSpan = baseTime - endTime + tZone.GetUtcOffset(baseTime);
                        from = Convert.ToInt64(closeTime) + 1;
                        to = Convert.ToInt64(Math.Abs(toSpan.TotalSeconds));

                    }
                    else
                    {
                        TimeSpan fromSpan = baseTime - startTime + tZone.GetUtcOffset(baseTime);
                        DateTime endTime = DateTime.Now;
                        TimeSpan toSpan = baseTime - endTime + tZone.GetUtcOffset(baseTime);
                        from = Convert.ToInt64(Math.Abs(fromSpan.TotalSeconds));
                        to = Convert.ToInt64(Math.Abs(toSpan.TotalSeconds));

                    }

                    DataTable dt = new DataTable("TradesHistory");
                    #region "Create DataTable for TradesHistory"

                    dt.Columns.Add("PK_TradeID", typeof(int));
                    dt.Columns.Add("Activation", typeof(int));
                    dt.Columns.Add("ClosePrice", typeof(float));
                    dt.Columns.Add("CloseTime", typeof(Int64));
                    dt.Columns.Add("Cmd", typeof(int));
                    dt.Columns.Add("Comment", typeof(string));
                    dt.Columns.Add("Commission", typeof(float));
                    dt.Columns.Add("CommissionAgent", typeof(float));
                    dt.Columns.Add("ConvRates", typeof(float));
                    dt.Columns.Add("ConvReserv", typeof(int));
                    dt.Columns.Add("Digits", typeof(int));
                    dt.Columns.Add("Expiration", typeof(Int64));
                    dt.Columns.Add("InternalID", typeof(int));
                    dt.Columns.Add("Login", typeof(int));
                    dt.Columns.Add("Magic", typeof(int));
                    dt.Columns.Add("MarginRate", typeof(float));
                    dt.Columns.Add("OpenPrice", typeof(float));
                    dt.Columns.Add("OpenTime", typeof(Int64));
                    dt.Columns.Add("OrderID", typeof(int));
                    dt.Columns.Add("Profit", typeof(float));
                    dt.Columns.Add("Sl", typeof(float));
                    dt.Columns.Add("Spread", typeof(int));
                    dt.Columns.Add("State", typeof(int));
                    dt.Columns.Add("Storage", typeof(float));
                    dt.Columns.Add("Symbol", typeof(string));
                    dt.Columns.Add("Taxes", typeof(float));
                    dt.Columns.Add("Timestamp", typeof(Int64));
                    dt.Columns.Add("Tp", typeof(float));
                    dt.Columns.Add("ValueDate", typeof(Int64));
                    dt.Columns.Add("Volume", typeof(Int32));
                    dt.Columns.Add("ActualVol", typeof(float));
                    dt.Columns.Add("MarginMode", typeof(int));

                    #endregion

                   
                    ConnectToManager(mgr,ServerName,ManagerId,ManagerPassword);

                    int totalUser = 0;
                    UserRecordNET[] userRecordInfo;
                    userRecordInfo = mgr.UsersRequest(ref totalUser);

                    if (userRecordInfo != null && totalUser > 0)
                    {
                        //Get List of User LoginID
                        var lstLoginId = userRecordInfo.Select(s => s.login).ToList();

                        #region BulkCopy

                        try
                        {

                            //CurrentDeskLog.Info(" --- Start Time For MT4 API : " + DateTime.Now + " --- ");
                            List<TradeRecordNET> lstTradeRecord = new List<TradeRecordNET>();
                            //Get History records for all logins and put in DataReader for SqlBulkCopy
                            foreach (var lid in lstLoginId)
                            {
                                int value = 0;
                                TradeRecordNET[] TradeInfo = null;
                                TradeInfo = mgr.TradesUserHistory(lid, from, to, ref value);


                                if (TradeInfo != null && value > 0)
                                {
                                    foreach (var item in TradeInfo)
                                    {

                                        lstTradeRecord.Add(item);
                                        #region "Actual Volume"
                                        var actualVol = 0F;
                                        Match match = Regex.Match(item.comment, @"Vol:\d+(\.\d+)?");
                                        if (match.Success)
                                        {
                                            Match match1 = Regex.Match(match.Value, @"\d+(\.\d+)?");
                                            if (match1.Success)
                                            {

                                                actualVol = match1.Value.FloatTryParse();
                                            }
                                        }
                                        else
                                        {
                                            actualVol = item.volume;
                                        }
                                        #endregion


                                        //Get margin mode
                                        //var symbol = lstSymbMarginMode.Find(i => i.Symbol == item.symbol);
                                        //int? marginMode = symbol == null ? null : symbol.MarginMode;

                                        //dt.Rows.Add(0, item.activation, item.close_price,
                                        //            item.close_time, item.cmd, item.comment,
                                        //            item.commission, item.commission_agent,
                                        //            item.conv_rates[0], item.conv_reserv,
                                        //            item.digits, item.expiration,
                                        //            item.internal_id,
                                        //            item.login, item.magic, item.margin_rate,
                                        //            item.open_price, item.open_time, item.order,
                                        //            item.profit, item.sl, item.spread,
                                        //            item.state,
                                        //            item.storage, item.symbol, item.taxes,
                                        //            item.timestamp, item.tp, item.value_date,
                                        //            item.volume, actualVol, marginMode);
                                    }

                                }
                            }


                           

                           
                            var sr = (from trade in lstTradeRecord
                                      select new CCobject
                                      {
                                          orderid = trade.order,
                                          login = trade.login,
                                          pnl = trade.profit,                                          
                                          cms = trade.commission,
                                          storage = trade.storage
                                      }).Take(5000).ToList();

                            var dt1 = DateTime.Now;
                            //foreach (var r in sr)
                            //{
                               
                            //}

                            Parallel.ForEach(sr, r => {

                                tradesCache.Put(r.login.ToString(), r);

                            });

                            var dt2 = DateTime.Now;
                            var df = dt2 - dt1;
                            CurrentDeskLog.Info("DF :" + df);

                            var dt3 = DateTime.Now;

                            marginCache.Put("tc", sr);

                        
                            var dt4 = DateTime.Now;
                            var df1 = dt4 - dt3;
                            CurrentDeskLog.Info("DF1 :" + df1);



                            //CurrentDeskLog.Info(" --- End  Time For MT4 API : " + DateTime.Now + " --- ");

                            //Calculate total page for pagesie=3000
                            int pageSize = 3000;
                            int totalpage = dt.Rows.Count / pageSize;
                            int mod = dt.Rows.Count % pageSize;
                            if (mod > 0)
                            {
                                totalpage = totalpage + 1;
                            }

                            if (dt.Rows.Count > 900000)
                            {
                                CurrentDeskLog.Info("HISTORY INFO: BuckCopy Starts at :" + DateTime.Now + " : Total Row : " + dt.Rows.Count);

                                #region "Create Table Page"
                                //Create DataPages for Parallel processing
                                List<DataTable> lstDataPage = new List<DataTable>();
                                for (int pageNo = 1; pageNo <= totalpage; pageNo++)
                                {
                                    DataTable dtPage =
                                        dt.Rows.Cast<System.Data.DataRow>().Skip((pageNo - 1) * pageSize).Take(pageSize).
                                            CopyToDataTable();
                                    lstDataPage.Add(dtPage);

                                }
                                #endregion

                                #region "Buck Copy"
                                Parallel.ForEach(lstDataPage, dtp =>
                                {
                                    try
                                    {
                                        using (SqlConnection connection = new SqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                                            {
                                                //bulkCopy.BatchSize = BATCHSIZE;
                                                //bulkCopy.DestinationTableName =
                                                //    "dbo.TradesHistory";

                                                //try
                                                //{
                                                //    bulkCopy.WriteToServer(dtp);
                                                //}
                                                //catch (Exception ex)
                                                //{
                                                //    CurrentDeskLog.Error(ex.Message, ex);
                                                //}
                                            }

                                            connection.Close();
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        CurrentDeskLog.Error(exception.Message, exception);
                                    }
                                });


                                #endregion

                                CurrentDeskLog.Info("HISTORY INFO: BuckCopy Ends at :" + DateTime.Now);
                            }
                        }
                        catch (Exception exception)
                        {
                            CurrentDeskLog.Error(exception.Message, exception);
                        }

                        #endregion

                    }
                }
                catch (Exception ex)
                {
                    CurrentDeskLog.Error(ex.Message, ex);
                }
            }
        }

        #endregion

        #region "Mt4 Functions"

        /// <summary>
        /// Get MT4 server config
        /// </summary>
        public void GetMT4Crdential(bool isLogged = false)
        {
            try
            {
                ServerName = CloudConfigurationManager.GetSetting("ServerName");
                ManagerId = Convert.ToInt32(CloudConfigurationManager.GetSetting("ManagerId"));
                ManagerPassword = CloudConfigurationManager.GetSetting("Password");

                if (isLogged)
                {
                    CurrentDeskLog.Info("GetMT4Crdential() Servername :- " + ServerName + ", ManagerId :- " + ManagerId + ", Password:-" + ManagerPassword);
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + "\n" + ex.ToString(), "MetaTrader GetMT4Crdential()");
                CurrentDeskLog.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Restart pumping
        /// </summary>
        public void ReStartPumping()
        {
            while (manager.IsConnected() == 0)
            {
                try
                {
                    GetMT4Crdential(true);
                    int status = manager.ManagerFactory();
                    status = manager.Connect("mtdem01.primexm.com:443");
                    status = manager.Login(900, "!FQS123!!");
                    if (status == 0)
                    {
                        CurrentDeskLog.Info("ReConnected for Pumping");

                        //Restart pummping
                        int flags = ((int)PumpingFlags.CLIENT_FLAGS_HIDEMAIL) | ((int)PumpingFlags.CLIENT_FLAGS_HIDENEWS) | ((int)PumpingFlags.CLIENT_FLAGS_HIDEONLINE);

                        PumpFuncExDelegate pumpEx = new PumpFuncExDelegate(CallBackMethodEx);

                        var iPumpRet = manager.PassMeAnExDelegate(pumpEx);
                        iPumpRet = manager.PumpingSwitchEx(flags, null);
                    }
                    else
                    {
                        var ErrorMessage = manager.ErrorDescription(status);
                        CurrentDeskLog.Info("Connection Failed For Pumping :" + ErrorMessage);

                        System.Threading.Thread.Sleep(5000);
                    }
                }
                catch (Exception ex)
                {
                    CurrentDeskLog.Error(ex.Message, ex);
                }

            }
        }

        public void ReStartPumpingA1()
        {
            while (managerA1.IsConnected() == 0)
            {
                try
                {

                    int status = managerA1.ManagerFactory();
                    status = managerA1.Connect(ServerName);
                    status = managerA1.Login(ManagerId, ManagerPassword);
                    if (status == 0)
                    {
                        CurrentDeskLog.Info("ReConnected for Pumping A1");

                        //Restart pummping
                        int flags = ((int)PumpingFlags.CLIENT_FLAGS_HIDEMAIL) | ((int)PumpingFlags.CLIENT_FLAGS_HIDENEWS) | ((int)PumpingFlags.CLIENT_FLAGS_HIDEONLINE);

                        PumpFuncExDelegate pumpEx = new PumpFuncExDelegate(CallBackMethodEx1);

                        var iPumpRet = managerA1.PassMeAnExDelegate(pumpEx);
                        iPumpRet = managerA1.PumpingSwitchEx(flags, null);
                    }
                    else
                    {
                        var ErrorMessage = managerA1.ErrorDescription(status);
                        CurrentDeskLog.Info("Connection Failed For Pumping A1:" + ErrorMessage);

                        System.Threading.Thread.Sleep(5000);
                    }
                }
                catch (Exception ex)
                {
                    CurrentDeskLog.Error(ex.Message, ex);
                }

            }
        }

        public void ReStartPumpingA2()
        {
            while (managerA2.IsConnected() == 0)
            {
                try
                {

                    int status = managerA2.ManagerFactory();
                    status = managerA2.Connect(ServerName);
                    status = managerA2.Login(ManagerId, ManagerPassword);
                    if (status == 0)
                    {
                        CurrentDeskLog.Info("ReConnected for Pumping A1");

                        //Restart pummping
                        int flags = ((int)PumpingFlags.CLIENT_FLAGS_HIDEMAIL) | ((int)PumpingFlags.CLIENT_FLAGS_HIDENEWS) | ((int)PumpingFlags.CLIENT_FLAGS_HIDEONLINE);

                        PumpFuncExDelegate pumpEx = new PumpFuncExDelegate(CallBackMethodEx2);

                        var iPumpRet = managerA2.PassMeAnExDelegate(pumpEx);
                        iPumpRet = managerA2.PumpingSwitchEx(flags, null);
                    }
                    else
                    {
                        var ErrorMessage = managerA2.ErrorDescription(status);
                        CurrentDeskLog.Info("Connection Failed For Pumping A1:" + ErrorMessage);

                        System.Threading.Thread.Sleep(5000);
                    }
                }
                catch (Exception ex)
                {
                    CurrentDeskLog.Error(ex.Message, ex);
                }

            }
        }

        public void ReStartPumpingA3()
        {
            while (managerA3.IsConnected() == 0)
            {
                try
                {

                    int status = managerA3.ManagerFactory();
                    status = managerA3.Connect(ServerNameL);
                    status = managerA3.Login(ManagerIdL, ManagerPasswordL);
                    if (status == 0)
                    {
                        CurrentDeskLog.Info("ReConnected for Pumping A1");

                        //Restart pummping
                        int flags = ((int)PumpingFlags.CLIENT_FLAGS_HIDEMAIL) | ((int)PumpingFlags.CLIENT_FLAGS_HIDENEWS) | ((int)PumpingFlags.CLIENT_FLAGS_HIDEONLINE);

                        PumpFuncExDelegate pumpEx = new PumpFuncExDelegate(CallBackMethodEx3);

                        var iPumpRet = managerA3.PassMeAnExDelegate(pumpEx);
                        iPumpRet = managerA3.PumpingSwitchEx(flags, null);
                    }
                    else
                    {
                        var ErrorMessage = managerA3.ErrorDescription(status);
                        CurrentDeskLog.Info("Connection Failed For Pumping A1:" + ErrorMessage);

                        System.Threading.Thread.Sleep(5000);
                    }
                }
                catch (Exception ex)
                {
                    CurrentDeskLog.Error(ex.Message, ex);
                }

            }
        }

        /// <summary>
        /// Disconnect connection when connection closes
        /// </summary>
        public void DisconnectPumping()
        {
            int status = manager.Disconnect();
            CurrentDeskLog.Info("Disconnected Pumping Connection ..... " + status);
        }

        public void DisconnectPumping1()
        {
            int status = managerA1.Disconnect();
            CurrentDeskLog.Info("Disconnected Pumping ConnectionA1 ..... " + status);
        }

        public void DisconnectPumping2()
        {
            int status = managerA2.Disconnect();
            CurrentDeskLog.Info("Disconnected Pumping ConnectionA2 ..... " + status);
        }
        
        public void DisconnectPumping3()
        {
            int status = managerA3.Disconnect();
            CurrentDeskLog.Info("Disconnected Pumping ConnectionA3 ..... " + status);
        }

        /// <summary>
        /// Contect to MT4
        /// </summary>
        /// <param name="mi"></param>
        public bool ConnectToManager(MT4ManLibraryNETv03.CMTManager mi,string servername,int id, string pwd)
        {
            var iSconnect = false;

            while (!iSconnect)
            {
                try
                {
                    GetMT4Crdential(false);
                    int iRet = mi.ManagerFactory();
                    iRet = mi.Connect(servername);

                    var errDesc = mi.ErrorDescription(iRet);
                    Trace.WriteLine("Connect Message: {0}", errDesc);

                    iRet = mi.Login(id, pwd);
                    errDesc = mi.ErrorDescription(iRet);
                    Trace.WriteLine("Login Message: {0}", errDesc);
                    if (iRet == 0)
                    {
                        iSconnect = true;
                    }
                    else
                    {
                        iSconnect = false;
                        Thread.Sleep(10000);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message + "\n" + ex.ToString(), "MetaTrader ConnectToManager()");
                    CurrentDeskLog.Error(ex.Message, ex);
                }
            }

            return iSconnect;
        }

        /// <summary>
        /// Extended pumping
        /// </summary>
        /// <param name="code"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="param"></param>
        protected void CallBackMethodEx(int code, int type, Object data, Object param)
        {
            switch (code)
            {
                case PUMP_START_PUMPING:
                    break;

                case PUMP_UPDATE_GROUPS:
                    CurrentDeskLog.Info("PUMP_UPDATE_GROUPS");
                    break;

                case PUMP_UPDATE_USERS:
                    break;

                case PUMP_UPDATE_ONLINE:
                    break;

                case PUMP_UPDATE_SYMBOLS:

                    #region "Synch Symbol"

                    try
                    {
                        int totalSymbl = 0;
                        ConSymbolNET[] symb;
                        symb = manager.SymbolsGetAll(ref totalSymbl);

                        if (taskSymbols == null || taskSymbols.IsCompleted)
                        {
                            taskSymbols = new Task(() =>
                            {
                                //Get symbols for a broker
                                var lstSpecificSymbol = symb.Where(s => s.symbol.EndsWith(BrokerSymbol)).ToList();
                                SynchSymbols(lstSpecificSymbol);
                            });

                            taskSymbols.Start();
                        }
                    }
                    catch (Exception ex)
                    {

                        CurrentDeskLog.Error(ex.Message,ex);
                    }
                    
                    #endregion
                    CurrentDeskLog.Info("PUMP_UPDATE_SYMBOLS");

                    break;

                case PUMP_UPDATE_MARGINCALL:
                    CurrentDeskLog.Info("PUMP_UPDATE_MARGINCALL");
                    break;

                case PUMP_STOP_PUMPING:

                    #region Restart pumping
                    CurrentDeskLog.Info("PUMP_STOP_PUMPING");
                    //Restart Pumping
                    try
                    {
                        Thread tRestart = new Thread(ReStartPumping);
                        tRestart.Start();
                    }
                    catch (Exception ex)
                    {

                        CurrentDeskLog.Error(ex.Message, ex);
                    }
                    #endregion

                    break;

                case PUMP_UPDATE_TRADES: // 9
                    #region "Update Trades"
                    try
                    {
                        if (data != null)
                        {
                            TradeRecordNET tinfo = (TradeRecordNET)data;
                            
                            #region "Synch Trade"

                            Trade trade = GetTrade(tinfo);
                            switch (type)
                            {
                                case (int)Transaction.TRANS_ADD:
                                    {

                                        if (lstSymbol.IndexOf(tinfo.symbol) == -1)
                                        {
                                            lstSymbol.Add(tinfo.symbol);
                                        }

                                        new Task(() =>
                                        {

                                            TradeBO tradeBO = new TradeBO();
                                            tradeBO.AddTrade(trade);

                                        }).Start();
                                    }
                                    break;

                                case (int)Transaction.TRANS_DELETE:
                                    {
                                        new Task(() =>
                                        {

                                            TradeBO tradeBO = new TradeBO();
                                            tradeBO.DeleteTrade(trade);

                                        }).Start();
                                    }
                                    break;

                                case (int)Transaction.TRANS_UPDATE:
                                    {
                                        new Task(() =>
                                        {

                                            TradeBO tradeBO = new TradeBO();
                                            tradeBO.UpdateTrade(trade);

                                        }).Start();
                                    }
                                    break;
                            }

                            #endregion
                            
                        }

                    }
                    catch (Exception ex)
                    {
                        CurrentDeskLog.Error(ex.Message, ex);
                    }
                        #endregion

                    break;

                case PUMP_UPDATE_BIDASK:

                    #region "Price Synch"
                    /*
                    try
                    {

                        
                        

                        //Price Pumping
                        if (lstSymbol.Count > 0)
                        {
                            if (taskPrice == null || taskPrice.IsCompleted)
                            {
                                taskPrice = new Task(() =>
                                {
                                    List<Price> lstPrice = new List<CurrentDesk.Models.Price>();

                                    Parallel.ForEach(lstSymbol, symbole =>
                                    {

                                        MT4ManLibraryNETv03.TickInfoNET[] TickInfoArray;
                                        int totalTicks = 0;
                                        TickInfoArray = manager.TickInfoLast(symbole, ref totalTicks);

                                        if (totalTicks > 0)
                                        {
                                            Price p = new Price();
                                            p.Ask = Convert.ToDecimal(TickInfoArray[0].ask);
                                            p.Bid = Convert.ToDecimal(TickInfoArray[0].bid);
                                            p.Symbole = TickInfoArray[0].symbol ?? string.Empty;

                                            DateTime dt = new DateTime(TickInfoArray[0].ctm);
                                            p.Time = TickInfoArray[0].ctm.TimeStampToDateTime();

                                            lstPrice.Add(p);
                                        }

                                    });

                                    if (lstPrice.Count > 0)
                                    {
                                        MetaTraderTrades metaTrader = new MetaTraderTrades();
                                        metaTrader.UpdatePrice(lstPrice);
                                    }
                                }, TaskCreationOptions.LongRunning);

                                taskPrice.Start();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    }
                    
                    */
                    #endregion

                    #region "Realtime Margin Synch"


                    try
                    {
                        var marginTotal1 = 0;
                        var mrgnLevelNET1 = manager.MarginsGet(ref marginTotal1);
                        List<MarginLevelNET> lstNewMargins = new List<MarginLevelNET>();


                        if (mrgnLevelNET1 != null)
                        {
                            if (arrMarginLevel == null)
                            {
                                arrMarginLevel = mrgnLevelNET1;
                            }
                            else
                            {
                                lstNewMargins = (from nmargin in mrgnLevelNET1
                                                 join omargin in arrMarginLevel
                                                     on nmargin.login equals omargin.login
                                                 where nmargin.equity != omargin.equity
                                                 select nmargin).ToList();


                            }

                            if (taskRealtimeMargin == null || taskRealtimeMargin.IsCompleted)
                            {
                                arrMarginLevel = mrgnLevelNET1;

                                if (lstTempMargins.Count > 0)
                                {
                                    var skipedMargins = (from lt in lstTempMargins
                                                         where !lstNewMargins.Select(s => s.login).Contains(lt.login)
                                                         select lt).ToList();

                                    lstNewMargins.AddRange(skipedMargins);
                                    lstTempMargins = new List<MarginLevelNET>();
                                }

                                taskRealtimeMargin = new Task(() =>
                                {

                                    if (lstNewMargins.Count > 0)
                                    {
                                        List<Margin> lstmargin = (from margin in lstNewMargins
                                                                  select new Margin
                                                                  {
                                                                      Group = margin.@group,
                                                                      Login = margin.login,
                                                                      Leverage = margin.leverage,
                                                                      Updated = margin.updated,
                                                                      Balance = Convert.ToDecimal(margin.balance),
                                                                      Equity = Convert.ToDecimal(margin.equity),
                                                                      Volume = margin.volume,
                                                                      Margin1 = Convert.ToDecimal(margin.margin),
                                                                      MarginFree = Convert.ToDecimal(margin.margin_free),
                                                                      MarginLevel = Convert.ToDecimal(margin.margin_level),
                                                                      MarginType = margin.margin_type,
                                                                      LevelType = margin.level_type

                                                                  }).ToList();


                                        try
                                        {
                                            foreach (var m in lstmargin)
                                            {
                                                marginCache.Put(m.Login.ToString(), m);
                                            }
                                        }
                                        catch (DataCacheException dataCacheEx)
                                        {

                                            CurrentDeskLog.Error(dataCacheEx.Message, dataCacheEx);
                                            #region "FailOver Caching"
                                           
                                            try
                                            {
                                                
                                                DataTable dt = new DataTable("Margins");                                                

                                                dt.Columns.Add("LoginId", typeof(int));
                                                dt.Columns.Add("Balance", typeof(decimal));
                                                dt.Columns.Add("Equity", typeof(decimal));
                                                dt.Columns.Add("Margin", typeof(decimal));

                                                foreach (var margin in lstmargin)
                                                {
                                                    dt.Rows.Add(margin.Login,margin.Balance,margin.Equity,margin.Margin1);
                                                }

                                                TradesHistoryBO tradesHistory = new TradesHistoryBO();
                                                string connectionString = tradesHistory.GetConnectionString();
                                                    using (SqlConnection connection = new SqlConnection(connectionString))
                                                    {
                                                        connection.Open();
                                                      SqlCommand sqlCmd = new SqlCommand("dbo.UpdateMargin", connection);
                                                      sqlCmd.CommandType = CommandType.StoredProcedure;
                                                      SqlParameter sqlParam = sqlCmd.Parameters.Add("@marginType", dt);
                                                      sqlParam.SqlDbType = SqlDbType.Structured;
                                                      int rn =  sqlCmd.ExecuteNonQuery();
                                                    }
                                               


                                            }
                                            catch (Exception ex)
                                            {
                                                CurrentDeskLog.Error(ex.Message, ex);
                                            }
                                           #endregion
                                        }

                                    }

                                });

                                taskRealtimeMargin.Start();
                            }
                            else
                            {
                                lstTempMargins.AddRange(lstNewMargins);
                            }
                        }
                        else
                        {
                            CurrentDeskLog.Info("MarginsGet returns NULL");

                        }
                    }
                    catch (Exception ex)
                    {
                        CurrentDeskLog.Error(ex.Message, ex);
                    }


                    #endregion

                    #region "Price Synch with Cache"


                    try
                    {
                        //Get All Symbols only and get the price from MT4 and save to "PriceCache"
                        var lstSymbols = lstSymbMarginMode.Select(s => s.Symbol).ToList();
                        if (taskPrice == null || taskPrice.IsCompleted)
                        {
                            taskPrice = new Task(() =>
                            {
                                foreach (var sym in lstSymbols)
                                {
                                    MT4ManLibraryNETv03.TickInfoNET[] TickInfoArray;
                                    int totalTicks = 0;
                                    TickInfoArray = manager.TickInfoLast(sym, ref totalTicks);
                                    try
                                    {
                                        if (TickInfoArray != null && totalTicks > 0)
                                        {
                                            PriceInfo pinfo = new PriceInfo();
                                            pinfo.Bid = TickInfoArray[0].bid;
                                            pinfo.Ask = TickInfoArray[0].ask;
                                            priceCache.Put(sym, pinfo);

                                        }
                                    }
                                    catch (DataCacheException exDataCache)
                                    {
                                        CurrentDeskLog.Error(exDataCache.Message, exDataCache);
                                    }
                                }
                                
                            });

                            taskPrice.Start();
                        }


                    }
                    catch (Exception ex)
                    {

                        CurrentDeskLog.Error(ex.Message, ex);
                    }
                    #endregion

                    break;

                case PUMP_PING:
                    var pingstatus = manager.Ping();
                    CurrentDeskLog.Info("PUMP_PING :" + pingstatus);
                    break;
                case TEST_LOCAL:
                    break;
                default:
                    code = 911;
                    break;
            }
        }

        protected void CallBackMethodEx1(int code, int type, Object data, Object param)
        {
            switch (code)
            {
                case PUMP_START_PUMPING:
                    break;

                case PUMP_UPDATE_GROUPS:
                    CurrentDeskLog.Info("PUMP_UPDATE_GROUPS A1");
                    break;

                case PUMP_UPDATE_USERS:
                    break;

                case PUMP_UPDATE_ONLINE:
                    break;

                case PUMP_UPDATE_SYMBOLS:

                    #region "Synch Symbol"

                    try
                    {
                        int totalSymbl = 0;
                        ConSymbolNET[] symb;
                        symb = managerA1.SymbolsGetAll(ref totalSymbl);

                        //if (taskSymbols == null || taskSymbols.IsCompleted)
                        //{
                        //    taskSymbols = new Task(() =>
                        //    {
                        //        //Get symbols for a broker
                        //        var lstSpecificSymbol = symb.Where(s => s.symbol.EndsWith(BrokerSymbol)).ToList();
                        //        SynchSymbols(lstSpecificSymbol);
                        //    });

                        //    taskSymbols.Start();
                        //}
                    }
                    catch (Exception ex)
                    {

                        CurrentDeskLog.Error(ex.Message, ex);
                    }

                    #endregion
                    CurrentDeskLog.Info("PUMP_UPDATE_SYMBOLS A1");

                    break;

                case PUMP_UPDATE_MARGINCALL:
                    CurrentDeskLog.Info("PUMP_UPDATE_MARGINCALL A1");
                    break;

                case PUMP_STOP_PUMPING:

                    #region Restart pumping
                    CurrentDeskLog.Info("PUMP_STOP_PUMPING A1");
                    //Restart Pumping
                    try
                    {
                        Thread tRestart = new Thread(ReStartPumpingA1);
                        tRestart.Start();
                    }
                    catch (Exception ex)
                    {

                        CurrentDeskLog.Error(ex.Message, ex);
                    }
                    #endregion

                    break;

                case PUMP_UPDATE_TRADES: // 9
                    #region "Update Trades"
                    try
                    {
                        if (data != null)
                        {
                            TradeRecordNET tinfo = (TradeRecordNET)data;

                            #region "Synch Trade"

                            Trade trade = GetTrade(tinfo);
                            switch (type)
                            {
                                case (int)Transaction.TRANS_ADD:
                                    {

                                        if (lstSymbol.IndexOf(tinfo.symbol) == -1)
                                        {
                                            lstSymbol.Add(tinfo.symbol);
                                        }

                                        new Task(() =>
                                        {

                                            TradeBO tradeBO = new TradeBO();
                                            tradeBO.AddTrade(trade);

                                        }).Start();
                                    }
                                    break;

                                case (int)Transaction.TRANS_DELETE:
                                    {
                                        new Task(() =>
                                        {

                                            TradeBO tradeBO = new TradeBO();
                                            tradeBO.DeleteTrade(trade);

                                        }).Start();
                                    }
                                    break;

                                case (int)Transaction.TRANS_UPDATE:
                                    {
                                        new Task(() =>
                                        {

                                            TradeBO tradeBO = new TradeBO();
                                            tradeBO.UpdateTrade(trade);

                                        }).Start();
                                    }
                                    break;
                            }

                            #endregion

                        }

                    }
                    catch (Exception ex)
                    {
                        CurrentDeskLog.Error(ex.Message, ex);
                    }
                    #endregion

                    break;

                case PUMP_UPDATE_BIDASK:

                    #region "Price Synch"
                    /*
                    try
                    {

                        
                        

                        //Price Pumping
                        if (lstSymbol.Count > 0)
                        {
                            if (taskPrice == null || taskPrice.IsCompleted)
                            {
                                taskPrice = new Task(() =>
                                {
                                    List<Price> lstPrice = new List<CurrentDesk.Models.Price>();

                                    Parallel.ForEach(lstSymbol, symbole =>
                                    {

                                        MT4ManLibraryNETv03.TickInfoNET[] TickInfoArray;
                                        int totalTicks = 0;
                                        TickInfoArray = manager.TickInfoLast(symbole, ref totalTicks);

                                        if (totalTicks > 0)
                                        {
                                            Price p = new Price();
                                            p.Ask = Convert.ToDecimal(TickInfoArray[0].ask);
                                            p.Bid = Convert.ToDecimal(TickInfoArray[0].bid);
                                            p.Symbole = TickInfoArray[0].symbol ?? string.Empty;

                                            DateTime dt = new DateTime(TickInfoArray[0].ctm);
                                            p.Time = TickInfoArray[0].ctm.TimeStampToDateTime();

                                            lstPrice.Add(p);
                                        }

                                    });

                                    if (lstPrice.Count > 0)
                                    {
                                        MetaTraderTrades metaTrader = new MetaTraderTrades();
                                        metaTrader.UpdatePrice(lstPrice);
                                    }
                                }, TaskCreationOptions.LongRunning);

                                taskPrice.Start();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    }
                    
                    */
                    #endregion

                    #region "Realtime Margin Synch"


                    try
                    {
                        var marginTotal1 = 0;
                        var mrgnLevelNET1 = managerA1.MarginsGet(ref marginTotal1);
                        List<MarginLevelNET> lstNewMargins = new List<MarginLevelNET>();


                        if (mrgnLevelNET1 != null)
                        {
                            if (arrMarginLevel1 == null)
                            {
                                arrMarginLevel1 = mrgnLevelNET1;
                            }
                            else
                            {
                                lstNewMargins = (from nmargin in mrgnLevelNET1
                                                 join omargin in arrMarginLevel1
                                                     on nmargin.login equals omargin.login
                                                 where nmargin.equity != omargin.equity
                                                 select nmargin).ToList();


                            }

                            if (taskRealtimeMargin1 == null || taskRealtimeMargin1.IsCompleted)
                            {
                                arrMarginLevel1 = mrgnLevelNET1;

                                if (lstTempMargins1.Count > 0)
                                {
                                    var skipedMargins = (from lt in lstTempMargins1
                                                         where !lstNewMargins.Select(s => s.login).Contains(lt.login)
                                                         select lt).ToList();

                                    lstNewMargins.AddRange(skipedMargins);
                                    lstTempMargins1 = new List<MarginLevelNET>();
                                }

                                taskRealtimeMargin1 = new Task(() =>
                                {

                                    if (lstNewMargins.Count > 0)
                                    {
                                        List<Margin> lstmargin = (from margin in lstNewMargins
                                                                  select new Margin
                                                                  {
                                                                      Group = margin.@group,
                                                                      Login = margin.login,
                                                                      Leverage = margin.leverage,
                                                                      Updated = margin.updated,
                                                                      Balance = Convert.ToDecimal(margin.balance),
                                                                      Equity = Convert.ToDecimal(margin.equity),
                                                                      Volume = margin.volume,
                                                                      Margin1 = Convert.ToDecimal(margin.margin),
                                                                      MarginFree = Convert.ToDecimal(margin.margin_free),
                                                                      MarginLevel = Convert.ToDecimal(margin.margin_level),
                                                                      MarginType = margin.margin_type,
                                                                      LevelType = margin.level_type

                                                                  }).ToList();


                                        try
                                        {
                                            foreach (var m in lstmargin)
                                            {
                                                marginCache1.Put(m.Login.ToString(), m);
                                            }
                                        }
                                        catch (DataCacheException dataCacheEx)
                                        {

                                            CurrentDeskLog.Error(dataCacheEx.Message, dataCacheEx);
                                            #region "FailOver Caching"
                                            /*
                                            try
                                            {

                                                DataTable dt = new DataTable("Margins");

                                                dt.Columns.Add("LoginId", typeof(int));
                                                dt.Columns.Add("Balance", typeof(decimal));
                                                dt.Columns.Add("Equity", typeof(decimal));
                                                dt.Columns.Add("Margin", typeof(decimal));

                                                foreach (var margin in lstmargin)
                                                {
                                                    dt.Rows.Add(margin.Login, margin.Balance, margin.Equity, margin.Margin1);
                                                }

                                                TradesHistoryBO tradesHistory = new TradesHistoryBO();
                                                string connectionString = tradesHistory.GetConnectionString();
                                                using (SqlConnection connection = new SqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    SqlCommand sqlCmd = new SqlCommand("dbo.UpdateMargin", connection);
                                                    sqlCmd.CommandType = CommandType.StoredProcedure;
                                                    SqlParameter sqlParam = sqlCmd.Parameters.Add("@marginType", dt);
                                                    sqlParam.SqlDbType = SqlDbType.Structured;
                                                    int rn = sqlCmd.ExecuteNonQuery();
                                                }



                                            }
                                            catch (Exception ex)
                                            {
                                                CurrentDeskLog.Error(ex.Message, ex);
                                            }
                                            */
                                            #endregion
                                        }

                                    }

                                });

                                taskRealtimeMargin1.Start();
                            }
                            else
                            {
                                lstTempMargins1.AddRange(lstNewMargins);
                            }
                        }
                        else
                        {
                            CurrentDeskLog.Info("MarginsGet returns NULL");

                        }
                    }
                    catch (Exception ex)
                    {
                        CurrentDeskLog.Error(ex.Message, ex);
                    }


                    #endregion

                    #region "Price Synch with Cache"


                    try
                    {
                        //Get All Symbols only and get the price from MT4 and save to "PriceCache"
                        var lstSymbols = lstSymbMarginMode.Select(s => s.Symbol).ToList();
                        if (taskPrice1 == null || taskPrice1.IsCompleted)
                        {
                            taskPrice1 = new Task(() =>
                            {
                                foreach (var sym in lstSymbols)
                                {
                                    MT4ManLibraryNETv03.TickInfoNET[] TickInfoArray;
                                    int totalTicks = 0;
                                    TickInfoArray = managerA1.TickInfoLast(sym, ref totalTicks);
                                    try
                                    {
                                        if (TickInfoArray != null && totalTicks > 0)
                                        {
                                            PriceInfo pinfo = new PriceInfo();
                                            pinfo.Bid = TickInfoArray[0].bid;
                                            pinfo.Ask = TickInfoArray[0].ask;
                                            priceCache1.Put(sym, pinfo);

                                        }
                                    }
                                    catch (DataCacheException exDataCache)
                                    {
                                        CurrentDeskLog.Error(exDataCache.Message, exDataCache);
                                    }
                                }

                            });

                            taskPrice1.Start();
                        }


                    }
                    catch (Exception ex)
                    {

                        CurrentDeskLog.Error(ex.Message, ex);
                    }
                    #endregion

                    break;

                case PUMP_PING:
                    var pingstatus = managerA1.Ping();
                    CurrentDeskLog.Info("PUMP_PING A1:" + pingstatus);
                    break;
                case TEST_LOCAL:
                    break;
                default:
                    code = 911;
                    break;
            }
        }

        protected void CallBackMethodEx2(int code, int type, Object data, Object param)
        {
            switch (code)
            {
                case PUMP_START_PUMPING:
                    break;

                case PUMP_UPDATE_GROUPS:
                    CurrentDeskLog.Info("PUMP_UPDATE_GROUPS A2");
                    break;

                case PUMP_UPDATE_USERS:
                    break;

                case PUMP_UPDATE_ONLINE:
                    break;

                case PUMP_UPDATE_SYMBOLS:

                    #region "Synch Symbol"

                    try
                    {
                        int totalSymbl = 0;
                        ConSymbolNET[] symb;
                        symb = managerA2.SymbolsGetAll(ref totalSymbl);

                        if (taskSymbols == null || taskSymbols.IsCompleted)
                        {
                            //taskSymbols = new Task(() =>
                            //{
                            //    //Get symbols for a broker
                            //    var lstSpecificSymbol = symb.Where(s => s.symbol.EndsWith(BrokerSymbol)).ToList();
                            //    SynchSymbols(lstSpecificSymbol);
                            //});

                            //taskSymbols.Start();
                        }
                    }
                    catch (Exception ex)
                    {

                        CurrentDeskLog.Error(ex.Message, ex);
                    }

                    #endregion
                    CurrentDeskLog.Info("PUMP_UPDATE_SYMBOLS A1");

                    break;

                case PUMP_UPDATE_MARGINCALL:
                    CurrentDeskLog.Info("PUMP_UPDATE_MARGINCALL A2");
                    break;

                case PUMP_STOP_PUMPING:

                    #region Restart pumping
                    CurrentDeskLog.Info("PUMP_STOP_PUMPING A2");
                    //Restart Pumping
                    try
                    {
                        Thread tRestart = new Thread(ReStartPumpingA2);
                        tRestart.Start();
                    }
                    catch (Exception ex)
                    {

                        CurrentDeskLog.Error(ex.Message, ex);
                    }
                    #endregion

                    break;

                case PUMP_UPDATE_TRADES: // 9
                    #region "Update Trades"
                    try
                    {
                        if (data != null)
                        {
                            TradeRecordNET tinfo = (TradeRecordNET)data;

                            #region "Synch Trade"

                            Trade trade = GetTrade(tinfo);
                            switch (type)
                            {
                                case (int)Transaction.TRANS_ADD:
                                    {

                                        if (lstSymbol.IndexOf(tinfo.symbol) == -1)
                                        {
                                            lstSymbol.Add(tinfo.symbol);
                                        }

                                        new Task(() =>
                                        {

                                            TradeBO tradeBO = new TradeBO();
                                            tradeBO.AddTrade(trade);

                                        }).Start();
                                    }
                                    break;

                                case (int)Transaction.TRANS_DELETE:
                                    {
                                        new Task(() =>
                                        {

                                            TradeBO tradeBO = new TradeBO();
                                            tradeBO.DeleteTrade(trade);

                                        }).Start();
                                    }
                                    break;

                                case (int)Transaction.TRANS_UPDATE:
                                    {
                                        new Task(() =>
                                        {

                                            TradeBO tradeBO = new TradeBO();
                                            tradeBO.UpdateTrade(trade);

                                        }).Start();
                                    }
                                    break;
                            }

                            #endregion

                        }

                    }
                    catch (Exception ex)
                    {
                        CurrentDeskLog.Error(ex.Message, ex);
                    }
                    #endregion

                    break;

                case PUMP_UPDATE_BIDASK:

                    #region "Price Synch"
                    /*
                    try
                    {

                        
                        

                        //Price Pumping
                        if (lstSymbol.Count > 0)
                        {
                            if (taskPrice == null || taskPrice.IsCompleted)
                            {
                                taskPrice = new Task(() =>
                                {
                                    List<Price> lstPrice = new List<CurrentDesk.Models.Price>();

                                    Parallel.ForEach(lstSymbol, symbole =>
                                    {

                                        MT4ManLibraryNETv03.TickInfoNET[] TickInfoArray;
                                        int totalTicks = 0;
                                        TickInfoArray = manager.TickInfoLast(symbole, ref totalTicks);

                                        if (totalTicks > 0)
                                        {
                                            Price p = new Price();
                                            p.Ask = Convert.ToDecimal(TickInfoArray[0].ask);
                                            p.Bid = Convert.ToDecimal(TickInfoArray[0].bid);
                                            p.Symbole = TickInfoArray[0].symbol ?? string.Empty;

                                            DateTime dt = new DateTime(TickInfoArray[0].ctm);
                                            p.Time = TickInfoArray[0].ctm.TimeStampToDateTime();

                                            lstPrice.Add(p);
                                        }

                                    });

                                    if (lstPrice.Count > 0)
                                    {
                                        MetaTraderTrades metaTrader = new MetaTraderTrades();
                                        metaTrader.UpdatePrice(lstPrice);
                                    }
                                }, TaskCreationOptions.LongRunning);

                                taskPrice.Start();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    }
                    
                    */
                    #endregion

                    #region "Realtime Margin Synch"


                    try
                    {
                        var marginTotal1 = 0;
                        var mrgnLevelNET1 = managerA2.MarginsGet(ref marginTotal1);
                        List<MarginLevelNET> lstNewMargins = new List<MarginLevelNET>();


                        if (mrgnLevelNET1 != null)
                        {
                            if (arrMarginLevel2 == null)
                            {
                                arrMarginLevel2 = mrgnLevelNET1;
                            }
                            else
                            {
                                lstNewMargins = (from nmargin in mrgnLevelNET1
                                                 join omargin in arrMarginLevel2
                                                     on nmargin.login equals omargin.login
                                                 where nmargin.equity != omargin.equity
                                                 select nmargin).ToList();


                            }

                            if (taskRealtimeMargin2 == null || taskRealtimeMargin2.IsCompleted)
                            {
                                arrMarginLevel2 = mrgnLevelNET1;

                                if (lstTempMargins2.Count > 0)
                                {
                                    var skipedMargins = (from lt in lstTempMargins2
                                                         where !lstNewMargins.Select(s => s.login).Contains(lt.login)
                                                         select lt).ToList();

                                    lstNewMargins.AddRange(skipedMargins);
                                    lstTempMargins2 = new List<MarginLevelNET>();
                                }

                                taskRealtimeMargin2 = new Task(() =>
                                {

                                    if (lstNewMargins.Count > 0)
                                    {
                                        List<Margin> lstmargin = (from margin in lstNewMargins
                                                                  select new Margin
                                                                  {
                                                                      Group = margin.@group,
                                                                      Login = margin.login,
                                                                      Leverage = margin.leverage,
                                                                      Updated = margin.updated,
                                                                      Balance = Convert.ToDecimal(margin.balance),
                                                                      Equity = Convert.ToDecimal(margin.equity),
                                                                      Volume = margin.volume,
                                                                      Margin1 = Convert.ToDecimal(margin.margin),
                                                                      MarginFree = Convert.ToDecimal(margin.margin_free),
                                                                      MarginLevel = Convert.ToDecimal(margin.margin_level),
                                                                      MarginType = margin.margin_type,
                                                                      LevelType = margin.level_type

                                                                  }).ToList();


                                        try
                                        {
                                            foreach (var m in lstmargin)
                                            {
                                                marginCache2.Put(m.Login.ToString(), m);
                                            }
                                        }
                                        catch (DataCacheException dataCacheEx)
                                        {

                                            CurrentDeskLog.Error(dataCacheEx.Message, dataCacheEx);
                                            #region "FailOver Caching"
                                            /*
                                            try
                                            {

                                                DataTable dt = new DataTable("Margins");

                                                dt.Columns.Add("LoginId", typeof(int));
                                                dt.Columns.Add("Balance", typeof(decimal));
                                                dt.Columns.Add("Equity", typeof(decimal));
                                                dt.Columns.Add("Margin", typeof(decimal));

                                                foreach (var margin in lstmargin)
                                                {
                                                    dt.Rows.Add(margin.Login, margin.Balance, margin.Equity, margin.Margin1);
                                                }

                                                TradesHistoryBO tradesHistory = new TradesHistoryBO();
                                                string connectionString = tradesHistory.GetConnectionString();
                                                using (SqlConnection connection = new SqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    SqlCommand sqlCmd = new SqlCommand("dbo.UpdateMargin", connection);
                                                    sqlCmd.CommandType = CommandType.StoredProcedure;
                                                    SqlParameter sqlParam = sqlCmd.Parameters.Add("@marginType", dt);
                                                    sqlParam.SqlDbType = SqlDbType.Structured;
                                                    int rn = sqlCmd.ExecuteNonQuery();
                                                }



                                            }
                                            catch (Exception ex)
                                            {
                                                CurrentDeskLog.Error(ex.Message, ex);
                                            }
                                             * */
                                            #endregion
                                        }

                                    }

                                });

                                taskRealtimeMargin2.Start();
                            }
                            else
                            {
                                lstTempMargins2.AddRange(lstNewMargins);
                            }
                        }
                        else
                        {
                            CurrentDeskLog.Info("MarginsGet returns NULL");

                        }
                    }
                    catch (Exception ex)
                    {
                        CurrentDeskLog.Error(ex.Message, ex);
                    }


                    #endregion

                    #region "Price Synch with Cache"


                    try
                    {
                        //Get All Symbols only and get the price from MT4 and save to "PriceCache"
                        var lstSymbols = lstSymbMarginMode.Select(s => s.Symbol).ToList();
                        if (taskPrice2 == null || taskPrice2.IsCompleted)
                        {
                            taskPrice2 = new Task(() =>
                            {
                                foreach (var sym in lstSymbols)
                                {
                                    MT4ManLibraryNETv03.TickInfoNET[] TickInfoArray;
                                    int totalTicks = 0;
                                    TickInfoArray = managerA2.TickInfoLast(sym, ref totalTicks);
                                    try
                                    {
                                        if (TickInfoArray != null && totalTicks > 0)
                                        {
                                            PriceInfo pinfo = new PriceInfo();
                                            pinfo.Bid = TickInfoArray[0].bid;
                                            pinfo.Ask = TickInfoArray[0].ask;
                                            priceCache2.Put(sym, pinfo);

                                        }
                                    }
                                    catch (DataCacheException exDataCache)
                                    {
                                        CurrentDeskLog.Error(exDataCache.Message, exDataCache);
                                    }
                                }

                            });

                            taskPrice2.Start();
                        }


                    }
                    catch (Exception ex)
                    {

                        CurrentDeskLog.Error(ex.Message, ex);
                    }
                    #endregion

                    break;

                case PUMP_PING:
                    var pingstatus = managerA2.Ping();
                    CurrentDeskLog.Info("PUMP_PING A2:" + pingstatus);
                    break;
                case TEST_LOCAL:
                    break;
                default:
                    code = 911;
                    break;
            }
        }

        protected void CallBackMethodEx3(int code, int type, Object data, Object param)
        {
            switch (code)
            {
                case PUMP_START_PUMPING:
                    break;

                case PUMP_UPDATE_GROUPS:
                    CurrentDeskLog.Info("PUMP_UPDATE_GROUPS A3");
                    break;

                case PUMP_UPDATE_USERS:
                    break;

                case PUMP_UPDATE_ONLINE:
                    break;

                case PUMP_UPDATE_SYMBOLS:

                    #region "Synch Symbol"

                    try
                    {
                        int totalSymbl = 0;
                        ConSymbolNET[] symb;
                        symb = managerA3.SymbolsGetAll(ref totalSymbl);

                        if (taskSymbols == null || taskSymbols.IsCompleted)
                        {
                            //taskSymbols = new Task(() =>
                            //{
                            //    //Get symbols for a broker
                            //    var lstSpecificSymbol = symb.Where(s => s.symbol.EndsWith(BrokerSymbol)).ToList();
                            //    SynchSymbols(lstSpecificSymbol);
                            //});

                            //taskSymbols.Start();
                        }
                    }
                    catch (Exception ex)
                    {

                        CurrentDeskLog.Error(ex.Message, ex);
                    }

                    #endregion
                    CurrentDeskLog.Info("PUMP_UPDATE_SYMBOLS A3");

                    break;

                case PUMP_UPDATE_MARGINCALL:
                    CurrentDeskLog.Info("PUMP_UPDATE_MARGINCALL A3");
                    break;

                case PUMP_STOP_PUMPING:

                    #region Restart pumping
                    CurrentDeskLog.Info("PUMP_STOP_PUMPING A3");
                    //Restart Pumping
                    try
                    {
                        Thread tRestart = new Thread(ReStartPumpingA3);
                        tRestart.Start();
                    }
                    catch (Exception ex)
                    {

                        CurrentDeskLog.Error(ex.Message, ex);
                    }
                    #endregion

                    break;

                case PUMP_UPDATE_TRADES: // 9
                    #region "Update Trades"
                    try
                    {
                        if (data != null)
                        {
                            TradeRecordNET tinfo = (TradeRecordNET)data;

                            #region "Synch Trade"

                            Trade trade = GetTrade(tinfo);
                            trade.Comment = trade.Comment + " LIVE ";
                            switch (type)
                            {
                                case (int)Transaction.TRANS_ADD:
                                    {

                                        if (lstSymbol.IndexOf(tinfo.symbol) == -1)
                                        {
                                            lstSymbol.Add(tinfo.symbol);
                                        }

                                        new Task(() =>
                                        {

                                            TradeBO tradeBO = new TradeBO();
                                            tradeBO.AddTrade(trade);

                                        }).Start();
                                    }
                                    break;

                                case (int)Transaction.TRANS_DELETE:
                                    {
                                        new Task(() =>
                                        {

                                            TradeBO tradeBO = new TradeBO();
                                            tradeBO.DeleteTrade(trade);

                                        }).Start();
                                    }
                                    break;

                                case (int)Transaction.TRANS_UPDATE:
                                    {
                                        new Task(() =>
                                        {

                                            TradeBO tradeBO = new TradeBO();
                                            tradeBO.UpdateTrade(trade);

                                        }).Start();
                                    }
                                    break;
                            }

                            #endregion

                        }

                    }
                    catch (Exception ex)
                    {
                        CurrentDeskLog.Error(ex.Message, ex);
                    }
                    #endregion

                    break;

                case PUMP_UPDATE_BIDASK:

                    #region "Price Synch"
                    /*
                    try
                    {

                        
                        

                        //Price Pumping
                        if (lstSymbol.Count > 0)
                        {
                            if (taskPrice == null || taskPrice.IsCompleted)
                            {
                                taskPrice = new Task(() =>
                                {
                                    List<Price> lstPrice = new List<CurrentDesk.Models.Price>();

                                    Parallel.ForEach(lstSymbol, symbole =>
                                    {

                                        MT4ManLibraryNETv03.TickInfoNET[] TickInfoArray;
                                        int totalTicks = 0;
                                        TickInfoArray = manager.TickInfoLast(symbole, ref totalTicks);

                                        if (totalTicks > 0)
                                        {
                                            Price p = new Price();
                                            p.Ask = Convert.ToDecimal(TickInfoArray[0].ask);
                                            p.Bid = Convert.ToDecimal(TickInfoArray[0].bid);
                                            p.Symbole = TickInfoArray[0].symbol ?? string.Empty;

                                            DateTime dt = new DateTime(TickInfoArray[0].ctm);
                                            p.Time = TickInfoArray[0].ctm.TimeStampToDateTime();

                                            lstPrice.Add(p);
                                        }

                                    });

                                    if (lstPrice.Count > 0)
                                    {
                                        MetaTraderTrades metaTrader = new MetaTraderTrades();
                                        metaTrader.UpdatePrice(lstPrice);
                                    }
                                }, TaskCreationOptions.LongRunning);

                                taskPrice.Start();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    }
                    
                    */
                    #endregion

                    #region "Realtime Margin Synch"


                    try
                    {
                        var marginTotal1 = 0;
                        var mrgnLevelNET1 = managerA3.MarginsGet(ref marginTotal1);
                        List<MarginLevelNET> lstNewMargins = new List<MarginLevelNET>();


                        if (mrgnLevelNET1 != null)
                        {
                            if (arrMarginLevel3 == null)
                            {
                                arrMarginLevel3 = mrgnLevelNET1;
                            }
                            else
                            {
                                lstNewMargins = (from nmargin in mrgnLevelNET1
                                                 join omargin in arrMarginLevel3
                                                     on nmargin.login equals omargin.login
                                                 where nmargin.equity != omargin.equity
                                                 select nmargin).ToList();


                            }

                            if (taskRealtimeMargin3 == null || taskRealtimeMargin3.IsCompleted)
                            {
                                arrMarginLevel3 = mrgnLevelNET1;

                                if (lstTempMargins3.Count > 0)
                                {
                                    var skipedMargins = (from lt in lstTempMargins3
                                                         where !lstNewMargins.Select(s => s.login).Contains(lt.login)
                                                         select lt).ToList();

                                    lstNewMargins.AddRange(skipedMargins);
                                    lstTempMargins3 = new List<MarginLevelNET>();
                                }

                                taskRealtimeMargin3 = new Task(() =>
                                {

                                    if (lstNewMargins.Count > 0)
                                    {
                                        List<Margin> lstmargin = (from margin in lstNewMargins
                                                                  select new Margin
                                                                  {
                                                                      Group = margin.@group,
                                                                      Login = margin.login,
                                                                      Leverage = margin.leverage,
                                                                      Updated = margin.updated,
                                                                      Balance = Convert.ToDecimal(margin.balance),
                                                                      Equity = Convert.ToDecimal(margin.equity),
                                                                      Volume = margin.volume,
                                                                      Margin1 = Convert.ToDecimal(margin.margin),
                                                                      MarginFree = Convert.ToDecimal(margin.margin_free),
                                                                      MarginLevel = Convert.ToDecimal(margin.margin_level),
                                                                      MarginType = margin.margin_type,
                                                                      LevelType = margin.level_type

                                                                  }).ToList();


                                        try
                                        {
                                            foreach (var m in lstmargin)
                                            {
                                                marginCache3.Put(m.Login.ToString(), m);
                                            }
                                        }
                                        catch (DataCacheException dataCacheEx)
                                        {

                                            CurrentDeskLog.Error(dataCacheEx.Message, dataCacheEx);
                                            #region "FailOver Caching"
                                            /*
                                            try
                                            {

                                                DataTable dt = new DataTable("Margins");

                                                dt.Columns.Add("LoginId", typeof(int));
                                                dt.Columns.Add("Balance", typeof(decimal));
                                                dt.Columns.Add("Equity", typeof(decimal));
                                                dt.Columns.Add("Margin", typeof(decimal));

                                                foreach (var margin in lstmargin)
                                                {
                                                    dt.Rows.Add(margin.Login, margin.Balance, margin.Equity, margin.Margin1);
                                                }

                                                TradesHistoryBO tradesHistory = new TradesHistoryBO();
                                                string connectionString = tradesHistory.GetConnectionString();
                                                using (SqlConnection connection = new SqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    SqlCommand sqlCmd = new SqlCommand("dbo.UpdateMargin", connection);
                                                    sqlCmd.CommandType = CommandType.StoredProcedure;
                                                    SqlParameter sqlParam = sqlCmd.Parameters.Add("@marginType", dt);
                                                    sqlParam.SqlDbType = SqlDbType.Structured;
                                                    int rn = sqlCmd.ExecuteNonQuery();
                                                }



                                            }
                                            catch (Exception ex)
                                            {
                                                CurrentDeskLog.Error(ex.Message, ex);
                                            }
                                             * */
                                            #endregion
                                        }

                                    }

                                });

                                taskRealtimeMargin3.Start();
                            }
                            else
                            {
                                lstTempMargins3.AddRange(lstNewMargins);
                            }
                        }
                        else
                        {
                            CurrentDeskLog.Info("MarginsGet returns NULL");

                        }
                    }
                    catch (Exception ex)
                    {
                        CurrentDeskLog.Error(ex.Message, ex);
                    }


                    #endregion

                    #region "Price Synch with Cache"


                    try
                    {
                        //Get All Symbols only and get the price from MT4 and save to "PriceCache"
                        var lstSymbols = lstSymbMarginMode.Select(s => s.Symbol).ToList();
                        if (taskPrice3 == null || taskPrice3.IsCompleted)
                        {
                            taskPrice3 = new Task(() =>
                            {
                                foreach (var sym in lstSymbols)
                                {
                                    MT4ManLibraryNETv03.TickInfoNET[] TickInfoArray;
                                    int totalTicks = 0;
                                    TickInfoArray = managerA3.TickInfoLast(sym, ref totalTicks);
                                    try
                                    {
                                        if (TickInfoArray != null && totalTicks > 0)
                                        {
                                            PriceInfo pinfo = new PriceInfo();
                                            pinfo.Bid = TickInfoArray[0].bid;
                                            pinfo.Ask = TickInfoArray[0].ask;
                                            priceCache3.Put(sym, pinfo);

                                        }
                                    }
                                    catch (DataCacheException exDataCache)
                                    {
                                        CurrentDeskLog.Error(exDataCache.Message, exDataCache);
                                    }
                                }

                            });

                            taskPrice3.Start();
                        }


                    }
                    catch (Exception ex)
                    {

                        CurrentDeskLog.Error(ex.Message, ex);
                    }
                    #endregion

                    break;

                case PUMP_PING:
                    var pingstatus = managerA3.Ping();
                    CurrentDeskLog.Info("PUMP_PING A3:" + pingstatus);
                    break;
                case TEST_LOCAL:
                    break;
                default:
                    code = 911;
                    break;
            }
        }

        /// <summary>
        /// Synch Symbols with Symbols table
        /// </summary>
        /// <param name="lstSymbols"></param>
        protected void SynchSymbols(List<ConSymbolNET> lstSymbols)
        {
            #region "Create DataTable for Symbol"

            DataTable dtSymbol = new DataTable("Symbol");
            dtSymbol.Columns.Add("PK_SymbolID", typeof(int));
            dtSymbol.Columns.Add("Currency", typeof(string));
            dtSymbol.Columns.Add("Description", typeof(string));
            dtSymbol.Columns.Add("MarginCurrency", typeof(string));
            dtSymbol.Columns.Add("MarginMode", typeof(int));
            dtSymbol.Columns.Add("ProfitMode", typeof(int));
            dtSymbol.Columns.Add("Symbol", typeof(string));
            dtSymbol.Columns.Add("Trade", typeof(int));
            dtSymbol.Columns.Add("Type", typeof(int));

            foreach (ConSymbolNET symboleNet in lstSymbols)
            {
                var newMode = new SymbolMarginMode();
                newMode.MarginMode = symboleNet.margin_mode;
                newMode.Symbol = symboleNet.symbol;
               
                // Added Symbols to List
                lstSymbMarginMode.Add(newMode);

                dtSymbol.Rows.Add(0, symboleNet.currency, symboleNet.description, symboleNet.margin_currency, symboleNet.margin_mode, symboleNet.profit_mode, symboleNet.symbol, symboleNet.trade, symboleNet.type);

            }

            #endregion

            //Clear Existing symbole and refresh Symbols with new symbols
            if (dtSymbol.Rows.Count > 0)
            {
                SymbolBO symbolBO = new SymbolBO();
                bool isClear = symbolBO.ClearExistingSymbol();
                if (isClear)
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(CurrentDesk.Repository.Utility.StaticCache.ConnectionString))
                        {
                            connection.Open();
                            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                            {
                                bulkCopy.BatchSize = 500;
                                bulkCopy.DestinationTableName =
                                    "dbo.Symbol";

                                try
                                {
                                    bulkCopy.WriteToServer(dtSymbol);
                                }
                                catch (Exception ex)
                                {
                                    CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                                }
                            }

                            connection.Close();
                        }
                    }
                    catch (Exception exception)
                    {
                        CommonErrorLogger.CommonErrorLog(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    }
                }

            }
        }

        #endregion

        /// <summary>
        /// Synch OpenTrades in AzureCache
        /// Get All openTrades from MetaTrader and Save it to TradesCache
        /// </summary>
        public void SynchTradesInCache()
        {

            while (true)
            {
                try
                {
                    bool isConnected = ConnectToManager(managerTrades,ServerName,ManagerId,ManagerPassword);
                    if (isConnected)
                    {
                        //Get trades from MT4
                        int tradesRef = 0;
                        TradeRecordNET[] lstTradeRecords = managerTrades.TradesRequest(ref tradesRef);
                        if (tradesRef > 0)
                        {

                            var lstTrades = (from trade in lstTradeRecords
                                             select new TradesInfo
                                             {
                                                 Order = trade.order,
                                                 Login = trade.login,
                                                 Profit = trade.profit,
                                                 Symbol = trade.symbol,
                                                 Cmd = trade.cmd,
                                                 OpenTime = trade.open_time,
                                                 OpenPrice = trade.open_price,
                                                 CloseTime = trade.close_time,
                                                 ClosePrice = trade.close_price,
                                                 TP = trade.tp,
                                                 SL = trade.sl,
                                                 Margin = trade.margin_rate,
                                                 Comment = trade.comment,
                                                 Storage = trade.storage,
                                                 Commsission = trade.commission
                                             }).ToList();

                            //Add to cache                           
                            try
                            {
                                var lstLogin = lstTrades.Select(l => l.Login).ToList();
                               

                                foreach (var tinfo in lstLogin)
                                {
                                    List<TradesInfo> lstTradesInfo = lstTrades.Where(l => l.Login == tinfo).ToList();
                                    tradesCache.Put(tinfo.ToString(), lstTradesInfo);
                                }
                            }
                            catch (DataCacheException ex)
                            {

                                CurrentDeskLog.Error(ex.Message,ex);

                                #region "Failover caching For Trades"

                                try
                                {

                                    DataTable dt = new DataTable("Trades");

                                    dt.Columns.Add("OrderId", typeof(int));
                                    dt.Columns.Add("Profit", typeof(double));
                                    dt.Columns.Add("ClosePrice", typeof(double));
                                    dt.Columns.Add("Commission", typeof(double));
                                    dt.Columns.Add("Storage", typeof(double));

                                    foreach (var trade in lstTrades)
                                    {
                                        dt.Rows.Add(trade.Order,trade.Profit,trade.ClosePrice,trade.Commsission,trade.Storage);
                                    }

                                    TradesHistoryBO tradesHistory = new TradesHistoryBO();
                                    string connectionString = tradesHistory.GetConnectionString();
                                    using (SqlConnection connection = new SqlConnection(connectionString))
                                    {
                                        connection.Open();
                                        SqlCommand sqlCmd = new SqlCommand("dbo.UpdateTrades", connection);
                                        sqlCmd.CommandType = CommandType.StoredProcedure;
                                        SqlParameter sqlParam = sqlCmd.Parameters.Add("@tradeType", dt);
                                        sqlParam.SqlDbType = SqlDbType.Structured;
                                        int rn = sqlCmd.ExecuteNonQuery();
                                    }
                                }
                                catch (Exception exSql)
                                {
                                    CurrentDeskLog.Error(exSql.Message, exSql);
                                }
                                #endregion
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    CurrentDeskLog.Error(ex.Message, ex);
                }
            }
        }

        public void SynchTradesInCacheMa1()
        {

            while (true)
            {
                try
                {

                 
                    bool isConnected = ConnectToManager(managerTradesA1, ServerName, ManagerId, ManagerPassword);
                    if (isConnected)
                    {

                        var t1 = DateTime.Now;
                        //Get trades from MT4
                        int tradesRef = 0;
                        TradeRecordNET[] lstTradeRecords = managerTradesA1.TradesRequest(ref tradesRef);
                        if (tradesRef > 0)
                        {

                            var lstTrades = (from trade in lstTradeRecords
                                             select new TradesInfo
                                             {
                                                 Order = trade.order,
                                                 Login = trade.login,
                                                 Profit = trade.profit,
                                                 Symbol = trade.symbol,
                                                 Cmd = trade.cmd,
                                                 OpenTime = trade.open_time,
                                                 OpenPrice = trade.open_price,
                                                 CloseTime = trade.close_time,
                                                 ClosePrice = trade.close_price,
                                                 TP = trade.tp,
                                                 SL = trade.sl,
                                                 Margin = trade.margin_rate,
                                                 Comment = trade.comment,
                                                 Storage = trade.storage,
                                                 Commsission = trade.commission
                                             }).ToList();

                            //Add to cache                           
                            try
                            {
                                var lstLogin = lstTrades.Select(l => l.Login).ToList();


                                foreach (var tinfo in lstLogin)
                                {
                                    List<TradesInfo> lstTradesInfo = lstTrades.Where(l => l.Login == tinfo).ToList();
                                    tradesCache1.Put(tinfo.ToString(), lstTradesInfo);
                                }
                            }
                            catch (DataCacheException ex)
                            {

                                CurrentDeskLog.Error(ex.Message, ex);
                              
                                #region "Failover caching For Trades"
                                /*
                                try
                                {

                                    DataTable dt = new DataTable("Trades");

                                    dt.Columns.Add("OrderId", typeof(int));
                                    dt.Columns.Add("Profit", typeof(double));
                                    dt.Columns.Add("ClosePrice", typeof(double));
                                    dt.Columns.Add("Commission", typeof(double));
                                    dt.Columns.Add("Storage", typeof(double));

                                    foreach (var trade in lstTrades)
                                    {
                                        dt.Rows.Add(trade.Order, trade.Profit, trade.ClosePrice, trade.Commsission, trade.Storage);
                                    }

                                    TradesHistoryBO tradesHistory = new TradesHistoryBO();
                                    string connectionString = tradesHistory.GetConnectionString();
                                    using (SqlConnection connection = new SqlConnection(connectionString))
                                    {
                                        connection.Open();
                                        SqlCommand sqlCmd = new SqlCommand("dbo.UpdateTrades", connection);
                                        sqlCmd.CommandType = CommandType.StoredProcedure;
                                        SqlParameter sqlParam = sqlCmd.Parameters.Add("@tradeType", dt);
                                        sqlParam.SqlDbType = SqlDbType.Structured;
                                        int rn = sqlCmd.ExecuteNonQuery();
                                    }
                                }
                                catch (Exception exSql)
                                {
                                    CurrentDeskLog.Error(exSql.Message, exSql);
                                }
                                   */
                                #endregion

                            }

                        }

                        var t2 = DateTime.Now;


                        var td = t2 - t1;

                        CurrentDeskLog.Error("A1 TradesCache :" + td);
                    }
                }
                catch (Exception ex)
                {
                    CurrentDeskLog.Error(ex.Message, ex);
                }
            }
        }

        public void SynchTradesInCacheMa2()
        {

            while (true)
            {
                try
                {
                    bool isConnected = ConnectToManager(managerTradesA2, ServerName, ManagerId, ManagerPassword);
                    if (isConnected)
                    {
                       
                        //Get trades from MT4
                        int tradesRef = 0;
                        TradeRecordNET[] lstTradeRecords = managerTradesA2.TradesRequest(ref tradesRef);
                        if (tradesRef > 0)
                        {

                            var lstTrades = (from trade in lstTradeRecords
                                             select new TradesInfo
                                             {
                                                 Order = trade.order,
                                                 Login = trade.login,
                                                 Profit = trade.profit,
                                                 Symbol = trade.symbol,
                                                 Cmd = trade.cmd,
                                                 OpenTime = trade.open_time,
                                                 OpenPrice = trade.open_price,
                                                 CloseTime = trade.close_time,
                                                 ClosePrice = trade.close_price,
                                                 TP = trade.tp,
                                                 SL = trade.sl,
                                                 Margin = trade.margin_rate,
                                                 Comment = trade.comment,
                                                 Storage = trade.storage,
                                                 Commsission = trade.commission
                                             }).ToList();

                            //Add to cache                           
                            try
                            {
                                var lstLogin = lstTrades.Select(l => l.Login).ToList();

                                var t1 = DateTime.Now;
                                foreach (var tinfo in lstLogin)
                                {
                                    List<TradesInfo> lstTradesInfo = lstTrades.Where(l => l.Login == tinfo).ToList();
                                    tradesCache2.Put(tinfo.ToString(), lstTradesInfo);
                                }
                                var t2 = DateTime.Now;

                                var td = t2 - t1;
                                CurrentDeskLog.Info("A2 Trades Cache :" + td + " , "+ lstTrades.Count);
                            }
                            catch (DataCacheException ex)
                            {

                                CurrentDeskLog.Error(ex.Message, ex);

                                #region "Failover caching For Trades"
                                /*
                                try
                                {

                                    DataTable dt = new DataTable("Trades");

                                    dt.Columns.Add("OrderId", typeof(int));
                                    dt.Columns.Add("Profit", typeof(double));
                                    dt.Columns.Add("ClosePrice", typeof(double));
                                    dt.Columns.Add("Commission", typeof(double));
                                    dt.Columns.Add("Storage", typeof(double));

                                    foreach (var trade in lstTrades)
                                    {
                                        dt.Rows.Add(trade.Order, trade.Profit, trade.ClosePrice, trade.Commsission, trade.Storage);
                                    }

                                    TradesHistoryBO tradesHistory = new TradesHistoryBO();
                                    string connectionString = tradesHistory.GetConnectionString();
                                    using (SqlConnection connection = new SqlConnection(connectionString))
                                    {
                                        connection.Open();
                                        SqlCommand sqlCmd = new SqlCommand("dbo.UpdateTrades", connection);
                                        sqlCmd.CommandType = CommandType.StoredProcedure;
                                        SqlParameter sqlParam = sqlCmd.Parameters.Add("@tradeType", dt);
                                        sqlParam.SqlDbType = SqlDbType.Structured;
                                        int rn = sqlCmd.ExecuteNonQuery();
                                    }
                                }
                                catch (Exception exSql)
                                {
                                    CurrentDeskLog.Error(exSql.Message, exSql);
                                }
                                   */
                                #endregion

                            }

                        }

                       
                    }
                }
                catch (Exception ex)
                {
                    CurrentDeskLog.Error(ex.Message, ex);
                }
            }
        }

        public void SynchTradesInCacheMa3()
        {

            while (true)
            {
                try
                {
                    bool isConnected = ConnectToManager(managerTradesA3, ServerNameL, ManagerIdL, ManagerPasswordL);
                    if (isConnected)
                    {

                       

                        //Get trades from MT4
                        int tradesRef = 0;
                        TradeRecordNET[] lstTradeRecords = managerTradesA3.TradesRequest(ref tradesRef);


                        if (tradesRef > 0)
                        {

                            var lstTrades = (from trade in lstTradeRecords
                                             select new TradesInfo
                                             {
                                                 Order = trade.order,
                                                 Login = trade.login,
                                                 Profit = trade.profit,
                                                 Symbol = trade.symbol,
                                                 Cmd = trade.cmd,
                                                 OpenTime = trade.open_time,
                                                 OpenPrice = trade.open_price,
                                                 CloseTime = trade.close_time,
                                                 ClosePrice = trade.close_price,
                                                 TP = trade.tp,
                                                 SL = trade.sl,
                                                 Margin = trade.margin_rate,
                                                 Comment = trade.comment,
                                                 Storage = trade.storage,
                                                 Commsission = trade.commission
                                             }).ToList();
                       
                            //Add to cache                           
                            try
                            {
                                var lstLogin = lstTrades.Select(l => l.Login).ToList();

                                var t1 = DateTime.Now;

                                
                                foreach (var tinfo in lstLogin)
                                {                                
                                    tradesCache3.Put(tinfo.ToString(), lstTrades.Where(l => l.Login == tinfo).ToList());
                                }

                                var t2 = DateTime.Now;

                                var td = t2 - t1;

                                CurrentDeskLog.Info ("A3 TradesCache  :" + td + " , " + lstTrades.Count);
                            }
                            catch (DataCacheException ex)
                            {

                                CurrentDeskLog.Error(ex.Message, ex);

                                #region "Failover caching For Trades"
                                /*
                                try
                                {

                                    DataTable dt = new DataTable("Trades");

                                    dt.Columns.Add("OrderId", typeof(int));
                                    dt.Columns.Add("Profit", typeof(double));
                                    dt.Columns.Add("ClosePrice", typeof(double));
                                    dt.Columns.Add("Commission", typeof(double));
                                    dt.Columns.Add("Storage", typeof(double));

                                    foreach (var trade in lstTrades)
                                    {
                                        dt.Rows.Add(trade.Order, trade.Profit, trade.ClosePrice, trade.Commsission, trade.Storage);
                                    }

                                    TradesHistoryBO tradesHistory = new TradesHistoryBO();
                                    string connectionString = tradesHistory.GetConnectionString();
                                    using (SqlConnection connection = new SqlConnection(connectionString))
                                    {
                                        connection.Open();
                                        SqlCommand sqlCmd = new SqlCommand("dbo.UpdateTrades", connection);
                                        sqlCmd.CommandType = CommandType.StoredProcedure;
                                        SqlParameter sqlParam = sqlCmd.Parameters.Add("@tradeType", dt);
                                        sqlParam.SqlDbType = SqlDbType.Structured;
                                        int rn = sqlCmd.ExecuteNonQuery();
                                    }
                                }
                                catch (Exception exSql)
                                {
                                    CurrentDeskLog.Error(exSql.Message, exSql);
                                }
                                   */
                                #endregion

                            }

                        }
                       
                    }

                   
                }
                catch (Exception ex)
                {
                    CurrentDeskLog.Error(ex.Message, ex);
                }
            }
        }
    }

    public class CCobject
    {
        public int orderid;
        public int login;
        public double pnl;
        public double cms;
        public double storage;
        public double price;
    }
}

