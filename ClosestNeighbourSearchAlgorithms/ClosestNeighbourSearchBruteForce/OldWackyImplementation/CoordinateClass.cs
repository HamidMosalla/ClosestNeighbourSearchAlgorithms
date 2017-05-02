namespace ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce.OldWackyImplementation
{
    public class CoordinateClass
    {
        public int CoordinateId { get; set; }
        public decimal? PotansielAvalie { get; set; }
        public int TartibInRoad { get; set; }
        public int? IdPlaceTree { get; set; }
        public string CoordinateName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override string ToString() => $"Id: {CoordinateId}; X: {Latitude}; Y: {Longitude}";
    }
}