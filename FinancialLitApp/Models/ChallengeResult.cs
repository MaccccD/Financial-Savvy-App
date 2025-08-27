using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialLitApp.Models
{
   public  class ChallengeResult
    {
        public List<SavingsItem> ItemsSelected { get; set; } = new List<SavingsItem>();
        public bool IsSuccess { get; set; }
        public decimal AmountSaved { get; set; }
        public decimal TargetAmount {  get; set; }
        public int AttemptsUsed { get; set; }
        public string FeedbackMessage { get; set; }
        public string LearningInsight { get; set; }
    }
}
