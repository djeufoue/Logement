using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Logement.Services
{
    public class SMSservice : ControllerBase
    {
        public class SmSSettings
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }

        private readonly SmSSettings _settings;

        public SMSservice(IConfiguration configuration, ILogger<SmSSettings> settings)
        {
            _settings = configuration.GetSection(nameof(SmSSettings)).Get<SmSSettings>();
        }

        [HttpPost]
        public async Task SendNewSMSAsync(string recipientPhoneNumber,string message)
        {
            string authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));

            string accessToken = await GetAccessToken(authorizationHeader);
            if (!string.IsNullOrEmpty(accessToken))
            {
                await SendSMS(accessToken, recipientPhoneNumber, "23758772239", message);
            }
        }

        static async Task<string?> GetAccessToken(string authorizationHeader)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + authorizationHeader);
                var content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                HttpResponseMessage response = await client.PostAsync("https://api.orange.com/oauth/v3/token", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);
                        string accessToken = jsonResponse.access_token;
                        return accessToken;
                    }
                    catch (Newtonsoft.Json.JsonException ex)
                    {
                        Console.WriteLine("Failed to parse JSON response: " + ex.Message);
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine("Failed to get access token. Status code: " + response.StatusCode);
                }
                return null;
            }
        }

        static async Task SendSMS(string accessToken, string recipientPhoneNumber, string devPhoneNumber, string message)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                var payload = new
                {
                    outboundSMSMessageRequest = new
                    {
                        address = "tel:+" + recipientPhoneNumber,
                        senderAddress = "tel:+" + devPhoneNumber,
                        outboundSMSTextMessage = new
                        {
                            message = message
                        }
                    }
                };

                string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("https://api.orange.com/smsmessaging/v1/outbound/tel%3A%2B" + devPhoneNumber + "/requests", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("SMS sent successfully.");
                    Console.WriteLine(responseContent);
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Failed to send SMS. Status code: " + response.StatusCode);
                    Console.WriteLine(responseContent);
                }
            }
        }
    }
}
