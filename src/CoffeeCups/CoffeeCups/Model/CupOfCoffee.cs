using System;

namespace CoffeeCups
{
    public class CupOfCoffee
    {
        [Newtonsoft.Json.JsonProperty("Id")]
        public string Id { get; set; }

        [Microsoft.WindowsAzure.MobileServices.Version]
        public string AzureVersion { get; set; }


        [Newtonsoft.Json.JsonProperty("userId")]
        public string UserId { get; set; }

        public DateTime DateUtc { get; set; }

        public bool MadeAtHome { get; set; }

        public string OS { get; set; }


        [Newtonsoft.Json.JsonIgnore]
        public string DateDisplay { get { return DateUtc.ToLocalTime().ToString("d"); } }

        [Newtonsoft.Json.JsonIgnore]
        public string TimeDisplay { get { return DateUtc.ToLocalTime().ToString("t") + " " + OS.ToString(); } }

        [Newtonsoft.Json.JsonIgnore]
        public string AtHomeDisplay { get { return MadeAtHome ? "Made At Home" : string.Empty; } }
    }
}

