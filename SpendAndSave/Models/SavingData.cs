using SQLite;

namespace SpendAndSave.Models
{
    public class SavingData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        
        public string SavingName { get; set; }

        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public string Username { get; set; }

        [NotNull]
        public decimal Amount { get; set; }

        [NotNull]
        public DateTime Date { get; set; }

    }
}
