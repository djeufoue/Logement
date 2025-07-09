using Logement.Data;
using Logement.Data.Enum;
using Logement.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Logement.Helpers
{
    public static class BasicHelpers
    {
        public static async Task<bool> GetCurrentApartmentOwner(ApplicationDbContext dbc, ILogger logger, long currentUserId, long apartmentId)
        {
            var apartment = await dbc.Apartments
                .Where(ap => ap.Id == apartmentId)
                .FirstOrDefaultAsync();

            if (apartment == null)
            {
                throw new ApplicationException($"The apartment with the ID: {apartmentId} does not exist or was deleted.");
            }

            var currentUser = await dbc.Users.FindAsync(currentUserId);
            if (currentUser == null)
            {
                logger.LogError("The current logged-in user does not exist or was deleted.");
                return false;
            }

            return apartment.LessorId == currentUser.Id;
        }

        public static string GetUserFullName(ApplicationUser? user)
        {
            if (user == null)
                return "";
            else if (!string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName))
                return $"{user.FirstName} {user.LastName}";
            else if (!string.IsNullOrEmpty(user.FirstName))
                return user.FirstName;
            else if (!string.IsNullOrEmpty(user.LastName))
                return user.LastName;
            else if (!string.IsNullOrEmpty(user.Email))
                return user.Email;
            else return "";
        }

        public static TenancyMemberRoleEnum GetTenancyMemberRole(string role)
        {
            return role.ToLower().Trim() switch
            {
                "Main Tenant" => TenancyMemberRoleEnum.LocatairePrincipal,
                "Co-tenant" => TenancyMemberRoleEnum.CoLocataire,
                "Child" => TenancyMemberRoleEnum.Enfant,
                _ => TenancyMemberRoleEnum.Unknown
            };
        }

        public static string GetApartmentTypeName(ApartmentTypeEnum type)
        {
            return type switch
            {
                ApartmentTypeEnum.Studio => "Studio",
                ApartmentTypeEnum.Room => "Room",
                ApartmentTypeEnum.Apartment => "Apartment",
                _ => "Unknown"
            };
        }
    }
}
