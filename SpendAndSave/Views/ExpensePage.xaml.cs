using Microsoft.Maui.Controls;
using SpendAndSave.Models;
using SpendAndSave.Data;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SpendAndSave.Views
{
    public partial class ExpensePage : ContentPage, INotifyPropertyChanged
    {
        private readonly ExpenseModel _viewModel;
        private readonly BudgetModel _budgetModel;
        private decimal balance;
        private decimal budgetBalance;
        private string _username; // Store the logged-in username

        public ObservableCollection<ExpenseData> Expenses { get; set; }

        private string _currentMonthYear;
        private Color _balanceTextColor;
        public string CurrentMonthYear
        {
            get => _currentMonthYear;
            set => SetProperty(ref _currentMonthYear, value);
        }

        public Color BalanceTextColor
        {
            get => _balanceTextColor;
            set
            {
                _balanceTextColor = value;
                OnPropertyChanged(nameof(BalanceTextColor));
            }
        }

        [Obsolete]
        public ExpensePage(string username)
        {
            InitializeComponent();
            BindingContext = this;
            _username = username; // Assign the username
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "users.db3");
            _viewModel = new ExpenseModel(dbPath, Navigation);
            _budgetModel = new BudgetModel(dbPath, Navigation);
            Expenses = new ObservableCollection<ExpenseData>();
            expensesCollectionView.ItemsSource = Expenses;
            LoadExpensesDate(DateTime.Now);
            MessagingCenter.Subscribe<MonthYearSelectionPage, DateTime>(this, "MonthYearSelected", (sender, date) =>
            {
                LoadExpensesDate(date);
            });
            // Subscribe to message for reloading data
            MessagingCenter.Subscribe<AddExpensePage>(this, "ExpenseUpdated", (sender) =>
            {
                LoadExpensesDate(DateTime.Now);
            });
        }

        private async void LoadExpensesDate(DateTime selectedDate)
        {
            try
            {
                var expenseList = await _viewModel.GetExpensesByMonthYearAsync(_username, selectedDate);
                var budget = await _budgetModel.GetBudgetsByMonthYearAsync(_username, selectedDate);
                Expenses.Clear();
                balance = 0;
                foreach (var item in expenseList)
                {
                    Expenses.Add(item);
                    balance += item.Amount;
                }

                budgetBalance = 0;
                foreach (var a in budget)
                {
                    budgetBalance += a.Amount;
                }
                BalanceTextColor = (budgetBalance > balance) ? Color.FromArgb("#4B8313") : Color.FromArgb("#C82020");
                budgetBalanceLabel.Text = $"${budgetBalance}";
                balanceLabel.Text = $"${balance}";
                CurrentMonthYear = selectedDate.ToString("MMMM-yyyy");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }


        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            // Pass the username to the AddExpensePage
            await Navigation.PushAsync(new AddExpensePage(_username));
        }

        private async void OnCalendarClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new MonthYearSelectionPage());
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var image = (Image)sender;
            var expense = (ExpenseData)image.BindingContext;

            bool confirm = await DisplayAlert("Delete", $"Are you sure you want to delete the expense for {expense.EntryType}?", "Yes", "No");
            if (confirm)
            {
                try
                {
                    await _viewModel.DeleteExpenseAsync(expense);
                    Expenses.Remove(expense);
                    balance -= expense.Amount;
                    balanceLabel.Text = $"${balance}";
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                }
            }
        }

        private async void OnUpdateButtonClicked(object sender, EventArgs e)
        {
            var image = (Image)sender;
            var expense = (ExpenseData)image.BindingContext;

            // Pass the expense data to the AddExpensePage for updating
            await Navigation.PushAsync(new AddExpensePage(_username, expense));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
