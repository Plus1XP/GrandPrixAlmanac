using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using GrandPrixAlmanac.Controllers;
using GrandPrixAlmanac.Models;

namespace GrandPrixAlmanac.ViewModels
{
    internal class StandingsViewModel : INotifyPropertyChanged
    {
        private readonly LogicController logicController;

        private string heading;

        private ObservableCollection<object> standingsTable;

        public StandingsViewModel()
        {
            this.logicController = new LogicController();

            this.CanSearchConstructors = false;

            this.Cmd = new AsyncRelayCommand(() => this.PopulateResults(this.UserSelectedYear, this.CanSearchConstructors));
        }

        public string AppVersion => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

        public string AppTitle => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName;

        public string WindowTitle => $"{this.AppTitle} v{this.AppVersion}";

        public string WindowColour => "#011627";

        public string FontColour => "#80a5ec";

        public string SearchTitle => "Search Year";

        public string SubmitButtonContent => "Go!";

        public string ConstructorsContent => "Constructors";

        public string DriversContent => "Drivers";

        //public bool CanPopulateResultsTable1 { get; set; }

        public bool CanSearchConstructors { get; set; }

        public string UserSelectedYear { get; set; }

        public string Heading
        {
            get => this.heading;
            set
            {
                this.heading = value;
                this.OnPropertyChanged("Heading");
            }
        }

        public ObservableCollection<object> StandingsTable
        {
            get => this.standingsTable;
            set
            {
                this.standingsTable = value;
                this.OnPropertyChanged("StandingsTable");
            }
        }

        public AsyncRelayCommand Cmd { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private async Task PopulateResults(string userSelectedYear, bool canSearchConstructors)
        {
            var validation = this.logicController.CheckDateEntryIsValid(userSelectedYear, canSearchConstructors);
            var warning = validation.Item1;
            var year = validation.Item2;

            if (warning != null)
            {
                this.ShowError(warning);
            }
            else
            {
                var standingsCollection = await this.logicController.GetResults(year, canSearchConstructors);
                this.Heading = standingsCollection.Item1;
                this.StandingsTable = standingsCollection.Item2;
                //this.UpdateCollection(header, standingsTable);
            }
        }

        //private void UpdateCollection(bool canPopulateResultsTable1, string header, ObservableCollection<object> standingsTable)
        //{
        //    if (canPopulateResultsTable1)
        //    {
        //        this.Heading1 = header;
        //        this.StandingsTable1 = standingsTable;
        //    }
        //    else
        //    {
        //        this.Heading2 = header;
        //        this.StandingsTable2 = standingsTable;
        //    }
        //}

        private void ShowError(string message)
        {
            MessageBox.Show(message);
        }
    }
}