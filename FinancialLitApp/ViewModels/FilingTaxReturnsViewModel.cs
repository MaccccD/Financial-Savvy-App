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
        private string totalFeedback = "";

        [ObservableProperty]
        private string overallMessage = "";

        //Tracking correctness:

        [ObservableProperty]
        private bool isPAYECorrect = false;

        [ObservableProperty]
        private bool isUIFCorrect = false;

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

        public  string PAYEFormulae => "Based on annual income:\n" +
                                       "Use Tax Bracket Formula: (R42,678 + 26% x (annual salary - R237,100)\n" +
                                       "Then subtract rebate value : R17,235 & divide by 12";

        public string UIFFormulae => "1% of gross monthly salary. (The maximum UIF is : R177.12 per month)";

        public string PensionFundFormulae => "4.7% of gross monthly salary";



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

            decimal.TryParse(PayeInput, out paye); // try parse in here converts the string typed into a decimal value
            decimal.TryParse(UifInput, out uif);
            decimal.TryParse(PensionFundInput, out pensionFund);

            CalculatedTotal = paye + uif + pensionFund;
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
            !string.IsNullOrWhiteSpace(PensionFundInput) && decimal.TryParse(PensionFundInput, out temp);

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
                explanation += $"1. Annual Salary: R{GrossSalaryMonthly:F2} x 12 = R{GrossSalaryAnnual:F2}\n";
                explanation += $"2. This annual salary falls within the tax bracket: R237,101 - R370,500\n";
                explanation += $"3. Tax calculation: R42,678 + (26% x (R{GrossSalaryAnnual} - R237,100))\n";
                explanation += $"4. Tax before rebate: R42, 678 + (0.26 x R{GrossSalaryAnnual - 237,100:F2}) = R68,773\n";
                explanation += $"5. Subtract primary rebate : R68,773 - R17,235 = R51,538\n";
                explanation += $"6. Monthly PAYE : R51,538/12 = R{correctPAYE:F2}\n\n";


                if( difference > 0 )
                {
                    explanation += $" You were R{Math.Abs(difference):F2} too high. Did you forget to subtract the rebate?";
                }
                else
                {
                    explanation += $"You were R{Math.Abs(difference):F2} too low. Make sure you're using the correct tax bracket.";
                }
                
                PayeFeedback = explanation;  //appending the explanations as part of the feedback for the PAYE aspect of the feedback to the user.
            }
        }

        private void GenerateUIFFeedback(decimal userUIF)
        {
            if (IsUIFCorrect)
            {
                UifFeedback = " Great Job! Your calculation is very accurate!";
            }
            else
            {
                var explanation = $"Not quite. You calculated R{userUIF}, but the correct UIF amount is R{correctUIF:F2}.\n\n";

                explanation += $"Here's a step-by-step guide on how to calculate  UIF:\n";
                explanation += $"1. Calculate 1% of gross salary: R{GrossSalaryMonthly:F2} x 0.01 = R{GrossSalaryMonthly * 0.01m:F2}\n";
                explanation += $"2. UIF has a monthly cap of R177.12\n";
                explanation += $"3. Take the lower amount: min(R{grossSalaryMonthly * 0.01m:F2}, R177.12) = R{correctUIF:F2}\n\n";


                if(userUIF > correctUIF)
                {
                    explanation += "Remember: UIF is capped at R177.12 per month, even if 1% of your salary is higher!";
                }
                else
                {
                    explanation += " Make sure you're calculating 1% of your gross monthly salary correctly.";
                }

                UifFeedback = explanation;
            }
        }

        private void GeneratePensionFeedback(decimal userPension)
        {
            if (IsPensionFundCorrect)
            {
                PensionFundFeedback = "Yayy, Great Job ! Your calculation is very spot on!";
            }
            else
            {
                var difference = userPension - correctPensionFund;
                var explanation = $"Not quite. You calculated R{userPension}, but the correct pension fund is: R{correctPensionFund:F2}\n\n";

                explanation += "Here's the correct way to calculate your pension fund correctly:\n";
                explanation += $"1. Calculate 4.7% of your gross monthly salary: R{GrossSalaryMonthly:F2} x 0.047 = R{correctPensionFund:F2}\n\n";

                if(Math.Abs(difference) < 50)
                {
                    explanation += "You're so close!! Double-check your percentage calculation( 4.7% not 5% or 10%).";

                }
                else
                {
                    explanation += "Make sure you're multiplying the gross salary with the percentage value(0.047) – which is 4.7%";
                }

                PensionFundFeedback = explanation;
            }
        }

        private void  GenerateTotalFeedback()
        {
            var userTotal = CalculatedTotal;
            var isCorrect = IsWithinMargin(userTotal, correctTotalDeductions, 15m);

            if (isCorrect)
            {
                TotalFeedback = $"Total Deductions: R{userTotal:F2} - Great Job!";
            }
            else
            {
                TotalFeedback = $"Total Deductions: R{userTotal:F2} (Should be R{correctTotalDeductions:F2})\n" +
                                $"Difference: R{Math.Abs(userTotal - correctTotalDeductions):F2}";
            }
        }


        private void ShowSuccessMessage()
        {
            OverallMessage = "Congratulations! Your tax return is filed correctly , YAYYYYYYYYY\n\n" +
                            $"Summary:\n" +
                            $"Gross Salary: R{GrossSalaryMonthly:F2}\n" +
                            $"Total Deductions: R{correctTotalDeductions:F2}\n" +
                            $"Net Take-Home: R{NetTakeHomePay:F2}\n\n" +
                            $"Key Challenge Take-Aways: Understanding how to calculate your tax deductions helps you: \n" +
                            $"- Verify your pay slip is correct\n" +
                            $"- Plan your finances accurately\n" +
                            $"- Maximize tax-deductible contributions (like pension)\n" +
                            $"- Avoid penalties from SARS\n\n" +
                            "You're now equipped to handle real tax filing with CONFIDENCE!";
        }

        private void ShowTryAgainMessage()
        {
            var correctCount = (IsPAYECorrect ? 1 : 0) + (IsUIFCorrect ? 1 : 0) + (IsPensionFundCorrect ? 1 : 0);// this is to keep track of the correct calculation of each deduction that the user has got correct.

            OverallMessage = $" Attempt {currentAttempt} of {MaxAttempts}\n\n" +
                             $"You got {correctCount} out of 3 deductions correct.\n\n" +
                             "Review the feedback above for each deduction and try again. " +
                             "Understanding these calculations is crucial for managing your finances!";
        }

        private void ShowFinalAttemptMessage()
        {
            OverallMessage = "Challenge Completed!\n\n" +
                             "While you didn't get everything correct this time, you've learned valuable lessons about tax calculations. \n\n " +
                             "The correct answers:\n" +
                             $"PAYE: R{correctPAYE:F2}\n" +
                             $"UIF: R{correctUIF:F2}\n" +
                             $"Pension Fund: R{correctPensionFund:F2}\n" +
                             $"Total deductions: R{correctTotalDeductions:F2}\n\n" +
                             "💡 Remember : Tax Filing seems complex, but breaking it down step-by-step makes it manageable. " +
                             "Review the explanations above and try attempt the challenge again to reinforce your learning!";

        }


        [RelayCommand]
        private void ShowCorrectReturn()
        {
            payeInput = correctPAYE.ToString("F2");
            uifInput = correctUIF.ToString("F2");
            pensionFundInput = correctPensionFund.ToString("F2");

            UpdateCalculatedTotal();

            OverallMessage = " Correct Return Displayed!\n\n" +
                            "This is what a correctly filed return looks like. Study  these values and the formulas to understand the calculations/\n\n" +
                            "💡 Tip: Try clearing these values and calculating the for yourself for practice.";
        }

        [RelayCommand]
        private void Reset()
        {
            payeInput = "";
            uifInput = "";
            pensionFundInput = "";
            calculatedTotal = 0m;

            ClearFeedback();

            ShowResults = false;
            CurrentAttempt = 1;
            IsPAYECorrect = false;
            IsUIFCorrect = false;
            IsPensionFundCorrect = false;
            IsAllCorrect = false;
            OverallMessage = "";
        }



        private void ClearFeedback()
        {
            PayeFeedback = "";
            UifFeedback = "";
            PensionFundFeedback = "";
            TotalFeedback = "";
            
        }
    }
}
