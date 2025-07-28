using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinancialLitApp.ViewModels;

namespace FinancialLitApp.Views.Pages
{
   
  public partial class ChallengesPage : ContentPage
    {
       public ChallengesPage()
        {
            InitializeComponent();
           
        }


        public void OnSavingsChallengeClicked(object sender, EventArgs e)
        {

        }

        public async Task NavigateToChallenge1()
        {
            await NavigateToChallenge1();
        }
    }
}
