using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FinancialLitApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

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
                new BudgetingItem {Id = 17, Name = "Skills Worships", Price = 110, category = itemCategory.Investment},
            };

            SelectedExpenses = new ObservableCollection<BudgetingItem>(); // so each time an avinale item is slected, it gets added to the selected expense collection.

            
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


        


    }
}
