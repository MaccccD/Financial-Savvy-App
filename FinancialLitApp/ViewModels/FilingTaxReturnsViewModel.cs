using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using FinancialLitApp.Services;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;


namespace FinancialLitApp.ViewModels
{
    public partial class FilingTaxReturnsViewModel : ObservableObject
    {
        //these are the given values:
        [ObservableProperty]
        private decimal grossSalaryMonthly = 28122.01m;
        [ObservableProperty]
        private decimal grossSalaryAnnual = 337464.12m;
        [ObservableProperty]
        private decimal netTakeHomePay = 22328.33m;


        //the correct answers (calculated internally after the user inputs their answer):

        private decimal correctPAYE = 4294.83m;
        private decimal correctUIF = 177.12m;
        private decimal correctPensionFund = 1321.73m;
        private decimal correctTotalDeductions = 5793.68m;


        // the values the user inputs:

        [ObservableProperty]
        private string payeInput = "";

        [ObservableProperty]
        private string uifInput = "";

        [ObservableProperty]
        private string pensionFundInput = "";

        [ObservableProperty]
        private decimal calculatedTotal = 0m;


        //the feedback :
        [ObservableProperty]
        private string payeFeedback = "";

        [ObservableProperty]
        private string uifFeedback = "";

        [ObservableProperty]
        private string pensionFundFeedback = "";

        [ObservableProperty]
        private string overallMessage = "";


        //Tracking correctness:

        [ObservableProperty]
        private bool isPAYECorrect = false;

        [ObservableProperty]
        private bool isUIForrect = false;

        [ObservableProperty]
        private bool isPensionFundCorrect = false;

        [ObservableProperty]
        private bool isAllCorrect = false;

        [ObservableProperty]
        private bool showResults = false;

        [ObservableProperty]
        private int currentAttempt = 1;

        [ObservableProperty]
        private int maxAttempts = 3;

        //the formulas that users are given ( for UI binding):

        private string PAYEFormulae => "Based on annual income:\n" +
                                       "Use Tax Bracket Formula: (R42,678 + 26% x (annual salary - R237,100)\n" +
                                       "Then subtract rebate value : R17,235 & divide by 12";

        private string UIFFormulae => "1% of gross monthly salary. (The maximum UIF is : R177.12 per month)";

        private string PensionFundFormulae => "4.7% of gross monthly salary";



        public FilingTaxReturnsViewModel()
        {
            
            // return;
        }

        //the auto calculation as the user types:
        partial void OnPayeInputChanged(string value)
        {
            UpdateCalculatedTotal();
        }

        partial void OnUifInputChanged(string value)
        {
            UpdateCalculatedTotal();
        }
        partial void OnPensionFundInputChanged(string value)
        {
            UpdateCalculatedTotal();
        }
        private void UpdateCalculatedTotal()
        {
            decimal paye = 0, uif = 0, pensionFund = 0;

            decimal.TryParse(payeInput, out paye); // try parse in here converts the string typed into a decimal value
            decimal.TryParse(uifInput, out uif);
            decimal.TryParse(pensionFundInput, out pensionFund);

            calculatedTotal = paye + uif + pensionFund;
        }



    }
}
