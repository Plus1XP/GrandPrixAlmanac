namespace GrandPrixAlmanac.Models
{
    public class DriversStandingsCollection : IStandingsCollection
    {
        public int Position { get; set; }
        // Added only in Drivers Standings
        public string Driver { get; set; }
        public string Constructor { get; set; }
        public float Points { get; set; }
        public int Wins { get; set; }
        public string Nationality { get; set; }
    }
}
