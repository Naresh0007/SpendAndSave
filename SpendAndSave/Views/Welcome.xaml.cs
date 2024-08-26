using SpendAndSave.Services;
using SpendAndSave.ViewModels;

namespace SpendAndSave.Views;

public partial class WelcomePage : ContentPage
{

    public WelcomePage()
    {
        InitializeComponent();
     
    }

   
    private async void OnGetStartedButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistrationPage());
    }
}
