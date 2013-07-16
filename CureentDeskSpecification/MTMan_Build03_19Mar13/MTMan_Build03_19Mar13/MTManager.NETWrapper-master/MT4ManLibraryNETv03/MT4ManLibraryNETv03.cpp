// This is the main DLL file.
// 8.24.2012 Sat
// build and exec status:
// 8.26.2012 COMPILE, RUNS RETURNS SummaryGetAll as a * ptr but not ready .NET
// MT4ManLibraryNETV03KEEPER MOSTCURRENT
// changed path to open mtmanapi.dll to use same path as that
// to which pointers to the MT4ManagerAPI.h. See line 12 next;
// 8.28.2012
// miktek
#include "stdafx.h"
#include <WinSock2.h>
#include <winsock.h>
#include "../mtlibs/MT4ManagerAPI.h"
#include <Windows.h>

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace System::Net::Sockets;

#using <mscorlib.dll>
#pragma comment(lib, "ws2_32.lib")

// NOTE: Result codes returned by m_manager are defined
// in an unnamed enum with the comment "Result codes" on about line 629 
// of the MT4ManagerAPI.h include file
// of interest:
// RET_OK = 0
// RET_OK_NONE 1
// RET_NO_CONNECT 6

// NOTE: Pumping mode flags are also fined in the MT4ManagerAPI.h
// about line 678. Can be bitmapped

// Currence exposure line 1183 is where to look for struct
// ExposureValue

// NOTE Calls to implement for Risk Management
//--- risk managementdeclared in MT4ManagerAPI.h
/***************

virtual SymbolSummary* __stdcall SummaryGetAll(int *total)                            =0; CHK
virtual int          __stdcall SummaryGet(LPCSTR symbol,SymbolSummary *info)          =0; CHK
virtual int          __stdcall SummaryGetByCount(const int symbol,SymbolSummary *info)=0; CHK
virtual int          __stdcall SummaryGetByType(const int sectype,SymbolSummary *info)=0; CHK
virtual int          __stdcall SummaryCurrency(LPSTR cur,const int maxchars)          =0; CHK
virtual ExposureValue* __stdcall ExposureGet(int *total)                              =0; CHK
virtual int          __stdcall ExposureValueGet(LPCSTR cur,ExposureValue *info)       =0; CHK

***********/

#include "MT4ManLibraryNETv03.h"

namespace MT4ManLibraryNETv03
{

	int CMTManager::PassMeADelegate(PumpFuncDelegate^ pumpFunc)
	{
		// create an instance of the PumpFuncDelegate
		// then put pumpFunc as the method to call
		keeperCallBack = pumpFunc;

		int iRet = 0;

		// The following 2 functions are for testing purposes only to verify that the delegate works
		// use the TestCallBack method that follows for this method e.g. TestCallBack();
		// miktek 9.9.2012 
		// pumpFunc(666);
		//keeperCallBack(777);

		return 12;
	}

	int CMTManager::TestCallBack() {

		keeperCallBack(777);
		return 1;
	}
	/// <summary>
	///
	/// </summary>
	/// <param name="dllPath"</param>

	int CMTManager::InitDLLPath(String^ dllPath)
	{ 
		// set protected DLL path to the
		// the path passed in.
		// miktek 7.24.12
		return RET_OK;
	}

	// we coulnd rename this ManagerFactory
	// 7.24.12 miktek
	int CMTManager::ManagerFactory()
	{ 
		int iRet;
		iRet = WinsockStartup();

		iRet = CreateMTManagerInstance();

		// NOTE this will move to a user enabled call
		// now for testing purpose we do it here

		return iRet; 
	};

	int CMTManager::IsConnected()
	{
		// TEST: determine return code returned
		// when we are not connected
		// the pass that to ErrorDescription

		if (m_manager == NULL) 
			return 0;

		int iRet = 0;
		// set up a const for CONNECTED NOT_CONNECTED TODO
		iRet = m_manager->IsConnected();

		return iRet;

	}
	// best on String^ to char* and char* to String^
	//http://gregs-blog.com/2008/01/30/part-1-how-to-make-native-calls-from-managed-code-in-ccli/

	String ^ CMTManager::ErrorDescription(const int code)
	{
		String ^ str = "ErrorDescription";

		// convert the String^ to an LPCSTR
		//
		LPCSTR lpcstrErrDesc =
			m_manager->ErrorDescription(code);

		// keep for mem
		// convert a String^ to a LPCSCTR
		// move this code to loing
		//LPCSTR lpcstrStr1 =
		//	(char*)Marshal::StringToHGlobalAnsi(str).ToPointer();

		// Convert a LPCSTR To a String^
		// when and if to call the following
		// Marshal::FreeHGlobal((IntPtr)str);

		// must pass in a manager pointer instead of a native pointer
		// but the IntPtr object has a constructor that will do this
		// conversion automagically.

		// how to convert the LPCSTR to a String^
		str = Marshal::PtrToStringAnsi(IntPtr((char *)lpcstrErrDesc));

		return str;

	}
	// pass in a function pointer of type MTAPI_NOTIFY_FUNC
	int CMTManager::dummyFunction(int code)
	{
		return (code);

	}

	int CMTManager::dummyPumpingMessages(int code)
	{
		return 1212;

	}
	// pass in a function pointer of type MTAPI_NOTIFY_FUNC
	void CMTManager::PumpingFunc(int code)
	{
		keeperCallBack(code);

	}

	// pass in a delegate from where?
	// pass in an event and an event handler
	int CMTManager::PumpingSwitch()
	{
		int iRet = 0;

		if (m_manager == NULL) 
			return 0;

		// 7.28 this compiles
		// step 1 COMPILES create the delegate
		PumpFuncDelegate^ pfDelegate = 
			gcnew PumpFuncDelegate(this, &CMTManager::PumpingFunc);

		// step 1a compiles lock it down
		GCHandle gch = GCHandle::Alloc(pfDelegate);

		// step 2 COMPILES ..get a func pointer for the delegate
		IntPtr pumpFuncIntPtr = Marshal::GetFunctionPointerForDelegate(pfDelegate);

		// step 3 COMPILES // now convert to the IntPtr to a pointer to a func
		// move this from a stack pass func to a instance based func
		// phase 2. 

		// pumpFunker will hold  the addr of the ToPointer
		// pumpFunker is stack based. Do I want to create an unmanaged instance?

		MTAPI_NOTIFY_FUNC pumpFunker = 
			static_cast<MTAPI_NOTIFY_FUNC>(pumpFuncIntPtr.ToPointer());

		// pumpFuncker now contains a func ptr of type MTAPI_NOTIFY_FUNC
		// which should be the address of CMTManager::PumpingFunc
		// step 4 COMPILES 7.28.12
		iRet = m_manager->PumpingSwitch((MTAPI_NOTIFY_FUNC)pumpFunker,
			NULL, 0, 0);

		return iRet;

	}

	int CMTManager::Ping()
	{
		int iRet = 765;
		if (m_manager == NULL) 
			return 0;

		iRet = m_manager->Ping();

		return iRet;

	}
	int CMTManager::Disconnect()
	{
		int iRet = 0;
		// throw an error here

		if (m_manager == NULL) 
			return 0;

		iRet = m_manager->Disconnect();

		return iRet;

	}

	int CMTManager::Login(int user, String ^pwd)
	{
		// HOW TO marshal String to Ansi and back again
		// http://gregs-blog.com/2008/01/30/part-1-how-to-make-native-calls-from-managed-code-in-ccli/
		int iRet = 0;
		// need much better error handling here
		// look in api.h handler. TODO miktek 8.12.12
		if (m_manager == NULL) 
			return 0;

		// verify user is numeric and within range
		// 
		// if we are not connected don't try to login
		// 
		iRet = m_manager->IsConnected();

		String^ str = pwd;

		LPCSTR lpscstrPwd =
			(char*)Marshal::StringToHGlobalAnsi(pwd).ToPointer();

		iRet = m_manager->Login(user, lpscstrPwd);

		return iRet;

	}
	int CMTManager::Connect(String^ ServerName)
	{
		// convert a String^ to lpstr
		int iRet = 999;


		// since it is passed in c++/cli as a tracking handle
		// it needs to converted to a LPCSTR char * type c++ string
		char * lpcServerName =
			(char *) Marshal::StringToHGlobalAnsi(ServerName).ToPointer();

		iRet = m_manager->Connect(lpcServerName);

		return iRet;
	}

	// Initialize Winsock startup. All pumping and connections
	// required Winsock support.
	int CMTManager::WinsockStartup()
	{
		WSADATA wsa;
		return(WSAStartup(0x0202,&wsa)!=0 ? RET_ERROR:RET_OK);
	}

	// Winsock resources are cleaned up explicitly here.
	// When the dll shut down all related winsock resources
	// including connections will be released.

	void CMTManager::WinsockCleanup()
	{
		WSACleanup();
	}
	// creates and instance of ManagerInterface using a function
	// call retrieved from the mtmanapi.dll 
	// 
	int CMTManager::CreateMTManagerInstance()
	{
		// error handling is in order 
		// consider test cases miktek 7.21.12

		//HMODULE hMod;
		HMODULE m_lib;

		typedef int (*MtManVersion_t)(void);
		MtManVersion_t    m_pfnManVersion;

		MtManCreate_t     m_pfnManCreate;

#ifdef	_WIN64	
		m_lib = ::LoadLibrary(L"mtmanapi64.dll");
#else
		m_lib = ::LoadLibrary(L"mtmanapi.dll");
#endif

		m_pfnManVersion=reinterpret_cast<MtManVersion_t>(::GetProcAddress(m_lib,"MtManVersion"));

		int version = 0;
		version = (*m_pfnManVersion)();

		m_pfnManCreate =reinterpret_cast<MtManCreate_t>(::GetProcAddress(m_lib,"MtManCreate"));

		// I want to reference the CManagerInterface declared public in the CMTManager
		CManagerInterface * manager;
		//manager = NULL;
		m_manager = NULL;

		if(m_pfnManCreate) {
			(*m_pfnManCreate)(version, &manager);
			//(*m_pfnManCreate)(version, &m_manager);
		}

		m_manager = manager;
		int iRet = 999;
		iRet = m_manager->IsConnected();

		return iRet;	
	}

	// RISK MANAGEMENT CALLS RMC begun ---------------------------------- 



	// exposureValue get is passed a currency string and a pointer to
	// a exposure value. ExposureValue struct 
	int CMTManager::ExposureValueGet(String ^ cur, ExposureValueNET^% info) //1
	{
		// lpcstr conversion goes here
		int iRet = 0;
		// TESTING PURPOSES ONLY String^ wiredCur = "AUD ";

		LPCSTR lpcstrCurr =
			(char*)Marshal::StringToHGlobalAnsi(cur).ToPointer();

		ExposureValue * myExposureValue = new ExposureValue();
		iRet = m_manager->ExposureValueGet(lpcstrCurr, myExposureValue);

		String ^ currencyType = gcnew String(myExposureValue->currency);

		info->strCurrencyType = currencyType;
		info->coverage = myExposureValue->coverage;
		info->clients = myExposureValue->clients;

		return iRet;
	}

	// this method sets the currency to get summarys about???
	// unverified but a guess.
	// works ok but how to test? miktek 9.9.12

	int CMTManager::SummaryCurrency(String^ cur,int maxchars) //1
	{
		// convert cur to lpcstr
		LPSTR lpcstrCurr =
			(char*)Marshal::StringToHGlobalAnsi(cur).ToPointer();

		int iResult = 0;

		maxchars = 4; // what value should this be?
		iResult = m_manager->SummaryCurrency(lpcstrCurr, maxchars);

		// convert LPCSTR BACK TO STRING?? do we??? unverified 9.10.12
		return 0;
	}

	int CMTManager::SummaryGetByCount(const int symbol, SymbolSummaryNET^% info)
	{
		int iRet = 0;

		SymbolSummaryNET ^ mySymbolSummaryNET = gcnew SymbolSummaryNET();

		SymbolSummary * mySymbolSummary = new SymbolSummary();

		iRet = m_manager->SummaryGetByCount(symbol, mySymbolSummary);

		// move contents from mySymbolSummary to info
		iRet = CopyToSymbolSummaryNET(info, mySymbolSummary);

		return iRet;

	}

	int CMTManager::SummaryGetByType(const int sectype,SymbolSummaryNET^% info)
	{
		int iRet = 0;

		SymbolSummary * mySymbolSummary = new SymbolSummary();

		iRet = m_manager->SummaryGetByType(sectype, mySymbolSummary);

		iRet = CopyToSymbolSummaryNET(info, mySymbolSummary);

		return iRet;
	}

	// status begun 9.9.12 miktek
	// question is what exactly is the symbol to be set to?

	int CMTManager::SummaryGet(String ^ symbol, SymbolSummaryNET^% info)
	{
		// convert to lpcstr
		// convert cur to lpcstr
		LPSTR lpSymbol =
			(char*)Marshal::StringToHGlobalAnsi(symbol).ToPointer();

		int iRet = 0;
		SymbolSummary * mySymbolSummary = new SymbolSummary();
		// AUDCAD gets it done 9.9.12
		iRet = m_manager->SummaryGet(lpSymbol, mySymbolSummary);

		// verification needed for this call 9.10.12 miktek
		iRet = CopyToSymbolSummaryNET(info, mySymbolSummary);

		return RET_OK;
	}

	int CMTManager::CopyToSymbolSummaryNET(SymbolSummaryNET^% info, SymbolSummary * symbolSummary)
	{
		// move copy contents of mySymbolSummary to SymbolSummaryNET
		// how to deref both c++ struct and CLR struct miktek 9.9.12

		String ^ strSymbol = gcnew String(symbolSummary->symbol);
		info->strSymbol = strSymbol;

		info->buylots = symbolSummary->buylots; // __int64 to Int64
		info->count = symbolSummary->count;
		info->digits = symbolSummary->digits;
		info->type = symbolSummary->type;

		info->orders = symbolSummary->orders;
		info->selllots = symbolSummary->selllots;
		info->buyprice = symbolSummary->buyprice;
		info->sellprice = symbolSummary->sellprice;

		info->profit = symbolSummary->profit;
		info->covorders = symbolSummary->covorders;

		info->covbuylots = symbolSummary->covbuylots;
		info->covbuyprice = symbolSummary->covbuyprice;
		info->covselllots = symbolSummary->covselllots;
		info->covsellprice = symbolSummary->covsellprice;
		info->covprofit = symbolSummary->covprofit;

		return RET_OK;
	}

	// have this return an array of SymbolSummaryNet records
	// array<SymbolSummaryNET^>^ 

	array<SymbolSummaryNET^>^ CMTManager::SummaryGetAll(int% total) 
	{
		int localTotal = 0;

		SymbolSummary * mySymbolSummary = new SymbolSummary();

		mySymbolSummary = m_manager->SummaryGetAll(&localTotal);

		array<SymbolSummaryNET^>^ SymbolSummaryArray;

		if (localTotal > 0) {

			SymbolSummaryArray =
				gcnew array<SymbolSummaryNET^>(localTotal);

			SymbolSummary * symbolSummaryTester;

			int iRet = 0;

			for (int iIndex = 0; iIndex <localTotal; iIndex++)
			{
				symbolSummaryTester = &mySymbolSummary[iIndex];
				SymbolSummaryArray[iIndex] = gcnew SymbolSummaryNET;

				// would this work here? TODO VERIFY THIS CALL - VERIFIED
				// passing 2 address ptrs on the stack to a function is cheaper and easier
				// to maintain than having at least four separate places where 
				// these two data structures  are copied. miktek 9.10.12

				iRet = CopyToSymbolSummaryNET(SymbolSummaryArray[iIndex], symbolSummaryTester);

			}
		}

		return SymbolSummaryArray;
	}

	// miktek Exposure get passes by references
	// and returns the total number of exposure records to read
	// note: this call is activated by Pumping Switch
	// miktek 8.12.2012
	array<ExposureValueNET^>^ CMTManager::ExposureGet(int% total)
	{
		// if I already have one of these I need to free it so
		// not as to overwhelm the heap miktek 8.26.2012
		// TODO if myExposure != NULL free myExposureValue 
		// please do so as this will fix a memory leak.
		// miktek
		int localTotal = 0;

		ExposureValue * myExposureVal = new ExposureValue();

		// if call is successful then localTotal will return the number of 
		// array elements returned in the array pointer myExposureValue
		// miktek 
		myExposureVal = m_manager->ExposureGet(&localTotal);

		// set return value. This code could be refactored but not now 9.10.12
		total  = localTotal;

		array<ExposureValueNET^>^ ExposedArray;

		if (localTotal > 0) {

			ExposedArray =
				gcnew array<ExposureValueNET ^>(localTotal);

			ExposureValue * exposureTester;

			for (int iIndex = 0; iIndex <localTotal;iIndex++) 
			{
				exposureTester = &myExposureVal[iIndex];

				ExposedArray[iIndex] = gcnew ExposureValueNET;

				String ^ currencyType = gcnew String(exposureTester->currency);

				ExposedArray[iIndex]->strCurrencyType = currencyType;
				ExposedArray[iIndex]->coverage = exposureTester->coverage;
				ExposedArray[iIndex]->clients = exposureTester->clients;
			}
		}

		return ExposedArray;
	}

	//UserRecordNET (Structure which contains the details of the User. It is wrapper of UserRecord structure).
	//Add the new user record in database.
	//If successful, return 0.
	int CMTManager::UserRecordNew(UserRecordNET ^ user)
	{
		int iRet = -1;

		UserRecord *nativeUser = new UserRecord();
		iRet = CopyToUserRecord(user, nativeUser);
		iRet = m_manager->UserRecordNew(nativeUser);
		if(iRet == RET_OK)
			iRet = CopyToUserRecordNET(nativeUser, user);

		delete nativeUser;
		nativeUser = NULL;
		return iRet;
	}

	//Update the user's record.
	//If successful, return 0.
	int CMTManager::UserRecordUpdate(const UserRecordNET ^ user)
	{
		int iRet = -1;

		UserRecord *nativeUser = new UserRecord();
		iRet = CopyToUserRecord(user, nativeUser);
		iRet = m_manager->UserRecordUpdate(nativeUser);

		delete nativeUser;
		nativeUser = NULL;
		return iRet;
	}

	//Values of UserRecordNET's data members is copied to UserRecord's data members.
	//If an exception occurs of "Buffer is to small". It is because size of UserRecordNET's
	//data member exceeds the size of UserRecord's data member while performing strcpy_s function.
	int CMTManager::CopyToUserRecord(const UserRecordNET ^ info, UserRecord *userRec)
	{
		marshal_context ^ context		= gcnew marshal_context();
		userRec->login					= info->login;
		userRec->enable					= info->enable;
		userRec->enable_change_password	= info->enable_change_password;
		userRec->enable_read_only		= info->enable_read_only;
		userRec->regdate				= info->regdate;
		userRec->lastdate				= info->lastdate;
		userRec->leverage				= info->leverage;
		userRec->agent_account			= info->agent_account;
		userRec->timestamp				= info->timestamp;
		userRec->balance				= info->balance;
		userRec->prevmonthbalance		= info->prevmonthbalance;
		userRec->prevbalance			= info->prevbalance;
		userRec->credit					= info->credit;
		userRec->interestrate			= info->interestrate;
		userRec->taxes					= info->taxes;
		userRec->prevmonthequity		= info->prevmonthequity;
		userRec->prevequity				= info->prevequity;
		userRec->send_reports			= info->send_reports;
		userRec->balance_status			= info->balance_status;
		userRec->user_color				= info->user_color;

		//Use http://msdn.microsoft.com/en-us/library/bb384856.aspx for conversion of String ^ into const char *.
		strcpy_s(userRec->group				, context->marshal_as<const char*>(info->group));
		strcpy_s(userRec->password			, context->marshal_as<const char*>(info->password));
		strcpy_s(userRec->password_investor	, context->marshal_as<const char*>(info->password_investor));
		strcpy_s(userRec->password_phone	, context->marshal_as<const char*>(info->password_phone));
		strcpy_s(userRec->name				, context->marshal_as<const char*>(info->name));
		strcpy_s(userRec->country			, context->marshal_as<const char*>(info->country));
		strcpy_s(userRec->city				, context->marshal_as<const char*>(info->city));
		strcpy_s(userRec->state				, context->marshal_as<const char*>(info->state));
		strcpy_s(userRec->zipcode			, context->marshal_as<const char*>(info->zipcode));
		strcpy_s(userRec->address			, context->marshal_as<const char*>(info->address));
		strcpy_s(userRec->phone				, context->marshal_as<const char*>(info->phone));
		strcpy_s(userRec->email				, context->marshal_as<const char*>(info->email));
		strcpy_s(userRec->comment			, context->marshal_as<const char*>(info->comment));
		strcpy_s(userRec->id				, context->marshal_as<const char*>(info->id));
		strcpy_s(userRec->status			, context->marshal_as<const char*>(info->status));
		strcpy_s(userRec->publickey			, context->marshal_as<const char*>(info->publickey));
		strcpy_s(userRec->unused			, context->marshal_as<const char*>(info->unused));
		strcpy_s(userRec->api_data			, context->marshal_as<const char*>(info->api_data));

		int nDataSize = 3; // array length of userRec->enable_reserved
		for(int i = 0; i < nDataSize; i++)
			userRec->enable_reserved[i] = info->enable_reserved[i];

		nDataSize = 1; //array length of userRec->reserved.
		for(int i = 0; i < nDataSize; i++)
			userRec->reserved[i] = info->reserved[i];

		nDataSize = 2; //array length of userRec->reserved2.
		for(int i = 0; i < nDataSize; i++)
			userRec->reserved2[i] = info->reserved2[i];

		delete context;

		return RET_OK;
	}

	//Values of UserRecord's data members is copied to UserRecordNET's data members.
	int CMTManager::CopyToUserRecordNET(const UserRecord *nativeUser, UserRecordNET ^ user)
	{
		user->login						= nativeUser->login;
		user->enable					= nativeUser->enable;
		user->enable_change_password	= nativeUser->enable_change_password;
		user->enable_read_only			= nativeUser->enable_read_only;
		user->regdate					= nativeUser->regdate;
		user->lastdate					= nativeUser->lastdate;
		user->leverage					= nativeUser->leverage;
		user->agent_account				= nativeUser->agent_account;
		user->timestamp					= nativeUser->timestamp;
		user->balance					= nativeUser->balance;
		user->prevmonthbalance			= nativeUser->prevmonthbalance;
		user->prevbalance				= nativeUser->prevbalance;
		user->credit					= nativeUser->credit;
		user->interestrate				= nativeUser->interestrate;
		user->taxes						= nativeUser->taxes;
		user->prevmonthequity			= nativeUser->prevmonthequity;
		user->prevequity				= nativeUser->prevequity;
		user->send_reports				= nativeUser->send_reports;
		user->balance_status			= nativeUser->balance_status;
		user->user_color				= nativeUser->user_color;

		//Conversion of char array into String.
		user->group						= gcnew String(nativeUser->group);
		user->password					= gcnew String(nativeUser->password);
		user->password_investor			= gcnew String(nativeUser->password_investor);
		user->password_phone			= gcnew String(nativeUser->password_phone);
		user->name						= gcnew String(nativeUser->name);
		user->country					= gcnew String(nativeUser->country);
		user->city						= gcnew String(nativeUser->city);
		user->state						= gcnew String(nativeUser->state);
		user->zipcode					= gcnew String(nativeUser->zipcode);
		user->address					= gcnew String(nativeUser->address);
		user->phone						= gcnew String(nativeUser->phone);
		user->email						= gcnew String(nativeUser->email);
		user->comment					= gcnew String(nativeUser->comment);
		user->id						= gcnew String(nativeUser->id);
		user->status					= gcnew String(nativeUser->status);
		user->publickey					= gcnew String(nativeUser->publickey);
		user->unused					= gcnew String(nativeUser->unused);
		user->api_data					= gcnew String(nativeUser->api_data);

		int nDataSize = 3; // array length of nativeUser->enable_reserved
		for(int i = 0; i < nDataSize; i++)
			user->enable_reserved[i] = nativeUser->enable_reserved[i];

		nDataSize = 1; //array length of nativeUser->reserved.
		for(int i = 0; i < nDataSize; i++)
			user->reserved[i] = nativeUser->reserved[i];

		nDataSize = 2; //array length of nativeUser->reserved2.
		for(int i = 0; i < nDataSize; i++)
			user->reserved2[i] = nativeUser->reserved2[i];

		return RET_OK;
	}

	//TradesUserHistory returns the list of closed Trade Records of a particular user. 
	//Trade records are fetched out on the basis of login of the user.
	array<TradeRecordNET^>^ CMTManager::TradesUserHistory(const int login,const Int64 from,const Int64 to,int% total)
	{
		array<TradeRecordNET^>^ TradeRecordArray;
		int iRet = -1;
		int localTotal = 0;

		//Number of records is passed into the localTotal.
		TradeRecord *tradeRec;
		tradeRec = m_manager->TradesUserHistory(login, from, to, &localTotal);
		//store the total in case, needed in future.
		total = localTotal;

		//copy the TradeRecord object values into the TradeRecordNET.
		if (localTotal > 0)
		{
			TradeRecordArray =	gcnew array<TradeRecordNET ^>(localTotal);
			TradeRecord * tradeTester;
			for (int iIndex = 0; iIndex < localTotal; iIndex++)
			{
				tradeTester = &tradeRec[iIndex];
				TradeRecordArray[iIndex] = gcnew TradeRecordNET;
				iRet = CopyToTradeRecordNET(TradeRecordArray[iIndex], tradeTester);
			}
		}

		return TradeRecordArray;
	}

	//TradesRequest returns the trades that have already been closed, 
	//and have a close price and a close time.
	array<TradeRecordNET^>^ CMTManager::TradesRequest(int% total)
	{
		array<TradeRecordNET^>^ TradeRecordArray;
		int iRet = -1;
		int nLocalTotal = 0;

		TradeRecord *tradeRec;
		tradeRec = m_manager->TradesRequest(&nLocalTotal);
		total = nLocalTotal;

		if(nLocalTotal > 0)
		{
			TradeRecordArray =	gcnew array<TradeRecordNET ^>(nLocalTotal);
			TradeRecord * tradeTester;
			for (int iIndex = 0; iIndex < nLocalTotal; iIndex++)
			{
				tradeTester = &tradeRec[iIndex];
				TradeRecordArray[iIndex] = gcnew TradeRecordNET;
				iRet = CopyToTradeRecordNET(TradeRecordArray[iIndex], tradeTester);
			}
		}

		return TradeRecordArray;
	}

	//TradesGet returns the list of open trades.
	array<TradeRecordNET^>^ CMTManager::TradesGet(int% total)
	{
		array<TradeRecordNET^>^ TradeRecordArray;
		int iRet = -1;
		int nLocalTotal = 0;

		TradeRecord *tradeRec;
		tradeRec = m_manager->TradesGet(&nLocalTotal);
		total = nLocalTotal;

		if(nLocalTotal > 0)
		{
			TradeRecordArray =	gcnew array<TradeRecordNET ^>(nLocalTotal);
			TradeRecord * tradeTester;
			for (int iIndex = 0; iIndex < nLocalTotal; iIndex++)
			{
				tradeTester = &tradeRec[iIndex];
				TradeRecordArray[iIndex] = gcnew TradeRecordNET;
				iRet = CopyToTradeRecordNET(TradeRecordArray[iIndex], tradeTester);
			}
		}

		return TradeRecordArray;
	}

	//Values of TradeRecord's data members is copied to TradeRecordNET's data members.
	int CMTManager::CopyToTradeRecordNET(TradeRecordNET^% info, TradeRecord *tradeRec)
	{
		//conversion of char array into String of C++/CLI.
		String ^ strSymbol		= gcnew String(tradeRec->symbol);
		info->symbol			= strSymbol;
		strSymbol				= gcnew String(tradeRec->comment);
		info->comment			= strSymbol;
		info->order				= tradeRec->order;
		info->login				= tradeRec->login;
		info->digits			= tradeRec->digits;
		info->cmd				= tradeRec->cmd;
		info->volume			= tradeRec->volume;
		info->open_time			= tradeRec->open_time;			//time_t of c++ is converted into Int64 of cli.
		info->state				= tradeRec->state;
		info->open_price		= tradeRec->open_price;
		info->sl				= tradeRec->sl;
		info->tp				= tradeRec->tp;
		info->close_time		= tradeRec->close_time;
		info->value_date		= tradeRec->value_date;
		info->expiration		= tradeRec->expiration;
		info->commission		= tradeRec->commission;
		info->conv_reserv		= tradeRec->conv_reserv;
		info->commission_agent	= tradeRec->commission_agent;
		info->storage			= tradeRec->storage;
		info->close_price		= tradeRec->close_price;
		info->profit			= tradeRec->profit;
		info->taxes				= tradeRec->taxes;
		info->magic				= tradeRec->magic;
		info->internal_id		= tradeRec->internal_id;
		info->activation		= tradeRec->activation;
		info->spread			= tradeRec->spread;
		info->margin_rate		= tradeRec->margin_rate;
		info->timestamp			= tradeRec->timestamp;

		int nDataSize			= 2;	//size of TradeRecord.conv_rates array;

		//Conversion of Unmanaged array (C++ array) into the Managed array (C++/CLI array).
		//Follow this link http://msdn.microsoft.com/en-us/library/ms146633.aspx for copying data
		//from a managed array to an unmanaged memory pointer, or from an unmanaged memory pointer to a managed array.
		Marshal::Copy(IntPtr((void *)tradeRec->conv_rates), info->conv_rates, 0, nDataSize);

		nDataSize = 4;		//size of TradeRecord.reserved array.
		Marshal::Copy(IntPtr((void *)tradeRec->reserved), info->reserved, 0, nDataSize);
		info->next = gcnew TradeRecordNET();

		//This code executes for WIN32 platform only. 
		//pin_ptr is used to pin the address of the TradeRecord->next which is __ptr32 pointer. 
		//On 32-bit system TradeRecord* matched __ptr32 pointer. But on WIN64 TradeRecord* is a 64-bit
		//pointer and TradeRecord->next is a __ptr32 pointer. Therefore, In WIN64 case 64-bit CLR 
		//does not support data declared with the __ptr32 modifier

#ifndef _WIN64
		//pin_ptr is used to pin the address of the tradeRec->next and 
		//then this pinned address is directly passed to the CopyToTradeRecordNET.
		//*pinned is NULL if there is no address stored of TradeRec in the next.
		pin_ptr<TradeRecord *> pinned = &tradeRec->next;
		if(*pinned)
			CopyToTradeRecordNET(info->next, *pinned);
#endif

		delete strSymbol;
		return RET_OK;
	}

	//This function is used to perform operations like Deleting, Enabling, Disabling, setting Leverages and setting group.
	//Following are the commands which tells what Operation is to be done on the user accounts:
	//GROUP_DELETE = 0
	//GROUP_ENABLE = 1,
	//GROUP_DISABLE = 2,
	//GROUP_LEVERAGE = 3,
	//GROUP_SETGROUP = 4;
	//There will be multiple login values and user account will be accessed through these login values.
	int CMTManager::UsersGroupOp(const GroupCommandInfoNET ^info, int % logins)
	{
		int iRet = 999;

		//Copying the values of GroupCommandInfoNET to the GroupCommandInfo.
		GroupCommandInfo cmd;

		//info->len tells the number of users account on which operation has to take place. 
		cmd.len = info->len;

		//info->command value tells, which operation(mentioned in function header) has to be performed on the selected users.
		cmd.command = info->command;

		//Conversion of String into character array.
		strcpy_s(cmd.newgroup, static_cast<char*>(Marshal::StringToHGlobalAnsi(info->newgroup).ToPointer()));

		//sets the leverage.
		cmd.leverage = info->leverage;

		//copying elements of array in c++ into the array of cli. 
		int nDataSize = sizeof(cmd.reserved)/sizeof(cmd.reserved[0]);
		for(int i = 0; i < nDataSize; i++)
			cmd.reserved[i] = info->reserved[i];

		//conversion of int ^ into int *.
		pin_ptr<int> log = &logins;
		int * nplogins = log;

		//Return 0 if operation is performed successfully on users account.
		iRet = m_manager->UsersGroupOp(&cmd, nplogins);

		return iRet;
	}
} // end namespace


