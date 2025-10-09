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
        private decimal currentCost;
        [ObservableProperty]
        private decimal targetAmount = 250m;

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


        public ObservableCollection<BudgetingItem> SelectedItems { get; set; }


        public BudgetingChallengeViewModel()

        {
            InitializeGame();

        }


        public void InitializeGame()
        {
            if (!IsGameActive)
            {
                return;
            }

        }
    }
}
