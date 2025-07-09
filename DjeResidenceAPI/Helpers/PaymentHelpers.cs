using Newtonsoft.Json;

namespace DjeResidenceAPI.Helpers
{
    public static class PaymentHelpers
    {
        public static string ParseTransactionStatus(string statusResponse)
        {
            try
            {
                var status = JsonConvert.DeserializeObject<dynamic>(statusResponse);
                return status.status?.ToString() ?? "UNKNOWN";
            }
            catch
            {
                return "UNKNOWN";
            }
        }
    }
}
