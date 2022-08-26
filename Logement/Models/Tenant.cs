using Logement.Data.Enum;

namespace Logement.Models
{
    /*
     * L'objectif principal de cette table est de recuperer les informations(personnel) sur le locataire
     * apres que celui-ci ait login
     */
    public class Tenant
    {
        public int Id { get; set; }

        public string TenantFirstName { get; set; }

        public string TenantLastName { get; set; }

        // Need to be seed
        public MaritalStatusEnum? MaritalStatus { get; set; }

        public string JobTitle { get; set; }
    }
}
