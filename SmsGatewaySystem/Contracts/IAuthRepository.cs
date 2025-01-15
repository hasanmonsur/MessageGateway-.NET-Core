using ModelsLibrary;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SmsGatewaySystem.Contracts
{
    public interface IAuthRepository
    {
        public Task<bool> FuncSaveAuthInfo(Authentication authInfo);
        //public Task<List<ServiceRoles>> FuncGetRoles(string userid);
        public Task<ApiAuthResponse> FuncGetAuthInfo(string userid, string userpass);
        //public Task<List<ServiceRoles>> FuncGetServiceRoles(string? userid);


    }
}
