using finance_tracker_api.Models;

namespace finance_tracker_api.Dtos
{
    public class PaginatedTransactionsDto
    {
        public int TotalCount { get; set; }
        public List<Transaction> Transactions { get; set; } = [];
    }
}
