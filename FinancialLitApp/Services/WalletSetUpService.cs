using System
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FinancialLitApp.Services
{

    public interface IWalletSetupService
    {
        Task<WalletSetupResult> SetupWalletWithBiometric(string userId, string username);
        Task<bool> RequireBiometricForBlockchain(string operation);
    }

    public class WalletSetupService : IWalletSetupService
    {
        //in here im creating a connection between the biometric and the wallet creation services. Ideally , before completing ; the user would have to authenticate to create a wallet
        private readonly IBiometricAuthService _biometricAuth;
        private readonly IBlockchainServices _blockchain;

        public WalletSetupService()
        {
            _biometricAuth = new BiometricAuthService();
            _blockchain = new BlockchainService();
        }

        public async Task<WalletSetupResult> SetupWalletWithBiometric(string userId, string username)
        {
            try
            {
                // Step 1: Checking if the biometric exists:
                var isBiometricAvailable = await _biometricAuth.IsBiometricAvailable();

                if (!isBiometricAvailable)
                {
                    return new WalletSetupResult
                    {
                        Success = false,
                        ErrorMessage = "Biometric authentication is not available on this device"
                    };
                }

                // Step 2: Checking if wallet already exists
                var hasWallet = await _blockchain.HasWallet();

                if (hasWallet)
                {
                    return new WalletSetupResult
                    {
                        Success = true,
                        WalletAddress = await _blockchain.GetWalletAddress(),
                        Message = "Wallet already exists"
                    };
                }

                // Step 3: Authenticate user with biometric before creating wallet
                var authenticated = await _biometricAuth.AuthenticateUser(
                    "Secure your blockchain wallet with biometric authentication");

                if (!authenticated)
                {
                    return new WalletSetupResult
                    {
                        Success = false,
                        ErrorMessage = "Biometric authentication required to create wallet"
                    };
                }

                // Step 4: Create blockchain wallet
                var walletAddress = await _blockchain.CreateWallet();

                if (string.IsNullOrEmpty(walletAddress))
                {
                    return new WalletSetupResult
                    {
                        Success = false,
                        ErrorMessage = "Failed to create blockchain wallet"
                    };
                }

                // Step 5: Get recovery phrase for backup
                var recoveryPhrase = await _blockchain.GetRecoveryPhrase();

                Debug.WriteLine($"Wallet setup complete for {username}: {walletAddress}");

                return new WalletSetupResult
                {
                    Success = true,
                    WalletAddress = walletAddress,
                    RecoveryPhrase = recoveryPhrase,
                    Message = "Wallet created successfully"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Wallet setup failed: {ex.Message}");
                return new WalletSetupResult
                {
                    Success = false,
                    ErrorMessage = $"Setup failed: {ex.Message}"
                };
            }
        }

        public async Task<bool> RequireBiometricForBlockchain(string operation)
        {
            // Always require biometric authentication for blockchain operations
            var authenticated = await _biometricAuth.AuthenticateUser(
                $"Authenticate to {operation}");

            return authenticated;
        }
    }

    public class WalletSetupResult
    {
        public bool Success { get; set; }
        public string WalletAddress { get; set; }
        public string RecoveryPhrase { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
    }




}
