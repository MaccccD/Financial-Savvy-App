using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinancialLitApp.ViewModels;

namespace FinancialLitApp.Views
{
   
  public partial class ChallengesPage : ContentPage
    {
       public ChallengesPage(ChallengesViewModel challengesPageViewModel)
        {
           // InitializeComponent();
            BindingContext = challengesPageViewModel;
        }
    }
}
