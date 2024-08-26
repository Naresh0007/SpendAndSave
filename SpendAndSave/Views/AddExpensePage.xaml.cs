using SpendAndSave.Models;
using SpendAndSave.Data;
using Microsoft.Maui.Storage;

namespace SpendAndSave.Views
{
    public partial class AddExpensePage : ContentPage
    {
        private readonly ExpenseModel _expenseModel;
        private readonly ExpenseData _expenseToUpdate;
        private readonly string _username; // Store the logged-in username
        private string _receiptPath;
        private List<CategoryData> _categories;


        public AddExpensePage(string username, ExpenseData expenseToUpdate = null)
        {
            InitializeComponent();

            // Initialize ExpenseModel with the correct parameters
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "users.db3");
            _expenseModel = new ExpenseModel(dbPath, Navigation);
            _username = username; // Assign the username
            _expenseToUpdate = expenseToUpdate;
            LoadCategories();
        }

        private async void LoadCategories()
        {
            var date = DateTime.Now;
            if (_expenseToUpdate != null)
            {
                date = _expenseToUpdate.Date;
            }
            var categories = await _expenseModel.GetCategoriesAsync(_username, date);
            _categories = categories;
            var categoryNames = _categories.Select(c => c.Name).ToList(); // Convert to list of strings
            //categoryNames.Add("Add new category...");
            categoryPicker.ItemsSource = categoryNames;

            if (_expenseToUpdate != null)
            {
                // Pre-load data if updating an expense
                expenseEntry.Text = _expenseToUpdate.EntryType;
                amountEntry.Text = _expenseToUpdate.Amount.ToString();
                categoryPicker.SelectedItem = _expenseToUpdate.Category;
                datePicker.Date = _expenseToUpdate.Date;
                locationEntry.Text = _expenseToUpdate.Location;
                _receiptPath = _expenseToUpdate.ReceiptPath;
                CapturedImage.Source = ImageSource.FromFile(_receiptPath);
            }
        }

        private async void OnDetectLocationClicked(object sender, EventArgs e)
        {
            try
            {
                // Check and request location permissions
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permission Denied", "Unable to access location. Please enable location permissions in your settings.", "OK");
                    return;
                }
                var location = await Geolocation.GetLastKnownLocationAsync();
                
                if (location != null)
                {
                    var placemarks = await Geocoding.GetPlacemarksAsync(location);
                    var placemark = placemarks?.FirstOrDefault();

                    if (placemark != null)
                    {
                        // Set location entry to suburb/locality or city
                        locationEntry.Text = !string.IsNullOrEmpty(placemark.SubLocality) ? placemark.SubLocality : placemark.Locality;
                        locationEntry.Text += !string.IsNullOrEmpty(placemark.AdminArea) ? placemark.AdminArea : " ";
                        locationEntry.Text += !string.IsNullOrEmpty(placemark.CountryName) ? placemark.CountryName : " ";

                    }
                    else
                    {
                        await DisplayAlert("Error", "Unable to find location details.", "OK");
                    }
                }
                else {
                    await DisplayAlert("Error", "Unable to find location details.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to detect location: {ex.Message}", "OK");
            }
        }

        private async void OnBrowseFilesClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync();
                if (result != null)
                {
                    _receiptPath = result.FullPath;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to browse files: {ex.Message}", "OK");
            }
        }

        private async void OnCameraClicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo != null)
                {
                    var newFile = Path.Combine(FileSystem.AppDataDirectory, $"{DateTime.Now:yyyyMMddHHmmss}.jpg");
                    using (var stream = await photo.OpenReadAsync())
                    using (var newStream = File.OpenWrite(newFile))
                        await stream.CopyToAsync(newStream);
                    _receiptPath = newFile;
                    CapturedImage.Source = ImageSource.FromFile(_receiptPath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to take photo: {ex.Message}", "OK");
            }
        }

        private async void OnSaveExpenseButtonClicked(object sender, EventArgs e)
        {
            // Check if all required fields are filled
            if (string.IsNullOrWhiteSpace(expenseEntry.Text))
            {
                await DisplayAlert("Validation Error", "Expense Entry Type is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(amountEntry.Text) || !decimal.TryParse(amountEntry.Text, out var amount) || amount <= 0)
            {
                await DisplayAlert("Validation Error", "Please enter a valid amount greater than zero.", "OK");
                return;
            }

            if (categoryPicker.SelectedItem == null || categoryPicker.SelectedItem.ToString() == "Add new category...")
            {
                await DisplayAlert("Validation Error", "Please select a valid category.", "OK");
                return;
            }

            if (datePicker.Date == DateTime.MinValue)
            {
                await DisplayAlert("Validation Error", "Please select a valid date.", "OK");
                return;
            }

            var expenseItem = new ExpenseData
            {
                EntryType = expenseEntry.Text,
                Amount = Convert.ToDecimal(amountEntry.Text),
                Category = categoryPicker.SelectedItem.ToString(),
                Date = datePicker.Date,
                Location = locationEntry.Text,  // Optional
                ReceiptPath = _receiptPath, // Set if a receipt is added
                Username = _username // Set the username
            };
            
            if (_expenseToUpdate != null)
            { 
                // Update existing expense
                expenseItem.Id = _expenseToUpdate.Id; // Ensure the ID is set for updating

            }
            await _expenseModel.SaveExpenseAsync(expenseItem);

            // Notify ExpensePage to reload data
            MessagingCenter.Send(this, "ExpenseUpdated");

            // Navigate back to the previous page
            await Navigation.PopAsync();
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnCategorySelected(object sender, EventArgs e)
        {
            if (categoryPicker.SelectedItem.ToString() == "Add new category...")
            {
                string result = await DisplayPromptAsync("New Category", "Enter the name of the new category:");
                if (!string.IsNullOrEmpty(result))
                {
                    await _expenseModel.AddCategoryAsync(result, _username);
                    LoadCategories();
                    categoryPicker.SelectedItem = result;
                }
            }
        }
    }
}
