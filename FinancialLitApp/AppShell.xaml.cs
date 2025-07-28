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

		//here i'm registering the routs for the oages that are not part of the main tab but are part of the ap:
		Routing.RegisterRoute("forgotpassword", typeof(ForgotPasswordPage));
		Routing.RegisterRoute("lessondetailpage", typeof(LessonDetailPage));
        Routing.RegisterRoute("savingschallenge", typeof(Savings));
        Routing.RegisterRoute("budgetingpage", typeof(Budgeting));



		//setting up the initial navigation based on the user's authentinication status
		SetInitialNavigation();

		//creating a connection between the iAuthenticator Service and the App Shell to check the lgoicn status of the user throught the app:
		MessagingCenter.Subscribe<object>(this, "UserLoggedIn", OnUserLoggedIn);
		MessagingCenter.Subscribe<object>(this, "UserLoggedOut", OnUserLoggedOut);

	}

	//private void RegisterRoutes()
	//{

	//}

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
    private void OnUserLoggedIn(object sender)
    {
        _isAuthenticated = true;
        ShowMainApp();

    }

    private void OnUserLoggedOut(object sender)
    {
        _isAuthenticated = false;
        ShowAuthenticationFlow();

        //clear any stored authentication data:
        Preferences.Clear();
        SecureStorage.RemoveAll();
    }



    private async void ShowMainApp()
	{
		//The authentication content becomes disbaled once the user has authenticated successfully.
		AccountSetUpContent.IsVisible = false;
		//LoginContent.IsVisible = false;

		//show the main app tabs:
		MainTabBar.IsVisible = true;

		//navigate the user to home page after the have authenticated:
		//Shell.Current.GoToAsync("//home");
		await NavigateToHomeAsync();

	}

	private async void ShowAuthenticationFlow()
	{
		//the app content will become disabled untill the user has authenticated:
		MainTabBar.IsVisible = false;

		//show the authentication content:
		AccountSetUpContent.IsVisible = true;
		LoginContent.IsVisible = true;

		//navigate the user to the login page once they have created an account :
		//Shell.Current.GoToAsync("//LoginPage");
		await NavigateToAccountSetUp();
	}


    //navigation helper methodds:
    private async Task NavigateToHomeAsync()
    {
        await WaitForShellAndNavigate("//home");
    }
    public async Task NavigateToAccountSetUp()
    {
        //await Shell.Current.GoToAsync("//accountsetuppage");
        await WaitForShellAndNavigate("//accountsetuppage");
    }
    private async Task NavigateToLoginAsync()
    {
        await WaitForShellAndNavigate("//login");
    }



    // Common method to handle Shell.Current null checking
    private async Task WaitForShellAndNavigate(string route)
    {
        int attempts = 0;
        while (Shell.Current == null && attempts < 20) // Increased attempts
        {
            await Task.Delay(50); //  the shorter  the delay, the more attempts
            attempts++; //increment the numbe rof attempts to get the shell content to initilaize as we add a delayer to get it to load or initialize properly
        }

        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync(route);
        }
        else
        {
            // Fallback: try using this instance directly as aopposed to waiting for sehll to take you to the actual page
            await this.GoToAsync(route);
        }
    }
  
	public async Task NavigateToLogin()
	{

		await Shell.Current.GoToAsync("//login");
	}
    public async Task NavigateToHome()
    {
        await Shell.Current.GoToAsync("//home");
    }
	
	//this method below is a nvaigation helper method that checks if the page click has a route as well as paramaters and makes navigatiuon easier
    public async Task NavigateToPage(string route, IDictionary<string, object> parameters = null)
    {
        if (parameters != null)
        {
            await Shell.Current.GoToAsync(route, parameters); //navogates to the specifc page based on the parameters passin 
        }
        else
        {
            await Shell.Current.GoToAsync(route); //still takes you to the pasoecifc page even withut parameters.

        }
    }


    //this method below is to protected the content in pages that are only supposed to be seen by authenticated users:
    protected override async void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

		//checking if the user is trying to access authenticated pages while they are not logged in:
		var targetRoute = args.Target.Location.OriginalString;
		var authenticatedRoutes = new[] { "home", "lessondetail", "challenges", "feedback", "rewards" };

		if (authenticatedRoutes.Any(route => targetRoute.Contains(route)) && !_isAuthenticated) {
			// so if the user is trying to access content that is supposed to be for users that are authenticated:
			args.Cancel();
			Device.BeginInvokeOnMainThread(async () =>
			{
				await NavigateToLogin();
			});
		}
    }


    //cleaming up the subcriptions:
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
		MessagingCenter.Unsubscribe<object>(this, "UserLoggedIn");
		MessagingCenter.Unsubscribe<object>(this, "UserLoggedOut");

    }
}

