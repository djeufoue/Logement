namespace Logement.Models
{
    public class CityPhoto
    {
        public long Id { get; set; }

        public long CityId { get; set; }
        public virtual City City { get; set; }

        public long Size { get; set; }
        public string ImageURL { get; set; }
    }
}
