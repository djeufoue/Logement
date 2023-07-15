namespace Logement.Models
{
    public class FileModel
    {
        public long Id { get; set; }

        public long? TenantId { get; set; }
        public virtual ApplicationUser Tenant { get; set; }

        public long ApartmentId { get; set; }
        public virtual Apartment Apartment { get; set; }

        public long CityId { get; set; }
        public virtual City City { get; set; }
        public string Name { get; set; }
        public long Size { get; set; } 
        public string FileURL { get; set; }
    }
}
