using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace FinancialLitApp.Services
{
    public interface IAuthenticationService
    {
        //in here I'm perfoming different task that relate to the account creation set up, login as well as the authication status.
         Task<AuthResult> RegisterAsync(string email, string password, string firstName, string lastName, string idNumber);
        Task<AuthResult> LoginAsync(string email, string password);
        Task<bool> LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<UserProfile> GetCurrentUserAsync();
    }

    public class AuthenticationService : IAuthenticationService
    {
        //this are token strings that I will use to check users's login and authentication status
        private const string TOKEN_KEY = "user_token";
        private const string USER_KEY = "user_profile";


        public async Task<AuthResult> RegisterAsync(string email, string password, string firstName, string lastName, string idNumber)
        { // this method is meant to register or record the user details as they are typed in
            try
            {
                //checking the create account details as they user types them in
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(idNumber))
                {
                    return new AuthResult
                    {
                        //retrun a message if not fields are filled in properly
                        IsSuccess = false,
                        ErrorMessage = "All fields are required. Please fill out all the required fields!"
                    };
                }

                if (!IsValidEmail(email)) // if the email address enetred is invalid and does nto follow the normal or correct format
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Please enter a valid email address."
                    };
                }

                if (!IsValidPassword(password)) // if the password entred is not the correct length.
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Password must be at least 6 characters long"
                    };
                }

                // Simulate API delay just to create a loading screen.
                await Task.Delay(1500);

                // Check if user already exists (placeholder)
                if (await UserExists(email))
                {
                    //checking if the user details have been typed in or registred before.
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "An account with this email already exists. Please use other details."
                    };
                }

               // saving all the user details entred
                var userProfile = new UserProfile
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    IDNumber = idNumber,
                    CreatedAt = DateTime.Now
                };

               // var token = GenerateToken(userProfile);

                // Store authentication data securely
              //  await SecureStorage.SetAsync(TOKEN_KEY, token); //secure storage is losing data during run time, causing the storage data to be emty by the time the same saved data is being retrieved
               // await SecureStorage.SetAsync(USER_KEY, JsonSerializer.Serialize(userProfile));

                Preferences.Set($"account_{email}", JsonSerializer.Serialize(userProfile));
                Preferences.Set($"password_{email}", password);

                System.Diagnostics.Debug.WriteLine($"Stored with Preferences for email: {email}");


                // Notify the app that user is logged in
                //  MessagingCenter.Send<object>(this, "UserLoggedIn");

                return new AuthResult
                {
                    IsSuccess = true,
                    User = userProfile,
                    Token = null // not token yet bc the user needs to now login after creating account first
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Registration  and account failed: {ex.Message}"

                };
            }
        }



        public async Task<AuthResult> LoginAsync(string email, string password) // this is the method /task that will validate the login details entred by the user.
        {
            try
            {
                // Here I'm checking the details the user types in in the field and clearing any white space.
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return new AuthResult
                    { //if the fields are null, i retrun a messsage as feedback.
                        IsSuccess = false,
                        ErrorMessage = "Email and password are required"
                    };
                }

                if (!IsValidEmail(email)) // checking if the user has typed in an email.
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Please enter a valid email address"
                    };
                }


                // Simulating an API delay ( for data loading purposes)
                await Task.Delay(1000);

                //check if the account details used by the user are matching in this section as well:
                var storedAccountJson = Preferences.Get($"account_{email}", string.Empty);
                var storedPassword = Preferences.Get($"password_{email}", string.Empty);

                System.Diagnostics.Debug.WriteLine($"Retrieved from Preferences - Account: {!string.IsNullOrEmpty(storedAccountJson)}, Password: {!string.IsNullOrEmpty(storedPassword)}");


                if (string.IsNullOrEmpty(storedAccountJson) || string.IsNullOrEmpty(storedPassword))
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Account not found. Please create an account first."
                    };
                }

                if (storedPassword != password) //checking if the password used to create the accoutn and the one used to login mathces 
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid email or password. Please try again."
                    };
                }

                // Login successful - deserialize user profile
                var userProfile = JsonSerializer.Deserialize<UserProfile>(storedAccountJson);
                var token = GenerateToken(userProfile);

                // Store authentication data securely using the (Maui's) platform API
                await SecureStorage.SetAsync(TOKEN_KEY, token);

                await SecureStorage.SetAsync(USER_KEY, JsonSerializer.Serialize(userProfile));

                // Notify the app's shell.xaml that user is logged in
                MessagingCenter.Send<object>(this, "UserLoggedIn");

                return new AuthResult
                {
                    //once user has logged in using their email address and password, send the notif to the "AppShell's code behind" so that the approrpiate UI shows based on this updated status
                    IsSuccess = true,
                    User = userProfile,
                    Token = token
                };

            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    //a message to catch when the login authentication has failed.
                    IsSuccess = false,
                    ErrorMessage = $"Login failed: {ex.Message}"
                };
            }
        }




        public async Task<bool> LogoutAsync() // this is the ogic for when the user logouts
        {
            try
            {
                // Clear stored authentication data that was stored in the storage:
                SecureStorage.Remove(TOKEN_KEY);
                SecureStorage.Remove(USER_KEY);

                // Notify the app that user is logged out
                MessagingCenter.Send<object>(this, "UserLoggedOut");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsAuthenticatedAsync() 
            // checking if the user is truely authenticated by checking the tokens that were stored in the "secure storage" api  when the user typed them in as part fo their user profile. 
        {
            try
            {
                var token = await SecureStorage.GetAsync(TOKEN_KEY); // that's how you accces data stpred in the storage
                return !string.IsNullOrEmpty(token) && IsTokenValid(token);
            }
            catch
            {
                return false;
            }
        }

        public async Task<UserProfile> GetCurrentUserAsync() // getting the user pfile details stored in the storage as an key if user truely authenticated
        { 
            try
            {
                var userJson = await SecureStorage.GetAsync(USER_KEY); // for the user profile.
                if (string.IsNullOrEmpty(userJson))
                    return null;

                return JsonSerializer.Deserialize<UserProfile>(userJson);
            }
            catch
            {
                return null;
            }
        }

        // Private helper methods
        private async Task<bool> AuthenticateUser(string email, string password)
        {
             // checking if the user's password is in this format:
            return password == "password123";
        }

        private async Task<bool> UserExists(string email)
        {
            //here i'm checking the maui storage api if the user account detai's exist or have been stored before 
            var existingAccount = await SecureStorage.GetAsync($"account_{email}");
            return !string.IsNullOrEmpty(existingAccount);
           

            /* Example implementation:
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://yourapi.com/users/exists?email={email}");
            var result = await response.Content.ReadAsStringAsync();
            return bool.Parse(result);
            */
        }

        private string GenerateToken(UserProfile user)
        {
            // this method delas with generating a token for the duration the user is logged in the app
            var tokenData = new
            {
                user_id = user.IDNumber,
                email = user.Email,
                expires_at = DateTime.Now.AddDays(7).ToString("O"),
                issued_at = DateTime.Now.ToString("O")
            };

            return JsonSerializer.Serialize(tokenData);
        }

        private bool IsTokenValid(string token) // checks yhe validitiy of the token
        {
            try
            {
                var tokenData = JsonSerializer.Deserialize<Dictionary<string, object>>(token);

                if (!tokenData.ContainsKey("expires_at"))
                    return false;

                var expiresAt = DateTime.Parse(tokenData["expires_at"].ToString());
                return expiresAt > DateTime.Now;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidEmail(string email)
        { // this method check if the emial address typed in is in the correct format of a genral email.
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

        private bool IsValidPassword(string password)
        { //checking the number of charatcers typed in for the password;
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 6;
        }
    }

    // Data models
    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public UserProfile User { get; set; }
        public string Token { get; set; }
    }

    public class UserProfile
    {
       // public string Id { get; set; }
        public string Email { get; set; }
        public string IDNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}
