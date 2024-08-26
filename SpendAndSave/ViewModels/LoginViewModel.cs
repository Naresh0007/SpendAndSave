using SpendAndSave.Models;
using SpendAndSave.Services;
using SpendAndSave.Views;

namespace SpendAndSave.ViewModels
{
	public class LoginViewModel
	{

		private readonly DatabaseService _databaseService;
        private readonly INavigation _navigation;
        private int _loggedInUserId;

        public LoginViewModel(DatabaseService databaseService, INavigation navigation)
        {
            _databaseService = databaseService;
            _navigation = navigation;
        }


        public async Task<bool> LoginAsync(string username, string password)
        {
            var user = await _databaseService.GetUserAsync(username, password);
            if (user != null)
            {
                // Save the user session or token as needed
                _loggedInUserId = user.Id; // Store the logged-in user's ID
                await _navigation.PushAsync(new DashboardPage(user));
                return true;
            }
            return false;
        }


            public async Task<LoginRequestModel> GetUserDetails(string username)
            {
                return await _databaseService.GetUserDetails(username);
            }

            public async Task<LoginRequestModel> GetLoggedInUserAsync()
        {
            if (_loggedInUserId != 0)
            {
                return await _databaseService.GetLoggedInUserAsync(_loggedInUserId);
            }

            return null; // No user logged in
        }


        public async Task<bool> RegisterAsync(string fullname, string email, string mobile, string username, string password)
        {
            var existingUser = await _databaseService.GetUserByUsernameAsync(username);
            System.Console.WriteLine($"existingUser {existingUser}");
            if (existingUser == null)
            {
                var user = new LoginRequestModel { FullName = fullname, Email = email, MobileNumber = mobile, UserName = username, Password = password };
                System.Console.WriteLine($"user {user}");
                await _databaseService.SaveUserAsync(user);
                return true;
            }
            return false;
        }

        public async Task<int> SaveUserAsync(LoginRequestModel user)
        {
           return await _databaseService.SaveUserAsync(user);
        }


        public void Logout()
        {
            // Clear the user session or token
        }

    }
}

