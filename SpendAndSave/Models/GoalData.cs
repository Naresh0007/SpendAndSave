using SQLite;

namespace SpendAndSave.Models
{
    public class GoalData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public decimal Amount { get; set; }

        public string GoalImage { get; set; }

        public decimal GoalProgress { get; set; }


        public string GoalProgressLabel { get; set; }

    }
}
