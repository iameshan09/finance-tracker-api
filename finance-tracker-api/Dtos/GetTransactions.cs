using finance_tracker_api.Models;
using System.ComponentModel.DataAnnotations;

namespace finance_tracker_api.Dtos
{
    public class GetTransactionsQueryDto
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Skip must be 0 or greater.")]
        public int Skip { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Limit must be 1 or greater.")]
        public int Limit { get; set; }

        [EnumDataType(typeof(TransactionType), ErrorMessage = "Type must be either 'Income' or 'Expense'.")]
        public TransactionType? Type { get; set; }
    }
}
