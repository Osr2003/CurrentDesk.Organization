#region HEADER
/******************************************************************
 * File Name     : BOMAMManagement.cs
 * Author        : Mukesh Nayak
 * Copyright     : Mindfire Solutions
 * Creation Date : 4th June 2013
 * Modified Date : 4th June 2013
 * Description   : This file Will Initiate the service. Service has got three task to do
 *                  a) Get all the asset manager and their corresponding client Account.
 *                  b) Loop through the open trades
 *                  c) Loop through Closed Trades
 * ****************************************************************/
#endregion

#region NAMESPACE
using CurrentDesk.Logging;
using CurrentDesk.Models;
using CurrentDesk.Repository.CurrentDesk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
#endregion

namespace CurrentDesk.BackOfficeMamService
{
    /// <summary>
    /// BOManagement.cs class to initiate Service.
    /// </summary>
    public class BOMAMManagement
    {
        #region VARIABLE
        public Dictionary<int, List<R_AssetManager_IntroducingBroker_ClientAccount>> assetManagerDict;
        public Dictionary<int, double> slaveAllocationDict;
        public bool isInitiated;
        public bool isStop;
        public Thread boMAMAssetMangerThread;
        public Thread boMAMOpenTradeThread;
        public Thread bobMAMCloseThread;
        public Thread alertThread;

        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// This Is a constructor class here 
        /// we are instantiating all the threads
        /// </summary>
        public BOMAMManagement()
        {            
            isStop = true;
            isInitiated = false;

            //Assign all the threads
            alertThread = new Thread(StartMonitoringAlerts);
            boMAMAssetMangerThread = new Thread(StartBOMAM);
            boMAMOpenTradeThread = new Thread(StartBOMAMOpenTrades);
            bobMAMCloseThread = new Thread(StartBOMAMClosedTrades);
           
            
        }
        #endregion

        #region COMMON
        /// <summary>
        /// This Function will start All threads
        /// </summary>
        public void StartThreads()
        {
            CheckAlerts();
            MonitorBOMAM();
            MonitorBOMAMOpenTrades();
            MonitorBOMAMClosedTrades();
        }

        /// <summary>
        /// This Function will stop all threads
        /// </summary>
        public void StopThreads()
        {

            StopBOMAM();
            StopBOMAMClosedTrades();
            StopBOMAMOpenTrade();
        }

        #endregion

        #region CHECK ALERTS

        /// <summary>
        /// This Function will start Check Alerts Thread
        /// </summary>
        public void CheckAlerts()
        {
            try
            {
                //if thread running than return
                if (alertThread.IsAlive)
                {
                    return;
                }

                //Otherwise set as backgorund and start
                alertThread.IsBackground = true;
                alertThread.Start();
            }
            catch (Exception exceptionMessage)
            {
                //Log Error
                CurrentDeskLog.Error("Checking Alert Issue :" + exceptionMessage);
            }
        }

        /// <summary>
        /// This Function will check if there 
        /// is any alert or not and updates the variable
        /// </summary>
        public void StartMonitoringAlerts()
        {
            try
            {
                var boMAMAlertBO = new BOMAMAlertBO();

                while (true)
                {
                    if (boMAMAlertBO.CheckAlerts())
                    {
                        //Update the alerts to true
                        boMAMAlertBO.UpdateAlerts(false);

                        //Mark Initiated as false
                        isInitiated = false;
                        isStop = true;
                        
                        Thread.Sleep(5000);                        
                    }
                }
            }
            catch (Exception exceptionMessage)
            {
                //Log Error
                CurrentDeskLog.Error("Monitoring Alert Issue :" + exceptionMessage);
            }
        }

        #endregion

        #region GET ASSET MANAGER

        /// <summary>
        /// This Function will start
        /// StartBOMAM Thread
        /// </summary>
        public void MonitorBOMAM()
        {
            try
            {
                //If already existing Return
                if (boMAMAssetMangerThread.IsAlive)
                {
                    return;
                }               
               
                //otherwise run the thread
                boMAMAssetMangerThread.IsBackground = true;
                boMAMAssetMangerThread.Start();

            }
            catch (Exception exceptionMessage)
            {
                //Log Error
                CurrentDeskLog.Error("Monitoring Asset Manager :" + exceptionMessage);
            }
        }

        /// <summary>
        /// This Function Will StartBOMAM once in a day and will get 
        /// all the asset amanger and their corresponding slave account
        /// </summary>
        public void StartBOMAM()
        {
            var assetmanagerIntroducingBrokerBO = new R_AssetManager_IntroducingBroker_ClientAccountBO();

            //keep looping continuously
            while (true)
            {
                if (isStop)
                {
                    slaveAllocationDict = new Dictionary<int, double>();
                    assetManagerDict = new Dictionary<int, List<R_AssetManager_IntroducingBroker_ClientAccount>>();

                    try
                    {
                        //Get All Asset Manager
                        var assetManagerList = assetmanagerIntroducingBrokerBO.GetAllBOAssetManager();

                        //If the list count is greater Than O
                        if (assetManagerList.Count > 0)
                        {
                            //Group All the Asset Manager By IBID
                            //var pairedAssetManager = assetManagerList.GroupBy(asstM => asstM.FK_IBID);
                            var pairedAssetManager = from astMgr in assetManagerList
                                                     group astMgr by new { astMgr.FK_IBID, astMgr.FK_AMClientAccountID }
                                                         into assetMangers
                                                         select assetMangers;

                            foreach (var pair in pairedAssetManager)
                            {
                                //Get the platform login of the Asset Manager and add it to the
                                //dictionary with PaltformLogin and Slave Account
                                var resultantKey = pair.First().IntroducingBroker.Client_Account.
                                        Where(x => x.IsTradingAccount == true &&
                                        x.PK_ClientAccountID == pair.First().FK_AMClientAccountID).FirstOrDefault();

                                var key = (int)resultantKey.PlatformLogin;
                             

                                //Loop Through The Slave Account 
                                foreach (var item in pair)
                                {
                                    //Add to the 
                                    slaveAllocationDict.Add((int)item.FK_ClientAccountID, (double)item.AllocationRatio);
                                }

                                //Add the Data to the AssetManager
                                assetManagerDict.Add(key, pair.ToList());
                            }

                            //Mark Initiated as true
                            isInitiated = true;
                        }

                        //Check If the difference is greater than 24
                        //isStop = (datetime - DateTime.UtcNow).Hours < 24 ? false : true;
                        isStop = false;
                    }
                    catch (Exception exceptionMessage)
                    {
                        //Log Error
                        CurrentDeskLog.Error("Error Getting AssetManager :" + exceptionMessage);
                    }
                }
            }
        }

        /// <summary>
        /// This Function will Abort the thread
        /// </summary>
        public void StopBOMAM()
        {
            try
            {
                //Check whether Thread is null or alive
                if (boMAMAssetMangerThread != null && boMAMAssetMangerThread.IsAlive)
                {
                    boMAMAssetMangerThread.Abort();
                }
            }
            catch (Exception exceptionMessage)
            {
                //Log Error
                CurrentDeskLog.Error("Stopping Asset manager :" + exceptionMessage);
            }

        }

        #endregion

        #region GET OPEN TRADES

        /// <summary>
        /// This Function will start a thread which will continuously get all open trades.
        /// </summary>
        public void MonitorBOMAMOpenTrades()
        {
            try
            {
                //If already existing return
                if (boMAMOpenTradeThread.IsAlive)
                {
                    return;
                }

                //Otherwise open in background 
                boMAMOpenTradeThread.IsBackground = true;
                boMAMOpenTradeThread.Start();
            }
            catch (Exception exceptionMessage)
            {
                //Log Error
                CurrentDeskLog.Error("Open Trades :" + exceptionMessage);
            }
        }

        /// <summary>
        /// This Function is running in thread
        /// it will get all the trades after the
        /// processed id and than populateBOBOpen Trades data.
        /// </summary>
        public void StartBOMAMOpenTrades()
        {
            var lastProcessedID = 0;
            var tradeBO = new TradeBO();
            var boMAMTradeBO = new BOMAMTradeBO();
            var clientAccountBO = new Client_AccountBO();

            while (true)
            {
                //Check If Initiated
                if (isInitiated)
                {
                    try
                    {
                        if (assetManagerDict.Keys.Count > 0)
                        {  
                            //If The LastProcessID is 0 get it from database
                            if (lastProcessedID == 0)
                            {
                                lastProcessedID = boMAMTradeBO.GetLastOpenTradeProcessedID();
                            }

                            //Get All Open Trades For All the asset Manager                            
                            var openTradeResult = tradeBO.GetAssetManagerOpenTrades(assetManagerDict.Keys.ToList(), lastProcessedID);

                            //Check If the Count > 0
                            if (openTradeResult.Count > 0)
                            {
                                lastProcessedID = openTradeResult.Max(x => x.PK_TradeID);
                                var boMAMTradeList = new List<BOMAMTrade>();

                                //Loop Through The Open trades
                                foreach (var item in openTradeResult)
                                {
                                    foreach (var res in assetManagerDict[(int)item.Login])
                                    {
                                        BOMAMTrade openBOMAMTrade = new BOMAMTrade();
                                        openBOMAMTrade.OrderID = (int)item.OrderID;
                                        openBOMAMTrade.OpenPrice = item.OpenPrice;
                                        openBOMAMTrade.ClosePrice = item.ClosePrice;
                                        openBOMAMTrade.OpenTime = item.OpenTime;
                                        openBOMAMTrade.CloseTime = item.CloseTime;
                                        openBOMAMTrade.Agent = "";
                                        openBOMAMTrade.FK_IBID = (int)res.FK_IBID;
                                        openBOMAMTrade.FK_ClientAccountID = (int)res.FK_ClientAccountID;
                                        openBOMAMTrade.Symbol = item.Symbol;
                                        openBOMAMTrade.LastIDProcessed = item.PK_TradeID;
                                        openBOMAMTrade.Size = item.Volume != null ? (double)item.Volume * slaveAllocationDict[(int)res.FK_ClientAccountID] : 0.0;
                                        openBOMAMTrade.Commission = item.Commission != null ? (double)item.Commission * slaveAllocationDict[(int)res.FK_ClientAccountID] : 0.0;
                                        openBOMAMTrade.Swap = item.Storage != null ? (double)item.Storage * slaveAllocationDict[(int)res.FK_ClientAccountID] : 0.0;
                                        openBOMAMTrade.Pnl = item.Profit != null ? (double)item.Profit * slaveAllocationDict[(int)res.FK_ClientAccountID] : 0.0;
                                        openBOMAMTrade.IsopenTrades = true;
                                        boMAMTradeList.Add(openBOMAMTrade);
                                    }
                                }

                                //If Count > 0 Add it to the database
                                if (boMAMTradeList.Count > 0)
                                {
                                    boMAMTradeBO.AddBOMAMOpenTrades(boMAMTradeList);
                                }
                            }
                        }

                    }
                    catch (Exception exceptionMessage)
                    {
                        //Log Error
                        CurrentDeskLog.Error("Monitoring Open Trades :" + exceptionMessage);
                    }
                }
            }
        }

        /// <summary>
        /// This Function will abort the BOMAMOpenTrades
        /// </summary>
        public void StopBOMAMOpenTrade()
        {
            try
            {
                //Check if thread exist or alive
                if (boMAMOpenTradeThread != null && boMAMOpenTradeThread.IsAlive)
                {
                    boMAMOpenTradeThread.Abort();
                }
            }
            catch (Exception exceptionMessage)
            {
                //Log Error
                CurrentDeskLog.Error("Stopping Open Trades :" + exceptionMessage);
            }
        }

        #endregion

        #region GET CLOSED TRADES

        /// <summary>
        /// This Function will start a thread which will get 
        /// all the closed trades
        /// </summary>
        public void MonitorBOMAMClosedTrades()
        {
            try
            {
                //Return if already alive
                if (bobMAMCloseThread.IsAlive)
                {
                    return;
                }
             
                //Otherwise set in background and start
                bobMAMCloseThread.IsBackground = true;
                bobMAMCloseThread.Start();
            }
            catch (Exception exceptionMessage)
            {
                //Log Error
                CurrentDeskLog.Error("Closed Trades :" + exceptionMessage);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartBOMAMClosedTrades()
        {
            var lastProcessedID = 0;
            var tradeHistoryBO = new TradesHistoryBO();
            var bobMAMTradeBO = new BOMAMTradeBO();

            while (true)
            {
                if (isInitiated)
                {
                    try
                    {
                        if (assetManagerDict.Keys.Count > 0)
                        {
                            //If The LastProcessID is 0 get it from database
                            if (lastProcessedID == 0)
                            {
                                //Get The Last ClosedTradeProcessedID
                                lastProcessedID = bobMAMTradeBO.GetLastClosedTradeProcessedID();
                            }

                            //Updates to Get Last ProcessedID
                            lastProcessedID = bobMAMTradeBO.UpdateBOMAMOpenToCloseTrade(assetManagerDict, lastProcessedID);
                        }

                    }
                    catch (Exception exceptionMessage)
                    {
                        //Log Error
                        CurrentDeskLog.Error("Monitoring Closed Trades :" + exceptionMessage);
                    }
                }
            }
        }

        /// <summary>
        /// This Function will abort the BOMAMClosedThreads
        /// </summary>
        public void StopBOMAMClosedTrades()
        {
            try
            {
                //Check Nullability and See if it is alive
                if (bobMAMCloseThread != null && bobMAMCloseThread.IsAlive)
                {
                    //Aborting threads
                    bobMAMCloseThread.Abort();
                }
            }
            catch (Exception exceptionMessage)
            {
                //Log Error
                CurrentDeskLog.Error("Stopping Closed Trades :" + exceptionMessage);
            }
        }

        #endregion
    }


}
