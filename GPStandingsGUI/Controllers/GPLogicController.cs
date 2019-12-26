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
    class GPLogicController
    {
        private APIController apiController;

        private string warning;

        public string Warning
        {
            get { return this.warning; }
            set
            {
                if (this.warning == null)
                {
                    this.warning = value;
                }
            }
        }

        public GPLogicController()
        {

        }

        public Tuple<string, int> CheckDateEntryIsValid(string SelectedYear, bool CanSearchConstructors)
        {
            this.warning = null;

            int year = 0;

            if (SelectedYear != null)
            {
                this.Warning = this.DateCharacterCheck(SelectedYear, this.Warning);
                year = this.ParseYear(SelectedYear);
                this.Warning = this.DateRangeCheck(CanSearchConstructors, year, this.Warning);
            }
            else
            {
                this.Warning = "Field is blank, please enter a numerical year";
            }

            return new Tuple<string, int>(this.Warning, year);
        }      

        private int ParseYear(string SelectedYear)
        {
            int.TryParse(SelectedYear, out int year);
            return year;
        }

        private string DateCharacterCheck(string selectedYear, string warning)
        {
            if (!selectedYear.All(char.IsNumber))
            {
                warning = "Not a number, please enter numerical characters\n";
            }
            if (selectedYear.Equals(string.Empty))
            {
                warning = "Please enter a numerical year";
            }
            return warning;
        }

        private string DateRangeCheck(bool canSearchConstructors, int year, string warning)
        {
            int currentYear = DateTime.Now.Year;

            if ((canSearchConstructors) && (year < 1958 || year > currentYear))
            {
                warning = $"Out of range, Search constructors from 1958 to {currentYear}";
            }
            if ((!canSearchConstructors) && (year < 1950 || year > currentYear))
            {
                warning = $"Out of range, Search drivers from 1950 to {currentYear}";
            }

            return warning;
        }

        public async Task<Tuple<string, ObservableCollection<object>>> GetResults(int year, bool canSearchConstructors)
        {
            if (canSearchConstructors)
            {
                string table = "constructorStandings";
                this.apiController = new APIController(year, table);
                return await this.apiController.ConstructorsStandings();

            }
            else
            {
                string table = "driverStandings";
                this.apiController = new APIController(year, table);
                return await this.apiController.DriversStandings();
            }
        }
    }
}
