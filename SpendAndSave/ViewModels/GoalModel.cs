using SQLite;
using SpendAndSave.Models;

namespace SpendAndSave.Data
{
    public class GoalModel
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly INavigation _navigation;

        // Constructor that initializes the database connection
        public GoalModel(string dbPath, INavigation navigation)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _navigation = navigation;
        }

        public Task<List<GoalData>> GetGoalsAsync(string username)
        {
            return _database.Table<GoalData>()
                            .Where(goal => goal.Username == username)
                            .ToListAsync();
        }
       
        public Task<int> SaveGoalAsync(GoalData Goal)
        {
            if (Goal.Id != 0)
            {
                return _database.UpdateAsync(Goal);
            }
            else
            {
                return _database.InsertAsync(Goal);
            }
        }

        public Task<int> DeleteGoalAsync(GoalData Goal)
        {
            return _database.DeleteAsync(Goal);
        }


        // Method to delete all Goals for a username
        public Task<int> DeleteAllGoalsAsync(string username)
        {
            return _database.ExecuteAsync($"DELETE FROM GoalData WHERE Username = ?", username);
        }

        // Methods for managing categories
        public Task<List<GoalData>> GetCategoriesAsync(string username)
        {
            return _database.Table<GoalData>()
                            .Where(category => category.Username == username)
                            .ToListAsync();
        }

        public async Task<bool> AddGoalAsync(string name, string username)
        {
            var goal = new GoalData { Name = name, Username = username };
            var existingGoal = _database.Table<GoalData>()
                           .Where(u => u.Username == username &&
                                       u.Name == name)
                           .FirstOrDefaultAsync();
            if (existingGoal == null)
            {
                await _database.InsertAsync(goal);
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
