using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialLitApp.Views.Pages
{
   public partial class LessonDetailPage : ContentPage
    {
        public LessonDetailPage() 
        { 
            InitializeComponent();
        }


        private async void OnSavingsLessonClicked(object sender, EventArgs e)
        {
            await NavigateToSavingsLesson();
            
        }

        private  async void OnBudgetingLessonClicked(object senderr, EventArgs e)
        {
            await NavigateToBudgetingLesson();
        }

        private async void OnFinancialPortfolioClicked(object sender, EventArgs e)
        {
            await NavigateToFinancialPortfolio();
        }

        private async Task NavigateToSavingsLesson()
        {
            try
            {
                Console.WriteLine("🔍 Starting savings lesson navigation...");
                Console.WriteLine($"🔍 Shell.Current is null: {Shell.Current == null}");
                //using relative routing here as well instead of absolute:
                // await Shell.Current.GoToAsync("savingslesson");
                await WaitForShellAndNavigate("savingslesson");

                Console.WriteLine("Navigation to the savings lesson has been  completed");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Navigation failed: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");

                await DisplayAlert("Navigation Error", $"Could not navigate to the Savings Lesson: {ex.Message}", "Alrightyy");
            }
            
        }

        private async Task NavigateToBudgetingLesson()
        {
            try
            {
                //await Shell.Current.GoToAsync("budgetinglesson");
                await WaitForShellAndNavigate("budgetinglesson");
            }
            catch(Exception ex)
            {
                await DisplayAlert("Navigation Error", $"Could not navigate to the Budgdeting lesson: {ex.Message}", "Alrightyyy");
            }
        }

        private async Task NavigateToFinancialPortfolio()
        {
            try
            {
                await WaitForShellAndNavigate("financialportfolio");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Navigation Error", $"Could not navigate to the Financial Portfolio Section: {ex.Message}", "Okayy");
            }
        }

        private async Task WaitForShellAndNavigate(string route) //creating a delay by waiting for the shell content to load bc its helps with the navigation
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
                await Shell.Current.GoToAsync(route);//so go where the page that needs to be lpaded or navigated to is 
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
