using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Security.Policy;
using System.Text;
using SmsGatewaySystem.Contracts;
using ModelsLibrary;
using MassTransit;
using MassTransit.Testing.Implementations;

namespace SmsGatewaySystem.Helpers
{
    public class SmsQueueHelper : ISmsQueueHelper
    {
        private readonly ILogger<SmsQueueHelper> _logger;
        private readonly ICommHelper _comm;
        private readonly IPublishEndpoint _publishEndpoint;

        public SmsQueueHelper(ILogger<SmsQueueHelper> logger, ICommHelper comm, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _comm = comm;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<SmsTranResponse> SendSmsToQueue(SmsTranData model)
        {           
            var smsResponse = new SmsTranResponse();
            try
            {
                switch (model.MnoCode)
                {
                    case "GP":
                        var smsGPMessage = new SmsGPMessage(model);
                        await _publishEndpoint.Publish(smsGPMessage);
                        smsResponse.Status = 200;
                        break;

                    case "RB":
                        var smsRBMessage = new SmsRBMessage(model);
                        await _publishEndpoint.Publish(smsRBMessage);
                        smsResponse.Status = 200;
                        break;

                    case "BL":
                        var smsBLMessage = new SmsBLMessage(model);
                        await _publishEndpoint.Publish(smsBLMessage);
                        smsResponse.Status = 200;
                        break;

                    case "TT":
                        var smsTTMessage = new SmsTTMessage(model);
                        await _publishEndpoint.Publish(smsTTMessage);
                        smsResponse.Status = 200;
                        break;

                    default:
                        smsResponse.Status = 201;
                        break;
                }                

            }
            catch (Exception es)
            {
                smsResponse.Status = 201;

                _logger.LogInformation(es.StackTrace);
            }
            finally
            {

            }


            smsResponse.CsmsId = model.CsmsId;
            smsResponse.Msisdn = model.MobileNo;
            smsResponse.RefId = model.RefId;


            return smsResponse;
        }

        /*
        public async Task<bool> SendSmsToQueue(SmsTranData model)
        {
            var sStat = false;
            var smsResponse = new SmsSslResponse();

            HttpClient httpClient = new HttpClient();

            try
            {
                var objCommModels = await _comm.FuncRetunCommData();

                var apiUrl = objCommModels.SMSUrl + "/api/v3/send-sms";
                //model.api_token = objCommModels.api_token;
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.PostAsJsonAsync(apiUrl, model);  // Blocking call! Program will wait here until a response is received or a timeout occurs.


                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body.
                    var result = await response.Content.ReadAsStringAsync();  //Make sure to add a reference to System.Net.Http.Formatting.dll
                    smsResponse = JsonConvert.DeserializeObject<SmsSslResponse>(result);
                }
            }
            catch (Exception es)
            {
                smsResponse = new SmsSslResponse();

                _logger.LogInformation(es.StackTrace);
            }
            finally
            {
                httpClient.Dispose();
            }

            return sStat;
        }
        */
    }
}
