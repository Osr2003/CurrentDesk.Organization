#region Header Information
/*******************************************************************
 * File Name     : MetaTraderTrades.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 12th March 2013
 * Modified Date : 12th March 2013
 * Description   : This file contains methods to check new trades
 *                 from Meta Trader api and insert them into database
 * *****************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.Logging;
using CurrentDesk.Models;
using CurrentDesk.Repository.CurrentDesk;
using System;
using System.Collections.Generic;
using CurrentDesk.Repository;
#endregion

namespace MetaTraderService
{
    /// <summary>
    /// This class contains methods to check new trades
    /// from Meta Trader api and insert them into database
    /// </summary>
    public class MetaTraderTrades
    {
        #region Variables
        private TradeBO tradeBO = new TradeBO();

        
        private Client_AccountBO clientAccBO = new Client_AccountBO();

        private PriceBO priceBO = new PriceBO();
        private MarginBO marginBO = new MarginBO();
        private UserRecordBO userRecordBO = new UserRecordBO();
        
        #endregion

        /// <summary>
        /// This method checks every open trade if present in database,
        /// if not then insert into Trades table
        /// </summary>
        /// <param name="openTrades">openTrades</param>
        public void InsertOpenTrades(dynamic openTrades)
        {
            try
            {
                //Get all trades from database
                var allTrades = tradeBO.GetAllTrades();

                //Iterate through each open trade from api
                foreach (var trade in openTrades)
                {
                    //If not present in db then insert
                    if (!CheckOpenTradeInExistingTrade(trade.order, allTrades))
                    {
                        tradeBO.InsertTrade(trade);
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This method checks if open trade order id
        /// is already present in db or not
        /// </summary>
        /// <param name="order">order</param>
        /// <param name="allTrades">allTrades</param>
        /// <returns>bool</returns>
        public bool CheckOpenTradeInExistingTrade(int order, List<Trade> allTrades)
        {
            try
            {
                //Iterate through each trade present in db
                foreach (var trade in allTrades)
                {
                    if (trade.OrderID == order)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This method gets all logins for which trading
        /// has been done on particular day
        /// </summary>
        /// <param name="checkDate">checkDate</param>
        /// <returns></returns>
        public List<int> GetAllLoginsTradedToday(DateTime checkDate)
        {
            try
            {
                return tradeBO.GetAllLoginsTradedToday(checkDate);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This method updates LastTradingDate in Client_Account
        /// table for all passed logins
        /// </summary>
        /// <param name="logins"></param>
        public void UpdateClientTradeDate(List<int> logins)
        {
            try
            {
                clientAccBO.UpdateClientTradeDate(logins);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }


        public void UpdatePrice(List<Price> lstPrice)
        {
            priceBO.InsertPrice(lstPrice);

        }

        public void UpdateMargin(List<Margin> lstMargin)
        {
            marginBO.UpdateMargin(lstMargin);
        }


        public void SaveTrades(List<Trade> lstTrade)
        {
            tradeBO.SaveOpenTrades(lstTrade);
        }


        /// <summary>
        /// Calculate Equity for clients
        /// </summary>
        /// <param name="loginid"></param>
        /// <returns></returns>
        public decimal GetValueForEquity(int loginid)
        {
            decimal equityForClients = 0;

            try
            {
                dynamic openTrade = (dynamic)tradeBO.GetUserOpenTrades(loginid);

                //Get Margin Balance
                decimal balance = marginBO.GetMarginBalance(loginid);

                //Get Credit amout
                decimal credit = Convert.ToDecimal(userRecordBO.CreditAmount(loginid));

                //Actual balance
                decimal actualbalance = balance - credit;


                var pnl = openTrade.GetType().GetProperty("Pnl").GetValue(openTrade, null);
                var commision = openTrade.GetType().GetProperty("Commision").GetValue(openTrade, null);
                var swap = openTrade.GetType().GetProperty("Swap").GetValue(openTrade, null);

                equityForClients = actualbalance + Convert.ToDecimal(pnl) + Convert.ToDecimal(commision) + Convert.ToDecimal(swap);

            }
            catch (Exception ex)
            {
                CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return equityForClients;
          
        }

    }
}
