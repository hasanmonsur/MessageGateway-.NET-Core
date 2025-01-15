using Dapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SmsGatewaySystem.Contracts;
using SmsGatewaySystem.Data;
using ModelsLibrary;

namespace SmsGatewaySystem.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DapperContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthRepository> _logger;
        public AuthRepository(DapperContext context, IConfiguration configuration, ILogger<AuthRepository> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> FuncSaveAuthInfo(Authentication authInfo)
        {
            var roleInfos = new ServiceRoles
            {
                UserId=authInfo.UserId,
                Service_role_id = "SERVICE",
                Status="Y"
            };
            var query = "INSERT INTO mnouser.authentication(userid, password, serviceid, status)VALUES (:userid, :password, :serviceid, 'Y')";
            var query1 = "INSERT INTO mnouser.serviceroles(userid, service_role_id, status) VALUES (:userid, :service_role_id, :status)";

            try
            {
                using (var connection = _context.CreateSmsDbConnection())
                {
                    var result = await connection.ExecuteAsync(query,authInfo);

                    if (result > 0)
                    {
                        await connection.ExecuteAsync(query1, roleInfos);
                        return true;
                    }
                    else return false;
                }
            }
            catch (Exception ex)
            {
                return false;
                _logger.LogInformation("FuncGetAuthInfo :" + ex.StackTrace);
            }
            finally
            {
                
            }            
        }

        public async Task<ApiAuthResponse> FuncGetAuthInfo(string userid, string password)
        {
            var auths = new ApiAuthResponse();
            var query = "SELECT u.userid, u.serviceid, r.service_role_id  FROM mnouser.authentication u, mnouser.serviceroles r where u.userid=:userid and u.userid=r.userid and u.password=:password and u.status='Y'";

            try
            {
                using (var connection = _context.CreateSmsDbConnection())
                {
                    auths = await connection.QueryFirstOrDefaultAsync<ApiAuthResponse>(query, new { userid, password });
                }
            }
            catch (Exception ex)
            {

                _logger.LogInformation("FuncGetAuthInfo :" + ex.StackTrace);
            }
            finally
            {

            }



            return auths;
        }

        //public async Task<List<ServiceRoles>> FuncGetRoles(string userid)
        //{
        //    var auths = new List<RoleModel>();

        //    var query = "SELECT userid, roleid FROM TBL_USER_ROLE where status='Y' and userid=@userid";
        //    using (var connection = _context.CreateConnection())
        //    {
        //        var auths1 = await connection.QueryAsync<RoleModel>(query, new { userid });
        //        auths = auths1.ToList();
        //    }

        //    return auths;
        //}
        //public async Task<List<ServiceRoles>> FuncGetServiceRoles(string? userid)
        //{
        //    var query = "SELECT * FROM TBL_USERINFO";
        //    using (var connection = _context.CreateConnection())
        //    {
        //        var auths = await connection.QueryAsync<ServiceRoles>(query);
        //        return auths.ToList();
        //    }
        //}


    }
}
