using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Logement.Services
{
    public class SMSservice : ControllerBase
    {
        private readonly string accountId = "AC12bbe02cf30a32cd67aa63abe49f3b72";
        private readonly string authToken = "1d37e369b329da4f06349e9fc35ab8b7";

        public class SmSSettings
        {
            public string AccountId { get; set; }
            public string AuthToken { get; set; }
        }

        private readonly SmSSettings _settings;

        public SMSservice(IConfiguration configuration, ILogger<SmSSettings> settings)
        {
            _settings = configuration.GetSection(nameof(SmSSettings)).Get<SmSSettings>();
        }

        [HttpPost]
        public async Task<ActionResult> SendSMS(string phoneNumber,string messageToBeSend)
        {
            //To do: Need to await this line of code
            TwilioClient.Init(_settings.AccountId, _settings.AuthToken);

            var message = MessageResource.Create(
                    body: messageToBeSend,
                    from: new Twilio.Types.PhoneNumber("+15075435245"),
                    to: phoneNumber
                );
            return StatusCode(200, new { message = message.Sid});
        }
    }
}
