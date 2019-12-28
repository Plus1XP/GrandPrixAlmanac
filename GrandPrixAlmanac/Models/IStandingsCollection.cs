namespace GrandPrixAlmanac.Models
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
