using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FinancialLitApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace FinancialLitApp.ViewModels
{
    public partial class BudgetingChallengeViewModel : ObservableObject
    {
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


        public ObservableCollection<BudgetingItem> AvailableExpenses { get; set; }
        public ObservableCollection<BudgetingItem> SelectedExpenses { get; set; }

        //tracking the spending of the user by category so they can get feedbakc on the quality of their budgeting skill being honed:
        private Dictionary<itemCategory, decimal> categorySpending = new Dictionary<itemCategory, decimal>();


        public BudgetingChallengeViewModel()

        {
            InitializeGame();
            return;
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
            };

            AvailableExpenses = new ObservableCollection<BudgetingItem>
            {
             //the WANTS - nice to haves :
                new BudgetingItem {Id = 1, Name = "Streaming Services", Price = 20, category = itemCategory.Want},
                new BudgetingItem {Id = 2, Name = "Restaurant Meal", Price = 60, category = itemCategory.Want},
                new BudgetingItem {Id = 3, Name = "New OOTD", Price = 100, category = itemCategory.Want},
                new BudgetingItem {Id = 4, Name = "Concert Tickets", Price = 150, category = itemCategory.Want},
                new BudgetingItem {Id = 5, Name = "Gaming", Price = 45, category = itemCategory.Want},

            };

            AvailableExpenses = new ObservableCollection<BudgetingItem>
            {
                 // the IMPULSE purchases:
               new BudgetingItem {Id = 1, Name = "Late Night Snacks", Price = 35, category = itemCategory.ImpulsePurchase},
               new BudgetingItem {Id = 2, Name = "50% of Shades", Price = 70, category = itemCategory.ImpulsePurchase},
               new BudgetingItem {Id = 3, Name = "In-App Purchases", Price = 40, category = itemCategory.ImpulsePurchase},
               new BudgetingItem {Id = 4, Name = "Coffee & Treats", Price = 110, category = itemCategory.ImpulsePurchase},
            };

            AvailableExpenses = new ObservableCollection<BudgetingItem>
            {
               // the INVESTEMENTS :
               new BudgetingItem {Id = 14, Name = "Online Course", Price = 120, category = itemCategory.Investment},
               new BudgetingItem {Id = 15, Name = "Books", Price = 90, category = itemCategory.Investment},
               new BudgetingItem {Id = 16, Name = "Gym Membership", Price = 80, category = itemCategory.Investment},
               new BudgetingItem {Id = 17, Name = "Skills Worships", Price = 110, category = itemCategory.Investment},
            };

            SelectedExpenses = new ObservableCollection<BudgetingItem>(); // so each time an available  item is slected, it gets added to the selected expense collection.

            
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
        private void CheckBudget()
        {
            var result = EvaluateBudget();
            ShowResults = true;

            var actualSavings = StartingAmount - CurrentCostsSpent;

            if (result.IsSuccess)
            {
                //shoew the user feedback that they stayed within budget and the target goal.!

                FeedbackMessage = $"Yayy!! 🥳🥳 Excellent Budgeting Skill!\n\n" +
                                  $"You spent R{CurrentCostsSpent:F0} and saved R{actualSavings:F0}," +
                                  $"meeting your target savings goal : R{TargetSavings:F0}\n\n" +
                                  $" 💡 Key Insight: Notice how planning your spending by creating a priority list allowed you to meet " +
                                  $"both needs AND savings goals. This is the real testament of budgeting in true practice!\n\n" +
                                   GetSpendingAnalysis();
                                

                IsGameActive = false;
            }
            else if(CurrentAttempt >= MaxAttempts)
                { // when the player has exceeded the available attempts:
                    FeedbackMessage = GetAttemptFinalAttempt(actualSavings);
                    isGameActive = false;
                }
            else
                {//going for another attempt :
                    currentAttempt++;
                    FeedbackMessage = GetAttemptFeedback(actualSavings);
                    ResetForNewAttempt();

                }
        }


        private BudgetResult EvaluateBudget()
        { //here i'm checking if the amount the user saved is equal to the target and whether that was a success.
            var actualSavings = StartingAmount - CurrentCostsSpent;
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
                analysis += $"Love to see it! ⭐ An investment effort made into the your future self";
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
