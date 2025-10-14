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
            };

        }
    }
}
