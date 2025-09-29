using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialLitApp.Views.Pages.Lessons
{
    public partial class SavingsLesson : ContentPage
    {
        public SavingsLesson()
        {
            InitializeComponent();
            Console.WriteLine("yay, initialization works");
        }

        private async void OnChallengeClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("savings");
            Console.WriteLine("the savings challenge page is currently loading");
        }
    }
}


