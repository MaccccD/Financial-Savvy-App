using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Diagnostics;
using FinancialLitApp.ViewModels;

namespace FinancialLitApp.Views.Pages
{
   
  public partial class ChallengesPage : ContentPage
    {
       public ChallengesPage()
        {
            InitializeComponent();
           
        }


        private async void OnSavingsChallengeClicked(object sender, EventArgs e)
        {
             await NavigateToSavingsChallenge();
        }

        private async void OnBudgetingChallengeClicked(object sender , EventArgs d)
        {
            await NavigateToBudgetingChallenge();
        }

        private async Task NavigateToSavingsChallenge()
        {
            try
            {
                // Navigate to specific savings challenge page
                await WaitForShellAndNavigate("//savingschallenge");
               // await Shell.Current.GoToAsync("//savingschallenge");
                // Or pass parameters: await Shell.Current.GoToAsync($"//challengedetail?challengeType=savings");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Navigation Error", $"Could not navigate to Savings Challenge: {ex.Message}", "Okayy");
            }

        }

        private async Task NavigateToBudgetingChallenge()
        {
            try
            {
                await WaitForShellAndNavigate("//budgetingchallenge");
                //await Shell.Current.GoToAsync("budgetingchallenge");
            }
            catch (Exception ex) 
            {
                await DisplayAlert("Navigate Error", $"Could not navigate to Budgeting Challenge: {ex.Message}", "Okayy");
            }
        }


        private async Task WaitForShellAndNavigate(string route)
        {
            int attempts = 0;
            while (Shell.Current == null && attempts < 20) // Increased attempts
            {
                await Task.Delay(50); //  the shorter  the delay, the more attempts
                attempts++; //increment the numbe rof attempts to get the shell content to initilaize as we add a delayer to get it to load or initialize properly
            }

            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync(route);
                Console.WriteLine("ayyy, shell is ready");
            }
            else
            {
                // Fallback: try using this instance directly as aopposed to waiting for sehll to take you to the actual page
              //  await this.GoToAsync(route);
            }
        }
    }
}
