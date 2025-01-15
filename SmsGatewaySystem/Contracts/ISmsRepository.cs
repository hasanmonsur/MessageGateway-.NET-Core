using ModelsLibrary;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SmsGatewaySystem.Contracts
{
    public interface ISmsRepository
    {
        public Task<bool> SaveSmsInfo(SmsTranData model);
        public Task<bool> UpdateSmsInfo(SmsTranData model, string statusmsg, string status);
        public Task<bool> CheckMobileNumber(string number);
        public Task<List<MNOInfo>> FuncReturnMnoInfo();
        public Task<List<ServiceType>> FuncReturnServiceInfo();
    }
}
