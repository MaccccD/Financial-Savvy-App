using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using FinancialLitApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialLitApp.ViewModels
{
    public partial class SavingsChallengeViewModel : ObservableObject
    {
        [ObservableProperty]
        private decimal startingAmount = 200m;

        [ObservableProperty]
        private decimal currentlySaved;

        [ObservableProperty]
        private decimal moneySpent;

        [ObservableProperty]
        private decimal targetAmount = 100m;

        [ObservableProperty]
        private decimal currentAttempt = 1;

        [ObservableProperty]
        private decimal maxAttempts = 3;

        [ObservableProperty]
        private bool isGameActive = true;

        [ObservableProperty]
        private string feedbackMessage = "";

        [ObservableProperty]
        private bool showResults = false;


        public ObservableCollection<SavingsItem> AvailableItems { get; set; }
        public ObservableCollection<SavingsItem> SelectedItems { get; set; }


        public SavingsChallengeViewModel()
        {
            InitializeGame();
        }

        private void InitializeGame() // when the game starts , all variables are initialized to  zero 
        {
            currentlySaved = startingAmount;
            moneySpent = 0;
            currentAttempt = 1;
            isGameActive = true;
            showResults = false;
            feedbackMessage = "";


            AvailableItems = new ObservableCollection<SavingsItem>
            { // the list of items that are needs to a Gen Z
                new SavingsItem
                {
                    Id = 1,
                    Name = "Groceries",
                    Price = 60m,
                    Category = ItemCategory.Need,
                    Description = "Weekly food shopping"
                },
                new SavingsItem
                {
                    Id = 2,
                    Name = "Transport",
                    Price = 40m,
                    Category = ItemCategory.Need,
                    Description = "Monthly transport pass"
                },
                new SavingsItem
                {
                    Id = 3,
                    Name = "iPhone Bill",
                    Price = 25m,
                    Category = ItemCategory.Need,
                    Description = "Monthly phone contract"
                },
                new SavingsItem
                {
                    Id = 4,
                    Name = "Rent Portion",
                    Price = 80m,
                    Category = ItemCategory.Need,
                    Description = "Monthly accommodation cost"
                },
                new SavingsItem
                {
                    Id = 5,
                    Name = "Church Conferences & Tithes",
                    Price = 100m,
                    Category = ItemCategory.Need,
                    Description = "Religious Cheerful Giving"
                },
                //wants items- all lifestyle items for a Gen Z :
                new SavingsItem
                {
                    Id = 6,
                    Name = "Streaming Services",
                    Price = 15m,
                    Category = ItemCategory.Want,
                    Description = "Netflix,HBO,AppleMusic,Spotify, etc."
                },
                new SavingsItem
                {
                    Id = 7,
                    Name = "Coffee & Snacks",
                    Price = 30m,
                    Category = ItemCategory.Want,
                    Description = "Daily coffee and treats over brunch"
                },
                new SavingsItem
                {
                    Id = 8,
                    Name = "New Clothes",
                    Price = 50m,
                    Category = ItemCategory.Want,
                    Description = "Fashion and accessories. Clearing Shein carts"
                },
                new SavingsItem
                {
                    Id = 9,
                    Name = "Gaming",
                    Price = 35m,
                    Category = ItemCategory.Want,
                    Description = "Games and in-app purchases/monthly subscriptions"
                },
                new SavingsItem
                {
                    Id = 10,
                    Name = "Social Events/Groove",
                    Price = 45m,
                    Category = ItemCategory.Want,
                    Description = "Movies, parties, dining out, Pantone Sundays"
                },
                new SavingsItem
                {
                    Id = 11,
                    Name = "Tech Gadgets",
                    Price = 70m,
                    Category = ItemCategory.Want,
                    Description = "Latest accessories and upgrades"
                },
                new SavingsItem
                {
                    Id = 12,
                    Name = "Face Beat & Frontal Wig Install",
                    Price = 120m,
                    Category = ItemCategory.Want,
                    Description = "Beauty Customization"
                }
            };
            SelectedItems = new ObservableCollection<SavingsItem>(); //  meaning each time an item s slected from the available , it gets stored in the list of Savings Items
        }

        
        


    }
}
