using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Xamarin.Forms;

namespace CoffeeCups
{
    public partial class CoffeesPage : ContentPage
    {
        private readonly CoffeesViewModel vm;

        public CoffeesPage()
        {
            InitializeComponent();
            BindingContext = vm = new CoffeesViewModel();
            ListViewCoffees.ItemTapped += (sender, e) =>
            {
                if (Device.OS == TargetPlatform.iOS || Device.OS == TargetPlatform.Android)
                    ListViewCoffees.SelectedItem = null;
            };

            if (Device.OS != TargetPlatform.iOS && Device.OS != TargetPlatform.Android)
            {
                ToolbarItems.Add(new ToolbarItem
                {
                    Text = "Refresh",
                    Command = vm.LoadCoffeesCommand
                });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            CrossConnectivity.Current.ConnectivityChanged += ConnecitvityChanged;
            OfflineStack.IsVisible = !CrossConnectivity.Current.IsConnected;

            vm.LoadCoffeesCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            CrossConnectivity.Current.ConnectivityChanged -= ConnecitvityChanged;
        }

        private void ConnecitvityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => { OfflineStack.IsVisible = !e.IsConnected; });
        }
    }
}