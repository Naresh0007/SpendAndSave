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
    public partial class EditProfilePage : ContentPage
    {
        private readonly LoginViewModel _viewModel;
        private string _username;
        private string _profileImagePath;
        public EditProfilePage(string username)
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
            _profileImagePath = userDetails.ProfileImagePath;

        }

        async void OnChangeProfilePictureClicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync();
                if (photo != null)
                {
                    var fileName = Path.Combine(FileSystem.AppDataDirectory, $"{Path.GetRandomFileName()}.jpg");

                    using (var stream = await photo.OpenReadAsync())
                    using (var newStream = File.OpenWrite(fileName))
                    {
                        await stream.CopyToAsync(newStream);
                    }

                    ProfilePicture.Source = ImageSource.FromFile(fileName);
                    _profileImagePath = fileName; // Store the file path in a variable
                }
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Error", "Photo gallery not supported on this device.", "OK");
            }
            catch (PermissionException)
            {
                await DisplayAlert("Error", "Permission to access the photo gallery denied.", "OK");
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "An error occurred while selecting the photo.", "OK");
            }
        }

        private async void OnUpdateButtonClicked(object sender, EventArgs e)
        {
            var userDetails = await _viewModel.GetUserDetails(_username);

            var editUser = new LoginRequestModel
            {
                Id = userDetails.Id,
                FullName = FullNameEntry.Text,
                Email = EmailEntry.Text,
                MobileNumber = MobileEntry.Text,
                UserName = UserNameEntry.Text,
                Password = userDetails.Password, // Keep the existing password
                ProfileImagePath = _profileImagePath
            };

            // Validate fields
            if (string.IsNullOrEmpty(FullNameEntry.Text))
            {
                await DisplayAlert("Validation Error", "Please enter your Full Name", "OK");
                return;
            }

            if (string.IsNullOrEmpty(EmailEntry.Text))
            {
                await DisplayAlert("Validation Error", "Please enter your Email", "OK");
                return;
            }

            if (string.IsNullOrEmpty(MobileEntry.Text))
            {
                await DisplayAlert("Validation Error", "Please enter your Mobile Number", "OK");
                return;
            }

            if (string.IsNullOrEmpty(UserNameEntry.Text))
            {
                await DisplayAlert("Validation Error", "Please enter a Username", "OK");
                return;
            }

                await DisplayAlert("Success", "Profile Update successful", "OK");

            await _viewModel.SaveUserAsync(editUser);

            await Navigation.PushAsync(new ProfilePage(_username));
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

    }
}
