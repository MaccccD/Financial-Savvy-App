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

        private async Task NavigateToSavingsLesson()  // Remove 'new' keyword
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
    }
}
