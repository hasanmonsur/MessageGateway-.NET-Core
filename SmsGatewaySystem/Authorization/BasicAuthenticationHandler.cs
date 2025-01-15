using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using System.Data;
using SmsGatewaySystem.Contracts;
using ModelsLibrary;

namespace SmsGatewaySystem.Authorization
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IAuthRepository _authRepo;
        private readonly ILoggerFactory _logger;


        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IAuthRepository authRepo,
            ISystemClock clock
            ) : base(options, logger, encoder, clock)
        {
            _authRepo = authRepo;
            _logger = logger;

        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            
            var user = new ApiAuthResponse(); string userpass = "";
            if (authHeader != null && authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Basic ".Length).Trim();
                Console.WriteLine(token);
                var credentialstring = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var credentials = credentialstring.Split(':');
                if (credentials.Count() > 0)
                {
                    user = await _authRepo.FuncGetAuthInfo(credentials[0], credentials[1]);
                }
                //if (user != null && await _authRepo.FuncCheckPassword(user, model.Password))

                Logger.LogInformation("credString: " + credentialstring);
                if (user != null)
                    if (!string.IsNullOrEmpty(user.UserId))
                    {
                        var claims = new[] { new Claim("name", credentials[0]), new Claim(ClaimTypes.Role, user.service_role_id) };


                        var identity = new ClaimsIdentity(claims, "Basic");

                        var claimsPrincipal = new ClaimsPrincipal(identity);
                        return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
                    }

                Response.StatusCode = 401;
                Response.Headers.Add("WWW-Authenticate", "Basic realm=\"dotnetthoughts.net\"");
                return await Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }
            else
            {
                Response.StatusCode = 401;
                Response.Headers.Add("WWW-Authenticate", "Basic realm=\"dotnetthoughts.net\"");
                return await Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }

        }
    }
}
