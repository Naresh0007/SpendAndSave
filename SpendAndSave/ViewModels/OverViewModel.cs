using SQLite;
using SpendAndSave.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace SpendAndSave.ViewModels
{

    public class OverViewModel
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly INavigation _navigation;
        public List<CategoryData> Data { get; set; }
        public ObservableCollection<CategoryData> Categories { get; set; }

        public OverViewModel(string dbPath, INavigation navigation)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _navigation = navigation;

        }

        public Task<List<CategoryData>> GetBudgetsByMonthYearAsync(string username, DateTime date)
        {

            DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            return _database.Table<CategoryData>()
                            .Where(category => category.Username == username &&
                                              category.Date >= firstDayOfMonth &&
                                              category.Date <= lastDayOfMonth)
                            .ToListAsync();
        }
    }
}
