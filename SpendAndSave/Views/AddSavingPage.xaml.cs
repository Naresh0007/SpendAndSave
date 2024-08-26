using SpendAndSave.Models;
using SpendAndSave.Data;

namespace SpendAndSave.Views
{
    public partial class AddSavingPage : ContentPage
    {
        private readonly SavingModel _SavingModel;  
        private readonly GoalModel _goalModel;  
        private readonly SavingData _savingToUpdate;
        private readonly string _username; // Store the logged-in username
        private string receiptPath;
        private List<GoalData> _goals;


        public AddSavingPage(string username, SavingData savingToUpdate = null, string goalName = null)
        {
            InitializeComponent();

            // Initialize ExpenseModel with the correct parameters
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "users.db3");
            _SavingModel = new SavingModel(dbPath, Navigation);
            _goalModel = new GoalModel(dbPath, Navigation);
            _username = username; // Assign the username
            _savingToUpdate = savingToUpdate;
            LoadGoals(goalName);
        }

        private async void LoadGoals(string goalName)
        {
            var goals = await _goalModel.GetGoalsAsync(_username);
            _goals = goals;
            var goalNames = _goals.Select(c => c.Name).ToList(); // Convert to list of strings
            goalPicker.ItemsSource = goalNames;

            if (_savingToUpdate != null)
            {
                savingEntry.Text = _savingToUpdate.SavingName;
                amountEntry.Text = _savingToUpdate.Amount.ToString();
                goalPicker.SelectedItem = _savingToUpdate.Name;
            }
            if (goalName != null)
            {
                goalPicker.SelectedItem = goalName;
            }
        }

        [Obsolete]
        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            // Check if all required fields are filled
            if (string.IsNullOrWhiteSpace(savingEntry.Text))
            {
                await DisplayAlert("Validation Error", "Please select a saving.", "OK");
                return;
            }
            if (goalPicker.SelectedItem == null)
            {
                await DisplayAlert("Validation Error", "Please select a goal you have set.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(amountEntry.Text) || !decimal.TryParse(amountEntry.Text, out var amount) || amount <= 0)
            {
                await DisplayAlert("Validation Error", "Please enter a valid amount greater than zero.", "OK");
                return;
            }

            var savingItem = new SavingData
            {
                Amount = Convert.ToDecimal(amountEntry.Text),
                Name = goalPicker.SelectedItem.ToString(),
                SavingName = savingEntry.Text,
                Username = _username,
                Date = DateTime.Now
            };

            if (_savingToUpdate != null)
            {
                // Update existing expense
                savingItem.Id = _savingToUpdate.Id; // Ensure the ID is set for updating

            }
            await _SavingModel.SaveSavingAsync(savingItem);

            // Notify GoalPage to reload data
            MessagingCenter.Send(this, "SavingUpdated");

            await Navigation.PushAsync(new GoalPage(_username));

            // Navigate back to the previous page
            //await Navigation.PopAsync();
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }


    }
}
