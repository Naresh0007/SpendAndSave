using SpendAndSave.Services;
using SpendAndSave.ViewModels;

namespace SpendAndSave.Views;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public LoginPage()
    {
        InitializeComponent();
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "users.db3");
        _viewModel = new LoginViewModel(new DatabaseService(dbPath), Navigation);
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        var username = UsernameEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(UsernameEntry.Text))
        {
            await DisplayAlert("Validation Error", "Username is required", "OK");
            return;
        }

        if (string.IsNullOrEmpty(PasswordEntry.Text))
        {
            //await DisplayAlert("Validation Error", "Password is required", "OK");
           // return;
        }

        var isAuthenticated = await _viewModel.LoginAsync(username, password);
        if (isAuthenticated)
        {
            Preferences.Set("UserAlreadyloggedIn", true);
            // Navigation to DashboardPage is handled in LoginViewModel

        }
        else
        {
            await DisplayAlert("Error", "Invalid username or password", "OK");
        }
    }

    private async void OnRegisterButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistrationPage());
    }
}
