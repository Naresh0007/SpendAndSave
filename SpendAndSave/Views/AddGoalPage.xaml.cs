using SpendAndSave.Models;
using SpendAndSave.Data;
using System.Collections.ObjectModel;

namespace SpendAndSave.Views
{
    public partial class AddGoalPage : ContentPage
    {
        private readonly GoalModel _goalModel;  
        private readonly SavingModel _savingModel;  
        private readonly GoalData _goalToUpdate;
        private readonly string _username; // Store the logged-in username
        private readonly string _goalName; // Store the logged-in username
        private string receiptPath;
        private List<GoalData> _categories;
        public ObservableCollection<SavingData> Savings { get; set; }

        [Obsolete]
        public AddGoalPage(string username, GoalData goalToUpdate = null)
        {
            InitializeComponent();

            // Initialize ExpenseModel with the correct parameters
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "users.db3");
            _goalModel = new GoalModel(dbPath, Navigation);
            _savingModel = new SavingModel(dbPath, Navigation);
            Savings = new ObservableCollection<SavingData>();
            _username = username; // Assign the username
            _goalToUpdate = goalToUpdate;
            savingsListView.ItemsSource = Savings;
            var goalsList = new string[] { "Car", "Education", "Emergency", "Health", "Holiday", "Furniture", "Renovation",
                    "House", "Investment", "Business", "Pet", "Retirement", "Wedding" };
            goalPicker.ItemsSource = goalsList;
            if (_goalToUpdate != null)
            {
                _goalName = _goalToUpdate.Name;
            } 
            LoadData();
            // Subscribe to message for reloading data
            MessagingCenter.Subscribe<AddGoalPage>(this, "GoalUpdated", (sender) =>
            {
                LoadData();
            });
            
        }
        private async void LoadData()
        {
            title.Text = "Add Goal";
            
            if (_goalToUpdate != null)
            {
                // Pre-load data if updating an goal
                title.Text = "Edit Goal";
               
                var goalName = "";
                amountEntry.Text = _goalToUpdate.Amount.ToString(); 
                if (goalPicker.ItemsSource.Cast<string>().Contains(_goalToUpdate.Name))
                {
                    goalPicker.SelectedItem = _goalToUpdate.Name;
                    goalName = _goalToUpdate.Name;
                }
                else
                {
                    goalPicker.SelectedItem = "Other...";
                    goalEntry.IsVisible = true;
                    goalEntry.Text = _goalToUpdate.Name;
                    goalName = _goalToUpdate.Name;
                }
                savingsView.IsVisible = true;
                try
                {
                    var savingList = await _savingModel.GetSavingsAsync(_username, goalName);
                    Savings.Clear();
                    foreach (var item in savingList)
                    {
                        Savings.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                }
            }
        }

        private void OnGoalPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (goalPicker == null)
            {
                
                return;
            }

            var selectedIndex = goalPicker.SelectedIndex;
            if (selectedIndex < 0 || goalPicker.ItemsSource == null)
            {
                
                return;
            }
            var selectedGoal = goalPicker.SelectedItem.ToString();
            if (selectedGoal == "Other...")
            {
                goalEntry.IsVisible = true;
            }
            else
            {
                goalEntry.IsVisible = false;
            }
        }


        private async void OnSaveGoalButtonClicked(object sender, EventArgs e)
        {
            // Check if all required fields are filled
            string goal;

            if (goalPicker.SelectedItem.ToString() == "Other...")
            {
                if (string.IsNullOrWhiteSpace(goalEntry.Text))
                {
                    await DisplayAlert("Validation Error", "Please enter a goal.", "OK");
                    return;
                }
                goal = goalEntry.Text;
            }
            else
            {
                goal = goalPicker.SelectedItem.ToString();
            }

            if (string.IsNullOrWhiteSpace(amountEntry.Text) || !decimal.TryParse(amountEntry.Text, out var amount) || amount <= 0)
            {
                await DisplayAlert("Validation Error", "Please enter a valid amount greater than zero.", "OK");
                return;
            }

            var goalItem = new GoalData
            {
                Amount = Convert.ToDecimal(amountEntry.Text),
                Name = goal,
                Username = _username // Set the username
            };

            if (_goalToUpdate != null)
            {
                // Update existing expense
                goalItem.Id = _goalToUpdate.Id; // Ensure the ID is set for updating

            }
            await _goalModel.SaveGoalAsync(goalItem);

            // Notify ExpensePage to reload data
            MessagingCenter.Send(this, "GoalUpdated");

            // Navigate back to the previous page
            await Navigation.PopAsync();
        }

        private async void OnAddSavingButtonClicked(object sender, EventArgs e)
        {
            if (_goalName != null)
            {
                await Navigation.PushAsync(new AddSavingPage(_username, null, _goalName));
            }
            else {
                await Navigation.PushAsync(new AddSavingPage(_username));
            }
            
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnDeleteSavingButtonClicked(object sender, EventArgs e)
        {
            var image = (Image)sender;
            var saving = (SavingData)image.BindingContext;

            bool confirm = await DisplayAlert("Delete", $"Are you sure you want to delete the budget for {saving.SavingName}?", "Yes", "No");
            if (confirm)
            {
                try
                {
                    await _savingModel.DeleteSavingAsync(saving);
                    Savings.Remove(saving);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                }
            }
        }

        private async void OnUpdateSavingButtonClicked(object sender, EventArgs e)
        {
            var image = (Image)sender;
            var saving = (SavingData)image.BindingContext;
            
            // Pass the goal data to the AddGoalPage for updating
            await Navigation.PushAsync(new AddSavingPage(_username, saving));
        }

    }
}
