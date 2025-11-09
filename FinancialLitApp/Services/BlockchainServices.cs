using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using System.Text.Json;
using System.Diagnostics;

namespace FinancialLitApp.Services
{
    public interface IBlockchainServices
    {// these are the different tasks that the user would do as they set up their wallet
        Task<bool> HasWallet();
        Task<string> CreateWallet();
        Task<string> GetWalletAddress();
        Task<decimal> GetTokenBalance();
        Task<bool> RecordAchievement(ChallengeAchievement achievement);
        Task<bool> AwardTokens(int amount, string reason);
        Task<List<ChallengeAchievement>> GetUserAchievements();
        Task<string> GetRecoveryPhrase();
    }

    public class BlockchainService : IBlockchainServices
    {
        private const string WALLET_KEY = "blockchain_wallet_key";
        private const string WALLET_ADDRESS_KEY = "blockchain_wallet_address";
        private const string RECOVERY_PHRASE_KEY = "blockchain_recovery_phrase";
        private const string ACHIEVEMENTS_KEY = "blockchain_achievements";
        private const string TOKEN_BALANCE_KEY = "blockchain_token_balance";

        private Account _account;
        private Web3 _web3;

        //so ideally you actually connect to a real blockchain network.
        //but for now im gonna be simulating the blockchain using a testnet

        private const string TESTNET_URL = "https://sepolia.infura.io/v3/YOUR_INFURA_KEY";

        public async Task<bool> HasWallet()
        {
            try
            {
                //storing the wallet key in the frameworks storage
                var privateKey = await SecureStorage.GetAsync(WALLET_KEY);
                return !string.IsNullOrEmpty(privateKey);
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> CreateWallet()
        {
            try
            {
                //generate new Ethereum acc:
                var ecKey = EthECKey.GenerateKey();
                var privateKey = ecKey.GetPrivateKeyAsBytes().ToHex();
                var account = new Account(privateKey);

                // Generate recovery phrase (mnemonic)
                var recoveryPhrase = GenerateRecoveryPhrase();

                // Store in the storage (encrypted by device)
                await SecureStorage.SetAsync(WALLET_KEY, privateKey);
                await SecureStorage.SetAsync(WALLET_ADDRESS_KEY, account.Address);
                await SecureStorage.SetAsync(RECOVERY_PHRASE_KEY, recoveryPhrase);
                await SecureStorage.SetAsync(TOKEN_BALANCE_KEY, "0");

                Debug.WriteLine($"Wallet has been created: {account.Address}");

                return account.Address;
            }
            catch(Exception ex)
            {
               Debug.WriteLine($"Wallet creation failed{ex.Message}");
                return null;
            }
        }

        public async Task<string> GetWalletAddress()
        {
            try
            {
                return await SecureStorage.GetAsync(WALLET_ADDRESS_KEY);
            }
            catch
            {
                return null;
            }
        }

        public async Task <decimal> GetTokenBalance()
        {
            try
            {
                var balanceStr = await SecureStorage.GetAsync(TOKEN_BALANCE_KEY);
                return decimal.TryParse(balanceStr, out var balance) ? balance : 0;
            }
            catch
            {
                return 0;
            }
        }


        public async Task<bool> RecordAchievement(ChallengeAchievement achievement)
        {
            try
            {
                await InitializeAccount();
                // Create achievement record
                achievement.WalletAddress = _account.Address;
                achievement.Timestamp = DateTime.UtcNow;
                achievement.BlockchainHash = await SignAchievement(achievement);

                // Store locally
                var achievements = await GetUserAchievements();
                achievements.Add(achievement);
                await SaveAchievements(achievements);

                Debug.WriteLine($"Achievement recorded: {achievement.ChallengeId}");
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Achievement failed to record:{ex.Message}");
                return false;
            }
        }


        public async Task <bool> AwardTokens(int amount, string reason)
        {
            try
            {
                var currentBalance = await GetTokenBalance();
                var  newBalance = currentBalance + amount;

                await SecureStorage.SetAsync(TOKEN_BALANCE_KEY, newBalance.ToString());

                // Record token transaction
                var transaction = new TokenTransaction
                {
                    Amount = amount,
                    Reason = reason,
                    Timestamp = DateTime.UtcNow,
                    NewBalance = newBalance
                };

                Debug.WriteLine($"Tokens awarded: {amount} ({reason})");
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Failed to award tokens: {ex.Message}");
                return false;
            }
        }



        public async Task <List<ChallengeAchievement>> GetUserAchievements()
        {
            try
            {
                var achievementsJson = await SecureStorage.GetAsync(ACHIEVEMENTS_KEY);
                if (string.IsNullOrEmpty(achievementsJson))
                    return new List<ChallengeAchievement>();

                return JsonSerializer.Deserialize<List<ChallengeAchievement>>(achievementsJson)
                    ?? new List<ChallengeAchievement>();
            }
            catch
            {
                return new List<ChallengeAchievement>();
            }
        }

        public async Task<string> GetRecoveryPhrase()
        {
            try
            {
                return await SecureStorage.GetAsync(RECOVERY_PHRASE_KEY);
            }
            catch
            {
                return null;
            }
        }

        //private helper methods:
        private async Task InitializeAccount()
        {
            if (_account != null) return;

            var privateKey = await SecureStorage.GetAsync(WALLET_KEY);
            if (string.IsNullOrEmpty(privateKey))
                throw new InvalidOperationException("No wallet found!!");

            _account = new Account(privateKey); 
          //  _web3 = new Web3(_account, TESTNET_URL); // Registering the web3 account based on the found wallet key 
        }

        private async Task<string> SignAchievement(ChallengeAchievement achievement)
        {
            await InitializeAccount();

            //create a has of the achievement data 
            var achievementData = JsonSerializer.Serialize(new
            {
                achievement.ChallengeId,
                achievement.Score,
                achievement.ChallengeName,
                achievement.CompletionDate,
                achievement.WalletAddress
            });


            //sign the data w a private key :
            var signer = new EthereumMessageSigner();
            var signature = signer.EncodeUTF8AndSign(
                achievementData,
                new EthECKey(_account.PrivateKey));

            return signature;
        }
        private async Task SaveAchievements(List<ChallengeAchievement> achievements)
        {
            var json = JsonSerializer.Serialize(achievements);
            await SecureStorage.SetAsync(ACHIEVEMENTS_KEY, json);
        }

        private string GenerateRecoveryPhrase()
        {
            // Simple 12-word recovery phrase generator
            // In production, use BIP39 standard mnemonic generation
            var words = new[]
            {
                "apple", "banana", "cherry", "dragon", "eagle", "falcon",
                "garden", "harmony", "island", "jungle", "kingdom", "legend",
                "mountain", "nature", "ocean", "palace", "queen", "river",
                "sunset", "thunder", "universe", "victory", "wisdom", "xenon"
            };

            var random = new Random();
            var phrase = new List<string>();

            for (int i = 0; i < 12; i++)
            {
                phrase.Add(words[random.Next(words.Length)]);
            }

            return string.Join(" ", phrase);
        }



    }

    // Data models
    public class ChallengeAchievement
    {
        public string ChallengeId { get; set; }
        public string ChallengeName { get; set; }
        public int Score { get; set; }
        public DateTime CompletionDate { get; set; }
        public string WalletAddress { get; set; }
        public DateTime Timestamp { get; set; }
        public string BlockchainHash { get; set; } // Signature proving authenticity
    }

    public class TokenTransaction
    {
        public int Amount { get; set; }
        public string Reason { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal NewBalance { get; set; }
    }
}
