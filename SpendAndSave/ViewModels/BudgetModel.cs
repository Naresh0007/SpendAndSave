using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpendAndSave.Models;

namespace SpendAndSave.Data
{
    public class BudgetModel
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly INavigation _navigation;

        // Constructor that initializes the database connection
        public BudgetModel(string dbPath, INavigation navigation)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _navigation = navigation;
        }

        public Task<List<CategoryData>> GetBudgetsAsync(string username)
        {
            return _database.Table<CategoryData>()
                            .Where(category => category.Username == username)
                            .ToListAsync();
        }

        // New method to get budgets by month and year
        public Task<List<CategoryData>> GetBudgetsByMonthYearAsync(string username, DateTime date)
        {

            DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            int month = date.Month;
            int year = date.Year;

            return _database.Table<CategoryData>()
                            .Where(category => category.Username == username &&
                                              category.Date >= firstDayOfMonth &&
                                              category.Date <= lastDayOfMonth)
                            .ToListAsync();
        }

        public Task<int> SaveBudgetAsync(CategoryData budget)
        {
            if (budget.Id != 0)
            {
                return _database.UpdateAsync(budget);
            }
            else
            {
                return _database.InsertAsync(budget);
            }
        }

        public Task<int> DeleteBudgetAsync(CategoryData budget)
        {
            return _database.DeleteAsync(budget);
        }


        // Method to delete all budgets for a username
        public Task<int> DeleteAllBudgetsAsync(string username)
        {
            return _database.ExecuteAsync($"DELETE FROM CategoryData WHERE Username = ?", username);
        }

        // Methods for managing categories
        public Task<List<CategoryData>> GetCategoriesAsync(string username)
        {
            return _database.Table<CategoryData>()
                            .Where(category => category.Username == username)
                            .ToListAsync();
        }

        public async Task<bool> AddCategoryAsync(string categoryName, string username, DateTime date)
        {
            DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var category = new CategoryData { Name = categoryName, Username = username };
            var existingCategory = _database.Table<CategoryData>()
                           .Where(u => u.Username == username &&
                                       u.Name == categoryName &&
                                       u.Date >= firstDayOfMonth &&
                                       u.Date <= lastDayOfMonth)
                           .FirstOrDefaultAsync();
            if (existingCategory == null)
            {
                await _database.InsertAsync(category);
                return true;
            }else
            {
                return false;
            }

        }
    }
}
