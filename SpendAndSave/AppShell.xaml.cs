using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace SpendAndSave;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        var isUserLoggedIn = Preferences.Get("UserAlreadyloggedIn", false);

        
            MyAppShell.CurrentItem = WelcomePage;

       

    }


    public void Logout()
    {
        // Clear all saved preferences
        Preferences.Clear();

        // Redirect to the login page
        CurrentItem = MyLoginPage;
    }
}

