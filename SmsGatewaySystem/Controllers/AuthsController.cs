using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary;
using Newtonsoft.Json;
using SmsGatewaySystem.Contracts;
using SmsGatewaySystem.Helpers;

namespace SmsGatewaySystem.Controllers
{
    //[Authorize(Roles = "ADMIN")]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthsController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly ILogger<AuthsController> _logger;
        private readonly ICommHelper _commRepo;

        public AuthsController(IAuthRepository authRepo, ILogger<AuthsController> logger, ICommHelper commRepo)
        {
            _authRepo = authRepo;
            _logger = logger;
            _commRepo = commRepo;
        }


        [HttpPost]
        public async Task<IActionResult> CreateAuthsInfo([FromBody] Authentication authsModel)
        {
            var authsResponse = new SmsClientResponse();

            bool opStat = false;

            try
            {
                var smsSettings = await _commRepo.FuncRetunCommData();

                _logger.LogInformation(JsonConvert.SerializeObject(authsModel));
                if (!string.IsNullOrEmpty(authsModel.UserId) && authsModel.Password.Length > 10)
                {
                   opStat = await _authRepo.FuncSaveAuthInfo(authsModel);
                }              

                
                if (opStat)
                {
                    authsResponse.StatusCode = 200;
                    authsResponse.StatusMsg = "Success";
                }
                else
                {
                    authsResponse.StatusCode = 201;
                    authsResponse.StatusMsg = "Failed...";
                    return Ok(authsResponse);
                }
                               

                return Ok(authsResponse);
            }
            catch (Exception es)
            {
                _logger.LogInformation("User Create...." + es.Message);
                authsResponse.StatusCode = 201;
                authsResponse.StatusMsg = "Exception...";
                return Ok(authsResponse);
            }
            finally
            {

            }
        }


    }
}
