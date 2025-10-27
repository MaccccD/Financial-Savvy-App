using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using FinancialLitApp.Services;

namespace FinancialLitApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IBiometricAuthService _biometricAuth;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool showBiometricBtn;

        [ObservableProperty]
        private bool showTraditionalLogin = true;

        [ObservableProperty]
        private string biometricBtnTxt = "Login with Fingerprint/Face";

        public LoginViewModel()
        {
            _biometricAuth = new BiometricAuthService();
        }

        public async Task InitializeAsync()
        {
           var isEnrolled = await _biometricAuth.IsUserEnrolled();
           // var authenticateUser = await _biomtricAuth.AuthenticateUser("Login to Financial Savvy App");
            var isAvailable = await _biometricAuth.IsBiometricAvailable();

            if (isEnrolled && isAvailable)
            {
                ShowBiometricBtn = true;

                var username = await _biometricAuth.GetStoredUsername();
                BiometricBtnTxt = $"Login as:{username ?? "User"}";

                ShowTraditionalLogin = false;

            }
            else
            {
                ShowBiometricBtn = true;
                ShowTraditionalLogin = false;
            }
        }

        [RelayCommand]
        public async Task BiometricLogin()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                ShowBiometricBtn = true;

                var authenticated = await _biometricAuth.AuthenticateUser(
                    "Login to Financial Savvy App");
               

                if (!authenticated)
                {
                    var userId = await _biometricAuth.GetStoredUserId();
                    //tell shell that user has authenticated:
                    MessagingCenter.Send<object>(this, "UserLoggedIn");

                    //then take user to home page :
                  
                    await Shell.Current.GoToAsync("//home");
                }
                else
                {
                    await Shell.Current.DisplayAlert(
                        "Authentication Failed",
                        "Biometric authentication failed! Please try again or use your password.",
                        "Okay");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Authentication Failed: {ex.Message}");
                await Shell.Current.DisplayAlert(
                    "Error",
                    "An error has occurred during the biometric login. Please try the traditional login.",
                    "Okay");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task Login()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert(
                    "Validation Error",
                    "Please enter/fill both your username and password",
                    "Okay");
                return;
            }

            try
            {
                IsBusy = true;

                var loginSuccess = await AuthenticateWithServer(Username, Password);
                if (loginSuccess)
                {
                    //notify app shell of the successful login:
                    MessagingCenter.Send<object, string>(this, "UserLoggedInWithId", Username);

                    Password = string.Empty;
                }
                else
                {
                    await Shell.Current.DisplayAlert(
                        "Login Failed!",
                        "Invalid Username or password",
                        "Okay");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"The login failed! {ex.Message}", ex);
                await Shell.Current.DisplayAlert(
                    "Error!!",
                    "An error occurred during the login process. Please try again",
                    "Okay");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void ShowTraditionalLoginForm()
        {
            ShowTraditionalLogin = true;
        }
        [RelayCommand]
        public async Task ForgotPassword()
        {
            await Shell.Current.GoToAsync("//forgotpassword");
        }


        private async Task <bool> AuthenticateWithServer(string username, string password)
        {
            await Task.Delay(1220);

            return username == "" && password == "";
        }



    }
}
