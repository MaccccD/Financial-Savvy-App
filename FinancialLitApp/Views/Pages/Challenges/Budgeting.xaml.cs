using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialLitApp.Views.Pages.Challenges
{
   public partial class Budgeting : ContentPage
    {
        public Budgeting() 
        {
            InitializeComponent();
            Console.WriteLine("the budgeting challenge will come in here");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Console.WriteLine("✅ Budgeting page appearing");
        }
    }
}
