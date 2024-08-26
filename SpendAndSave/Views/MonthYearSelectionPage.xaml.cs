using Microsoft.Maui.Controls;
using System;

namespace SpendAndSave.Views
{
    public partial class MonthYearSelectionPage : ContentPage
    {
        public MonthYearSelectionPage()
        {
            InitializeComponent();
            // Set default selected month and year to current month and year
            monthPicker.SelectedIndex = DateTime.Now.Month - 1;
            yearPicker.SelectedItem = DateTime.Now.Year;
        }

        private async void OnOkButtonClicked(object sender, EventArgs e)
        {
            if (monthPicker.SelectedIndex != -1 && yearPicker.SelectedItem != null)
            {
                int selectedMonth = monthPicker.SelectedIndex + 1;
                int selectedYear = (int)yearPicker.SelectedItem;

                DateTime selectedDate = new DateTime(selectedYear, selectedMonth, 1);

                MessagingCenter.Send(this, "MonthYearSelected", selectedDate);
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Invalid Selection", "Please select both month and year.", "OK");
            }
        }

        private async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
