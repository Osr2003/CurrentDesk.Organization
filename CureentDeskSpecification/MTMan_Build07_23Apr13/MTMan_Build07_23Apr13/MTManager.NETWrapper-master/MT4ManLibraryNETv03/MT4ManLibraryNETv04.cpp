// This is the main DLL file.
// 8.24.2012 Sat
// build and exec status:
// 8.26.2012 COMPILE, RUNS RETURNS SummaryGetAll as a * ptr but not ready .NET
// MT4ManLibraryNETV03KEEPER MOSTCURRENT
// miktek
#include "stdafx.h"
#include <WinSock2.h>
#include <winsock.h>
//#include "../mtlibs/MT4ManagerAPI.h"
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
//--- risk management
/***************

   virtual SymbolSummary* __stdcall SummaryGetAll(int *total)                            =0;
   virtual int          __stdcall SummaryGet(LPCSTR symbol,SymbolSummary *info)          =0;
   virtual int          __stdcall SummaryGetByCount(const int symbol,SymbolSummary *info)=0;
   virtual int          __stdcall SummaryGetByType(const int sectype,SymbolSummary *info)=0;
   virtual int          __stdcall SummaryCurrency(LPSTR cur,const int maxchars)          =0;
   virtual ExposureValue* __stdcall ExposureGet(int *total)                              =0;
   virtual int          __stdcall ExposureValueGet(LPCSTR cur,ExposureValue *info)       =0;

***************/

  public struct ExposureValueNET
  {
   //char              currency[4];         // currency len = 4 chars
   double            clients;             // clients volume
   double            coverage;            // coverage volume
  };
  
   struct SymbolSummaryNET
  {
   //char              symbol[12];          // symbol len = 12 chars
   int               count;               // symbol counter
   int               digits;              // floating point digits
   int               type;                // symbol type (symbol group index)
   //--- clients summary
   int               orders;              // number of client orders
   __int64           buylots;             // buy volume
   __int64           selllots;            // sell volume
   double            buyprice;            // average buy price
   double            sellprice;           // average sell price
   double            profit;              // clients profit
   //--- coverage summary
   int               covorders;           // number of coverage orders
   __int64           covbuylots;          // buy volume
   __int64           covselllots;         // sell volume
   double            covbuyprice;         // average buy price
   double            covsellprice;        // average sell price
   double            covprofit;           // coverage profit
  };


/**************************
   struct SymbolSummary
  {
   char              symbol[12];          // symbol len = 12 chars
   int               count;               // symbol counter
   int               digits;              // floating point digits
   int               type;                // symbol type (symbol group index)
   //--- clients summary
   int               orders;              // number of client orders
   __int64           buylots;             // buy volume
   __int64           selllots;            // sell volume
   double            buyprice;            // average buy price
   double            sellprice;           // average sell price
   double            profit;              // clients profit
   //--- coverage summary
   int               covorders;           // number of coverage orders
   __int64           covbuylots;          // buy volume
   __int64           covselllots;         // sell volume
   double            covbuyprice;         // average buy price
   double            covsellprice;        // average sell price
   double            covprofit;           // coverage profit
  }; 
  
 *******************/

#include "MT4ManLibraryNETv03.h"

namespace MT4ManLibraryNETv03
{

	int CMTManager::PassMeADelegate(PumpFuncDelegate^ pumpFunc)
	{
		// create an instance of the PumpFuncDelegate
		// then put pumpFunc as the method to call
		keeperCallBack = pumpFunc;

		int iRet = 0;

		pumpFunc(666);
		keeperCallBack(777);

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
		int iRet = 765;
		if (m_manager == NULL) 
			return 0;

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
		int iRet = 765;
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

		// put a thread wait here then call
		int myCount = 0;
		int myRetCode = 0;

		//myRetCode = ExposureGet(myCount);
		// it may be wise to disconnect after this or put a disconnect
		// in the passablePumpFunc that after it receives at least 1 msg
		// it disconnects. I am thinking the PumpingSwitch callback
		// goes out of scope when the app quits miktek 7.28.12
		//
		
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
		// 8.12.12 ServerName = "mtdem01.primexm.com:443";
		// is now being passed in.
		
		// since it is passed in c++/cli as a tracking handle
		// it needs to converted to a LPCSTR char * type c++ string

		char * native_string =
			(char *) Marshal::StringToHGlobalAnsi(ServerName).ToPointer();

		iRet = m_manager->Connect(native_string);
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

		typedef int (*MtManCreate_t)(int version, CManagerInterface **man);
		MtManCreate_t     m_pfnManCreate;
			
		m_lib = ::LoadLibrary(L"c:\\mtlibs\\mtmanapi.dll");
		
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

	// RISK MANAGEMENT CALLS RMC begun 
	
	// low risk call going for c# to here but
	// but not from here to mtmanapi.dll

	// miktek Exposure get passes by references
	// and returns the total number of exposure records to read
	// note: this call is activated by Pumping Switch
	// miktek 8.12.2012
	int CMTManager::ExposureGet(int% total)
	{
		// 8.12.12 Can I get a version of this that
		// works just locally.
		// Using classic c pointers etc as opposed 
		// to gcnew. is this smart?

		// this struct gets returned to .NET client
		ExposureValueNET ^ myExposureNET = gcnew ExposureValueNET();

		// returned by call to ExposureGet

		// if I already have one of these I need to free it so
		// not as to overwhelm the heap miktek 8.26.2012
		ExposureValue * myExposureVal = new ExposureValue();

		// consider ExposureValue ^ myExposureVal = gcnew ExposureValue;
		// then marshal or get the exposurevalue struct first?
		// what to return to c# managed code?
		int localTotal = 555; // set arbritrary value to see if its modified

		myExposureVal = m_manager->ExposureGet(&localTotal);
		
		if (myExposureVal != NULL || localTotal > 0) {
			
			myExposureNET->clients = myExposureVal->clients;
			myExposureNET->coverage = myExposureVal->coverage;

			// heavy handed conversion routine
			// to char currency[4] to a string
			// but easier to read at least for me.
			String ^ s = gcnew String(myExposureVal->currency);

			myExposureNET->CurrencyType = s; // copy from char [] to string first

			int TotalRecved = localTotal;
			total = localTotal; 
		}
		
		return localTotal;
	}

	// exposureValue get is passed a currency string and a pointer to
	// a exposure value. ExposureValue struct 
	int CMTManager::ExposureValueGet(String ^ cur, ExposureValueNET * info) //1
	{
		// lpcstr conversion goes here
		int iRet = 0;
		LPCSTR lpcstrCurr =
			(char*)Marshal::StringToHGlobalAnsi(cur).ToPointer();

		ExposureValue * infoWIN32 = new ExposureValue();
		iRet = m_manager->ExposureValueGet(lpcstrCurr, infoWIN32);

		// copy returned value in infoWIN32 into infoNET structure
		// that uses a string instead of char[4] array

		return iRet;
	}

	int CMTManager::SummaryCurrency(String^ cur,int maxchars) //1
	{
		LPSTR lpcstrCurr =
			(char*)Marshal::StringToHGlobalAnsi(cur).ToPointer();

		maxchars = 4; // what value should this be?
		m_manager->SummaryCurrency(lpcstrCurr, maxchars);

		return 0;

	}

	// test #1 can I return a ExposureValue by value
	// I don't think so as it is stack based and the 
	// address will be gone when the function returns from the call

	// test #2 I need to return a pointer on the gc to the caller

	int CMTManager::GetExposureValue(int code)
	{
		//ExposureValue^ myExposureValue = gcnew ExposureValue();

		return 90;
	}

	int CMTManager::SummaryGetByCount(const int symbol,SymbolSummary *info)
	{

		return 1;
	}

	int CMTManager::SummaryGetByType(const int sectype,SymbolSummary *info)
	{

		return 1;
	}

	int	CMTManager::SummaryGetAll(int% total) 
	{
		SymbolSummary * mySymbolSummary = new SymbolSummary();

		return 9;
	}

} // end namespace


