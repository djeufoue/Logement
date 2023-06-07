namespace Logement.Models
{
    public class City
    {
        public long Id { get; set; }

        public long LandLordId { get; set; }
        public virtual ApplicationUser LandLord { get; set; }
        public string Name { get; set; }
        public long NumbersOfApartment { get; set; }

        public string Town { get; set; }
        public string LocatedAt { get; set; }

        //public int? NumberOfParkingSpaces { get; set; }

        public string Floor { get; set; }

        public DateTime DateAdded { get; set; }
    }
}
