using ClosestNeighbourSearchAlgorithms.Models;

namespace ClosestNeighbourSearchAlgorithms.Contracts
{
    public interface ICoordinate
    {
        long CoordinateId { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
        bool Used { get; set; }
        string ToString();
        bool Equals(object obj);
        int GetHashCode();
        double Distance(Coordinate otherCoordinate);
        int CompareTo(Coordinate other);
    }
}