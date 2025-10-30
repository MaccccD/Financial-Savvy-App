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
using FinancialLitApp.Services;
//using Google.Android.Material.AppBar;
//using Android.App;


namespace FinancialLitApp;

public partial class AppShell : Shell
{
	public bool _isAuthenticated = false;
    private IBiometricAuthService _biometricAuth;
	public AppShell()
	{
		InitializeComponent(); //loads the XAML Files

        _biometricAuth = new BiometricAuthService();

        //here i'm registering the routes for the pages that are not part of the main tab but are part of the app architecture:
        Routing.RegisterRoute("savings", typeof(Savings)); //currently testing. [this works btw]
        Routing.RegisterRoute("budgeting", typeof(Budgeting)); //the route to the budgeting challenge
        Routing.RegisterRoute("savingslesson", typeof(SavingsLesson)); // the route to the savings lesson
        Routing.RegisterRoute("budgetinglesson", typeof(BudgetingLesson)); //the route to the budgeting lesson
        Routing.RegisterRoute("filingtaxreturnslesson", typeof(FinancialPortfolio)); // the route to the filing tax returns lesson
        Routing.RegisterRoute("filingtaxreturns", typeof(FilingTaxReturns)); //the route to the filing tax returns challenge
        Routing.RegisterRoute("forgotpassword", typeof(ForgotPasswordPage));
		Routing.RegisterRoute("lessondetailpage", typeof(LessonDetailPage));
       


        
		//setting up the initial navigation based on the user's authentication status
		SetInitialNavigation();

		//creating a connection between the iAuthenticator Service and the App Shell to check the login status of the user throughout the app:
		MessagingCenter.Subscribe<object>(this, "UserLoggedIn", OnUserLoggedIn);
		MessagingCenter.Subscribe<object>(this, "UserLoggedOut", OnUserLoggedOut);

	}

	//private void RegisterRoutes()
	//{

	//}

	private async void SetInitialNavigation()
	{
        var isEnrolled = await _biometricAuth.IsUserEnrolled();

        if (isEnrolled)
        {
            var username = await _biometricAuth.GetStoredUsername();

            var authenticated = await _biometricAuth.AuthenticateUser($"Heyy, Welcome Back! {username ?? "User"}!");

            if (authenticated)
            {
                _isAuthenticated = true;
                ShowMainApp();
                return;
            }
            else
            {
                await DisplayBiometricFailureOptions();
                //ShowAuthenticationFlow();
            }
           
        }

        //if user is not enrolled nor authenticated , show the normal way to authenticate that i had initially set up:
        ShowAuthenticationFlow();
    }


    private  async Task DisplayBiometricFailureOptions()
    {
        var retry = await DisplayAlert(
            "Authentication Failed",
            "Would you like to try again? or Login with your account ?",
            "Try Again",
            "Login");

        if (retry)
        {
            //do the authentication again:
            var authenticate = await _biometricAuth.AuthenticateUser();

            if (authenticate)
            {
                _isAuthenticated = true;
                ShowMainApp();
                
            }
            else
            {
                _isAuthenticated = false;
                ShowAuthenticationFlow();
            }
        }
        else
        {
            ShowAuthenticationFlow();
        }
    }
    private void OnUserLoggedIn(object sender)
    {
        //this is the login without the enrollment:
        _isAuthenticated = true;
        ShowMainApp();

    }

    private async void OnUserLoggedInWithId(object sender, string userId)
    {
        ///after successful traditional login, offer to set up the biometric access:
        var isBiometricAvailable = await _biometricAuth.IsBiometricAvailable();

        if (isBiometricAvailable)
        {
            var setupBiometric = await DisplayAlert(
                "Quick Login Set Up",
                "Would you like to use your fingerprint or face recognition for faster login next time? You won't need to remember your password!",
                "Yes, Set It Up",
                "No Thanks");

            if (setupBiometric)
            {
                //get username from auth system:
                var username = userId;

                var enrolled = await _biometricAuth.EnrollUser(userId, username);

                if (enrolled)
                {
                    await DisplayAlert(
                        "Success!",
                        "Biometric login is now enabled. Next time just can your finger print or face!",
                        "Got It");
                }
                else
                {
                    await DisplayAlert(
                        "Set Up Cancelled",
                        "You can set up biometric login later in the settings page!",
                        "Okay!");
                }
            }
        }

        _isAuthenticated = true;
        ShowMainApp();

    }      

    private async void OnUserLoggedOut(object sender)
    {
        _isAuthenticated = false;

        var isEnrolled = await _biometricAuth.IsUserEnrolled();

        if (isEnrolled)
        {
            var removeBiometricData = await DisplayAlert(
                "Logout",
                "Do you want to remove biometric login from this device?",
                "Yes, remove it",
                "No, keep it!");

            if (removeBiometricData)
            {
                 await _biometricAuth.ClearEnrollment();
                await DisplayAlert(
                    "Removed!",
                    "Biometric login has been successfully removed. You'll need to login with your credentials next time.",
                    "Okay");

            }
        }
        ShowAuthenticationFlow();

        //clear any stored authentication data:
        Preferences.Clear();
        SecureStorage.RemoveAll();
    }



    private async void ShowMainApp()
	{
		//The authentication content becomes disabled once the user has authenticated successfully.
		//AccountSetUpContent.IsVisible = false;
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
		//AccountSetUpContent.IsVisible = true;
		LoginContent.IsVisible = true;

		//navigate the user to the login page once they have created an account :
		//Shell.Current.GoToAsync("//LoginPage");
	//	await NavigateToAccountSetUp();
	}


    //navigation helper methods:
    private async Task NavigateToHomeAsync()
    {
        await WaitForShellAndNavigate("//home");
    }
    //public async Task NavigateToAccountSetUp()
    //{
    //    //await Shell.Current.GoToAsync("//accountsetuppage");
    //    await WaitForShellAndNavigate("//accountsetuppage");
    //}
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

            //checking if the user has biometric enrolled:
            var isEnrolled = await _biometricAuth.IsUserEnrolled();

            if (isEnrolled)
            {
                //try biometric auth:
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var authenticated = await _biometricAuth.AuthenticateUser(
                        "Authenticate to access this account");

                    if (authenticated)
                    {
                        _isAuthenticated = true;
                        await Shell.Current.GoToAsync(targetRoute);
                    }
                    else
                    {
                        await DisplayAlert(
                            "Authentication Required",
                            "You must authenticate to access this account",
                            "Okay");
                        await NavigateToLogin();
                    }
                });

            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await NavigateToLogin();
                });
            }
			
		}
    }


    //cleaning up the subscriptions:
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
		MessagingCenter.Unsubscribe<object>(this, "UserLoggedIn");
        MessagingCenter.Unsubscribe<object>(this, "UserLoggedInWithId");
		MessagingCenter.Unsubscribe<object>(this, "UserLoggedOut");

    }
}

