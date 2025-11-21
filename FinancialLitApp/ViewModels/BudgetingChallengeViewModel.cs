using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FinancialLitApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using FinancialLitApp.Handlers;
using System.Diagnostics;

namespace FinancialLitApp.ViewModels
{
    public partial class BudgetingChallengeViewModel : ObservableObject
    {
        private readonly ChallengeCompletionHandler _completionHandler;
        [ObservableProperty]
        private decimal startingAmount = 500m;

        [ObservableProperty]
        private decimal currentCostsSpent = 0m;

        [ObservableProperty]
        private decimal remainingBudget;
 
        [ObservableProperty]
        private decimal targetSavings = 250m;

        [ObservableProperty]
        private int currentAttempt = 1;

        [ObservableProperty]
        private int maxAttempts = 3;

        [ObservableProperty]
        private bool isGameActive = true;

        [ObservableProperty]
        private string feedbackMessage = "";

        [ObservableProperty]
        private bool showResults = false;

        [ObservableProperty]
        private string warningMessage = "";

        [ObservableProperty]
        private decimal tokenBalance = 0m;

        [ObservableProperty]
        private bool showTokenBalance = false;

        public ObservableCollection<BudgetingItem> AvailableExpenses { get; set; }
        public ObservableCollection<BudgetingItem> SelectedExpenses { get; set; }

        // So i separated the collections for each category - these will only contain items for that category
        public ObservableCollection<BudgetingItem> NeedItems { get; set; }
        public ObservableCollection<BudgetingItem> WantItems { get; set; }
        public ObservableCollection<BudgetingItem> ImpulsePurchaseItems { get; set; }
        public ObservableCollection<BudgetingItem> InvestmentItems { get; set; }

        //tracking the spending of the user by category so they can get feedback on the quality of their budgeting skill being honed:
        private Dictionary<itemCategory, decimal> categorySpending = new Dictionary<itemCategory, decimal>();


        public BudgetingChallengeViewModel()

        {
            _completionHandler = new ChallengeCompletionHandler();
            InitializeGame();
            _ = LoadTokenBalance();
        }

        public async Task LoadTokenBalance()
        {
            try
            {
                TokenBalance = await _completionHandler.GetTokenBalance();
                ShowTokenBalance = TokenBalance > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load token balance{ex.Message}");
            }
        }


        public void InitializeGame()
        {
            CurrentCostsSpent = 0m;
            RemainingBudget = StartingAmount - TargetSavings; // R250 available to spend
            CurrentAttempt = 1;
            IsGameActive = true;
            ShowResults = false;
            FeedbackMessage = "";
            WarningMessage = "";

            //initialize the category spending:
            categorySpending = new Dictionary<itemCategory, decimal>
            {
                {itemCategory.Need, 0 },
                {itemCategory.Want, 0},
                {itemCategory.ImpulsePurchase, 0},
                {itemCategory.Investment, 0},
            };

            // a realistic Gen Z expense list with their categories:

            AvailableExpenses = new ObservableCollection<BudgetingItem>
            {
                // THE NEED items 
                new BudgetingItem {Id = 1, Name = "Groceries", Price = 80, category = itemCategory.Need},
                new BudgetingItem {Id = 2, Name = "Transport Pass", Price = 50, category = itemCategory.Need},
                new BudgetingItem {Id = 3, Name = "iPhone Bill", Price = 30, category = itemCategory.Need},
                new BudgetingItem {Id = 4, Name = "Toiletries", Price = 25, category = itemCategory.Need},  



                //the WANTS - nice to haves :
                new BudgetingItem {Id = 5, Name = "Streaming Services", Price = 20, category = itemCategory.Want},
                new BudgetingItem {Id = 6, Name = "Restaurant Meal", Price = 60, category = itemCategory.Want},
                new BudgetingItem {Id = 7, Name = "New OOTD", Price = 100, category = itemCategory.Want},
                new BudgetingItem {Id = 8, Name = "Concert Tickets", Price = 150, category = itemCategory.Want},
                new BudgetingItem {Id = 9, Name = "Gaming", Price = 45, category = itemCategory.Want},


                // the IMPULSE purchases:
                new BudgetingItem {Id = 10, Name = "Late Night Snacks", Price = 35, category = itemCategory.ImpulsePurchase},
                new BudgetingItem {Id = 11, Name = "50% of Shades", Price = 70, category = itemCategory.ImpulsePurchase},
                new BudgetingItem {Id = 12, Name = "In-App Purchases", Price = 40, category = itemCategory.ImpulsePurchase},
                new BudgetingItem {Id = 13, Name = "Coffee & Treats", Price = 110, category = itemCategory.ImpulsePurchase},


                // the INVESTEMENTS :
                new BudgetingItem {Id = 14, Name = "Online Course", Price = 120, category = itemCategory.Investment},
                new BudgetingItem {Id = 15, Name = "Books", Price = 90, category = itemCategory.Investment},
                new BudgetingItem {Id = 16, Name = "Gym Membership", Price = 80, category = itemCategory.Investment},
                new BudgetingItem {Id = 17, Name = "Skills Workshops", Price = 110, category = itemCategory.Investment},
            };

            SelectedExpenses = new ObservableCollection<BudgetingItem>(); // so each time an available item is selected, it gets added to the selected expense collection.

            // appending the separate item category collection to the budget item:
            NeedItems = new ObservableCollection<BudgetingItem>();
            WantItems = new ObservableCollection<BudgetingItem>();
            ImpulsePurchaseItems = new ObservableCollection<BudgetingItem>();
            InvestmentItems = new ObservableCollection<BudgetingItem>();

            // and then populate each category into the available items by using switch case that add each item according to its category in available expenses.

            foreach ( var item in AvailableExpenses)
            {
                switch (item.category)
                {
                    case itemCategory.Need:
                        NeedItems.Add(item);
                        break;

                    case itemCategory.Want:
                        WantItems.Add(item);
                        break;
                    case itemCategory.ImpulsePurchase:
                        ImpulsePurchaseItems.Add(item);
                        break;

                    case itemCategory.Investment:
                        InvestmentItems.Add(item);
                        break;
                }
            }

            
        }

        [RelayCommand]
        private void SpendOnItem(BudgetingItem item)
        {
            if(!IsGameActive || item.isSelected) return;

            //checking if spending on item would exceed the remaining budget:

            var budgetLeft = RemainingBudget - CurrentCostsSpent;
            if(item.Price > budgetLeft)
            {
                WarningMessage = $"You cannot afford this item. You only have R{budgetLeft:F0} left in your budget.";
                return;
            }

            //if an item is selected, it then becomes hidden from the available list, just like how buying an item from the shop means the stop loses that specific item for real money:
            item.isSelected = true;

            SelectedExpenses.Add(item);

            CurrentCostsSpent += item.Price;

            // category spending :
            categorySpending[item.category] += item.Price; // s here i'm just tracking the category the item spent on is from:

            //UpdateRealTimeFeedback();
                        
        }

        private void RealTimeFeedback() // feedback based on what remaining amount oa player has in 
        {
            var remaining = RemainingBudget - CurrentCostsSpent;

            if(remaining == 0)
            {
                WarningMessage = $"Perfect! 🎯 You've used your entire budget. Ready to see the result of your choices ?";
            }
            else if (remaining < 50)
            {
                WarningMessage = $"Warning! You oly have R{remaining:F0} left in your budget.";
            }
            else if (remaining < 100)
            {
                WarningMessage = $"You have R{remaining} left to spend while staying within budget";
            }
            else {
                WarningMessage = $"Looking very good buddy! Your Remaining budget is :  R{remaining:F0}";
            }

            //additional feedback based on spending made on impulse purchases and investments:

            var impulsePurchases = categorySpending[itemCategory.ImpulsePurchase];
            var investment = categorySpending[itemCategory.Investment];

            if(impulsePurchases > investment & impulsePurchases > 0)
            {
                FeedbackMessage = $"Just a Tip 💡: You're spending a lot more on items that aren't necessary. Try refactor your priorities.! ";
            }
            if(investment > 100)
            {
                FeedbackMessage = $"Great job. You're investing a lot more into your future ! Clock it !";

            }
            else if(categorySpending[itemCategory.Need] < 100)
            {
                FeedbackMessage = $"Pro Tip 💡: You might wanna make sure that you cover the things YOU NEED FIRST before you spend on the 'nice to haves':)";
            }
            else
            {
                FeedbackMessage = "";
            }
        }
        [RelayCommand]
        private async Task CheckBudget()
        {
            var actualSavings = StartingAmount - CurrentCostsSpent;
            if (actualSavings == 500)
            {
                Shell.Current.DisplayAlert("You need to actually spend on an items before you check your budget", "Okay", "");
            }//checking if the person actually spent before giving results!
            
            var result = EvaluateBudget();
            ShowResults = true;

          //  var actualSavings = StartingAmount - CurrentCostsSpent;

           
            if (result.IsSuccess)
            {
                await SaveProgressLocally(result);

                int score = CalculateScore(result);
                //show the user feedback that they stayed within budget and the target goal.!

                FeedbackMessage = $"Yayy!! 🥳🥳 Excellent Budgeting Skill!\n\n" +
                                  $"You spent R{CurrentCostsSpent:F0} and saved R{actualSavings:F0}," +
                                  $"meeting your target savings goal : R{TargetSavings:F0}\n\n" +
                                  $" 💡 Key Insight: Notice how planning your spending by creating a priority list allowed you to meet " +
                                  $"both needs AND savings goals. This is the real testament of budgeting in true practice!\n\n" +
                                   GetSpendingAnalysis();
                                

                IsGameActive = false;

                //now handle the blockchain recording of the wallet creation and earning tokens:
                await HandleBlockchainCompletion(score);
            }
            else if(CurrentAttempt >= MaxAttempts)
                { // when the player has exceeded the available attempts:
                    FeedbackMessage = GetAttemptFinalAttempt(actualSavings);
                    isGameActive = false;

                //save progress locally as well:
                 await SaveProgressLocally(result);
                }
            else
                {//going for another attempt :
                    CurrentAttempt++;
                    FeedbackMessage = GetAttemptFeedback(actualSavings);
                    ResetForNewAttempt();
                }
        }
        private async Task HandleBlockchainCompletion(int score)
        {
            try
            {
                // Handle blockchain recording with biometric authentication
                var completionResult = await _completionHandler.HandleChallengeCompletion(
                    challengeId: "budgeting_challenge_1",
                    challengeName: "Budgeting Basics",
                    score: score,
                    challengeType: "budgeting");

                if (completionResult.NeedsWalletSetup)
                {
                    // User doesn't have a wallet - offer to set one up
                    var setupWallet = await Application.Current.MainPage.DisplayAlert(
                        "🎁 Earn 100 Tokens & Certificates!",
                        "Create a blockchain wallet to:\n" +
                        "✓ Earn 100 tokens for this challenge\n" +
                        "✓ Get a permanent achievement certificate\n" +
                        "✓ Build your verifiable skill portfolio",
                        "Set Up Wallet",
                        "Maybe Later");

                    if (setupWallet)
                    {
                        //await Shell.Current.GoToAsync("walletsetup");
                        await Shell.Current.DisplayAlert("Wallet Set Up successfully!","Okay", "");
                    }
                }
                else if (completionResult.Success && completionResult.BlockchainRecorded)
                {
                    // SUCCESS - Achievement recorded on blockchain!
                    TokenBalance = await _completionHandler.GetTokenBalance();
                    ShowTokenBalance = true;

                    await Application.Current.MainPage.DisplayAlert(
                        "🎉 Achievement Unlocked!",
                        $"Congratulations! You earned {completionResult.TokensEarned} tokens!\n\n" +
                        $"✓ Achievement permanently recorded on blockchain\n" +
                        $"✓ You can now verify this completion anytime\n\n" +
                        $"💰 Total Tokens: {TokenBalance}",
                        "Awesome!");
                }
                else if (completionResult.SavedLocally)
                {
                    // User chose to save locally only
                    await Application.Current.MainPage.DisplayAlert(
                        "✓ Progress Saved",
                        "Your achievement has been saved locally.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Blockchain completion error: {ex.Message}");
              
            }
        }

        private int CalculateScore(BudgetResult result)
        {//in here i am creating score points based on tbe user's performance in the challenge and decisions made 
            int score = 0;

            // Base score for completing challenge (40 points)
            score += 40;

            // Bonus for staying within budget (20 points)
            if (result.AmountSaved >= result.TargetSavings)
                score += 20;

            // Bonus for good spending priorities (20 points)
            if (categorySpending[itemCategory.Need] > categorySpending[itemCategory.Want])
                score += 10;

            if (categorySpending[itemCategory.ImpulsePurchase] < 50)
                score += 10;

            // Bonus for investment spending (10 points)
            if (categorySpending[itemCategory.Investment] > 0)
                score += 10;

            // Bonus for completing on first attempt (10 points)
            if (result.AttemptsUsed == 1)
                score += 10;

            return Math.Min(score, 100); // Cap at 100
        }
        private async Task SaveProgressLocally(BudgetResult result)
        {
            try
            {
                await SecureStorage.SetAsync("budgeting_challenge_completed", "true");
                await SecureStorage.SetAsync("budgeting_challenge_score", CalculateScore(result).ToString());
                await SecureStorage.SetAsync("budgeting_challenge_date", DateTime.UtcNow.ToString());
                await SecureStorage.SetAsync("budgeting_challenge_attempts", result.AttemptsUsed.ToString());

                Debug.WriteLine($"Challenge progress saved locally");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to save locally: {ex.Message}");
            }
        }



        private BudgetResult EvaluateBudget()
        { //here i'm checking if the amount the user saved is equal to the target and whether that was a success.
            
            var actualSavings = StartingAmount - CurrentCostsSpent;
            if (actualSavings == 500)
            {
                Shell.Current.DisplayAlert("You need to actually spend on an items before you check your budget", "Okay", "");
            }//checking if the person actually spent before giving results!

            var isSuccess = actualSavings >= TargetSavings;// when the savings are equal to the set target

            return new BudgetResult
            {
                IsSuccess = isSuccess,
                AmountSpent = CurrentCostsSpent,
                AmountSaved = actualSavings,
                TargetSavings = TargetSavings,
                AttemptsUsed = CurrentAttempt,
                ExpensesSelected = new List<BudgetingItem>(SelectedExpenses)
            }; 
        }


        private String GetSpendingAnalysis()
        {
            var analysis = "📊 Your Spending Breakdown:\n";
            analysis += $"Needs: {categorySpending[itemCategory.Need]:F0}\n\n";
            analysis += $"Wants: {categorySpending[itemCategory.Want]:F0}\n\n";
            analysis += $"Impulse Purchases: {categorySpending[itemCategory.ImpulsePurchase]:F0}\n\n";
            analysis += $"Investment: {categorySpending[itemCategory.Investment]:F0}\n\n";

            //some key insights:

            if (categorySpending[itemCategory.Need] > categorySpending[itemCategory.Want])
            {
                analysis += $"🫰 Very great spending decisions here! You prioritized what you NEED over what you WANT. - smart budgeting!";
            }
            if (categorySpending[itemCategory.ImpulsePurchase] > 50)
            {
                analysis += $"\n Maybe consider how some impulse purchases could have been avoided.";
            }
            if (categorySpending[itemCategory.Investment] > 50)
            {
                analysis += $" Love to see it! ⭐ An investment effort made into the your future self.";
            }

            return analysis; 


        }

        private string GetAttemptFeedback(decimal actualSavings)
        {
            var overspent = CurrentCostsSpent - (StartingAmount - TargetSavings);

            var feedback = $"Attempt {CurrentAttempt - 1} Complete!\n\n";
            feedback += $"You Spent: R{CurrentCostsSpent:F0}, but you could only spend : R{RemainingBudget:F0}" +
                        $"to reach your savings goal.\n\n";

            //category spending insight:
            if (categorySpending[itemCategory.ImpulsePurchase] > 0)
            {
                feedback += $"💡Pro Tip: You spent R{categorySpending[itemCategory.ImpulsePurchase]:F0)}" +
                            $"on impulse purchases. Maybe try cutting down on these items to stay within budget.";

             
            }
            if (categorySpending[itemCategory.Want]> 100)
            {
                feedback += $"💡 Pro Tip: You spent R{categorySpending[itemCategory.Want]:F0}" +
                            $"on Wants. Try to send less on things you WANT over things you NEED. You can survive without WANTS.\n\n";
            }

            feedback += $"Attempt: {CurrentAttempt} of {MaxAttempts}.";
            return feedback;
        }

        private String GetAttemptFinalAttempt(decimal actualSavings)
        {

            var feedback = "Challenge Complete!  Great Learning Experience!\n\n";

            feedback += $"While you didn't hit the exact target , you learned valuable budgeting lessons!.Here is a recap of your final results:";
            feedback += $"Final Results: \n";
            feedback += $"- Started With : R{StartingAmount:F0}\n";
            feedback += $"- You spent : R{CurrentCostsSpent:F0}\n";
            feedback += $"- You saved: R{actualSavings:F0}\n";

            feedback += GetSpendingAnalysis();

            feedback += "\n\n 💡 Remember: Budgeting is about making conscious choices. " +
                        "Each attempt taught you more more about balancing needs, wants , and savings!";


            return feedback;

        }

        [RelayCommand]
        private void ResetChallenge()
        {
            InitializeGame();
        }

        private void ResetForNewAttempt()
        {
            foreach( var expense in AvailableExpenses)
            {
                expense.isSelected = false; //so unselect it and then it un-hides..
            }

            SelectedExpenses.Clear();
            CurrentCostsSpent = 0m;
            CurrentAttempt = 1;
            WarningMessage = "";
            FeedbackMessage = "";

            foreach(var key in categorySpending.Keys.ToList())
            {
                categorySpending[key] = 0; // resetting the items that belong in each category
            }
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






        ///helper lass for results:
        public class BudgetResult
        {
            public bool IsSuccess { get; set; }
            public decimal AmountSpent { get; set; }
            public decimal AmountSaved { get; set; }

            public decimal TargetSavings { get; set; }

            public int AttemptsUsed { get; set; }
            public List<BudgetingItem> ExpensesSelected { get; set; }
        }
    }


}
