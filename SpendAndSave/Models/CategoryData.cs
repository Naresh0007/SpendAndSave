using SQLite;

namespace SpendAndSave.Models
{
    public class CategoryData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }

    
        public string Username { get; set; }

      
        public decimal Amount { get; set; }


        public DateTime Date { get; set; }

    }
}
