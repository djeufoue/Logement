namespace DjeResidenceAPI.Models.Entities
{
    public class Property
    {
        public int Id { get; set; }

        public long LandLordId { get; set; }
        public virtual ApplicationUser LandLord { get; set; }
        public string Name { get; set; } = string.Empty;
        public long NumbersOfApartment { get; set; }

        public string Town { get; set; }
        public string LocatedAt { get; set; }

        public string Floor { get; set; }

        public DateTime DateAdded { get; set; }
    }
}
