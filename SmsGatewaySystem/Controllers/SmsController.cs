using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog;
using Serilog.Core;
using System.Data;
using System.Security.Cryptography;
using SmsGatewaySystem.Contracts;
using SmsGatewaySystem.Helpers;
using ModelsLibrary;
using AutoMapper;

namespace SmsGatewaySystem.Controllers
{
    /// SMS Middleware API Develop for SSLwireless API Delivery
    /// Md. Hasan Monsur
    /// </summary>
    [Authorize(Roles = "SERVICE")]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SmsController : ControllerBase
    {
        private readonly ISmsQueueHelper _smsQueueHelper;
        private readonly ISmsRepository _dataRepo;
        private readonly ILogger<SmsController> _logger;
        private readonly ICommHelper _commRepo;
        private readonly IMapper _mapper;

        public SmsController(ISmsQueueHelper smsQueueHelper, ISmsRepository dataRepo, ILogger<SmsController> logger, ICommHelper commRepo,IMapper mapper)
        {
            _smsQueueHelper = smsQueueHelper;
            _dataRepo = dataRepo;
            _logger = logger;
            _commRepo = commRepo;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> SendLocalSms([FromBody] SmsClientRequest smsModel)
        {
            var tranid = new TransactionUniqueId();

            var smsClientResponse = new SmsClientResponse();
            var smsresponse = new SmsTranResponse();
            

            bool sStat = false;

            try
            {
                var smsSettings = await _commRepo.FuncRetunCommData();

                _logger.LogInformation(JsonConvert.SerializeObject(smsModel));
                if (!string.IsNullOrEmpty(smsModel.CsmsId) && smsModel.CsmsId.Length <= 20)
                {
                    Random _rdm = new Random();
                    string middle = "";
                    if (smsModel.CsmsId.Length < 17)
                        middle = _rdm.Next(1, 9999).ToString("D4");
                    else if (smsModel.CsmsId.Length > 16 && smsModel.CsmsId.Length < 19)
                        middle = _rdm.Next(1, 99).ToString("D2");

                    smsModel.CsmsId = smsModel.CsmsId + middle;
                }
                else smsModel.CsmsId = tranid.Next();


                if (smsModel.Sms.ToLower().Contains("otp"))
                {
                    smsClientResponse.StatusMsg = "Your SMS Channel is not valid";
                    return Ok(smsClientResponse);
                }

                else if (!await _dataRepo.CheckMobileNumber(smsModel.Msisdn))
                {
                    smsClientResponse.StatusMsg = "Mobile No is not valid length";
                    return Ok(smsClientResponse);
                }
                else if (smsModel.CsmsId == "" || smsModel.CsmsId == null)
                {
                    smsClientResponse.StatusMsg = "ClientTranId can not blank";
                    return Ok(smsClientResponse);
                }

                string myParameters = "", sid = "";

                
                if (smsModel.Msisdn.Length == 11 && smsModel.Msisdn.Substring(0, 2) != "00")
                {
                    smsModel.Msisdn = "88" + smsModel.Msisdn;
                }
                //  pull service list
                var mnoList=await _dataRepo.FuncReturnMnoInfo();

                //  pull MNO List
                var serviceList=await _dataRepo.FuncReturnServiceInfo();

                var smsdata = new SmsTranData();
                smsdata.RefId = tranid.Next();
                smsdata.CsmsId = smsModel.CsmsId;
                smsdata.Status = 1;
                smsdata.StatusMsg = "queue";
                smsdata.Message = smsModel.Sms;
                smsdata.MobileNo = smsModel.Msisdn;
                var mnoData = mnoList.FirstOrDefault(m => m.MnoPrefix == smsdata.MobileNo.Substring(0,5));
                smsdata.MnoCode = mnoData.MnoCode;
                smsdata.MnoPrefix = mnoData.MnoPrefix;

                smsdata.ServiceId = smsModel.Sid;
                var serviceData = serviceList.FirstOrDefault(m => m.ServiceId == smsdata.ServiceId);
                smsdata.SmsType = serviceData.SmsType;
                smsdata.PriorityId = serviceData.PriorityId;


                smsresponse = await _smsQueueHelper.SendSmsToQueue(smsdata);

                if (smsresponse.Status==200)
                {

                    await _dataRepo.SaveSmsInfo(smsdata);

                    var clientdata = _mapper.Map<SmsClientInfo>(smsresponse);
                    smsClientResponse.SmsInfo.Add(clientdata);
                    smsClientResponse.StatusCode = 200;
                    smsClientResponse.StatusMsg = "SMS Send Success!!";
                }
                else
                {
                    //await _dataRepo.UpdateSmsInfo(smsrequest, "Failed", "F");

                    smsClientResponse.StatusCode = 201;
                    smsClientResponse.StatusMsg = "SMS Send  Fail ..!!";
                    return Ok(smsClientResponse);
                }
     

                return Ok(smsClientResponse);
            }
            catch (Exception es)
            {
                _logger.LogInformation("SMS Send Exception" + es.Message);
                
                smsClientResponse.StatusCode = 203;
                smsClientResponse.StatusMsg = "SMS Send Exception ..!!";

                return Ok(smsClientResponse);
            }
            finally
            {

            }
        }


        /*
        [HttpPost]
        public async Task<IActionResult> SendBulkLocalSms([FromBody] List<SmsModel> smsListModel)
        {
            var tranid = new TransactionUniqueId();
            var smsClientResponse = new SmsClientResponse();

            var smsrequest = new SmsRequest();
            var smsresponse = new SmsResponse();
            bool sStat = false;

            try
            {
                var smsSettings = await _commRepo.FuncRetunCommData();
              foreach( var smsModel in smsListModel)
                { 
                _logger.LogInformation(JsonConvert.SerializeObject(smsModel));
                if (!string.IsNullOrEmpty(smsModel.ClientTranId) && smsModel.ClientTranId.Length <= 20)
                {
                    Random _rdm = new Random();
                    string middle = "";
                    if (smsModel.ClientTranId.Length < 17)
                        middle = _rdm.Next(1, 9999).ToString("D4");
                    else if (smsModel.ClientTranId.Length > 16 && smsModel.ClientTranId.Length < 19)
                        middle = _rdm.Next(1, 99).ToString("D2");

                    smsrequest.csms_id = smsModel.ClientTranId + middle;
                }
                else smsrequest.csms_id = tranid.Next();


                if (smsModel.Message.ToLower().Contains("otp"))
                {
                    smsClientResponse.StatusMessage = "Your SMS Channel is not valid";
                    return Ok(smsClientResponse);
                }

                else if (!await _dataRepo.CheckMobileNumber(smsModel.PhoneNumber))
                {
                    smsClientResponse.StatusMessage = "Mobile No is not valid length";
                    return Ok(smsClientResponse);
                }
                else if (smsrequest.csms_id == "" || smsrequest.csms_id == null)
                {
                    smsClientResponse.StatusMessage = "ClientTranId can not blank";
                    return Ok(smsClientResponse);
                }

                string myParameters = "", sid = "";



                if (smsModel.PhoneNumber.Length == 11 && smsModel.PhoneNumber.Substring(0, 2) != "00")
                {
                    smsrequest.sid = smsSettings.SslsidSms;
                    smsrequest.msisdn = "88" + smsModel.PhoneNumber;
                    smsrequest.sms = smsModel.Message;
                }
                else
                {
                    smsrequest.sid = smsSettings.FT_SslsidSms;
                    smsrequest.msisdn = smsModel.PhoneNumber;
                    smsrequest.sms = smsModel.Message;
                }

                bool opStat = await _dataRepo.SaveSmsInfo(smsrequest, smsModel.serviceUser);
                if (opStat)
                {
                    smsresponse = await _messageSendHelper.SendSmsInfo(smsrequest);
                    if (smsresponse.status_code == 200)
                    {
                        var smsDeliveryStatus = "";
                        var smsInfo = smsresponse.smsinfo;
                        if (smsInfo.Count() > 0)
                        {
                            smsDeliveryStatus = smsInfo[0].sms_status;
                        }

                        if (smsDeliveryStatus == "SUCCESS")
                        {
                            sStat = true;
                            await _dataRepo.UpdateSmsInfo(smsrequest, smsDeliveryStatus, "Y");
                        }
                        else
                        {
                            sStat = false;
                            await _dataRepo.UpdateSmsInfo(smsrequest, "Failed", "F");
                        }

                    }
                    else
                    {
                        sStat = false;
                        await _dataRepo.UpdateSmsInfo(smsrequest, "Failed", "F");
                    }


                }
                else
                {
                    smsClientResponse.StatusCode = "201";
                    smsClientResponse.StatusMessage = "ClientTranId Duplicate ..!!";
                    return Ok(smsClientResponse);
                }

              }

                if (sStat)
                {
                    smsClientResponse.ClientTranId = smsrequest.csms_id;
                    smsClientResponse.StatusCode = "200";
                    smsClientResponse.StatusMessage = "SMS Send Success!!";
                }
                else
                {
                    smsClientResponse.ClientTranId = smsrequest.csms_id;
                    smsClientResponse.StatusCode = "201";
                    smsClientResponse.StatusMessage = "SMS Send  Fail ..!!";
                }

                return Ok(smsClientResponse);
            }
            catch (Exception es)
            {
                _logger.LogInformation("SMS Send Exception" + es.Message);
                smsClientResponse.ClientTranId = smsrequest.csms_id;
                smsClientResponse.StatusCode = "203";
                smsClientResponse.StatusMessage = "SMS Send Exception ..!!";

                return Ok(smsClientResponse);
            }
            finally
            {

            }
        }

        [HttpPost]
        public async Task<IActionResult> SendForeignSms([FromBody] SmsModel smsModel)
        {
            var tranid = new TransactionUniqueId();
            var smsClientResponse = new SmsClientResponse();

            var smsrequest = new SmsRequest();
            var smsresponse = new SmsResponse();
            bool sStat = false;

            try
            {
                //_logger.LogInformation(JsonConvert.SerializeObject(smsModel));
                if (!string.IsNullOrEmpty(smsModel.ClientTranId) && smsModel.ClientTranId.Length <= 20)
                {
                    Random _rdm = new Random();
                    string middle = "";
                    if (smsModel.ClientTranId.Length < 17)
                        middle = _rdm.Next(1, 9999).ToString("D4");
                    else if (smsModel.ClientTranId.Length > 16 && smsModel.ClientTranId.Length < 19)
                        middle = _rdm.Next(1, 99).ToString("D2");

                    smsrequest.csms_id = smsModel.ClientTranId + middle;
                }
                else smsrequest.csms_id = tranid.Next();

                var smsSettings = await _commRepo.FuncRetunCommData();
                if (!smsModel.Message.ToLower().Contains("otp"))
                {
                    smsClientResponse.StatusMessage = "Your SMS Channel is not valid";
                    return Ok(smsClientResponse);
                }

                else if (!await _dataRepo.CheckMobileNumber(smsModel.PhoneNumber))
                {
                    smsClientResponse.StatusMessage = "Mobile No is not valid length";
                    return Ok(smsClientResponse);
                }
                else if (smsrequest.csms_id == "" || smsrequest.csms_id == null)
                {
                    smsClientResponse.StatusMessage = "ClientTranId can not blank";
                    return Ok(smsClientResponse);
                }

                string myParameters = "", sid = "";



                if (smsModel.PhoneNumber.Length == 11 && smsModel.PhoneNumber.Substring(0, 2) != "00")
                {
                    smsrequest.sid = smsSettings.SslsidOtp;
                    smsrequest.msisdn = "88" + smsModel.PhoneNumber;
                    smsrequest.sms = smsModel.Message;
                }
                else
                {
                    smsrequest.sid = smsSettings.FT_SslsidOtp;
                    smsrequest.msisdn = smsModel.PhoneNumber;
                    smsrequest.sms = smsModel.Message;
                }

                bool opStat = await _dataRepo.SaveSmsInfo(smsrequest, smsModel.serviceUser);
                if (opStat)
                {

                    smsresponse = await _messageSendHelper.SendSmsInfo(smsrequest);
                    if (smsresponse.status_code == 200)
                    {
                        var smsDeliveryStatus = "";
                        var smsInfo = smsresponse.smsinfo;
                        if (smsInfo.Count() > 0)
                        {
                            smsDeliveryStatus = smsInfo[0].sms_status;
                        }

                        if (smsDeliveryStatus == "SUCCESS")
                        {
                            sStat = true;
                            await _dataRepo.UpdateSmsInfo(smsrequest, smsDeliveryStatus, "Y");
                        }
                        else
                        {
                            sStat = false;
                            await _dataRepo.UpdateSmsInfo(smsrequest, "Failed", "F");
                        }

                    }
                    else
                    {
                        sStat = false;
                        await _dataRepo.UpdateSmsInfo(smsrequest, "Failed", "F");
                    }
                }
                else
                {
                    smsClientResponse.StatusCode = "201";
                    smsClientResponse.StatusMessage = "ClientTranId Duplicate ..!!";
                    return Ok(smsClientResponse);
                }


                if (sStat)
                {
                    smsClientResponse.ClientTranId = smsrequest.csms_id;
                    smsClientResponse.StatusCode = "200";
                    smsClientResponse.StatusMessage = "SMS Send Success!!";
                }
                else
                {
                    smsClientResponse.ClientTranId = smsrequest.csms_id;
                    smsClientResponse.StatusCode = "201";
                    smsClientResponse.StatusMessage = "SMS Send  Fail ..!!";
                }

                return Ok(smsClientResponse);

            }
            catch (Exception es)
            {
                _logger.LogInformation("OTP SMS Send Exception" + es.Message);
                smsClientResponse.ClientTranId = smsrequest.csms_id;
                smsClientResponse.StatusCode = "203";
                smsClientResponse.StatusMessage = "OTP SMS Send Exception ..!!";

                return Ok(smsClientResponse);
            }
            finally
            {

            }
        } */
    }
}