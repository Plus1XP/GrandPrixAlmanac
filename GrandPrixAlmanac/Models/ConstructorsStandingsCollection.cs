namespace GrandPrixAlmanac.Models
{
    public class ConstructorsStandingsCollection : IStandingsCollection
    {
        public int Position { get; set; }
        public string Constructor { get; set; }
        public float Points { get; set; }
        public int Wins { get; set; }
        public string Nationality { get; set; }
    }
}
