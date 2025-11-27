using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;

namespace FinancialLitApp.Models
{
    public  class SavingsItem
    {
        //in here are the variables that relate to the challenge such as the name of the item , its price, description etc.
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsSelected { get; set; }

        public ItemCategory Category { get; set; }


    }

    public enum ItemCategory // this enum houses the distinction between what item  users would  categorize as a need or want 
    {
        Need,
        Want
    }
}
