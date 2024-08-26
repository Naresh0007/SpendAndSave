using SpendAndSave.Models;
using SpendAndSave.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Linq;
namespace SpendAndSave.Views
{
    public partial class GoalPage : ContentPage, INotifyPropertyChanged
    {
        private readonly GoalModel _viewModel;
        private readonly SavingModel _savingModel;
        private string _username; // Store the logged-in username
        private decimal balance;

        public ObservableCollection<GoalData> Goals { get; set; }

        [Obsolete]
        public GoalPage(string username)
        {
            InitializeComponent();
            BindingContext = this;
            _username = username; // Assign the username
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "users.db3");
            _viewModel = new GoalModel(dbPath, Navigation);
            _savingModel = new SavingModel(dbPath, Navigation);
            Goals = new ObservableCollection<GoalData>();
            goalsListView.ItemsSource = Goals;
            BindingContext = this;
            LoadData();
            // Subscribe to message for reloading data
            MessagingCenter.Subscribe<AddGoalPage>(this, "GoalUpdated", (sender) =>
            {
                LoadData();
            });
            MessagingCenter.Subscribe<AddSavingPage>(this, "SavingUpdated", (sender) =>
            {
                LoadData();
            });

        }
        private async void LoadData()
        {
            try
            {
                var goalList = await _viewModel.GetGoalsAsync(_username);
                Goals.Clear();
                foreach (var item in goalList)
                {
                    item.GoalImage =GetImageSource(item.Name);
                    try
                    {
                        balance = 0;
                        var savingList = await _savingModel.GetSavingsAsync(_username, item.Name);
                        foreach (var a in savingList)
                        {
                            balance += a.Amount;
                        }
                       
                        decimal progress = balance / item.Amount;
                        item.GoalProgress = progress;
                        item.GoalProgressLabel = $"{progress * 100:0.00}% out of ${item.Amount}";
                        Goals.Add(item);
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                    }
                   
                }
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

        [Obsolete]
        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddGoalPage(_username));
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var image = (Image)sender;
            var goal = (GoalData)image.BindingContext;

            bool confirm = await DisplayAlert("Delete", $"Are you sure you want to delete the budget for {goal.Name}?", "Yes", "No");
            if (confirm)
            {
                try
                {
                    await _viewModel.DeleteGoalAsync(goal);
                    Goals.Remove(goal);
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
            var goal = (GoalData)image.BindingContext;
            goal.GoalImage = "";
            
            // Pass the goal data to the AddGoalPage for updating
            await Navigation.PushAsync(new AddGoalPage(_username, goal));
        }
        public string GetImageSource(string goalName)
        {
            return goalName.ToLower() switch
            {
                "car" => "car.png",
                "education" => "collage.png",
                "emergency" => "siren.png",
                "health" => "healthcare.png",
                "holiday" => "holiday.png",
                "furniture" => "furniture.png",
                "renovation" => "renovation.png",
                "house" => "house.png",
                "investment" => "profit.png",
                "business" => "business.png",
                "pet" => "pet.png",
                "retirement" => "retirement.png",
                "wedding" => "wedding.png",
                _ => "default_icon.png",
            };
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

