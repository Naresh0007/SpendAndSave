using SQLite;
using SpendAndSave.Models;

namespace SpendAndSave.Data
{
    public class SavingModel
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly INavigation _navigation;

        // Constructor that initializes the database connection
        public SavingModel(string dbPath, INavigation navigation)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _navigation = navigation;
        }

        public Task<List<SavingData>> GetSavingsAsync(string username, string name)
        {
            return _database.Table<SavingData>()
                            .Where(saving => saving.Username == username &&
                                           saving.Name == name)
                            .ToListAsync();
        }

        public Task<int> SaveSavingAsync(SavingData Saving)
        {
            if (Saving.Id != 0)
            {
                return _database.UpdateAsync(Saving);
            }
            else
            {
                return _database.InsertAsync(Saving);
            }
        }

        public Task<int> DeleteSavingAsync(SavingData Saving)
        {
            return _database.DeleteAsync(Saving);
        }


        // Method to delete all Goals for a username
        public Task<int> DeleteAllSavingAsync(string username)
        {
            return _database.ExecuteAsync($"DELETE FROM SavingData WHERE Username = ?", username);
        }

        // Methods for managing categories
        public Task<List<SavingData>> GetCategoriesAsync(string username)
        {
            return _database.Table<SavingData>()
                            .Where(category => category.Username == username)
                            .ToListAsync();
        }

        
    }
}
