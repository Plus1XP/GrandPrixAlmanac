namespace GPStandingsGUI.Models
{
    public class ConstructorModel
    {
        public Mrdata MRData { get; set; }

        public class Mrdata
        {
            public string xmlns { get; set; }
            public string series { get; set; }
            public string url { get; set; }
            public string limit { get; set; }
            public string offset { get; set; }
            public string total { get; set; }
            public Standingstable StandingsTable { get; set; }
        }

        public class Standingstable
        {
            public string season { get; set; }
            public Standingslist[] StandingsLists { get; set; }
        }

        public class Standingslist
        {
            public string season { get; set; }
            public string round { get; set; }
            public Constructorstanding[] ConstructorStandings { get; set; }
        }

        public class Constructorstanding
        {
            public string position { get; set; }
            public string positionText { get; set; }
            public string points { get; set; }
            public string wins { get; set; }
            public Constructor Constructor { get; set; }
        }

        public class Constructor
        {
            public string constructorId { get; set; }
            public string url { get; set; }
            public string name { get; set; }
            public string nationality { get; set; }
        }
    }
}
