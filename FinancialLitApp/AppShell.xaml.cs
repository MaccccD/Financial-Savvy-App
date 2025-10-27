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
using FinancialLitApp.Views.Pages.Lessons;
//using Google.Android.Material.AppBar;
//using Android.App;


namespace FinancialLitApp;

public partial class AppShell : Shell
{
	public bool _isAuthenticated = false;
	public AppShell()
	{
		InitializeComponent(); //loads the XAML Files

        //here i'm registering the routes for the pages that are not part of the main tab but are part of the app architecture:
        Routing.RegisterRoute("savings", typeof(Savings)); //currrently testingg. [this works btw]
        Routing.RegisterRoute("budgeting", typeof(Budgeting)); //the route to the budgeting challenge
        Routing.RegisterRoute("investment", typeof(Investment)); //the route to the investment challenge[might change this later tbh)
        Routing.RegisterRoute("savingslesson", typeof(SavingsLesson)); // the route to the savings lesson
        Routing.RegisterRoute("budgetinglesson", typeof(BudgetingLesson)); //the route to the budgeting lesson
        Routing.RegisterRoute("filingtaxreturns", typeof(FinancialPortfolio)); //the route to the financial portfolio section lesson
        Routing.RegisterRoute("forgotpassword", typeof(ForgotPasswordPage));
		Routing.RegisterRoute("lessondetailpage", typeof(LessonDetailPage));
        Routing.RegisterRoute("budgetingchallenge", typeof(Budgeting));



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


    //navigation helper methods:
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
            attempts++; //increment the number of attempts to get the shell content to initialize as we add a delayer to get it to load or initialize properly
        }

        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync(route);
        }
        else
        {
            // Fallback: try using this instance directly as opposed to waiting for shell to take you to the actual page
            await this.GoToAsync(route);
        }
    }
  
	public async Task NavigateToLogin()
	{

		await Shell.Current.GoToAsync("//login"); // note the absolute routing
	}
    public async Task NavigateToHome()
    {
        await Shell.Current.GoToAsync("//home");
    }
	
	//this method below is a navigation helper method that checks if the page click has a route as well as parameters and makes navigation easier
    public async Task NavigateToPage(string route, IDictionary<string, object> parameters = null)
    {
        if (parameters != null)
        {
            await Shell.Current.GoToAsync(route, parameters); //navigates to the specific page based on the parameters passing 
        }
        else
        {
            await Shell.Current.GoToAsync(route); //still takes you to the specific page even without parameters.

        }
    }


    //this method below is to protected the content in pages that are only supposed to be seen by authenticated users:
    protected override async void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

		//checking if the user is trying to access authenticated pages while they are not logged in:
		var targetRoute = args.Target.Location.OriginalString;
		var authenticatedRoutes = new[] { "home", "lessondetail", "challenges", "feedback"};

		if (authenticatedRoutes.Any(route => targetRoute.Contains(route)) && !_isAuthenticated) {
			// so if the user is trying to access content that is supposed to be for users that are authenticated:
			args.Cancel();
			Device.BeginInvokeOnMainThread(async () =>
			{
				await NavigateToLogin();
			});
		}
    }


    //cleaning up the subscriptions:
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
		MessagingCenter.Unsubscribe<object>(this, "UserLoggedIn");
		MessagingCenter.Unsubscribe<object>(this, "UserLoggedOut");

    }
}

