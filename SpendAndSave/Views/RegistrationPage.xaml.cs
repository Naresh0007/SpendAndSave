using SpendAndSave.Services;
using SpendAndSave.ViewModels;
using SpendAndSave.Models;

namespace SpendAndSave.Views;

public partial class RegistrationPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public RegistrationPage()
    {
        InitializeComponent();
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "users.db3");
        _viewModel = new LoginViewModel(new DatabaseService(dbPath), Navigation);
    }
   

    private async void OnRegisterButtonClicked(object sender, EventArgs e)
    {
        var newUser = new LoginRequestModel
        {
            FullName = FullNameEntry.Text,
            Email = EmailEntry.Text,
            MobileNumber = MobileEntry.Text,
            UserName = UserNameEntry.Text,
            Password = PasswordEntry.Text
        };

        var fullname = FullNameEntry.Text;
        var email = EmailEntry.Text;
        var mobile = MobileEntry.Text;
        var username = UserNameEntry.Text;
        var password = PasswordEntry.Text;
        var confirmPassword = ConfirmPasswordEntry.Text;

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

        if (string.IsNullOrEmpty(PasswordEntry.Text))
        {
            await DisplayAlert("Validation Error", "Please enter a Password", "OK");
            return;
        }

        if (string.IsNullOrEmpty(ConfirmPasswordEntry.Text))
        {
            await DisplayAlert("Validation Error", "Please confirm your Password", "OK");
            return;
        }

        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            await DisplayAlert("Validation Error", "Passwords do not match", "OK");
            return;
        }


        var isRegistered = await _viewModel.RegisterAsync(fullname, email, mobile, username, password);
        if (isRegistered)
        {
            await DisplayAlert("Success", "Registration successful", "OK");
            await Navigation.PushAsync(new DashboardPage(newUser));
            // Navigation to DashboardPage is handled in LoginViewModel

        }
        else
        {
            await DisplayAlert("Error", "Username already exists", "OK");
        }
    }

    private async void OnSignInButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
    }
}
