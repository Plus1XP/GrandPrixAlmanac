using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using GrandPrixAlmanac.Models;

namespace GrandPrixAlmanac.Controllers
{
    internal class APIController
    {
        private readonly APIHelper apiHelper;

        private readonly string apiURL;
        private ConstructorModel constructorModel;
        private DriverModel driverModel;

        //int year;
        //string table;
        //private readonly string apiURL2 = string.Format("https://ergast.com/api/f1/{0}/{1}.json", year, table);

        public APIController(int year, string table)
        {
            this.constructorModel = new ConstructorModel();
            this.driverModel = new DriverModel();

            this.apiHelper = new APIHelper();
            this.apiHelper.InitializeClient();

            this.apiURL = $"https://ergast.com/api/f1/{year}/{table}.json";
        }

        public async Task<Tuple<string, ObservableCollection<object>>> ConstructorsStandings()
        {
            var standingsCollection = new ObservableCollection<object>();

            this.constructorModel = await this.GetModelData(this.constructorModel, this.apiURL);

            var heading =
                $"{this.constructorModel.MRData.StandingsTable.StandingsLists[0].season} Formula 1 Season - {this.constructorModel.MRData.StandingsTable.StandingsLists[0].round} Rounds";

            foreach (var constructor in this.constructorModel.MRData.StandingsTable.StandingsLists[0].ConstructorStandings)
                standingsCollection.Add(new ConstructorsStandingsCollection
                {
                    Position = int.Parse(constructor.position),
                    Constructor = constructor.Constructor.name,
                    Points = float.Parse(constructor.points),
                    Wins = int.Parse(constructor.wins),
                    Nationality = constructor.Constructor.nationality
                });

            return new Tuple<string, ObservableCollection<object>>(heading, standingsCollection);
        }

        public async Task<Tuple<string, ObservableCollection<object>>> DriversStandings()
        {
            var standingsCollection = new ObservableCollection<object>();

            this.driverModel = await this.GetModelData(this.driverModel, this.apiURL);

            var heading =
                $"{this.driverModel.MRData.StandingsTable.StandingsLists[0].season} Formula 1 Season - {this.driverModel.MRData.StandingsTable.StandingsLists[0].round} Rounds";

            foreach (var driver in this.driverModel.MRData.StandingsTable.StandingsLists[0].DriverStandings)
                standingsCollection.Add(new DriversStandingsCollection
                {
                    Position = int.Parse(driver.position),
                    Driver =
                        $"{driver.Driver.givenName[0]}. {driver.Driver.familyName} ({(driver.Driver.permanentNumber != null ? driver.Driver.permanentNumber : "--")})",
                    Points = float.Parse(driver.points),
                    Wins = int.Parse(driver.wins),
                    Constructor = driver.Constructors[0].name,
                    Nationality = driver.Driver.nationality
                });

            return new Tuple<string, ObservableCollection<object>>(heading, standingsCollection);
        }

        // async stops app freezing by running process similanteously.
        private async Task<T> GetModelData<T>(T type, string url)
        {
            // make a new request from api client and wait for reponse then dispose.
            using (var apiResponse = await this.apiHelper.ApiClient.GetAsync(url))
            {
                if (apiResponse.IsSuccessStatusCode)
                {
                    // Using newton converter to take specified json data and convert to specified type (DriverStandings properies)
                    // By calling .Result you are synchronously reading the result,
                    var modelData = await apiResponse.Content.ReadAsAsync<T>();

                    return modelData;
                }
                throw new Exception(apiResponse.ReasonPhrase);
            }
        }
    }
}