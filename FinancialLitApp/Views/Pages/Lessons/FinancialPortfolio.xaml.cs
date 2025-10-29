using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialLitApp.Views.Pages.Lessons
{
    public partial class FinancialPortfolio : ContentPage
    {
        public FinancialPortfolio() 
        {
            InitializeComponent();
            Console.WriteLine("Initialize successful, yay!");
        }

        private async void OnTaxReturnChallengeClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("filingtaxreturns");
            Console.WriteLine("Yayy, the tex return challenge has loaded !");
        }
    }
}
