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

        private string heading1;

        private string heading2;

        private ObservableCollection<Models.GPStandingsCollection> standingsTable1;

        private ObservableCollection<Models.GPStandingsCollection> standingsTable2;

        public bool CanPopulateResultsTable1 { get; set; }

        public bool CanSearchConstructors { get; set; }

        public string UserSelectedYear { get; set; }

        public string Heading1 { get { return this.heading1; } set { this.heading1 = value; this.OnPropertyChanged("Heading1"); } }

        public string Heading2 { get { return this.heading2; } set { this.heading2 = value; this.OnPropertyChanged("Heading2"); } }

        public ObservableCollection<Models.GPStandingsCollection> StandingsTable1
        {
            get { return this.standingsTable1; }
            set { this.standingsTable1 = value; this.OnPropertyChanged("StandingsTable1"); }
        }

        public ObservableCollection<Models.GPStandingsCollection> StandingsTable2
        {
            get { return this.standingsTable2; }
            set { this.standingsTable2 = value; this.OnPropertyChanged("StandingsTable2"); }
        }       

        public Models.AsyncRelayCommand Cmd1 { get; private set; }

        public Models.AsyncRelayCommand Cmd2 { get; private set; }

        public string WindowTitle { get { return "GP Standings v2.1.1"; } }

        public string SubmitButtonContent { get { return "Go!"; } }

        public string ConstructorsContent { get { return "Constructors"; } }

        public string DriversContent { get { return "Drivers"; } }

        public GPStandingsViewModel()
        {
            this.gpLogicController = new Controllers.GPLogicController();

            this.CanSearchConstructors = false;

            this.Cmd1 = new Models.AsyncRelayCommand(() => this.PopulateResults(this.CanPopulateResultsTable1 = true));
            this.Cmd2 = new Models.AsyncRelayCommand(() => this.PopulateResults(this.CanPopulateResultsTable1 = false));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private async Task PopulateResults(bool canPopulateResultsTable1)
        {
            Tuple<string, int> validation = gpLogicController.CheckDateEntryIsValid(this.UserSelectedYear, this.CanSearchConstructors);
            string warning = validation.Item1;
            int year = validation.Item2;

            if (warning != null)
            {
                this.ShowError(warning);
            }
            else
            {
                Tuple<string, ObservableCollection<Models.GPStandingsCollection>> tableResults = await gpLogicController.GetResults(year, this.CanSearchConstructors);
                string header = tableResults.Item1;
                ObservableCollection<Models.GPStandingsCollection> table = tableResults.Item2;
                this.UpdateCollection(canPopulateResultsTable1, header, table);
            }
        }

        private void UpdateCollection(bool canPopulateResultsTable1, string header, ObservableCollection<Models.GPStandingsCollection> table)
        {
            if (canPopulateResultsTable1)
            {
                this.Heading1 = header;
                this.StandingsTable1 = table;
            }
            else
            {
                this.Heading2 = header;
                this.StandingsTable2 = table;
            }
        }

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