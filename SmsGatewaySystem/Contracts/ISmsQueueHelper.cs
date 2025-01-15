using ModelsLibrary;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SmsGatewaySystem.Contracts
{
    public interface ISmsQueueHelper
    {
        public Task<SmsTranResponse> SendSmsToQueue(SmsTranData model);

    }
}
