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
    public partial class BudgetPage : ContentPage, INotifyPropertyChanged
    {
        private readonly BudgetModel _viewModel;
        private decimal balance;
        private string _username; // Store the logged-in username

        public ObservableCollection<CategoryData> Budgets { get; set; }

        private string _currentMonthYear;
        public string CurrentMonthYear
        {
            get => _currentMonthYear;
            set => SetProperty(ref _currentMonthYear, value);
        }

        [Obsolete]
        public BudgetPage(string username)
        {
            InitializeComponent();
            BindingContext = this;
            _username = username; // Assign the username
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "users.db3");
            _viewModel = new BudgetModel(dbPath, Navigation);
            Budgets = new ObservableCollection<CategoryData>();
            budgetsCollectionView.ItemsSource = Budgets;
            LoadBudgetsDate(DateTime.Now);
            MessagingCenter.Subscribe<MonthYearSelectionPage, DateTime>(this, "MonthYearSelected", (sender, date) =>
            {
                LoadBudgetsDate(date);
            });
            // Subscribe to message for reloading data
            MessagingCenter.Subscribe<AddBudgetPage>(this, "BudgetUpdated", (sender) =>
            {
                LoadBudgetsDate(DateTime.Now);
            });
        }
        private async void LoadBudgetsDate(DateTime selectedDate)
        {
            try
            {
                var budgetList = await _viewModel.GetBudgetsByMonthYearAsync(_username, selectedDate);
                Budgets.Clear();
                balance = 0;
                foreach (var item in budgetList)
                {
                    Budgets.Add(item);
                    balance += item.Amount;
                }

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
            // Pass the username to the AddBudgetPage
            await Navigation.PushAsync(new AddBudgetPage(_username));
        }

        private async void OnCalendarClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new MonthYearSelectionPage());
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var image = (Image)sender;
            var budget = (CategoryData)image.BindingContext;

            bool confirm = await DisplayAlert("Delete", $"Are you sure you want to delete the budget for {budget.Name}?", "Yes", "No");
            if (confirm)
            {
                try
                {
                    await _viewModel.DeleteBudgetAsync(budget);
                    Budgets.Remove(budget);
                    balance -= budget.Amount;
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
            var budget = (CategoryData)image.BindingContext;

            // Pass the budget data to the AddBudgetPage for updating
            await Navigation.PushAsync(new AddBudgetPage(_username, budget));
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

