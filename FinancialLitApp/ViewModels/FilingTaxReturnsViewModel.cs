using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using FinancialLitApp.Services;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FinancialLitApp.Handlers;
using System.Diagnostics;


namespace FinancialLitApp.ViewModels
{
    public partial class FilingTaxReturnsViewModel : ObservableObject
    {
        private readonly ChallengeCompletionHandler _completionHandler;
        //these are the given values:
        [ObservableProperty]
        private decimal grossSalaryMonthly = 28122.01m;
        [ObservableProperty]
        private decimal grossSalaryAnnual = 337464.12m;
        [ObservableProperty]
        private decimal netTakeHomePay = 22328.33m;

        [ObservableProperty]
        private decimal tokenBalance = 0m;

        [ObservableProperty]
        private bool showTokenBalance = false;

        [ObservableProperty]
        private int score = 0;

        [ObservableProperty]
        private bool isChallengeComplete = false;
        //the correct answers (calculated internally after the user inputs their answer):

        private decimal correctPAYE = 4294.83m;
        private decimal correctUIF = 177.12m;
        private decimal correctPensionFund = 1321.74m;
        private decimal correctTotalDeductions = 5793.68m;


        // the values the user inputs:

        [ObservableProperty]
        private string payeInput = "";

        [ObservableProperty]
        private string uifInput = "";

        [ObservableProperty]
        private string pensionFundInput = "";

        [ObservableProperty]
        private decimal calculatedTotal = 0m;
        //the feedback :
        [ObservableProperty]
        private string payeFeedback = "";

        [ObservableProperty]
        private string uifFeedback = "";

        [ObservableProperty]
        private string pensionFundFeedback = "";

        [ObservableProperty]
        private string totalFeedback = "";

        [ObservableProperty]
        private string overallMessage = "";

        //Tracking correctness:

        [ObservableProperty]
        private bool isPAYECorrect = false;

        [ObservableProperty]
        private bool isUIFCorrect = false;

        [ObservableProperty]
        private bool isPensionFundCorrect = false;

        [ObservableProperty]
        private bool isAllCorrect = false;

        [ObservableProperty]
        private bool showResults = false;

        [ObservableProperty]
        private int currentAttempt = 1;

        [ObservableProperty]
        private int maxAttempts = 3;

        //the formulas that users are given ( for UI binding):

        public  string PAYEFormulae => "Based on annual income:\n" +
                                       "Use Tax Bracket Formula: (R42,678 + ( 26% x (annual salary - R237,100)\n" +
                                       "Then subtract rebate value : R17,235 & divide by 12";

        public string UIFFormulae => "1% of gross monthly salary. (The maximum UIF is : R177.12 per month)";

        public string PensionFundFormulae => "4.7% of gross monthly salary";



        public FilingTaxReturnsViewModel()
        {
            _completionHandler = new ChallengeCompletionHandler();
            _ = LoadTokenBalance();
            // return;
        }

        public async Task LoadTokenBalance()
        {
            try
            {
                TokenBalance = await  _completionHandler.GetTokenBalance();
                ShowTokenBalance = TokenBalance > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Tokens failed to load. {ex.Message}");
            }
        }

        //the auto calculation as the user types:
        partial void OnPayeInputChanged(string value)
        {
            UpdateCalculatedTotal();
            
            
        }

        partial void OnUifInputChanged(string value)
        {
            UpdateCalculatedTotal();
            
        }
        partial void OnPensionFundInputChanged(string value)
        {
            UpdateCalculatedTotal();
          
        }
        private void UpdateCalculatedTotal()
        {
            decimal paye = 0, uif = 0, pensionFund = 0;

            decimal.TryParse(PayeInput, out paye); // try parse in here converts the string typed into a decimal value
            decimal.TryParse(UifInput, out uif);
            decimal.TryParse(PensionFundInput, out pensionFund);

            CalculatedTotal = paye + uif + pensionFund;

            //if (IsAllCorrect)
            //{
            //    ShowSuccessMessage();
            //}
        }



        [RelayCommand]
        private async void FileReturn()
        {
            ClearFeedback();
            // Validate inputs
            if (!ValidateInputs())
            {
                OverallMessage = "⚠️ Please enter all deduction amounts...";
                return;
            }

            // Parse user inputs (convert string to decimal)
            decimal userPAYE = decimal.Parse(PayeInput);      
            decimal userUIF = decimal.Parse(UifInput);    
            decimal userPension = decimal.Parse(PensionFundInput); 

            // Check correctness
            IsPAYECorrect = IsWithinMargin(userPAYE, correctPAYE, 10m);
            IsUIFCorrect = IsWithinMargin(userUIF, correctUIF, 10m);
            IsPensionFundCorrect = IsWithinMargin(userPension, correctPensionFund, 10m);
            IsAllCorrect = true;

            //show the total:
            UpdateCalculatedTotal();
            // ⭐ THIS IS WHERE FEEDBACK IS TRIGGERED ⭐
            GeneratePAYEFeedback(userPAYE);      
            GenerateUIFFeedback(userUIF);        
            GeneratePensionFeedback(userPension); 
            GenerateTotalFeedback();   
            
            // calculate score:

            try
            {
                var finalScore = CalculateFinalScore();

                if(finalScore < 50)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Challenge Incomplete:(",
                        $"Score too low: {finalScore}. Please Try Again to earn tokens",
                        "Okay");
                    return;
                }


                isChallengeComplete = true;

                await SaveProgressLocally(finalScore);

                await HandleBlockchainCompletion(finalScore);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Challenge completion error!:{ex.Message}");
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "An error occurred while completing the challenge!",
                    "Okay");
            }
        }

        


        private bool ValidateInputs()
        {
            decimal temp;
            return !string.IsNullOrWhiteSpace(PayeInput) && decimal.TryParse(PayeInput, out temp) &&
            !string.IsNullOrWhiteSpace(UifInput) && decimal.TryParse(UifInput, out temp) &&
            !string.IsNullOrWhiteSpace(PensionFundInput) && decimal.TryParse(PensionFundInput, out temp);

        }

        private bool IsWithinMargin(decimal userValue, decimal correctValue, decimal margin)
        {
            return Math.Abs(userValue - correctValue) <= margin; // here i'm checking if the value the users input is within the margin even when rounded of in relation to the correct value
        }
        private int CalculateFinalScore()
        {
            // Your existing logic to calculate score based on challenge performance
            int score = 0;
            // Base score for completing challenge (40 points)
            score += 40;
            
            if (correctPAYE >= 4300)
                score += 30;

            if(correctPensionFund >= 1400)
                score += 20;

            if (correctUIF >= 178)
                score += 10;

            // Bonus for completing on first attempt (10 points)
            if (currentAttempt == 1)
                score += 10;

            return Math.Min(score, 100);
        }


        //the PAYE feedback :

        private void GeneratePAYEFeedback(decimal userPAYE)
        {
            if (IsPAYECorrect)
            {
                PayeFeedback = "Great Job! Your PAYE calculation is spot on!";
            }
            else
            {
                var difference = userPAYE - correctPAYE;
                var explanation = $"Not quite :(You calculated R{userPAYE:F2}, but the correct amount is R{correctPAYE:F2}\n";

                explanation += "Here's a step-by-step guide on how you calculate PAYE correctly:\n";
                explanation += $"1. Annual Salary: R{GrossSalaryMonthly:F2} x 12 = R{GrossSalaryAnnual:F2}\n";
                explanation += $"2. This annual salary falls within the tax bracket: R237,101 - R370,500\n";
                explanation += $"3. Tax calculation: R42,678 + (26% x (R{GrossSalaryAnnual} - R237,100))\n";
                explanation += $"4. Tax before rebate: R42, 678 + (0.26 x R{GrossSalaryAnnual - 237,100:F2}) = R68,773\n";
                explanation += $"5. Subtract primary rebate : R68,773 - R17,235 = R51,538\n";
                explanation += $"6. Monthly PAYE : R51,538/12 = R{correctPAYE:F2}\n\n";


                if( difference > 0 )
                {
                    explanation += $" You were R{Math.Abs(difference):F2} too high. Did you forget to subtract the rebate?";
                }
                else
                {
                    explanation += $"You were R{Math.Abs(difference):F2} too low. Make sure you're using the correct tax bracket.";
                }
                
                PayeFeedback = explanation;  //appending the explanations as part of the feedback for the PAYE aspect of the feedback to the user.
            }
        }

        private void GenerateUIFFeedback(decimal userUIF)
        {
            if (IsUIFCorrect)
            {
                UifFeedback = " Great Job! Your calculation is very accurate!";
            }
            else
            {
                var explanation = $"Not quite. You calculated R{userUIF}, but the correct UIF amount is R{correctUIF:F2}.\n\n";

                explanation += $"Here's a step-by-step guide on how to calculate  UIF:\n";
                explanation += $"1. Calculate 1% of gross salary: R{GrossSalaryMonthly:F2} x 0.01 = R{GrossSalaryMonthly * 0.01m:F2}\n";
                explanation += $"2. UIF has a monthly cap of R177.12\n";
                explanation += $"3. Take the lower amount: min(R{GrossSalaryMonthly * 0.01m:F2}, R177.12) = R{correctUIF:F2}\n\n";


                if(userUIF > correctUIF)
                {
                    explanation += "Remember: UIF is capped at R177.12 per month, even if 1% of your salary is higher!";
                }
                else
                {
                    explanation += " Make sure you're calculating 1% of your gross monthly salary correctly.";
                }

                UifFeedback = explanation;
            }
        }

        private void GeneratePensionFeedback(decimal userPension)
        {
            if (IsPensionFundCorrect)
            {
                PensionFundFeedback = "Yayy, Great Job ! Your calculation is very spot on!";
            }
            else
            {
                var difference = userPension - correctPensionFund;
                var explanation = $"Not quite. You calculated R{userPension}, but the correct pension fund is: R{correctPensionFund:F2}\n\n";

                explanation += "Here's the correct way to calculate your pension fund correctly:\n";
                explanation += $"1. Calculate 4.7% of your gross monthly salary: R{GrossSalaryMonthly:F2} x 0.047 = R{correctPensionFund:F2}\n\n";

                if(Math.Abs(difference) < 50)
                {
                    explanation += "You're so close!! Double-check your percentage calculation( 4.7% not 5% or 10%).";

                }
                else
                {
                    explanation += "Make sure you're multiplying the gross salary with the percentage value(0.047) – which is 4.7%";
                }

                PensionFundFeedback = explanation;
            }
        }

        private void  GenerateTotalFeedback()
        {
            var userTotal = CalculatedTotal;
            var isCorrect = IsWithinMargin(userTotal, correctTotalDeductions, 15m);

            if (isCorrect)
            {
                TotalFeedback = $"Total Deductions: R{userTotal:F2} - Great Job!";
            }
            else
            {
                TotalFeedback = $"Total Deductions: R{userTotal:F2} (Should be R{correctTotalDeductions:F2})\n" +
                                $"Difference: R{Math.Abs(userTotal - correctTotalDeductions):F2}";
            }
        }
        private async Task SaveProgressLocally(int score)
        {
            try
            {
                var challengeKey = "Filing_Tax_Returns_Challenge"; 

                await SecureStorage.SetAsync($"{challengeKey}_completed", "true");
                await SecureStorage.SetAsync($"{challengeKey}_score", score.ToString());
                await SecureStorage.SetAsync($"{challengeKey}_date", DateTime.UtcNow.ToString());

                Debug.WriteLine($"Challenge progress saved locally: {score}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to save locally: {ex.Message}");
            }
        }
        private void ShowSuccessMessage()
        {
            OverallMessage = "Congratulations! Your tax return is filed correctly , YAYYYYYYYYY\n\n" +
                            $"Summary:\n" +
                            $"Gross Salary: R{GrossSalaryMonthly:F2}\n" +
                            $"Total Deductions: R{correctTotalDeductions:F2}\n" +
                            $"Net Take-Home: R{NetTakeHomePay:F2}\n\n" +
                            $"Key Challenge Take-Aways: Understanding how to calculate your tax deductions helps you: \n" +
                            $"- Verify your pay slip is correct\n" +
                            $"- Plan your finances accurately\n" +
                            $"- Maximize tax-deductible contributions (like pension)\n" +
                            $"- Avoid penalties from SARS\n\n" +
                            "You're now equipped to handle real tax filing with CONFIDENCE!";
        }

        private async Task HandleBlockchainCompletion(int score)
        {
            try
            {
                // THIS IS THE MAGIC - Handles biometric auth + blockchain recording
                var result = await _completionHandler.HandleChallengeCompletion(
                    challengeId: "Filing_Tax_Returns_3", 
                    challengeName: "Tax Returns Challenge", 
                    score: score,
                    challengeType: "tax");

                if (result.NeedsWalletSetup)
                {
                    // so if user doesn't have wallet - offer setup
                    var setupWallet = await Application.Current.MainPage.DisplayAlert(
                        "🎁 Earn Tokens & Certificates!",
                        $"Create a blockchain wallet to:\n" +
                        $"✓ Earn tokens for this challenge\n" +
                        $"✓ Get a permanent achievement certificate\n" +
                        $"✓ Build your verifiable skill portfolio",
                        "Set Up Wallet",
                        "Maybe Later");

                    if (setupWallet)
                    {
                        await Shell.Current.GoToAsync("walletsetup");
                    }
                }
                else if (result.Success && result.BlockchainRecorded)
                {
                    // SUCCESS! Achievement recorded on blockchain
                    TokenBalance = await _completionHandler.GetTokenBalance();
                    ShowTokenBalance = true;

                    await Application.Current.MainPage.DisplayAlert(
                        "🎉 Achievement Unlocked!",
                        $"Congratulations! You earned {result.TokensEarned} tokens!\n\n" +
                        $"✓ Achievement permanently recorded on blockchain\n" +
                        $"✓ You can verify this completion anytime\n\n" +
                        $"💰 Total Tokens: {TokenBalance}",
                        "Awesome!");
                }
                else if (result.SavedLocally)
                {
                    // User chose local save only
                    await Application.Current.MainPage.DisplayAlert(
                        "✓ Progress Saved",
                        "Your achievement has been saved locally.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Blockchain completion error: {ex.Message}");
                // Don't show error - challenge still completed locally
            }
        }

        private void ShowTryAgainMessage()
        {
            var correctCount = (IsPAYECorrect ? 1 : 0) + (IsUIFCorrect ? 1 : 0) + (IsPensionFundCorrect ? 1 : 0);// this is to keep track of the correct calculation of each deduction that the user has got correct.

            OverallMessage = $" Attempt {currentAttempt} of {MaxAttempts}\n\n" +
                             $"You got {correctCount} out of 3 deductions correct.\n\n" +
                             "Review the feedback above for each deduction and try again. " +
                             "Understanding these calculations is crucial for managing your finances!";
        }

        private void ShowFinalAttemptMessage()
        {
            OverallMessage = "Challenge Completed!\n\n" +
                             "While you didn't get everything correct this time, you've learned valuable lessons about tax calculations. \n\n " +
                             "The correct answers:\n" +
                             $"PAYE: R{correctPAYE:F2}\n" +
                             $"UIF: R{correctUIF:F2}\n" +
                             $"Pension Fund: R{correctPensionFund:F2}\n" +
                             $"Total deductions: R{correctTotalDeductions:F2}\n\n" +
                             "💡 Remember : Tax Filing seems complex, but breaking it down step-by-step makes it manageable. " +
                             "Review the explanations above and try attempt the challenge again to reinforce your learning!";

        }


        [RelayCommand]
        private void ShowCorrectReturn()
        {
            payeInput = correctPAYE.ToString("F2");
            uifInput = correctUIF.ToString("F2");
            pensionFundInput = correctPensionFund.ToString("F2");


            UpdateCalculatedTotal();

            OverallMessage = " Correct Return Displayed!\n\n" +
                            "This is what a correctly filed return looks like. Study  these values and the formulas to understand the calculations/\n\n" +
                            "💡 Tip: Try clearing these values and calculating the for yourself for practice.";
        }

        [RelayCommand]
        private void Reset()
        {
            payeInput = "";
            uifInput = "";
            pensionFundInput = "";
            calculatedTotal = 0m;

            ClearFeedback();

            ShowResults = false;
            CurrentAttempt = 1;
            IsPAYECorrect = false;
            IsUIFCorrect = false;
            IsPensionFundCorrect = false;
            IsAllCorrect = false;
            OverallMessage = "";
        }
        [RelayCommand]
        private async Task ViewAchievements()
        {
            try
            {
                var achievements = await _completionHandler.GetUserAchievements();

                if (achievements == null || !achievements.Any())
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "No Achievements Yet",
                        "Complete challenges to earn blockchain-verified achievements!",
                        "OK");
                    return;
                }

                var achievementList = string.Join("\n\n", achievements.Select(a =>
                    $"✓ {a.ChallengeName}\n" +
                    $"   Score: {a.Score}\n" +
                    $"   Date: {a.CompletionDate:d}"));

                await Application.Current.MainPage.DisplayAlert(
                    "🏆 Your Achievements",
                    achievementList,
                    "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"View achievements error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ViewTokenBalance()
        {
            try
            {
                var balance = await _completionHandler.GetTokenBalance();
                TokenBalance = balance;

                await Application.Current.MainPage.DisplayAlert(
                    "💰 Your Token Balance",
                    $"{balance} Tokens\n\n" +
                    "Earn more by completing challenges!\n\n" +
                    "Token Value:\n" +
                    "• Savings: 40 tokens\n" +
                    "• Budgeting: 50 tokens\n" +
                    "• Filing Tax Returns: 100 tokens\n" +
                    "• Advanced: 150 tokens",
                    "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"View balance error: {ex.Message}");
            }
        }


        private void ClearFeedback()
        {
            PayeFeedback = "";
            UifFeedback = "";
            PensionFundFeedback = "";
            TotalFeedback = "";
            
        }
    }
}
