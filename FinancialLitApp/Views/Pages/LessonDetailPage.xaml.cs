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

        private async Task NavigateToSavingsLesson()
        {
            try
            {
                await Shell.Current.GoToAsync("savingslesson");
            }
            catch (Exception ex)
            {
                await DisplayAlert(" Content coming right up", $"{ex.Message}","Okay");
            }
        }

        private async Task NavigateToBudgetingLesson()
        {
            try
            {
                await Shell.Current.GoToAsync("budgetinglesson");
            }
            catch(Exception ex)
            {
                await DisplayAlert("Content coming right up!", $"{ex.Message}", "Alrightyy");
            }
        }
    }
}
