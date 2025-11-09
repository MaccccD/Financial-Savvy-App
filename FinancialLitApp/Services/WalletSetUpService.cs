using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FinancialLitApp.Services
{
    public interface IWalletSetupService
    {
        Task<WalletSetupResult> SetupWalletBiometric(string userId, string username);
        Task<bool> RequireBiometricForBlockchain(string operation);
    }



    public class WalletSetupService : IWalletSetupService
    {
        private readonly IBiometricAuthService _biometricAuth;
        private readonly IBlockchainServices _blockchain;

        public WalletSetupService()
        {
            _biometricAuth = new BiometricAuthService();
            _blockchain = new BlockchainService();
        }

        public async Task<WalletSetUpResult> SetUpWalletWithBiometric(string userId, string username)
        {
            try
            {
                var isBiometricAvailable = _biometricAuth.IsBiometricAvailable();

                if (!isBiometricAvailable)
                {
                    return new WalletSetUpResult
                    {
                        Success = false,
                        ErrorMessage = "Biometric authentication is not available on this device."
                    };
                }

                var hasWallet = await _blockchain.HasWallet();

                if (hasWallet)
                {
                    return WalletSetupResult
                }
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


      
    
}
