using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPStandingsGUI.Models
{
    class DriversStandingsCollection : IStandingsCollection
    {
        public int Position { get; set; }
        public string Driver { get; set; }
        public string Constructor { get; set; }
        public float Points { get; set; }
        public int Wins { get; set; }
        public string Nationality { get; set; }
    }
}
