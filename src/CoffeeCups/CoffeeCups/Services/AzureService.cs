using System;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System.Diagnostics;
using Xamarin.Forms;
using CoffeeCups;
using System.IO;
using System.Threading;
using Plugin.Connectivity;


[assembly: Dependency(typeof(AzureService))]
namespace CoffeeCups
{
    public class AzureService
    {

        public MobileServiceClient Client { get; set; } = null;
        IMobileServiceSyncTable<CupOfCoffee> _coffeeTable;
        public static bool UseAuth { get; set; } = false;

        public async Task Initialize()
        {
            if (Client?.SyncContext?.IsInitialized ?? false)
                return;

            const string appUrl = "http://coffeeapp777.azurewebsites.net";


#if AUTH      
            Client = new MobileServiceClient(appUrl, new AuthHandler());

            if (!string.IsNullOrWhiteSpace (Settings.AuthToken) && !string.IsNullOrWhiteSpace (Settings.UserId)) {
                Client.CurrentUser = new MobileServiceUser (Settings.UserId);
                Client.CurrentUser.MobileServiceAuthenticationToken = Settings.AuthToken;
            }
#else
            //Create our client

            Client = new MobileServiceClient(appUrl);

#endif

            //InitialzeDatabase for path
            var path = "Store.db";
            path = Path.Combine(MobileServiceClient.DefaultDatabasePath, path);

            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore(path);

            //Define table
            store.DefineTable<CupOfCoffee>();

            //Initialize SyncContext
            await Client.SyncContext.InitializeAsync(store);

            //Get our sync table that will call out to azure
            _coffeeTable = Client.GetSyncTable<CupOfCoffee>();


        }

        public async Task SyncCoffee()
        {
            try
            {
                if (!CrossConnectivity.Current.IsConnected)
                    return;

                await _coffeeTable.PullAsync("allCoffee", _coffeeTable.CreateQuery());

                await Client.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to sync coffees, that is alright as we have offline capabilities: " + ex);
            }

        }

        public async Task<IEnumerable<CupOfCoffee>> GetCoffees()
        {
            //Initialize & Sync
            await Initialize();
            await SyncCoffee();

            return await _coffeeTable.OrderBy(c => c.DateUtc).ToEnumerableAsync(); ;

        }

        public async Task<CupOfCoffee> AddCoffee(bool atHome)
        {
            await Initialize();

            var coffee = new CupOfCoffee
            {
                DateUtc = DateTime.UtcNow,
                MadeAtHome = atHome,
                OS = Device.OS.ToString()
            };

            await _coffeeTable.InsertAsync(coffee);

            await SyncCoffee();

            return coffee;
        }

    }
}

