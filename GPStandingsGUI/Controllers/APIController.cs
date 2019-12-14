using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GPStandingsGUI.Controllers
{
    class APIController
    {
        private Models.ConstructorModel constructor;
        private Models.DriverModel driver;

        private readonly Models.APIHelper apiHelper;
        
        private readonly string apiURL;

        public APIController(int year, string table)
        {
            this.constructor = new Models.ConstructorModel();
            this.driver = new Models.DriverModel();

            this.apiHelper = new Models.APIHelper();
            this.apiHelper.InitializeClient();

            this.apiURL = $"https://ergast.com/api/f1/{year}/{table}.json";
        }

        public async Task<Tuple<string, ObservableCollection<Models.GPStandingsCollection>>> ConstructorsStandings()
        {
            ObservableCollection<Models.GPStandingsCollection> standingsTable = new ObservableCollection<Models.GPStandingsCollection>();
            
            this.constructor = await this.GetModelData(this.constructor, this.apiURL);

            string heading =
                $"{this.constructor.MRData.StandingsTable.StandingsLists[0].season} Formula 1 Season - {this.constructor.MRData.StandingsTable.StandingsLists[0].round} Rounds";

            foreach (Models.ConstructorModel.Constructorstanding constructor in this.constructor.MRData.StandingsTable.StandingsLists[0].ConstructorStandings)
            {
                standingsTable.Add(new Models.GPStandingsCollection()
                {
                    Position = int.Parse(constructor.position),
                    Constructor = constructor.Constructor.name,
                    Points = float.Parse(constructor.points),
                    Wins = int.Parse(constructor.wins),
                    Nationality = constructor.Constructor.nationality
                });
            }

            return new Tuple<string, ObservableCollection<Models.GPStandingsCollection>>(heading, standingsTable);

        }

        public async Task<Tuple<string, ObservableCollection<Models.GPStandingsCollection>>> DriversStandings()
        {
            ObservableCollection<Models.GPStandingsCollection> standingsTable = new ObservableCollection<Models.GPStandingsCollection>();
            
            this.driver = await this.GetModelData(this.driver, this.apiURL);

            string heading =
                $"{this.driver.MRData.StandingsTable.StandingsLists[0].season} Formula 1 Season - {driver.MRData.StandingsTable.StandingsLists[0].round} Rounds";

            foreach (Models.DriverModel.Driverstanding driver in this.driver.MRData.StandingsTable.StandingsLists[0].DriverStandings)
            {
                standingsTable.Add(new Models.GPStandingsCollection()
                {
                    Position = int.Parse(driver.position),
                    Driver =
                        $"{driver.Driver.givenName[0]}. {driver.Driver.familyName} ({(driver.Driver.permanentNumber != null ? driver.Driver.permanentNumber : "--")})",
                    Points = float.Parse(driver.points),
                    Wins = int.Parse(driver.wins),
                    Constructor = driver.Constructors[0].name,
                    Nationality = driver.Driver.nationality
                });
            }

            return new Tuple<string, ObservableCollection<Models.GPStandingsCollection>>(heading, standingsTable);

        }

        // async stops app freezing by running process similanteously.
        private async Task<T> GetModelData<T>(T type, string url)
        {
            // make a new request from api client and wait for reponse then dispose.
            using (HttpResponseMessage apiResponse = await this.apiHelper.ApiClient.GetAsync(url))
            {
                if (apiResponse.IsSuccessStatusCode)
                {
                    // Using newton converter to take specified json data and convert to specified type (DriverStandings properies)
                    // By calling .Result you are synchronously reading the result,
                    T modelData = await apiResponse.Content.ReadAsAsync<T>();

                    return modelData;
                }
                else
                {
                    throw new Exception(apiResponse.ReasonPhrase);
                }
            }
        }
    }
}
