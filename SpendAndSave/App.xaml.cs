using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using SpendAndSave.Data;

namespace SpendAndSave;

public partial class App : Application
{
	public App()
	{
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NCaF1cWWhBYVB3WmFZfVpgdV9EaFZVTWYuP1ZhSXxXdk1jWH9fcnxQRmFeVkM=");
        InitializeComponent();
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
        {
            h.PlatformView.BackgroundTintList =
                Android.Content.Res.ColorStateList.ValueOf(Colors.White.ToAndroid());
        });

        MainPage = new AppShell();
    }

}

