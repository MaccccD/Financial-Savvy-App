using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using FinancialLitApp.Services;

namespace FinancialLitApp.Handlers
{
    public class ChallengeCompletionHandler
    {
        private readonly IBlockchainServices _blockchain;
        private readonly IWalletSetupService _walletSetup;


        //the token rewards for different challenges:
        private readonly Dictionary<string, int> _tokenRewards = new()
        {
            {"savings", 50 },
            {"budgeting", 50 },
            {"taxreturns", 100 }
        };

        public ChallengeCompletionHandler()
        {
            _blockchain = new BlockchainService();
            _walletSetup = new WalletSetupService();

        }
        public async Task<ChallengeCompletionResult> HandleChallengeCompletion(
           string challengeId,
           string challengeName,
           int score,
           string challengeType)
        {
            var result = new ChallengeCompletionResult
            {
                ChallengeId = challengeId,
                Score = score
            };

            try
            {
                // Seeing if user has a blockchain wallet
                var hasWallet = await _blockchain.HasWallet();

                if (!hasWallet)
                {
                    // Offer to create wallet
                    result.NeedsWalletSetup = true;
                    result.Message = "Create a wallet to earn tokens and permanent achievement records!";
                    return result;
                }

                // Ask if user wants to record on blockchain
                var wantsBlockchain = await Application.Current.MainPage.DisplayAlert(
                    "🎉 Challenge Complete!",
                    $"Score: {score}\n\n" +
                    $"Record this achievement on blockchain?\n" +
                    $"✓ Permanent verification\n" +
                    $"✓ Earn {GetTokenReward(challengeType)} tokens",
                    "Yes, Record It",
                    "Save Locally Only");

                if (!wantsBlockchain) // so if the user does not want to record their token on the blockchain.
                {
                    result.SavedLocally = true;
                    result.Message = "Achievement saved locally";
                    return result;
                }

                // Require biometric to sign blockchain transaction
                var authenticated = await _walletSetup.RequireBiometricForBlockchain(
                    "record your achievement");

                if (!authenticated)
                {
                    result.Message = "Authentication cancelled";
                    return result;
                }

                // Create achievement record
                var achievement = new ChallengeAchievement
                {
                    ChallengeId = challengeId,
                    ChallengeName = challengeName,
                    Score = score,
                    CompletionDate = DateTime.UtcNow
                };

                // Record on blockchain
                var recorded = await _blockchain.RecordAchievement(achievement);

                if (recorded)
                {
                    // Award tokens
                    var tokenAmount = GetTokenReward(challengeType);
                    await _blockchain.AwardTokens(tokenAmount, $"Completed {challengeName}");

                    result.Success = true;
                    result.BlockchainRecorded = true;
                    result.TokensEarned = tokenAmount;
                    result.Message = $"Achievement recorded! You earned {tokenAmount} tokens.";
                }
                else
                {
                    result.Message = "Failed to record achievement";
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Challenge completion handling failed: {ex.Message}");
                result.Message = "An error occurred";
                return result;
            }
        }

        public async Task<List<ChallengeAchievement>> GetUserAchievements()
        {
            return await _blockchain.GetUserAchievements();
        }

        public async Task<decimal> GetTokenBalance()
        {
            return await _blockchain.GetTokenBalance();
        }

        private int GetTokenReward(string challengeType)
        {
            return _tokenRewards.TryGetValue(challengeType.ToLower(), out var reward)
                ? reward
                : 25; // Default reward
        }
    }

    public class ChallengeCompletionResult
    {
        public bool Success { get; set; }
        public string ChallengeId { get; set; }
        public int Score { get; set; }
        public bool BlockchainRecorded { get; set; }
        public bool SavedLocally { get; set; }
        public int TokensEarned { get; set; }
        public bool NeedsWalletSetup { get; set; }
        public string Message { get; set; }
    }

}

