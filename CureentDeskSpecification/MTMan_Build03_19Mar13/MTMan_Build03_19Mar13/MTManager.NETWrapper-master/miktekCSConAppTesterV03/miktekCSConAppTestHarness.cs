using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MT4ManLibraryNETv03;

// COMPILES AND WORKS AS OF 14:16 8.26.12
// declare a structure returned by ExposureGet
// modification dates: 8.15.2012
// need to get function calls passing, returning char[4] type data 
// returning back and forth. mikteck 8.15.2012
// Code Location on my machine: MT4ManWrapper.08.27.2012
//
namespace miktekCSConAppTesterV03
{
    // For the SymbolSummary convert __int64 to Int64
    // convert fixed length arrays e.g. char symbol[12]
    // miktek 8.7.12
    // miktek 8.16.2012 

    public class miktekCSConAppTestHarness
    { 

        // For the SymbolSummary convert __int64 to Int64
        // convert fixed length arrays e.g. char symbol[12]
        // miktek 8.7.12
        // miktek 8.16.2012 
        //public static MT4ManLibraryNETv03.CMTManager myNewOutsideManager =
        //    new MT4ManLibraryNETv03.CMTManager();

        // do I need one or two re thread mgmmt
        // unknow for know. Seems like 1 should do it.

        public static MT4ManLibraryNETv03.CMTManager myManager = 
            new CMTManager();

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
       
        protected static int globalTotal = 0;
    
        //User Group commands..
        //const int GROUP_DELETE = 0;
        //const int GROUP_ENABLE = 1;
        //const int GROUP_DISABLE = 2;
        //const int GROUP_LEVERAGE = 3;
        //const int GROUP_SETGROUP = 4;

        enum GroupOperation : int { GROUP_DELETE = 0, GROUP_ENABLE, GROUP_DISABLE, GROUP_LEVERAGE, GROUP_SETGROUP};

        // enum is proving a little intractable
        // would need to be move, declared and 
        // delegate prototype changed.
        // deferred 8.7.12 miktek

        // the following method is passed to the MT4Man Wrapper
        // and serves as an async callback.
        // problem being the instance of wrapper manager either
        // myOutsideManager or myManager are set to null
        // when the asynch call chain gets here? 
       
        protected static void CallBackMethod(int code)
        {
            // consider adding a collection of some kind queue, stack, etc
            // and put messages types on the queue
            // then have a registered event that processes
            // the queue when a message has arrived
            // or more brute forcish have a loop that processes
            // keystrokes, etc then allocate part of the cycle to proces
            // the que.
            
            String ErrorMessage = "Call back static callback";
           
            // note Access to class attributes works
            // within the async call back miktek 8.09.12

            int iRet = 0;

            // access to a global instance for counting
            miktekCSConAppTestHarness.globalTotal++;

            switch (code)
            {
                case PUMP_START_PUMPING: 
                    ErrorMessage = "PUMP_START_PUMPING";
                    break;
                
                case PUMP_UPDATE_GROUPS :
                    ErrorMessage = "PUMP_UPDATE_GROUPS";
                    break;

                case PUMP_UPDATE_USERS:
                    ErrorMessage = "PUMP_UPDATE_USERS";
                    break;

                case PUMP_UPDATE_ONLINE:
                    ErrorMessage = "PUMP_UPDATE_ONLINE";
                    break;

                case PUMP_UPDATE_SYMBOLS: // 1
                    ErrorMessage = "PUMP_UPDATE_SYMBOLS";
                    break;

                case PUMP_STOP_PUMPING: // 14
                    ErrorMessage = "PUMP_STOP_PUMPING";
                    iRet = myManager.dummyFunction(14);
                    break;

                case PUMP_UPDATE_TRADES: // 9
                    ErrorMessage = "PUMP_UPDATE_TRADES";
                    // this version is working as of 8.26.2012
                    // 
                    int intTotal = 0;

                    ExposureValueNET[] ExposedArray;
                    ExposedArray = myManager.ExposureGet(ref intTotal);

                    TradeRecordNET[] TradeInfo;
                    int val = 0;
                    TradeInfo = myManager.TradesGet(ref val);
                    break;

                case PUMP_UPDATE_BIDASK:
                    ErrorMessage = "PUMP_UPDATE_BIDASK";
                    int iRecCount = 0;
                    ExposureValueNET[] ExposedArray2;
                    ExposedArray2 = myManager.ExposureGet(ref iRecCount);
                    
                    break;
                case PUMP_PING:
                    // 8.26.2012
                    ErrorMessage = "PUMP_PING AND PINGING BACK";
                    iRet = myManager.Ping();
                    break;
                case TEST_LOCAL:
                    iRet = myManager.dummyFunction(34);                    
                    ErrorMessage = "THIS IS A TEST 777";
                    break;
                default:
                    ErrorMessage = "UNKNOWN";
                    code = 911;
                    break;
            }

            Console.WriteLine("MSG: {0}, CODE: {1} TOTAL MSGS: {2}", 
                ErrorMessage, code, miktekCSConAppTestHarness.globalTotal);

        }
        // log notes:
        // Editing codebase to allow calls passing and receiving structures
        // miktek 8.15.2011

        public static void Main(string[] args)
        {
            int iRet = 999;
                    
            // ErrorMEssage will be used to access error message
            // from the mt4 wrapper.
            String ErrorMessage = "none received";

            Console.WriteLine("testing 8.28.12");

            iRet = myManager.ManagerFactory();

            // hardwire the server name for short term.
            // put into a config.xml file for longer term sanity
    
            String ServerName = "mtdem01.primexm.com:443";     
            // connect to the server
            iRet = myManager.Connect(ServerName);

            // get errorMessage from the CMTManager
            ErrorMessage = myManager.ErrorDescription(iRet);        
            Console.WriteLine("Connect ErrMsg: {0}", ErrorMessage);

            // login in credentials. Don't dawdle here.
            // you have aprox. 15 seconds between connecting to the server
            // to enter your creds. Otherwise, say after 15 seconds, the server
            // drops your connection

            iRet = myManager.Login(900, "!FQS123!!");
            ErrorMessage = myManager.ErrorDescription(iRet);
            Console.WriteLine("Login ErrMsg: {0}", ErrorMessage);

            if (iRet != 0)
                return;

            int choice = 0;
            do
            {
                Console.WriteLine("\nEnter your choice(1,2,3,4,5) to:\n1. Add the User.\n2. Update the User."+
                    "\n3. History of closed trades.\n4. TradesRequest.\n5. Delete User.\n6. Exit.\n");
                string sChoice = Console.ReadLine();
                choice = int.Parse(sChoice);
                switch (choice)
                {
                    case 1:
                        UserRecordNET user = new UserRecordNET();
                        user.group = "FQ-IB-One";
                        Console.WriteLine("\nEnter Name:");
                        user.name = Console.ReadLine();
                        user.password = "NewUser";
                        iRet = myManager.UserRecordNew(user);
                        if (iRet == 0)
                        {
                            Console.WriteLine("\nUser Added Successfully.");
                            Console.WriteLine("User Login value: " + user.login);
                        }
                        else
                        {
                            Console.WriteLine("\nUser cannot added successfully.");
                            ErrorMessage = myManager.ErrorDescription(iRet);
                            Console.WriteLine("Login ErrMsg: {0}", ErrorMessage);
                        }

                        break;

                    case 2:
                        UserRecordNET existingUser = new UserRecordNET();
                        existingUser.group = "FQ-IB-One";
                        Console.WriteLine("\nEnter Login to update:");
                        string tempLogin = Console.ReadLine();
                        existingUser.login = int.Parse(tempLogin);
                        Console.WriteLine("\nEnter Name:");
                        existingUser.name = Console.ReadLine();
                        existingUser.password = "NewUser";
                        iRet = myManager.UserRecordUpdate(existingUser);
                       
                        if (iRet == 0)
                            Console.WriteLine("\nUser Modified Successfully.\n");
                        else
                        {
                            Console.WriteLine("\nUser cannot Modified successfully.\n");
                            ErrorMessage = myManager.ErrorDescription(iRet);
                            Console.WriteLine("ErrMsg: {0}", ErrorMessage);
                        }
                        break;

                    case 3:

                        TimeZone tZone = TimeZone.CurrentTimeZone;
                        DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        TimeSpan fromSpan = baseTime - startTime + tZone.GetUtcOffset(baseTime); 
                        DateTime endTime = new DateTime(2012, 12, 28, 0, 0, 0, 0);
                        TimeSpan toSpan = baseTime - endTime + tZone.GetUtcOffset(baseTime);
                        Int64 from = Convert.ToInt64(Math.Abs(fromSpan.TotalSeconds));
                        Int64 to = Convert.ToInt64(Math.Abs(toSpan.TotalSeconds));
                        Console.WriteLine("\nEnter Login:");
                        string strLogin = Console.ReadLine();
                        int value = 0;
                        TradeRecordNET[] TradeInfo;
                        TradeInfo = myManager.TradesUserHistory(int.Parse(strLogin), from, to, ref value);
                        
                        if (TradeInfo != null)
                            Console.WriteLine("Deals: " + TradeInfo[0].order + " Login: " + TradeInfo[0].login +
                                          " Type: " + TradeInfo[0].cmd + "\nSymbol: " + TradeInfo[0].symbol +
                                          " TradeInfo[0].conv_rates[1]: " + TradeInfo[0].conv_rates[1] +
                                          "Closed Price: "+TradeInfo[0].close_price);
                        else
                            Console.WriteLine("No trade Info.\n");

                        break;

                    case 4:
                        int val = 0;
                        TradeInfo = myManager.TradesRequest(ref val);
                        if (TradeInfo != null)
                            Console.WriteLine("Deals: " + TradeInfo[0].order + " Login: " + TradeInfo[0].login +
                                          " Type: " + TradeInfo[0].cmd + "\nSymbol: " + TradeInfo[0].symbol +
                                          " TradeInfo[0].conv_rates[1]: " + TradeInfo[0].conv_rates[1]+
                                          "\nTotal No. of Trades: "+val);

                        break;

                    case 5:
         
                        GroupCommandInfoNET info = new GroupCommandInfoNET();
                        info.command = (int)GroupOperation.GROUP_DELETE;
                        
                        //set the leverage accordingly..               
                        info.leverage = 0;
                        Console.WriteLine("Enter the number of login values to be entered:");
                        string strUserAccountsLength = Console.ReadLine();
                        int nUserAcctLength = int.Parse(strUserAccountsLength);
                        
                        //info.len consist the number of user accounts..
                        info.len = nUserAcctLength;

                        int[] logins = new int[nUserAcctLength];
                        Console.WriteLine();

                        for (int i = 0; i < nUserAcctLength; i++)
                        {
                            Console.Write(" "+ (i+1) +". Enter Login value: ");
                            logins[i] = int.Parse(Console.ReadLine());
                            Console.WriteLine();
                        }

                        iRet = myManager.UsersGroupOp(info, ref logins[0]);
                        ErrorMessage = myManager.ErrorDescription(iRet);
                        Console.WriteLine("User Delete ErrMsg: {0}", ErrorMessage);
                        break;

                    default:
                        break;
                }

            } while (choice > 0 && choice < 6);

            // declare a PumpFuncDelegate and then create an instance
            // of it. This will be used to start up the pumping asnycn
            // call back mechanism from the mtmanapi.dll, up through
            // the c++/cli wrapper to the C# client code here.

            MT4ManLibraryNETv03.PumpFuncDelegate myPumpFunc =
               new MT4ManLibraryNETv03.PumpFuncDelegate(miktekCSConAppTestHarness.CallBackMethod);

            // now we pass the delegate named myPumpFunc which acutally
            // is a function pointer the CallBackMethod declared 
            iRet = myManager.PassMeADelegate(myPumpFunc);

           
            // enable the pumping switch
            myManager.PumpingSwitch();

            
            ExposureValueNET ExposureInfo = new ExposureValueNET();
            SymbolSummaryNET SummaryInfo = new SymbolSummaryNET();

            int iCnt;

            // SummaryGet is passed a ref to a SymbolSummaryNET record
            // which will be populated by the SummaryGet call
            // and returned. miktek 9.10.12

            string symbol = "AUDCAD"; // FROM MTSampleAPI exec'ing.
            iCnt = myManager.SummaryGet(symbol, ref SummaryInfo); 
            
            SymbolSummaryNET[] SymbolSummaryArray;

            // pass in and return a array Symbol Summary records
            SymbolSummaryArray = myManager.SummaryGetAll(ref iCnt); // risk mgmt call #4

            int iResult = 0;
            String cur = "AUD ";
            const int maxchars = 256;

            iResult = myManager.SummaryCurrency(cur, maxchars);

            // for testing what value should this be?
            const int sectype = 10;

            iResult = myManager.SummaryGetByType(sectype, ref SummaryInfo);

            const int symbolType = 10;

            iResult = myManager.SummaryGetByCount(symbolType, ref SummaryInfo);

            iResult = myManager.ExposureValueGet(cur, ref ExposureInfo);

            ExposureValueNET[] ExposedArray; // miktek 9.9.12
           
            int iRecCount = 0;
            int iLoopCtr = 0;
            while (iLoopCtr < 4)
            {
                ExposedArray = myManager.ExposureGet(ref iRecCount);
                Console.WriteLine("ExposureGet attempt: {0} Recs Gotten: ", iLoopCtr, iRecCount);
                iLoopCtr++;
            }
           
            int iKey = 0;

            iKey = Console.Read();
            myManager.Disconnect();
         
            Console.WriteLine("End of test run 9.10.12");

            iKey = Console.Read();     
        }  
    } // end class
} // end namespace
