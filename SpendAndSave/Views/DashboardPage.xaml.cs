using SpendAndSave.Models;
using SpendAndSave.Services;
using SpendAndSave.ViewModels;
using System;
using System.IO;
using Microsoft.Maui.Controls;

namespace SpendAndSave.Views
{
    public partial class DashboardPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private readonly LoginViewModel _viewModel;
        private string _username;
        private const string DefaultProfileImage = "user.png";

        public DashboardPage(LoginRequestModel user)
        {
            InitializeComponent();

            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "users.db3");
            _databaseService = new DatabaseService(dbPath);
            _viewModel = new LoginViewModel(_databaseService, Navigation);
            _username = user.UserName;

            // Initialize UI elements with user data
            usernameLabel.Text = user.FullName;
            LoadProfilePicture(user.ProfileImagePath);
        }

        private void LoadProfilePicture(string profileImagePath)
        {
            if (string.IsNullOrEmpty(profileImagePath))
            {
                ProfileImage.Source = DefaultProfileImage;
            }
            else
            {
                ProfileImage.Source = ImageSource.FromFile(profileImagePath);
            }
        }

        private async void OnExpenseTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ExpensePage(_username));
        }

        private async void OnBudgetTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BudgetPage(_username));
        }

        private async void OnOverviewTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new OverViewPage(_username));
        }

        private async void OnGoalTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GoalPage(_username));
        }
        private async void OnProfileTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage(_username));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //LoadProfilePicture();
            //LoadUserDetails();
        }

        private void OnMenuButtonClicked(object sender, EventArgs e)
        {
            // Handle the menu button click event here.
            // For example, open a side menu or navigate to another page.
            DisplayAlert("Menu Button Clicked", "The menu button was clicked.", "OK");
        }

        private void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            ((AppShell)Shell.Current).Logout();

      
        }

        

        async void Button1_Clicked(System.Object sender, System.EventArgs e)
        {
            var result = await MediaPicker.CapturePhotoAsync();
            var stream = await result.OpenReadAsync();
            //resultImage.Source = ImageSource.FromStream(() => stream);
        }

       
    }
}
