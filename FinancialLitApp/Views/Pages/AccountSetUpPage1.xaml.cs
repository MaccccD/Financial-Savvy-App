using FinancialLitApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialLitApp.Views.Pages
{
   public partial class AccountSetUpPage1 : ContentPage
    {
        private readonly IAuthenticationService _authService;
        public AccountSetUpPage1()
        {
            InitializeComponent();
            _authService = new AuthenticationService();
            Console.WriteLine("XAML Bindings successfuly initilized");
            // Setting up return key behaviors , just as in the login page  for smooth form navigation
            FirstNameEntry.Completed += (s, e) => LastNameEntry.Focus();
            LastNameEntry.Completed += (s, e) => EmailEntry.Focus();
            EmailEntry.Completed += (s, e) => PasswordEntry.Focus();
            PasswordEntry.Completed += (s, e) => ConfirmPasswordEntry.Focus();
            ConfirmPasswordEntry.Completed += async (s, e) => await RegisterAsync();

        }

        private async void OnCreateAccountClicked(object sender , EventArgs e)
        {
            await RegisterAsync();
        }

        private async Task RegisterAsync()
        {
            ErrorLabel.IsVisible = false;

            // Gettinng  input values and trimming  whitespace
            var firstName = FirstNameEntry.Text?.Trim();
            var lastName = LastNameEntry.Text?.Trim();
            var email = EmailEntry.Text?.Trim();
            var IdNumber = IdNumberEntry.Text?.Trim();
            var password = PasswordEntry.Text;
            var confirmPassword = ConfirmPasswordEntry.Text;

            // Validate all required fields
            if (string.IsNullOrWhiteSpace(firstName))
            {
                ShowError("Please enter your first name");
                FirstNameEntry.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                ShowError("Please enter your last name");
                LastNameEntry.Focus();
                return;
            }
            if(string.IsNullOrWhiteSpace(IdNumber))
            {
                ShowError("Please enter your ID Number");
                IdNumberEntry.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Please enter your email address");
                EmailEntry.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter a password");
                PasswordEntry.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                ShowError("Please confirm your password");
                ConfirmPasswordEntry.Focus();
                return;
            }

            // Validate password match
            if (password != confirmPassword)
            {
                ShowError("Passwords do not match. Please ensure that the password tyoed in matches!");
                ConfirmPasswordEntry.Focus();
                return;
            }

            // Validate password strength
            if (password.Length < 6)
            {
                ShowError("Password must be at least 6 characters long");
                PasswordEntry.Focus();
                return;
            }

            // Validate email format
            if (!IsValidEmail(email))
            {
                ShowError("Please enter a valid email address");
                EmailEntry.Focus();
                return;
            }

            // Show loading state
            SetLoadingState(true);

            try
            {
                var inputResult = await _authService.RegisterAsync(email, firstName, lastName, password, IdNumber);
                if (inputResult.IsSuccess) 
                {
                    //App shelllhandles logic here 

                    //show a welcome message
                    await DisplayAlert("Welcome!", $"Welcome to the Financial Savvy platform, {firstName}!", "Get Started");
                }
                else
                {
                    ShowError(inputResult.ErrorMessage);
                }
            }
            catch
            (Exception ex)
            {
                ShowError($"Rehgistration failed: {ex.Message}");
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private async void OnSignInTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//login");
        }


        //error handling feedback display
        private void ShowError(string message)
        {
            ErrorLabel.Text = message;
            ErrorLabel.IsVisible = true;
        }
        private void SetLoadingState(bool isLoading)
        {
            LoadingIndicator.IsVisible = isLoading;
            LoadingIndicator.IsRunning = isLoading;
            CreateAccountButton.IsEnabled = !isLoading;

            // Disable all input fields during loading
            FirstNameEntry.IsEnabled = !isLoading;
            LastNameEntry.IsEnabled = !isLoading;
            EmailEntry.IsEnabled = !isLoading;
            PasswordEntry.IsEnabled = !isLoading;
            IdNumberEntry.IsEnabled = !isLoading;   
            ConfirmPasswordEntry.IsEnabled = !isLoading;
           

            if (isLoading)
            {
                CreateAccountButton.Text = "Creating Account...";
            }
            else
            {
                CreateAccountButton.Text = "Create Account";
            }
        }

        private bool IsValidEmail(string email)
        { //checking if the format of the email is correct.
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ClearForm();
        }

        private void ClearForm()
        {
            FirstNameEntry.Text = string.Empty;
            LastNameEntry.Text = string.Empty;
            EmailEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            IdNumberEntry.Text = string.Empty;
            ConfirmPasswordEntry.Text = string.Empty;
            ErrorLabel.IsVisible = false;
            SetLoadingState(false);
        }
    }
}
