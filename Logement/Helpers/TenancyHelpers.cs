using Logement.Data.Enum;
using Logement.Models;
using Logement.ViewModels;

namespace Logement.Helpers
{
    public class TenancyHelpers
    {
        public static (string subject, string body) GenerateEmailContent<T>(T model, string currentUserFullName, string recipientName)
        {
            string subject = $"You have been added as a tenant by {currentUserFullName}.";

            string housingType = "";
            string locatedAt = "";
            string area = "";
            string numberOfRooms = "";
            string numberOfBathrooms = "";
            string propertyName = "";

            if (model is Tenancy tenancy)
            {
                housingType = tenancy.Apartment!.Type.ToString();
                locatedAt = tenancy.Apartment.City.LocatedAt;
                area = $"{tenancy.Apartment.RoomArea}";
                numberOfRooms = $"{tenancy.Apartment.NumberOfRooms}";
                numberOfBathrooms = $"{tenancy.Apartment.NumberOfbathRooms}";
                propertyName = tenancy.Apartment.City.Name;
            }
            else if (model is CityMemberViewModel vm)
            {
                housingType = BasicHelpers.GetApartmentTypeName(vm.AppartmentMember.Type);
                locatedAt = vm.AppartmentMember.LocatedAt!;
                area = $"{vm.AppartmentMember.RoomArea}";
                numberOfRooms = $"{vm.AppartmentMember.NumberOfRooms}";
                numberOfBathrooms = $"{vm.AppartmentMember.NumberOfbathRooms}";
                propertyName = vm.AppartmentMember.CityName!;
            }
            else
            {
                throw new ArgumentException("Unsupported model type.");
            }

            string body = $"<p style='margin-bottom: 20px;'>Dear {recipientName},</p>";

            body += $"<div style='margin-bottom: 20px;'>";
            body += $"<p>Housing type: {housingType}</p>";
            body += $"<p>Located at: {locatedAt}</p>";
            body += $"<p>City name: {propertyName}</p>";
            body += $"<p>Area: {area} m²</p>";
            body += $"<p>Number of bedrooms: {numberOfRooms}</p>";
            body += $"<p>Number of bathrooms: {numberOfBathrooms}</p>";
            body += $"<p>Thanks for trusting us.</p>";
            body += "</div>";

            body += "<div>";
            body += "<p style='margin-top: 20px;'>Best regards,</p>";
            body += "<p>your landlord</p>";
            body += "</div>";

            return (subject, body);
        }

        public static string GenerateSmsContent<T>(T model, string currentUserFullName)
        {
            string housingType = "";
            string locatedAt = "";
            string area = "";
            string numberOfRooms = "";
            string numberOfBathrooms = "";
            string propertyName = "";

            if (model is Tenancy tenancy)
            {
                housingType = tenancy.Apartment!.Type.ToString();
                locatedAt = tenancy.Apartment.City.LocatedAt;
                area = $"{tenancy.Apartment.RoomArea}";
                numberOfRooms = $"{tenancy.Apartment.NumberOfRooms}";
                numberOfBathrooms = $"{tenancy.Apartment.NumberOfbathRooms}";
                propertyName = tenancy.Apartment.City.Name;
            }
            else if (model is CityMemberViewModel vm)
            {
                housingType = BasicHelpers.GetApartmentTypeName(vm.AppartmentMember.Type);
                locatedAt = vm.AppartmentMember.LocatedAt!;
                area = $"{vm.AppartmentMember.RoomArea}";
                numberOfRooms = $"{vm.AppartmentMember.NumberOfRooms}";
                numberOfBathrooms = $"{vm.AppartmentMember.NumberOfbathRooms}";
                propertyName = vm.AppartmentMember.CityName!;
            }
            else
            {
                throw new ArgumentException("Unsupported model type.");
            }

            string body = $"You have been added as a tenant by Mr {currentUserFullName}.\n\n";
            body += $"Housing type: {housingType}\n";
            body += $"Located at: {locatedAt}\n";
            body += $"City name: {propertyName}\n";
            body += $"Area: {area} m²\n";
            body += $"Number of bedrooms: {numberOfRooms}\n";
            body += $"Number of bathrooms: {numberOfBathrooms}\n\n";
            body += "Thanks for trusting us\n";
            body += "Best regards,\n";
            body += "your landlord";

            return body;
        }
    }
}
