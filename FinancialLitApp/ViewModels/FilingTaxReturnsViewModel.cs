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
        [ObservableProperty]
        private int grossSalary;
    }
}
