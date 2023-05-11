namespace Logement.Models
{
    public class CityPhoto
    {
        public long Id { get; set; }

        public long CityId { get; set; }
        public virtual City City { get; set; }

        public string CityOrApartement { get; set; }
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
