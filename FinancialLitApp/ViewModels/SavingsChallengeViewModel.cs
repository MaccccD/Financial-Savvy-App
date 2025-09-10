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
        private int currentAttempt = 1;

        [ObservableProperty]
        private int maxAttempts = 3;

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
            CurrentlySaved = StartingAmount;
            MoneySpent = 0;
            CurrentAttempt = 1;
            IsGameActive = true;
            ShowResults = false;
            FeedbackMessage = "";


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
                new SavingsItem
                {
                    Id = 6,
                    Name = "Car Insurance",
                    Price = 100m,
                    Category = ItemCategory.Need,
                    Description = "In case of accidents that occur on the road."

                }
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

        [RelayCommand] // this command is used to bind data such as the UI  to methods in the View Models without doreclty refrencing the View from the View Model
        private void SelectItem(SavingsItem item)
        {
            if (!IsGameActive) return; // exit the function when the game is not active !

            //toggle selection between selected items :
            item.IsSelected =  !item.IsSelected;

            if (item.IsSelected)
            {
                SelectedItems.Add(item); // add item that has been slected to the list
                MoneySpent += item.Price; //  take away the amount of money the item costs from the money that you had to spend
                Console.WriteLine(MoneySpent);
            }
            else
            {
                SelectedItems.Remove(item);// remove the ote from the list if was not selcted;
                MoneySpent -= item.Price;
            }

            //update the value of the currently saved amount after spending, directly deonstrating Versl's theory n action:
            CurrentlySaved = StartingAmount - MoneySpent;
            Console.WriteLine(CurrentlySaved);

            UpdateRealTimeFeedback(); // the moment where Hot reload works best, uodating UI data innreal time as changes occur.
        }


        [RelayCommand]
        private void CheckResult()
        {
            var result = EvaluateChallenge();
            ShowResults = true;

            if (result.IsSuccess)
            {
                FeedbackMessage = $"🎉Congratulations! You've managed to successfully save R{currentlySaved:F2}" +
                                  $"by making conscious spending choices. /n/n" +
                                  $"💡Learning Insight : Notice how your active decision to prioritize and " +
                                  $"limit spending activity directly built your saving ability. Each choice you made " +
                                  $"strengthened your financial discipline. This is exactly how real world " +
                                  $"savings enhances financial literacy through practice!";
                IsGameActive = false;

            }

            else if (CurrentAttempt >= MaxAttempts)
            {
                //if the user has exeeded the number of attempts they have , reuslting in failure:
                FeedbackMessage = GetFailureFeedback(result);
                IsGameActive = false;
            }
            else
            {
                //give theuser another opportunity to try the challenge again :
                CurrentAttempt++;
                FeedbackMessage = GetAttemptFeedback(result);
                ResetForNewAttempt();
            }
        }

        [RelayCommand]
        private void ResetChallenge()
        {
            InitializeGame();

        }
        private void UpdateRealTimeFeedback() // show the messages needed to display based on the user prgress towards their target savings goals vs what they currently saved based on their spending habits
        {
            if(CurrentlySaved == TargetAmount)
            {
                FeedbackMessage = $"Perfect!! You've hit your target. Click 'Check Result' to complete the challenge.";

            }
            else if(CurrentlySaved < TargetAmount)
            {
                var overspent = TargetAmount - CurrentlySaved; // to determine overspending, i am checking the difference between what was the target amount and the currrently saved amount
                FeedbackMessage = $"⚠️ You've spent R{overspent:F2} too much. Consider removing some 'wants' to reach your saving's goal.";

            }
            else
            {
                var undersaved = CurrentlySaved - TargetAmount;
                FeedbackMessage = $"💰 Great work man ! You're R{undersaved:F2} above your target. You can afford to get your self a small treat , just as a token of appreciation:)";
            }
        }

        private ChallengeResult EvaluateChallenge()
        {
            var result = new ChallengeResult
            {
                //here i'm defining what constitutes a good savings challenge game or a successfull game:

                IsSuccess = CurrentlySaved >= TargetAmount, // the mney the user has saved has to exceed what was the target savings goal
                AmountSaved = TargetAmount,
                TargetAmount = TargetAmount,
                AttemptsUsed = CurrentAttempt,
                ItemsSelected = new List<SavingsItem>(SelectedItems)

            };
            return result;
        }

        private string GetAttemptFeedback(ChallengeResult result)
        {
            var needSelected = SelectedItems.Where(i => i.Category == ItemCategory.Need).ToList();
            var wantsSelected = SelectedItems.Where(i => i.Category == ItemCategory.Want).ToList();

            var feedback = $"Attempt {CurrentAttempt - 1} Complete!/n/n";

            if(CurrentlySaved < TargetAmount)
            {
                feedback += $"💡 You spent R{MoneySpent:F2} but needed to save R{TargetAmount:F2}.";

                if (wantsSelected.Any())
                {
                    var wantsTotal = wantsSelected.Sum(i => i.Price); // here i'm ca;cumating the su of wants that have been selected that contrbited to not meeting the thategt savings goal to help give users insght on what went wrong or where they need to reduce :
                    feedback += $"/n/n Strategy Tip: You selected R{wantsTotal:F2} worth of 'wants." +
                               $"Try reducing these optional purchases while keeping your essential 'needs." +
                               $"Remember: every spending choice you make directly impacts your savings ability. Spending a lot on 'wants' subsequently affects your savings amount drastically.";
                }
                else
                {
                    feedback += $"/n/n You focused on needs. Which is smart! However, try reducing quantities or " +
                                $"finding alternatives to reach your savings target.";

                }
            }

            feedback += $"/n/n Attempt {CurrentAttempt} of {MaxAttempts} - You've got this! Let's goo!";
            return feedback ;

        }

        private string GetFailureFeedback(ChallengeResult result)
        {
            var feedback = "Challenge Complete - Learning Opportunity!";

            feedback += $"Hey! So while you didn't reach the exact target this time, you've experienced first hand " +
                        $"how spending decisions directly impact in your ability to save. This practice itself " +
                        $"is buliding your financial literacy. You have grasped the key insght of what was the point of this challenge and well done for that !";

            feedback += $"💡 Key Insight: Notice how actively managing your spending choices made you more" +
                         $"aware of the relationship between consumption and saving. This awareness is a crucial " +
                         $"financial skill you've just developed through practice./n/n";

            var needsVsWants = AnalyzeSpendingPattern();
            feedback += needsVsWants;

            return feedback ;
        }

        private string AnalyzeSpendingPattern()
        {
            //below i'm basically calculating the amont of money spent on items that were categorised as needs or wants
            var needsSpent = SelectedItems.Where(i => i.Category == ItemCategory.Need).Sum(i => i.Price);//meant to return a sum of the amount of money that was spent on items categorsed as needs
            var wantsSpent = SelectedItems.Where(i => i.Category == ItemCategory.Want).Sum(i => i.Price);//meant to return a sum of the amount of money that was spent on items categorised as wants


            //the similated analysis by tracking the disparirties between the amount of money spent on need vs the money spent on wants:
            var analysis = "📊 Your Spending Pattern: /n";
            analysis += $"-Needs: R{needsSpent:F2}/n"; // how much spend on items that were categorised as needs
            analysis += $"-Wants: R{wantsSpent:F2}/n/n";

            if(wantsSpent > needsSpent)
            {
                analysis += " Next time, try prioritizing needs over wants to maximize your savings potential!";

            }
            else
            {
                analysis += " Ayy, very well done on prioritizing needs over wants! Fine-tune your choices to hit the exact target.!";
            }
            return analysis ;   
        }

        private void ResetForNewAttempt()
        {
            //clear out all selection for a new attempts:
            foreach (var item in AvailableItems)
            {
                item.IsSelected = false; // ensure that no items are selcted when this reset happens
            }

            SelectedItems.Clear(); // remove items from the  observable list collections that were selected.
            MoneySpent = 0;
            CurrentlySaved = StartingAmount;
            Console.WriteLine("Yay, attempt resetted");
        }
    }
}
