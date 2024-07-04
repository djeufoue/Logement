using Logement.Data.Enum;

namespace Logement.ViewModels
{
    public class TenancyMemberModel
    {
        public int TenancyId { get; set; }

        public long UserId { get; set; }

        public TenancyMemberRoleEnum Role { get; set; }
        public bool EmailSent { get; set; }
    }
}
