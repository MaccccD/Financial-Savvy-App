using FinancialLitApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialLitApp.Views.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly IAuthenticationService _authService;
        public LoginPage() 
        {
            InitializeComponent();
            _authService = new AuthenticationService();
            Console.WriteLine("the login page xaml bindings have initialized");

            //setting up return key behavior so the when a person is done filling n one entry, it highhlightes the next:
            EmailEntry.Completed += (s, e) => PasswordEntry.Focus();
            PasswordEntry.Completed += async (s, e) => await LoginAsync();
        }

        private async void OnLoginClicked (object sender, EventArgs e)
        {
            await LoginAsync();
        }

        private async Task LoginAsync()
        {
            //ensure that any error messages are hiden first
            ErrorLabel.IsVisible = false;

            //input values as they are typed in:
            var email = EmailEntry.Text?.Trim();
            var password = PasswordEntry.Text;

            //validation of details logged in:
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
               {
                ShowError("Please enter both email and password");
                return;
               }
              //show loading state after the error label message has shown:
               SetLoadingState(true);

            try
            {
                //attempting login :
                var result = await _authService.LoginAsync(email, password);

                if (result.IsSuccess)
                {
                    //App shell will handle the logic for when a user logs in sucessfully
                }
                else
                {
                    ShowError(result.ErrorMessage);
                }
            }
            catch (Exception ex) 
            {
                ShowError($"Login failed: {ex.Message}");
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private async void OnForgotPasswordTapped(object sender , EventArgs e)
        {
            //  await Shell.Current.GoToAsync("ForgotPasswordPage");
            await WaitForShellAndNavigate("//forgotpasswordpage");
        }

        private async void OnSignUpTapped(object sender ,EventArgs e)
        {
            await Shell.Current.GoToAsync("HomePage");
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

        private void ShowError(string message)
        {
            ErrorLabel.Text = message;
            ErrorLabel.IsVisible = true;
        }

        private void SetLoadingState(bool isLoading)
        {
            LoadingIndicator.IsVisible = isLoading;
            LoadingIndicator.IsRunning = isLoading;
            LogInBtn.IsEnabled = !isLoading;
            EmailEntry.IsEnabled = !isLoading;
            PasswordEntry.IsEnabled = !isLoading;

            if (isLoading)
            {
                LogInBtn.Text = "Signing In...";
                Console.WriteLine("I'm working yayy!");
            }
            else
            {
                LogInBtn.Text = "Sign In";
            }
        }
    }
}
