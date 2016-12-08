using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace CoffeeCups.Droid
{
    [Activity(Label = "CoffeeCups", Icon = "@drawable/icon", MainLauncher = true, Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            ToolbarResource = Resource.Layout.toolbar;
            TabLayoutResource = Resource.Layout.tabs;

            base.OnCreate(bundle);

            Forms.Init(this, bundle);
            CurrentPlatform.Init();

            LoadApplication(new App());
        }
    }
}