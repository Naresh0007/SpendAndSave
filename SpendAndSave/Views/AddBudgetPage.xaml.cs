using SpendAndSave.Models;
using SpendAndSave.Data;


namespace SpendAndSave.Views
{
    public partial class AddBudgetPage : ContentPage
    {
        private readonly BudgetModel _budgetModel;
        private readonly CategoryData _budgetToUpdate;
        private readonly string _username; // Store the logged-in username
        private string receiptPath;
        private List<CategoryData> _categories;


        public AddBudgetPage(string username, CategoryData budgetToUpdate = null)
        {
            InitializeComponent();

            // Initialize ExpenseModel with the correct parameters
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "users.db3");
            _budgetModel = new BudgetModel(dbPath, Navigation);
            _username = username; // Assign the username
            _budgetToUpdate = budgetToUpdate;
            datePicker.Date = DateTime.Today;
            if (_budgetToUpdate != null)
            {
                // Pre-load data if updating an budget
                categoryEntry.Text = _budgetToUpdate.Name;
                amountEntry.Text = _budgetToUpdate.Amount.ToString();
                datePicker.Date = _budgetToUpdate.Date;
            }
          
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            // Preserve the day part of the selected date and set it to the first day of the month
            var selectedDate = e.NewDate;
            datePicker.Date = new DateTime(selectedDate.Year, selectedDate.Month, 1);
        }


        private async void OnSaveBudgetButtonClicked(object sender, EventArgs e)
        {
            // Check if all required fields are filled
            if (string.IsNullOrWhiteSpace(categoryEntry.Text))
            {
                await DisplayAlert("Validation Error", "Category is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(amountEntry.Text) || !decimal.TryParse(amountEntry.Text, out var amount) || amount <= 0)
            {
                await DisplayAlert("Validation Error", "Please enter a valid amount greater than zero.", "OK");
                return;
            }
          

            if (datePicker.Date == DateTime.MinValue)
            {
                await DisplayAlert("Validation Error", "Please select a valid date.", "OK");
                return;
            }

            var budgetItem = new CategoryData
            {
                Amount = Convert.ToDecimal(amountEntry.Text),
                Name = categoryEntry.Text,
                Date = datePicker.Date,
                Username = _username // Set the username
            };

            if (_budgetToUpdate != null)
            {
                // Update existing expense
                budgetItem.Id = _budgetToUpdate.Id; // Ensure the ID is set for updating

            }
            await _budgetModel.SaveBudgetAsync(budgetItem);

            // Notify ExpensePage to reload data
            MessagingCenter.Send(this, "BudgetUpdated");

            // Navigate back to the previous page
            await Navigation.PopAsync();
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

    }
}
