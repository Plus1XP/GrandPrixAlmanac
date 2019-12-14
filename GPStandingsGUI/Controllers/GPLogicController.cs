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

        public GPLogicController()
        {
            //apiController = new APIController();
        }

        public Tuple<string, int> CheckDateEntryIsValid(string SelectedYear, bool CanSearchConstructors)
        {
            string warning = null;

            int year = 0;

            if (SelectedYear != null)
            {
                warning = this.DateCharacterCehck(SelectedYear, warning);
                year = this.ParseYear(SelectedYear);
                warning = this.DateRangeCheck(CanSearchConstructors, year, warning);
            }
            else
            {
                warning = "Field is blank, please enter a numerical year";
            }

            return new Tuple<string, int>(warning, year);
        }

        private int ParseYear(string SelectedYear)
        {
            int.TryParse(SelectedYear, out int year);
            return year;
        }

        private string DateCharacterCehck(string selectedYear, string warning)
        {
            if (!selectedYear.All(char.IsNumber))
            {
                warning = "Not a number, please enter numerical characters";
            }
            if (selectedYear.Equals(string.Empty))
            {
                MessageBox.Show("Please enter a numerical year");
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

        public async Task<Tuple<string, ObservableCollection<Models.GPStandingsCollection>>> GetResults(int year, bool canSearchConstructors)
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
