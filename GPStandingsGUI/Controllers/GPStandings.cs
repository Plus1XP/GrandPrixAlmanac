using System;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;

namespace GPStandingsGUI.Controllers
{
    class GPStandings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Models.ConstructorModel constructor;
        private Models.DriverModel driver;
        private Models.APIHelper APIhelper;

        public AsyncRelayCommand Cmd { get; private set; }

        private ObservableCollection<Models.ConstructorsCollection> constructorsCollection;
        private ObservableCollection<Models.DriversCollection> driversCollection;

        private ObservableCollection<Models.GPStandingsCollection> gpStandings;

        public ObservableCollection<Models.GPStandingsCollection> GpStandings { get { return gpStandings; } set { gpStandings = value; OnPropertyChanged("GpStandings"); } }

        private bool canSearchConstructors;

        public bool CanSearchConstructors { get { return canSearchConstructors; } set { canSearchConstructors = value; OnPropertyChanged("CanSearchConstructors"); } }

        private string userYear;

        public string UserYear { get { return userYear; } set { userYear = value; OnPropertyChanged("UserYear"); } }

        private int year;

        public int Year { get { return year; } set { year = value; OnPropertyChanged("Year"); } }

        private string heading;

        public string Heading { get { return heading; } set { heading = value; OnPropertyChanged("Heading"); } }

        public string ButtonContent { get { return "Go!"; } }  

        public GPStandings()
        {
            this.constructor = new Models.ConstructorModel();
            this.driver = new Models.DriverModel();

            constructorsCollection = new ObservableCollection<Models.ConstructorsCollection>();
            driversCollection = new ObservableCollection<Models.DriversCollection>();
            GpStandings = new ObservableCollection<Models.GPStandingsCollection>();

            Cmd = new AsyncRelayCommand(() => ProcessResults(CheckDateEntryIsValid()));

            this.APIhelper = new Models.APIHelper();
            this.APIhelper.InitializeClient();
            this.CanSearchConstructors = false;
        }

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        //public string CheckEntry(string userEntry)
        //{
        //    string warning;

        //    int currentYear = DateTime.Now.Year;

        //    int.TryParse(userEntry, out int year);

        //    if (!userEntry.All(char.IsNumber))
        //    {
        //        return warning = "Not a number";
        //    }
        //    if ((this.isSearchingConstructors) && (year < 1958 || year > currentYear))
        //    {
        //        return warning = "Out of range, Search constructors from 1958";
        //    }
        //    if ((!this.isSearchingConstructors) && (year < 1950 || year > currentYear))
        //    {
        //        return warning = "Out of range, Search drivers from 1950";
        //    }
        //    else
        //    {
        //        return warning = null;
        //    }
        //}

        public string CheckDateEntryIsValid()
        {
            string warning = null;

            int currentYear = DateTime.Now.Year;

            int.TryParse(UserYear, out int parsedYear);

            this.Year = parsedYear;

            if (!UserYear.All(char.IsNumber))
            {
                warning = "Not a number";
            }
            if ((this.CanSearchConstructors) && (Year < 1958 || Year > currentYear))
            {
                warning = "Out of range, Search constructors from 1958";
            }
            if ((!this.CanSearchConstructors) && (Year < 1950 || Year > currentYear))
            {
                warning = "Out of range, Search drivers from 1950";
            }

            return warning;
        }

        //public async Task Inbetween(string warning, int year)
        //{
            //await this.ProcessResults(warning);
            //Cehck Results
        //}

        public async Task ProcessResults(string warning)
        {
            if (warning != null)
            {
                MessageBox.Show(warning);
            }
            else
            {
                await this.GetResults(); //pass in year

            }
        } 

        public void HideColumn()
        {
            if (this.CanSearchConstructors)
            {
                //return Visibility.Hidden;
                //resultsDataGrid1.Columns[1].Visibility = Visibility.Hidden;
            }
        }

        public async Task GetResults()//int year)
        {
            string url;

            if (this.CanSearchConstructors)
            {
                url = $"https://ergast.com/api/f1/{Year}/constructorStandings.json";

                await this.Results(url);
            }
            else
            {
                url = $"http://ergast.com/api/f1/{Year}/driverStandings.json";
                await this.Results(url);
            }
        }

        private async Task Results(string url)
        {
            this.GpStandings = new ObservableCollection<Models.GPStandingsCollection>();

            if (CanSearchConstructors)
            {
                this.constructor = await this.GetModelData(this.constructor, url);

                this.Heading = $"{constructor.MRData.StandingsTable.StandingsLists[0].season} Formula 1 Season - {constructor.MRData.StandingsTable.StandingsLists[0].round} Rounds";

                foreach (var constructor in this.constructor.MRData.StandingsTable.StandingsLists[0].ConstructorStandings)
                {
                    this.GpStandings.Add(new Models.GPStandingsCollection()
                    {
                        Position = int.Parse(constructor.position),
                        Constructor = constructor.Constructor.name,
                        Points = float.Parse(constructor.points),
                        Wins = int.Parse(constructor.wins),
                        Nationality = constructor.Constructor.nationality
                    });
                }
            }

            else
            {
                this.driver = await this.GetModelData(this.driver, url);

                this.Heading = $"{driver.MRData.StandingsTable.StandingsLists[0].season} Formula 1 Season - {driver.MRData.StandingsTable.StandingsLists[0].round} Rounds";

                foreach (var driver in this.driver.MRData.StandingsTable.StandingsLists[0].DriverStandings)
                {
                    this.GpStandings.Add(new Models.GPStandingsCollection()
                    {
                        Position = int.Parse(driver.position),
                        Driver = $"{driver.Driver.givenName[0]}. {driver.Driver.familyName} ({(driver.Driver.permanentNumber != null ? driver.Driver.permanentNumber : "--")})",
                        Points = float.Parse(driver.points),
                        Wins = int.Parse(driver.wins),
                        Constructor = driver.Constructors[0].name,
                        Nationality = driver.Driver.nationality
                    });
                }
            }
        }

        public async Task<ObservableCollection<Models.GPStandingsCollection>> GetStandings(int year)
        {
            string url;

            if (this.CanSearchConstructors)
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
            this.GpStandings = new ObservableCollection<Models.GPStandingsCollection>();

            if (CanSearchConstructors)
            {
                this.constructor = await this.GetModelData(this.constructor, url);

                foreach (var constructor in this.constructor.MRData.StandingsTable.StandingsLists[0].ConstructorStandings)
                {
                    this.GpStandings.Add(new Models.GPStandingsCollection()
                    {
                        Position = int.Parse(constructor.position),
                        Constructor = constructor.Constructor.name,
                        Points = float.Parse(constructor.points),
                        Wins = int.Parse(constructor.wins),
                        Nationality = constructor.Constructor.nationality
                    });
                }

                return this.GpStandings;
            }

            else
            {
                this.driver = await this.GetModelData(this.driver, url);

                foreach (var driver in this.driver.MRData.StandingsTable.StandingsLists[0].DriverStandings)
                {
                    this.GpStandings.Add(new Models.GPStandingsCollection()
                    {
                        Position = int.Parse(driver.position),
                        Driver = $"{driver.Driver.givenName[0]}. {driver.Driver.familyName} ({(driver.Driver.permanentNumber != null ? driver.Driver.permanentNumber : "--")})",
                        Points = float.Parse(driver.points),
                        Wins = int.Parse(driver.wins),
                        Constructor = driver.Constructors[0].name,
                        Nationality = driver.Driver.nationality
                    });
                }

                return this.GpStandings;
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
    }
}
