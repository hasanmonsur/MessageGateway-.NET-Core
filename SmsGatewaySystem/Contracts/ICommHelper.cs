using SmsGatewaySystem.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SmsGatewaySystem.Contracts
{
    public interface ICommHelper
    {

        public Task<CommModels> FuncRetunCommData();



    }
}
