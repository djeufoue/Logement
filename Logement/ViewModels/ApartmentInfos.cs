using Twilio.Types;

namespace Logement.ViewModels
{
    public class ApartmentInfos
    {
        public long Id { get; set; }
        public decimal Price { get; set; }
        public string LocatedAt { get; set; }
        public byte[]? Data { get; set; }
        public string? ContentType { get; set; }
    }
}
