using FinancialLitApp.ViewModels;
using FinancialLitApp.Views;
using FinancialLitApp.Views.DetailPages;
using FinancialLitApp.Views.Pages;
using FinancialLitApp.Views.Pages.Challenges;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using CommunityToolkit.Mvvm.Messaging;
//using Android.Telephony;
using System.Linq.Expressions;
using System.Diagnostics;
//using Google.Android.Material.AppBar;
//using Android.App;


namespace FinancialLitApp;

public partial class AppShell : Shell
{
	public bool _isAuthenticated = false;
	public AppShell()
	{
		InitializeComponent(); //loads the XAML Files

		//here i'm registering the routs for the oages that are not part of the main tab
		Routing.RegisterRoute("forgotpassword", typeof(ForgotPasswordPage));
		Routing.RegisterRoute("lessondetailpage", typeof(LessonDetailPage));


		//setting up the initial navigation based on the user's authentinication status
		SetInitialNavigation();

		//creating a connection between the iAuthenticator Service and the App Shell to check the lgoicn status of the user throught the app:
		MessagingCenter.Subscribe<object>(this, "UserLoggedIn", OnUserLoggedIn);
		MessagingCenter.Subscribe<object>(this, "UserLoggedOut", OnUserLoggedOut);

	}

	private void RegisterRoutes()
	{

	}

	private void SetInitialNavigation()
	{
		if (_isAuthenticated)
		{
			ShowMainApp();
		}
		else
		{
			ShowAuthenticationFlow();
		}
	}
	

	

	 private void ShowMainApp()
	{
		//LoginContent.visible = false;
	}

	private void ShowAuthenticationFlow()
	{

	}

	private void OnUserLoggedIn(object sender)
	{
		_isAuthenticated = true;
		ShowMainApp();

	}

	private void OnUserLoggedOut(object sender)
	{
		_isAuthenticated = false;
		ShowAuthenticationFlow();

		//clear any stored authetnication data:
		Preferences.Clear();
		SecureStorage.RemoveAll();
	}
}

