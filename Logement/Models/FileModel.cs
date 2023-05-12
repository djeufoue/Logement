namespace Logement.Models
{
    public class FileModel
    {
        public long Id { get; set; }

        public long? TenantId { get; set; }
        public virtual ApplicationUser Tenant { get; set; }

        public string Name { get; set; }
        public long Size { get; set; } 
        public string FileURL { get; set; }
    }
}
