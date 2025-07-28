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


        private async Task OnSavingsLessonClicked(object sender, EventArgs e)
        {
            await NavigateToSavingsLesson();
        }


        private async new Task NavigateToSavingsLesson()
        {
            try
            {
                await Shell.Current.GoToAsync("savingslesson");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Navigation Error", $"{ex.Message}", "Okay");
            }
        }
    }
}
