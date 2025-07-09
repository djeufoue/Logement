using RestSharp;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DjeResidenceAPI.Services
{
    public class OrangeMoneyService
    {
        private readonly string _baseUrl = "https://api.orange.com";
        private readonly string _clientId;
        private readonly string _clientSecret;

        public OrangeMoneyService(IConfiguration configuration)
        {
            // Load credentials from appsettings.json
            var orangeSettings = configuration.GetSection("OrangeSettings").Get<OrangeSettings>();
            if (orangeSettings == null)
                throw new Exception("OrangeSettings configuration section is missing.");

            _clientId = orangeSettings.ClientId;
            _clientSecret = orangeSettings.ClientSecret;
        }

        /// <summary>
        /// Retrieves an access token from Orange Money API.
        /// </summary>
        /// <returns>The access token as a string.</returns>
        public async Task<string> GetAccessToken()
        {
            var client = new RestClient($"{_baseUrl}/oauth/v3/token");

            var request = new RestRequest
            {
                Method = Method.Post // Explicitly setting the HTTP method
            };

            // Add headers and parameters
            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            request.AddHeader("Authorization", $"Basic {credentials}");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");

            var response = await client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                var tokenResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
                return tokenResponse.access_token;
            }
            throw new Exception($"Failed to get access token. Response: {response.Content}");
        }

        /// <summary>
        /// Transfers money using the Orange Money API.
        /// </summary>
        public async Task<string> TransferMoney(string payerPhone, string payeePhone, decimal amount, string transactionId)
        {
            string token = await GetAccessToken();
            var client = new RestClient($"{_baseUrl}/orange-money-webpay/dev/v1/transfers");
            var request = new RestRequest
            {
                Method = Method.Post // Explicitly setting the HTTP method
            };

            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Content-Type", "application/json");

            var payload = new
            {
                payer = new { phoneNumber = payerPhone },
                payee = new { phoneNumber = payeePhone },
                amount = amount.ToString("F2"),
                currency = "XAF",
                description = "Payment for services",
                transactionId = transactionId
            };

            request.AddJsonBody(payload);

            var response = await client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            throw new Exception($"Transfer failed. Response: {response.Content}");
        }

        /// <summary>
        /// Gets the status of a transaction.
        /// </summary>
        public async Task<string> GetTransactionStatus(string transactionId)
        {
            string token = await GetAccessToken();
            var client = new RestClient($"{_baseUrl}/orange-money-webpay/dev/v1/transactions/{transactionId}");

            var request = new RestRequest
            {
                Method = Method.Get // Correct method for fetching status
            };

            request.AddHeader("Authorization", $"Bearer {token}");

            var response = await client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            throw new Exception($"Failed to get transaction status. Response: {response.Content}");
        }

        /// <summary>
        /// OrangeSettings model for appsettings.json
        /// </summary>
        private class OrangeSettings
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }
    }
}
