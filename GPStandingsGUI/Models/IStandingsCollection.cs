using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPStandingsGUI.Models
{
    interface IStandingsCollection
    {
        int Position { get; set; }
        string Constructor { get; set; }
        float Points { get; set; }
        int Wins { get; set; }
        string Nationality { get; set; }
    }
}
