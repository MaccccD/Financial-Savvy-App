using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialLitApp.Views.Pages.Lessons
{
    public partial class BudgetingLesson : ContentPage
    {
        public BudgetingLesson() 
        {
            InitializeComponent();
            Console.WriteLine("initializing successful");
        }

        private async void OnBudgetingChallengeClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("budgeting");
            Console.WriteLine("navigation successful");
        }
    }



}
