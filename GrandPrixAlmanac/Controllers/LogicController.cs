using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GrandPrixAlmanac.Controllers
{
    internal class LogicController
    {
        private APIController apiController;

        private string warning;

        public string Warning
        {
            get => this.warning;
            set
            {
                if (this.warning == null)
                    this.warning = value;
            }
        }

        public Tuple<string, int> CheckDateEntryIsValid(string selectedYear, bool CanSearchConstructors)
        {
            this.warning = null;

            var year = 0;

            if (selectedYear != null)
            {
                this.Warning = this.DateCharacterCheck(selectedYear, this.Warning);
                year = this.ParseYear(selectedYear);
                this.Warning = this.DateRangeCheck(CanSearchConstructors, year, this.Warning);
            }
            else
            {
                this.Warning = "Field is blank, please enter a numerical year";
            }

            return new Tuple<string, int>(this.Warning, year);
        }

        private int ParseYear(string selectedYear)
        {
            int.TryParse(selectedYear, out int year);
            return year;
        }

        private string DateCharacterCheck(string selectedYear, string warning)
        {
            if (!selectedYear.All(char.IsNumber))
                warning = "Not a number, please enter numerical characters\n";
            if (selectedYear.Equals(string.Empty))
                warning = "Please enter a numerical year";
            return warning;
        }

        private string DateRangeCheck(bool canSearchConstructors, int year, string warning)
        {
            var currentYear = DateTime.Now.Year;

            if (canSearchConstructors && (year < 1958 || year > currentYear))
                warning = $"Out of range, Search constructors from 1958 to {currentYear}";
            if (!canSearchConstructors && (year < 1950 || year > currentYear))
                warning = $"Out of range, Search drivers from 1950 to {currentYear}";

            return warning;
        }

        public async Task<Tuple<string, ObservableCollection<object>>> GetResults(int year, bool canSearchConstructors)
        {
            if (canSearchConstructors)
            {
                var table = "constructorStandings";
                this.apiController = new APIController(year, table);
                return await this.apiController.ConstructorsStandings();
            }
            else
            {
                var table = "driverStandings";
                this.apiController = new APIController(year, table);
                return await this.apiController.DriversStandings();
            }
        }
    }
}