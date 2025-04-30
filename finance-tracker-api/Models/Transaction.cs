namespace finance_tracker_api.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Amount { get; set; } = 0.0;
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime Date { get; set; }

    }

    public enum TransactionType
    {
        Income,
        Expense
    }

    public enum TransactionStatus
    {
        Inactive,
        Active
    }
}

