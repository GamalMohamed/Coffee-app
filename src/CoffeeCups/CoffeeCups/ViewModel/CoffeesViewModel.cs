using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmHelpers;
using Xamarin.Forms;

namespace CoffeeCups
{
    public class CoffeesViewModel : BaseViewModel
    {
        private readonly AzureService _azureService;

        private bool _atHome;

        private string _loadingMessage;

        private ICommand _addCoffeeCommand;

        private ICommand _loadCoffeesCommand;

        public CoffeesViewModel()
        {
            _azureService = DependencyService.Get<AzureService>();
        }

        public ObservableRangeCollection<CupOfCoffee> Coffees { get; } =
            new ObservableRangeCollection<CupOfCoffee>();

        public ObservableRangeCollection<Grouping<string, CupOfCoffee>> CoffeesGrouped { get; } =
            new ObservableRangeCollection<Grouping<string, CupOfCoffee>>();

        public string LoadingMessage
        {
            get { return _loadingMessage; }
            set { SetProperty(ref _loadingMessage, value); }
        }

        public ICommand LoadCoffeesCommand =>
            _loadCoffeesCommand ?? (_loadCoffeesCommand = new Command(async () => await ExecuteLoadCoffeesCommandAsync()))
            ;

        public bool AtHome
        {
            get { return _atHome; }
            set { SetProperty(ref _atHome, value); }
        }

        public ICommand AddCoffeeCommand =>
            _addCoffeeCommand ?? (_addCoffeeCommand = new Command(async () => await ExecuteAddCoffeeCommandAsync()));

        private async Task ExecuteLoadCoffeesCommandAsync()
        {
            if (IsBusy)
                return;

            try
            {
                LoadingMessage = "Loading Coffees...";
                IsBusy = true;
                var coffees = await _azureService.GetCoffees();
                Coffees.ReplaceRange(coffees);

                SortCoffees();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OH NO!" + ex);

                await
                    Application.Current.MainPage.DisplayAlert("Sync Error", "Unable to sync coffees, you may be offline",
                        "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void SortCoffees()
        {
            var groups = from coffee in Coffees
                orderby coffee.DateUtc descending
                group coffee by coffee.DateDisplay
                into coffeeGroup
                select new Grouping<string, CupOfCoffee>($"{coffeeGroup.Key} ({coffeeGroup.Count()})", coffeeGroup);

            CoffeesGrouped.ReplaceRange(groups);
        }

        private async void CheckCoffeeLimit()
        {
            var todayDate = DateTime.Now.ToLocalTime().ToString("d");

            var group = from coffee in Coffees
                where coffee.DateDisplay == todayDate
                select new CupOfCoffee();

            if (@group?.Count() >= 4)
            {
                await
                    Application.Current.MainPage.DisplayAlert("CAUTION", "Take care..That's too many coffees for a day!",
                        "OK");
            }
        }

        private async Task ExecuteAddCoffeeCommandAsync()
        {
            if (IsBusy)
                return;

            try
            {
                LoadingMessage = "Adding Coffee...";
                IsBusy = true;


                var coffee = await _azureService.AddCoffee(AtHome);
                Coffees.Add(coffee);
                SortCoffees();
                CheckCoffeeLimit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OH NO!" + ex);
            }
            finally
            {
                LoadingMessage = string.Empty;
                IsBusy = false;
            }
        }
    }
}