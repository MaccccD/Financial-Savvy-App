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
          //  await DisplayAlert("Test", "Button clicked!", "OK");
            await NavigateToSavingsChallenge();
        }

        private async void OnBudgetingChallengeClicked(object sender , EventArgs d)
        {
            await NavigateToBudgetingChallenge();
        }
        
        public async void OnFileTaxReturnsChallengeClicked (object sender, EventArgs i)
        {
            await NavigateToFileTaxReturnsChallenge();
        }
        private async Task NavigateToSavingsChallenge()
        {
            try
            {
                Console.WriteLine("🔍 Starting savings challenge navigation...");
                Console.WriteLine($"🔍 Shell.Current is null: {Shell.Current == null}");

                //i changed the absolute routing to relative navigation ( from '//savings' to just savings'):
                await WaitForShellAndNavigate("savings");//relative routing 

                Console.WriteLine("✅ Navigation command completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Navigation failed: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");

                await DisplayAlert("Navigation Error", $"Could not navigate to Savings Challenge: {ex.Message}", "Okayy");
            }

        }

        private async Task NavigateToBudgetingChallenge()
        {
            try
            {
                await WaitForShellAndNavigate("budgeting"); // changed from absolute navigation to  relative nav
                //await Shell.Current.GoToAsync("budgeting challenge");
            }
            catch (Exception ex) 
            {
                await DisplayAlert("Navigate Error", $"Could not navigate to Budgeting Challenge: {ex.Message}", "Okayy");
            }
        }
        

        private async Task NavigateToFileTaxReturnsChallenge()
        {
            try
            {
                await WaitForShellAndNavigate("filingtaxreturns");
            }
            catch(Exception ex)
            {
                await DisplayAlert("Challenge not available yet", $"{ex.Message}", "Danko");
            }
        }

        private async Task WaitForShellAndNavigate(string route)
        {
            Console.WriteLine($"🔍 Attempting to navigate to: {route}");
            int attempts = 0;
            while (Shell.Current == null && attempts < 20) // Increased attempts
            {
                Console.WriteLine($"🔍 Waiting for Shell... Attempt {attempts + 1}");
                await Task.Delay(50); //  the shorter  the delay, the more attempts
                attempts++; //increment the number of attempts to get the shell content to initilaize as we add a delayer to get it to load or initialize properly
            }

            if (Shell.Current != null)
            {
                Console.WriteLine($"✅ Shell is ready, navigating to: {route}");
                await Shell.Current.GoToAsync(route);
                Console.WriteLine("ayyy, shell is ready");
            }
            else
            {
                Console.WriteLine("❌ Shell.Current is still null after waiting");
                throw new InvalidOperationException("Shell.Current is not available");
                // Fallback: try using this instance directly as aopposed to waiting for shell to take you to the actual page
                //  await this.GoToAsync(route);
            }
        }
    }
}
