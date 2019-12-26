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
using System.Collections.Specialized;

namespace GPStandingsGUI.ViewModels
{
    class GPStandingsViewModel : INotifyPropertyChanged
    {
        private readonly Controllers.GPLogicController gpLogicController;

        private string heading;

        //private string heading2;

        private ObservableCollection<object> standingsTable;

        //private ObservableCollection<object> standingsTable2;

        //public bool CanPopulateResultsTable1 { get; set; }

        public bool CanSearchConstructors { get; set; }

        //public bool CanSearchConstructors2 { get; set; }

        public string UserSelectedYear { get; set; }

        //public string UserSelectedYear2 { get; set; }

        public string Heading { get { return this.heading; } set { this.heading = value; this.OnPropertyChanged("Heading"); } }

        //public string Heading2 { get { return this.heading2; } set { this.heading2 = value; this.OnPropertyChanged("Heading2"); } }

        public ObservableCollection<object> StandingsTable
        {
            get { return this.standingsTable; }
            set { this.standingsTable = value; this.OnPropertyChanged("StandingsTable"); }
        }

        //public ObservableCollection<object> StandingsTable2
        //{
        //    get { return this.standingsTable2; }
        //    set { this.standingsTable2 = value; this.OnPropertyChanged("StandingsTable2"); }
        //}

        public Models.AsyncRelayCommand Cmd { get; private set; }

        //public Models.AsyncRelayCommand Cmd2 { get; private set; }

        public string WindowTitle
        {
            get { return $"GP Standings v{System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion.ToString()}"; }
        }

        public string WindowColour { get { return "#011627"; } }

        public string FontColour { get { return "#80a5ec"; } }

        public string SearchTitle { get { return "Search Year"; } }

        public string SubmitButtonContent { get { return "Go!"; } }

        public string ConstructorsContent { get { return "Constructors"; } }

        public string DriversContent { get { return "Drivers"; } }

        public GPStandingsViewModel()
        {
            this.gpLogicController = new Controllers.GPLogicController();

            this.CanSearchConstructors = false;
            //this.CanSearchConstructors2 = false;

            this.Cmd = new Models.AsyncRelayCommand(() => this.PopulateResults(this.UserSelectedYear, this.CanSearchConstructors));
            //this.Cmd2 = new Models.AsyncRelayCommand(() => this.PopulateResults(this.UserSelectedYear2, this.CanSearchConstructors2, this.CanPopulateResultsTable1 = false));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private async Task PopulateResults(string userSelectedYear, bool canSearchConstructors)
        {
            Tuple<string, int> validation = gpLogicController.CheckDateEntryIsValid(userSelectedYear, canSearchConstructors);
            string warning = validation.Item1;
            int year = validation.Item2;

            if (warning != null)
            {
                this.ShowError(warning);
            }
            else
            {
                Tuple<string, ObservableCollection<object>> standingsCollection = await gpLogicController.GetResults(year, canSearchConstructors);
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

        private void HideColumn(bool CanSearchConstructors)
        {
            if (CanSearchConstructors)
            {
                //return Visibility.Hidden;
                //resultsDataGrid1.Columns[1].Visibility = Visibility.Hidden;
            }
        }

        private void ShowtextName(object param)
        {
            string name = param as string;
            MessageBox.Show(name);
        }
    }
}