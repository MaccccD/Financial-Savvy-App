package mono.android.app;

public class ApplicationRegistration {

	public static void registerApplications ()
	{
				// Application and Instrumentation ACWs must be registered first.
		mono.android.Runtime.register ("FinancialLitApp.MainApplication, FinancialLitApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", crc64cceef0ac8ce98ffc.MainApplication.class, crc64cceef0ac8ce98ffc.MainApplication.__md_methods);
		mono.android.Runtime.register ("Microsoft.Maui.MauiApplication, Microsoft.Maui, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", crc6488302ad6e9e4df1a.MauiApplication.class, crc6488302ad6e9e4df1a.MauiApplication.__md_methods);
		
	}
}
