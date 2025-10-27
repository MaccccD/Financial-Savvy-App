using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System.Text.Json;
using System.Reflection.Metadata;

namespace FinancialLitApp.Services
{
    public interface IBiometricAuthService
    {
        //okay so these are the different tasks that would encapsulate the biometric authentication of the user for the first time when using the app:
        Task<bool> IsBiometricAvailable();
        Task<bool> AuthenticateUser(string reason = "Authenticate to continue");
        Task<bool> EnrollUser(string userId, string username);
        Task<bool> GetStoredUserId();
        Task<bool> GetStoredUsername();
        Task<bool> IsUserEnrolled();
        Task ClearEnrollment();
    }
  public   class BiometricAuthService : IBiometricAuthService
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
    }
}
