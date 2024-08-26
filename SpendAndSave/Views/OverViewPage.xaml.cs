using Microsoft.Maui.Controls;
using SpendAndSave.Models;
using SpendAndSave.Data;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;

namespace SpendAndSave.Views
{
    public partial class OverViewPage : ContentPage, INotifyPropertyChanged
    {
        private readonly BudgetModel _viewModel;
        private readonly ExpenseModel _expenseModel;
        private string _username; // Store the logged-in username
        private int balance; 

        public ObservableCollection<CategoryData> Data { get; set; }

        private string _currentMonthYear;
        public string CurrentMonthYear
        {
            get => _currentMonthYear;
            set
            {
                if (_currentMonthYear != value)
                {
                    _currentMonthYear = value;
                    OnPropertyChanged();
                }
            }
        }

        public OverViewPage(string username)
        {
            InitializeComponent();
            BindingContext = this;
            _username = username; // Assign the username
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "users.db3");
            _viewModel = new BudgetModel(dbPath, Navigation);
            _expenseModel = new ExpenseModel(dbPath, Navigation);
            Data = new ObservableCollection<CategoryData>();
            LoadBudgetsDate(DateTime.Now);
            MessagingCenter.Subscribe<MonthYearSelectionPage, DateTime>(this, "MonthYearSelected", (sender, date) =>
            {
                LoadBudgetsDate(date);
            });
        }

        private async void LoadBudgetsDate(DateTime selectedDate)
        {
            try
            {
                var budgetList = await _viewModel.GetBudgetsByMonthYearAsync(_username, selectedDate);
                Data.Clear(); // Clear previous data
                foreach (var item in budgetList)
                {
                    Data.Add(item);
                }

                var expenseList = await _expenseModel.GetExpensesByMonthYearAsync(_username, selectedDate);

                var categorySummaries = expenseList
                    .GroupBy(expense => expense.Category)
                    .Select(group => new
                    {
                        CategoryName = group.Key,
                        TotalAmount = group.Sum(expense => expense.Amount)
                    })
                    .ToArray();

                foreach (var category in categorySummaries)
                {
                    // Create BoxView
                    
                    // Create Label
                    var label = new Label
                    {
                        Text = $"{category.CategoryName}: {category.TotalAmount:C}",
                        HorizontalOptions = LayoutOptions.Start,
                        FontSize = 18
                    };

                    // Create StackLayout to hold BoxView and Label
                    var stackLayout = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Children = { label }
                    };

                    // Add StackLayout to the parent layout (BoxViewContainer)
                    BoxViewContainer.Children.Add(stackLayout);
                }

                CurrentMonthYear = selectedDate.ToString("MMMM-yyyy");
                OnPropertyChanged(nameof(Data));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnCalendarClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new MonthYearSelectionPage());
        }
    }
}
