using SpendAndSave.ViewModels;
using SpendAndSave.Models;
using SpendAndSave.Services;
using SpendAndSave.Data;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SpendAndSave.Views
{
    public partial class ProfilePage : ContentPage
    {
        private readonly LoginViewModel _viewModel;
        private string _username;
      
        public ProfilePage(string username)
        {
            InitializeComponent();
            BindingContext = this;
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "users.db3");
            _viewModel = new LoginViewModel(new DatabaseService(dbPath), Navigation);
            _username = username;
            LoadData();
        }

        private async void LoadData()
        {
            var userDetails = await _viewModel.GetUserDetails(_username);
            FullNameEntry.Text = userDetails.FullName;
            UserNameEntry.Text = userDetails.UserName;
            EmailEntry.Text = userDetails.Email;
            MobileEntry.Text = userDetails.MobileNumber;
            ProfilePicture.Source = userDetails.ProfileImagePath;
        }

        private async void OnUpdateButtonClicked(object sender, EventArgs e)
        {
            
            // Pass the budget data to the AddBudgetPage for updating
            await Navigation.PushAsync(new EditProfilePage(_username));
        }
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

    }
}
