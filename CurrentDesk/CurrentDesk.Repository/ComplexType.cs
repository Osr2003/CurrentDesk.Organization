using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CurrentDesk.Repository
{
    public class TradesInfo
    {
        public int Order;
        public int Login;
        public double Profit;
        public string Symbol;
        public int Cmd;
        public long OpenTime;
        public double OpenPrice;
        public long CloseTime;
        public double ClosePrice;
        public double TP;
        public double SL;
        public double Margin;
        public string Comment;
        public double Storage;
        public double Commsission;
    }

    public class MarginDetails
    {
        public int Pl;
        public string Bal;
        public string Equ;
        public string Pnl;
    }

    public class PriceInfo
    {
        public double Bid;
        public double Ask;
    }

    public class ClientCommission
    {
        public int ClientAccountId;

        public decimal Commission;
    }

    public class GroupSpread
    {

        public string GroupName;

        public int SpreadDiff;

    }

}
