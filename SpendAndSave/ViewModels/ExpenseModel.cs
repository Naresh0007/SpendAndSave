using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpendAndSave.Models;

namespace SpendAndSave.Data
{
    public class ExpenseModel
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly INavigation _navigation;

        // Constructor that initializes the database connection
        public ExpenseModel(string dbPath, INavigation navigation)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _navigation = navigation;
        }

        public Task<List<ExpenseData>> GetExpensesAsync(string username)
        {
            return _database.Table<ExpenseData>()
                            .Where(expense => expense.Username == username)
                            .ToListAsync();
        }

        // New method to get expenses by month and year
        public Task<List<ExpenseData>> GetExpensesByMonthYearAsync(string username, DateTime date)
        {

            DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            int month = date.Month;
            int year = date.Year;

            return _database.Table<ExpenseData>()
                            .Where(expense => expense.Username == username &&
                                              expense.Date >= firstDayOfMonth &&
                                              expense.Date <= lastDayOfMonth)
                            .ToListAsync();
        }

        public Task<int> SaveExpenseAsync(ExpenseData expense)
        {
            if (expense.Id != 0)
            {
                return _database.UpdateAsync(expense);
            }
            else
            {
                return _database.InsertAsync(expense);
            }
        }

        public Task<int> DeleteExpenseAsync(ExpenseData expense)
        {
            return _database.DeleteAsync(expense);
        }


        // Method to delete all expenses for a username
        public Task<int> DeleteAllExpensesAsync(string username)
        {
            return _database.ExecuteAsync($"DELETE FROM ExpenseData WHERE Username = ?", username);
        }

        // Methods for managing categories
        public Task<List<CategoryData>> GetCategoriesAsync(string username, DateTime date)
        {
            DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            return _database.Table<CategoryData>()
                            .Where(category => category.Username == username &&
                                               category.Date >= firstDayOfMonth &&
                                               category.Date <= lastDayOfMonth)
                            .ToListAsync();
        }

        public async Task AddCategoryAsync(string categoryName, string username)
        {
            var category = new CategoryData { Name = categoryName, Username = username };
            await _database.InsertAsync(category);
        }
    }
}
