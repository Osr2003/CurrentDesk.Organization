// MT4ManLibraryNETv03.h
// build and exec status:
// 8.26.2012 COMPILE, RUNS RETURNS SummaryGetAll as a * ptr but not ready .NET
// 8.28.2012 Version to DropBox
// miktek
#pragma once

#include "stdafx.h"
#include <WinSock2.h>
#include <Windows.h>
#include <winsock.h>
#include "../mtlibs/MT4ManagerAPI.h"
#include <msclr\marshal.h>
#using <mscorlib.dll>

using namespace System;
using namespace msclr::interop;

namespace MT4ManLibraryNETv03 {

	// added 8.23.12 miktek
	// compile status
	// broke the build here
	/*public value struct ExposureValue
        {
           //char currency[4]; 
		   String ^ strCurr;
           double   clients;          // clients volume
           double   coverage;         // coverage volume
        };
	*/
	// trying to recover from brreaking things 

	/*struct ExposureValue
	  {
	   char              currency[4];         // currency
	   double            clients;             // clients volume
	   double            coverage;            // coverage volume
	  };
	  */
	// changed this from a value to ref for use in making arrays
	// of ExposureValueNET miktek 9.9.12
	//
	public ref struct ExposureValueNET
	  {
	   // String currency how to have an array
	   String^	strCurrencyType;
	   double	clients;             // clients volume
	   double   coverage;            // coverage volume
	  };
	
	public ref struct SymbolSummaryNET
	{
	   String ^          strSymbol;          // symbol
	   int               count;               // symbol counter
	   int               digits;              // floating point digits
	   int               type;                // symbol type (symbol group index)
	   //--- clients summary
	   int               orders;              // number of client orders
	   Int64             buylots;             // buy volume
	   Int64             selllots;            // sell volume
	   double            buyprice;            // average buy price
	   double            sellprice;           // average sell price
	   double            profit;              // clients profit
	   //--- coverage summary
	   int               covorders;           // number of coverage orders
	   long           covbuylots;          // buy volume
	   long           covselllots;         // sell volume
	   double            covbuyprice;         // average buy price
	   double            covsellprice;        // average sell price
	   double            covprofit;           // coverage profit
	  };

	//Wrapper of UserRecord structure.
	public ref struct UserRecordNET
	{
		//--- common settings
		int               login;                  // login
		String ^          group;                  // length should be less than 16, see declaration UserRecord.group
		String ^          password;               // length should be less than 16, see declaration UserRecord.password
		
		//--- access flags
		int               enable;                 // enable
		int               enable_change_password; // allow to change password
		int               enable_read_only;       // allow to open/positions (TRUE-may not trade)
		array<int> ^      enable_reserved;        // for future use
		
		//--
		String ^          password_investor;      // read-only mode password, length should be less than 16, see declaration UserRecord.password_investor
		String ^          password_phone;         // phone password, length should be less than 32, see declaration UserRecord.password_phone
		String ^          name;                   // length should be less than 128, see declaration UserRecord.name
		String ^          country;				  // length should be less than 32, see declaration UserRecord.country
		String ^          city;                   // length should be less than 32, see declaration UserRecord.city
		String ^          state;                  // length should be less than 32, see declaration UserRecord.state
		String ^          zipcode;                // length should be less than 16, see declaration UserRecord.zipcode
		String ^          address;                // length should be less than 128, see declaration UserRecord.address
		String ^          phone;                  // length should be less than 32, see declaration UserRecord.phone
		String ^          email;                  // length should be less than 48, see declaration UserRecord.email
		String ^          comment;                // length should be less than 64, see declaration UserRecord.comment
		String ^          id;                     // SSN (IRD), length should be less than 32, see declaration UserRecord.id
		String ^          status;                 // length should be less than 16, see declaration UserRecord.status
		Int64			  regdate;                // registration date
		Int64			  lastdate;               // last connection time
		
		//--- trade settings
		int               leverage;               // leverage
		int               agent_account;          // agent account
		Int64			  timestamp;              // timestamp
		array<int> ^      reserved;				  // for future use
		double            balance;                // balance
		double            prevmonthbalance;       // previous month balance
		double            prevbalance;            // previous day balance
		double            credit;                 // credit
		double            interestrate;           // accumulated interest rate
		double            taxes;                  // taxes
		double            prevmonthequity;        // previous month equity
		double            prevequity;             // previous day equity
		array<double> ^   reserved2;              // for future use

		//---
		String ^          publickey;			  // public key, length should be less than 272, see declaration UserRecord.publickey
		int               send_reports;           // enable send reports by email
		int               balance_status;         // internal use
		COLORREF          user_color;             // color got to client (used by MT Manager)
		
		//---
		String ^          unused;				  // for future use, length should be less than 40, see declaration UserRecord.unused
		String ^          api_data;				  // for API usage, length should be less than 16, see declaration UserRecord.api_data
		
		//Initializing default values.
		UserRecordNET() : group(""), password(""), name(""), country(""), city(""), 
						  state(""), zipcode(""), address(""), phone(""), email(""), 
						  comment(""), id(""), status(""), publickey(""), unused(""), 
						  api_data(""), password_investor(""), password_phone("")
		{
			enable_reserved = gcnew array<int>(3);		//see declaration UserRecord.enable_reserved
			reserved		= gcnew array<int>(1);		//see declaration UserRecord.reserved
			reserved2		= gcnew array<double>(2);	//see declaration UserRecord.reserved2
			user_color		= USER_COLOR_NONE;
			send_reports	= 1;
		}
	};

	//Wrapper of TradeRecord structure.
	public ref struct TradeRecordNET
	{
		int					order;				// order ticket
		int					login;				// owner's login
		String ^			symbol;				// security
		int					digits;				// security precision
		int					cmd;				// trade command
		int					volume;				// volume

		//---
		Int64				open_time;			// open time
		int					state;				// reserved
		double				open_price;			// open price
		double				sl,tp;				// stop loss & take profit
		Int64				close_time;			// close time
		Int64				value_date;			// value date
		Int64				expiration;			// pending order's expiration time
		int					conv_reserv;		// reserved
		array<double> ^		conv_rates;			// convertation rates from profit currency to group deposit currency

		//first element-for open time, second element-for close time
		double				commission;			// commission
		double				commission_agent;	// agent commission
		double				storage;			// order swaps
		double				close_price;		// close price
		double				profit;				// profit
		double				taxes;				// taxes
		int					magic;				// special value used by client experts
		String ^			comment;			// comment
		int					internal_id;		// for internal usage (must be used by API)
		int					activation;			// used by MT Manager
		int					spread;				// spread
		double				margin_rate;		// margin convertation rate (rate of convertation from margin currency to deposit one)
		Int64				timestamp;			// timestamp
		array<int> ^		reserved;			// reserved
		TradeRecordNET ^ 	next;				// internal data

		//--
		TradeRecordNET()
		{
			conv_rates = gcnew array<double>(2);
			reserved = gcnew array<int>(4);
		}
	};

	//Users Group Operation.
	public ref struct GroupCommandInfoNET
	{
		int				len;		// length of users list
		int				command;	// group command
		String ^		newgroup;	// new group
		int				leverage;	// new leverage
		array <int> ^	reserved;	// reserved

		//--
		GroupCommandInfoNET()
		{
			newgroup = "";
			reserved = gcnew array<int>(8);
		}
	};

	public delegate void PumpFuncDelegate(int);
		// should be protected but later 7.27.12
		
	public ref class CMTManager
	{
		protected:
			int WinsockStartup();
			void WinsockCleanup();
			void PumpingFunc(int code);

		public: 
			
			// this should not be public
			// not should any ot the other function typedef
			// etc miktek 7.21.12
			// add typedef. will it compile 7.28.12
			// step 3a compiles
			typedef int (__stdcall *PASSEDPUMPFUNC)(int);
			CManagerInterface * m_manager;

		protected:
			String ^mtmanapiPath;
			int PumpCalls;
			PumpFuncDelegate^ keeperCallBack;

		public :
			// added 8.3.2012
			int PassMeADelegate(PumpFuncDelegate^ pumpFunc);
			int TestCallBack();
			int InitDLLPath(String^ dllPath);
			int ManagerFactory();

			int CreateMTManagerInstance();

			// pass an instance of a pumping func
			// delegate from he client to he wrapper
			// before calling the PumpingSwith
			// CMTManager exposes a delegate
			// void PumpingFunc(int code);

			int PumpingSwitch();

			String ^ ErrorDescription(int code);

			int Connect(String^ ServerName);
			int Login(int user, String^ password);
			
			int IsConnected();
			int Disconnect();
			int Ping();
			
			// dummy testing functions
			int dummyFunction(int code);
			int dummyPumpingMessages(int code);
			//int dummyPassClass(iVal);

			// risk-related all 7 so far miktek 8.16.2011

			// MODIFY ExposureGet to return a ptr to a structure 
			// then return a ref to a struct

			array<ExposureValueNET^>^ ExposureGet(int% total); // 8.26.12 working so so	
			int ExposureValueGet(String ^ cur, ExposureValueNET^% info);
			
			int SummaryCurrency(String ^ cur, int maxchars);

			array<SymbolSummaryNET^>^ SummaryGetAll(int% code);

			int SummaryGet(String ^ symbol, SymbolSummaryNET^% info);

			int SummaryGetByCount(const int symbol, SymbolSummaryNET^% info);
			int SummaryGetByType(const int sectype, SymbolSummaryNET^% info);

			//User's APIs
			int UserRecordNew(UserRecordNET ^ user);
			int UserRecordUpdate(const UserRecordNET ^ user);
			int UsersGroupOp(const GroupCommandInfoNET ^info, int % logins);

			//Trades related APIs
			array<TradeRecordNET^>^ TradesUserHistory(const int login, const Int64 from, const Int64 to, int% total);
			array<TradeRecordNET^>^ TradesRequest(int% total);
			array<TradeRecordNET^>^ TradesGet(int% total);
			
			//Utilities
			int CopyToSymbolSummaryNET(SymbolSummaryNET^% info, SymbolSummary * symbolSummary);
			int CopyToUserRecord(const UserRecordNET^ info, UserRecord* userRec);
			int CopyToTradeRecordNET(TradeRecordNET^% info, TradeRecord *tradeRec);
			int CopyToUserRecordNET(const UserRecord* userRec, UserRecordNET^ info);
	};
}
