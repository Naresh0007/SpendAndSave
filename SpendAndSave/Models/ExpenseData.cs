using SQLite;

namespace SpendAndSave.Models
{
    public class ExpenseData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Username { get; set; }

        [NotNull]
        public string EntryType { get; set; }  // e.g., "Groceries", "Rent", etc.

        [NotNull]
        public decimal Amount { get; set; }

        [NotNull]
        public string Category { get; set; }  // e.g., "Expenses", "Savings"

        [NotNull]
        public DateTime Date { get; set; }

        public string Location { get; set; }  // Optional

        public string ReceiptPath { get; set; }  // Path to the receipt image (optional)
    }
}
