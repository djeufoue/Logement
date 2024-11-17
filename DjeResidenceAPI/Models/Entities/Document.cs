using DjeResidenceAPI.Data.Enum;
using System;

namespace DjeResidenceAPI.Models.Entities
{
    public class Document
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";

        public DocumentTypeEnum Type { get; set; }

        public int? PropertyId { get; set; }
        public virtual Property Property { get; set; }

        public int? ApartmentId { get; set; }
        public virtual Apartment Apartment { get; set; }

        public int? TenancyId { get; set; }
        public virtual Tenancy Tenancy { get; set; }

        public DateTimeOffset DateCreated { get; set; }
        
        public bool IsArchived { get; set; }
    }
}
