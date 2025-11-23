using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System.Text.Json;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace FinancialLitApp.Services
{
    public interface IBiometricAuthService
    {
        //okay so these are the different tasks that would encapsulate the biometric authentication of the user for the first time when using the app:
        Task<bool> IsBiometricAvailable();
        Task<bool> AuthenticateUser(string reason = "Authenticate to continue");
        Task<bool> EnrollUser(string userId, string username);
        Task<string> GetStoredUserId();
        Task<string> GetStoredUsername();
        Task<bool> IsUserEnrolled();
        Task ClearEnrollment();
    }
  public class BiometricAuthService : IBiometricAuthService
    {
        private const string USER_ID_KEY = "biometric_user_id";
        private const string USERNAME_KEY = "biometric_username";
        private const string ENROLLMENT_DATE_KEY = "biometric_enrollment_date";

        public async Task <bool> IsBiometricAvailable()
        {
            try
            {
                var availability = await CrossFingerprint.Current.GetAvailabilityAsync();
                return availability == FingerprintAvailability.Available;
                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Biometric check failed! Fingerprint does not exist : {ex.Message}");
                return false;
            }
        }

        public async Task <bool> AuthenticateUser(string reason = "Authenticate to continue")
        {
            try
            {
                var availability = await CrossFingerprint.Current.GetAvailabilityAsync();
                Debug.WriteLine($"Fingerprint found {availability}");
                Console.WriteLine(availability);


                if(availability != FingerprintAvailability.Available)
                {
                    //so if the finger print is not found in the storage collection :
                    
                    System.Diagnostics.Debug.WriteLine($"Biometric is not available: {availability}");
                    return false;
                }

                var request = new AuthenticationRequestConfiguration("Financial Savvy App", reason)
                {
                    AllowAlternativeAuthentication = true,
                    ConfirmationRequired = false,
                    FallbackTitle = "Use Device Pin/Pattern"
                };

                var result = await CrossFingerprint.Current.AuthenticateAsync(request);

                if (result.Authenticated)
                {
                    System.Diagnostics.Debug.WriteLine($"Authentication successful! : {result}");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Authentication failed!: {result.ErrorMessage}");
                    return false;
                }


            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Authentication error: {ex.Message}");
                return false;
            }

           
        }
        public async Task <bool> EnrollUser(string userId, string username)
        {
            try
            {
                //start by authenticating the users to ensure it is really them setting the login for their account:
                var authenticated = await AuthenticateUser("Set up biometric login for your account");

                if (!authenticated)
                {
                    System.Diagnostics.Debug.WriteLine($"Enrollment cancelled bc authentication failed");
                    return false;
                }

                // in here i will the stores the user details in the secure storage:
                await SecureStorage.SetAsync(USER_ID_KEY, userId);
                await SecureStorage.SetAsync(USERNAME_KEY, username);
                await SecureStorage.SetAsync(ENROLLMENT_DATE_KEY, DateTime.UtcNow.ToString("o"));

                System.Diagnostics.Debug.WriteLine($"Enrollment successful!! {username}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Enrollment failed@ : {ex.Message}");
                return false;
                
            }
        }

        public async Task <string> GetStoredUserId()
        {
            try
            {
                return await SecureStorage.GetAsync(USER_ID_KEY);
            }
            catch (Exception ex) 
            {
                System.Diagnostics.Debug.WriteLine($"Failed to retrieve user Id:{ex.Message}");
                return null;
            }
        }
        
        public async Task <string> GetStoredUsername()
        {
            try
            {
                return await SecureStorage.GetAsync(USERNAME_KEY);    
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to retrieve username: {ex.Message}");
                return null;
            }
        }

        public async Task <bool> IsUserEnrolled()
        {
            var userId = await GetStoredUserId();//so if user has been enrolled, i will return the user id key here // if u reading voetsek nya mmao!!!
            return !string.IsNullOrEmpty( userId );
        }


        public async Task ClearEnrollment()
        {
            try
            {
                SecureStorage.Remove(USER_ID_KEY);
                SecureStorage.Remove(USERNAME_KEY);
                SecureStorage.Remove(ENROLLMENT_DATE_KEY);
                System.Diagnostics.Debug.WriteLine("All enrollment has been cleared!");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Enrollment clear ed out  successfully. :{ex.Message}");
            }
        }

    }
}
