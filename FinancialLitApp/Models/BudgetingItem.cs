using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialLitApp.Models
{
  public  class BudgetingItem
    {
        // each item has a name and a price tag and a category of what it falls under
        public int Id { get; set; } 
        public string Name { get; set; }

        public int Price { get; set; }

        public bool isSelected { get; set; }
        

        public itemCategory category { get; set; }

    }


    public enum itemCategory // this enum houses the categories of the expenses the  user will see 
    {
        Need,
        Want,
        ImpulsePurchase,
        Investment
    }
}
