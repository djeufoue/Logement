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
    }
}
