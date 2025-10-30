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

        [RelayCommand]
        private void FileReturn()
        {
            ClearFeedback();

            
        }


        private bool ValidateInputs()
        {
            decimal temp;
            return !string.IsNullOrWhiteSpace(PayeInput) && decimal.TryParse(PayeInput, out temp) &&
            !string.IsNullOrWhiteSpace(UifInput) && decimal.TryParse(UifInput, out temp) &&
            !string.IsNullOrWhiteSpace(pensionFundInput) && decimal.TryParse(pensionFundInput, out temp);

        }

        private bool IsWithinMargin(decimal userValue, decimal correctValue, decimal margin)
        {
            return Math.Abs(userValue - correctValue) <= margin; // here i'm checking if the value the users input is within the margin even when rounded of in relation to the correct value
        }



        //the PAYE feedback :

        private void GeneratePAYEFeedback(decimal userPAYE)
        {
            if (IsPAYECorrect)
            {
                PayeFeedback = "Great Job! Your PAYE calculation is spot on!";
            }
            else
            {
                var difference = userPAYE - correctPAYE;
                var explanation = $"Not quite :(You calculated R{userPAYE:F2}, but the correct amount is R{correctPAYE:F2}\n";

                explanation += "Here's a step-by-step guide on how you calculate PAYE correctly:\n";
                explanation += $"1. Annual Salary: R{grossSalaryMonthly:F2} x 12 = R{GrossSalaryAnnual:F2}\n";
                explanation += $"2. This annual salary falls within the tax bracket: R237,101 - R370,500\n";
                explanation += $"3. Tax calculation: R42,678 + (26% x (R{grossSalaryAnnual} - R237,100))\n";
            }
        }





        private void ClearFeedback()
        {
            payeFeedback = "";
            uifFeedback = "";
            pensionFundFeedback = "";
            
        }
    }
}
