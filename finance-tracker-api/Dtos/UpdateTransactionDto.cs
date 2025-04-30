using finance_tracker_api.Models;
using System.ComponentModel.DataAnnotations;

namespace finance_tracker_api.Dtos
{
    public class UpdateTransactionDto
    {

        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public double Amount { get; set; }

        [Required]
        [EnumDataType(typeof(TransactionType), ErrorMessage = "Type must be either 'Income' or 'Expense'.")]
        public TransactionType Type { get; set; }
    }
}
