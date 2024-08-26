using System;
using System.Windows.Input;

namespace SpendAndSave.ViewModels
{
	public class DashboardPageViewModel
	{

		public ICommand LogoutCommand { get; }
		public ICommand TakePhoto { get; }
		public ICommand UploadImage { get; }

        public DashboardPageViewModel()
		{

			LogoutCommand = new Command(PerformLogoutOperation);
			TakePhoto = new Command(PerformPictureClickOperation);
			UploadImage = new Command(PerformUploadPhotoOperation);

        }

        private async void PerformLogoutOperation(object obj)
        {

			await Shell.Current.GoToAsync("//Login");

        }

        private async void PerformPictureClickOperation(object obj)
        {

            if (MediaPicker.Default.IsCaptureSupported)
            {
                //Take Picture
                FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    // save the file into local storage
                    string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                    using Stream sourceStream = await photo.OpenReadAsync();
                    using FileStream localFileStream = File.OpenWrite(localFilePath);

                    await sourceStream.CopyToAsync(localFileStream);
                }
            }
        }

        private async void PerformUploadPhotoOperation(object obj)
        {

            if (MediaPicker.Default.IsCaptureSupported)
            {
                //Take Picture
                FileResult StoragePicture = await MediaPicker.Default.PickPhotoAsync();

                if (StoragePicture != null)
                {
                    // save the file into local storage
                    string localFilePath = Path.Combine(FileSystem.CacheDirectory, StoragePicture.FileName);

                    using Stream sourceStream = await StoragePicture.OpenReadAsync();
                    using FileStream localFileStream = File.OpenWrite(localFilePath);

                    await sourceStream.CopyToAsync(localFileStream);
                    await Shell.Current.DisplayAlert("Select Picture", localFileStream.Name, "ok");
                }
            }
        }
    }
}

