using System;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GPStandingsGUI.Controllers
{
    class GPStandings
    {
        private Models.ConstructorModel constructor;
        private Models.DriverModel driver;        
        private Models.APIHelper APIhelper;
        
        public ObservableCollection<Models.ConstructorsCollection> constructorsCollection;
        public ObservableCollection<Models.DriversCollection> driversCollection;
        public ObservableCollection<Models.GPStandingsCollection> gpStandings;

        public bool isSearchingConstructors;


        public GPStandings()
        {
            this.constructor = new Models.ConstructorModel();
            this.driver = new Models.DriverModel();
            this.APIhelper = new Models.APIHelper();
            this.APIhelper.InitializeClient();
            this.isSearchingConstructors = false;
        }

        public string CheckEntry(string userEntry)
        {
            string message;

            int currentYear = DateTime.Now.Year;

            int.TryParse(userEntry, out int year);

            if (!userEntry.All(char.IsNumber))
            {
                return message = "Not a number";
            }
            if ((this.isSearchingConstructors) && (year < 1958 || year > currentYear))
            {
                return message = "Out of range, Search constructors from 1958";
            }
            if ((!this.isSearchingConstructors) && (year < 1950 || year > currentYear))
            {
                return message = "Out of range, Search drivers from 1950";
            }
            else
            {
                return message = null;
            }
        }

        public async Task<ObservableCollection<Models.GPStandingsCollection>> GetStandings(int year)
        {
            string url;

            if (this.isSearchingConstructors)
            {
                url = $"https://ergast.com/api/f1/{year}/constructorStandings.json";

                return await this.GetGPStandings(url);
            }
            else
            {
                url = $"http://ergast.com/api/f1/{year}/driverStandings.json";
                return await this.GetGPStandings(url);
            }
        }

        private async Task<ObservableCollection<Models.GPStandingsCollection>> GetGPStandings(string url)
        {
            this.gpStandings = new ObservableCollection<Models.GPStandingsCollection>();

            if (isSearchingConstructors)
            {
                this.constructor = await this.GetModelData(this.constructor, url);

                foreach (var constructor in this.constructor.MRData.StandingsTable.StandingsLists[0].ConstructorStandings)
                {
                    this.gpStandings.Add(new Models.GPStandingsCollection()
                    {
                        Position = int.Parse(constructor.position),
                        Constructor = constructor.Constructor.name,
                        Points = float.Parse(constructor.points),
                        Wins = int.Parse(constructor.wins),
                        Nationality = constructor.Constructor.nationality
                    });
                }

                return this.gpStandings;
            }

            else
            {
                this.driver = await this.GetModelData(this.driver, url);

                foreach (var driver in this.driver.MRData.StandingsTable.StandingsLists[0].DriverStandings)
                {
                    this.gpStandings.Add(new Models.GPStandingsCollection()
                    {
                        Position = int.Parse(driver.position),
                        Driver = $"{driver.Driver.givenName[0]}. {driver.Driver.familyName} ({(driver.Driver.permanentNumber != null ? driver.Driver.permanentNumber : "--")})",
                        Points = float.Parse(driver.points),
                        Wins = int.Parse(driver.wins),
                        Constructor = driver.Constructors[0].name,
                        Nationality = driver.Driver.nationality
                    });
                }

                return this.gpStandings;
            }
        }

        private async Task<ObservableCollection<Models.ConstructorsCollection>> GetConstructorsStandings(string url)
        {
            this.constructorsCollection = new ObservableCollection<Models.ConstructorsCollection>();

            this.constructor = await this.GetModelData(this.constructor, url);

            foreach (var constructor in this.constructor.MRData.StandingsTable.StandingsLists[0].ConstructorStandings)
            {
                this.constructorsCollection.Add(new Models.ConstructorsCollection()
                {
                    Position = int.Parse(constructor.position),
                    Constructor = constructor.Constructor.name,
                    Points = float.Parse(constructor.points),
                    Wins = int.Parse(constructor.wins),
                    Nationality = constructor.Constructor.nationality
                });
            }

            return this.constructorsCollection;

        }

        private async Task<ObservableCollection<Models.DriversCollection>> GetDriversStandings(string url)
        {
            this.driversCollection = new ObservableCollection<Models.DriversCollection>();

            this.driver = await this.GetModelData(this.driver, url);

            foreach (var driver in this.driver.MRData.StandingsTable.StandingsLists[0].DriverStandings)
            {
                this.driversCollection.Add(new Models.DriversCollection()
                {
                    Position = int.Parse(driver.position),
                    Driver = $"{driver.Driver.givenName[0]}. {driver.Driver.familyName} ({(driver.Driver.permanentNumber != null ? driver.Driver.permanentNumber : "--")})",
                    Points = float.Parse(driver.points),
                    Wins = int.Parse(driver.wins),
                    Constructor = driver.Constructors[0].name,
                    Nationality = driver.Driver.nationality
                });
            }

            return this.driversCollection;

        }

        // async stops app freezing by running process similanteously.
        private async Task<T> GetModelData<T>(T type, string url)
        {
            // make a new request from api client and wait for reponse then dispose.
            using (HttpResponseMessage APIResponse = await this.APIhelper.ApiClient.GetAsync(url))
            {
                if (APIResponse.IsSuccessStatusCode)
                {
                    // Using newton converter to take specified json data and convert to specified type (DriverStandings properies)
                    // By calling .Result you are synchronously reading the result,
                    T modelData = await APIResponse.Content.ReadAsAsync<T>();

                    return modelData;
                }
                else
                {
                    throw new Exception(APIResponse.ReasonPhrase);
                }
            }
        }

        //private async Task<string> GetConstructorsStandings(string url)
        //{
        //    this.constructor = await this.GetModelData(this.constructor, url);

        //    StringBuilder constructorsStandingsList = new StringBuilder();

        //    constructorsStandingsList.AppendLine(
        //        $"Formula 1 {constructor.MRData.StandingsTable.StandingsLists[0].season} - Round {constructor.MRData.StandingsTable.StandingsLists[0].round}");
        //    constructorsStandingsList.AppendLine();

        //    foreach (var constructor in this.constructor.MRData.StandingsTable.StandingsLists[0].ConstructorStandings)
        //    {
        //        constructorsStandingsList.AppendLine(
        //            $"{constructor.position} - {constructor.Constructor.name} : {constructor.points} ({constructor.wins}) - {constructor.Constructor.nationality}");
        //    }

        //    return constructorsStandingsList.ToString();
        //}

        //private string GetDriversStandings(string url)
        //{
        //    this.driver = this.GetModelData(this.driver, url);

        //    StringBuilder driversStandingsList = new StringBuilder();

        //    driversStandingsList.AppendLine(
        //        $"Formula 1 {driver.MRData.StandingsTable.StandingsLists[0].season} - Round {driver.MRData.StandingsTable.StandingsLists[0].round}");
        //    driversStandingsList.AppendLine();

        //    foreach (var driver in this.driver.MRData.StandingsTable.StandingsLists[0].DriverStandings)
        //    {
        //        driversStandingsList.AppendLine(
        //            $"{driver.position} - {driver.Driver.familyName} ({driver.Driver.permanentNumber}) : {driver.points} ({driver.wins}) - {driver.Constructors[0].name}");
        //    }

        //    return driversStandingsList.ToString();
        //}       
    }
}
