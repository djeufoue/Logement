namespace Logement.Models
{
    public class Fichier
    {
        public long Id { get; set; }
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
     
        public string FileName { get; set; }
        public string CityOrApartement { get; set; }

        public long? ApartmentId { get; set; }
        public virtual Apartment? Apartment { get; set; }

        public long? CityId { get; set; }
        public virtual City? City { get; set; }

        public DateTime UploadDate { get; set; }

    }
}
