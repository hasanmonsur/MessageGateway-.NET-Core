using Dapper;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using SmsGatewaySystem.Contracts;
using SmsGatewaySystem.Data;
using ModelsLibrary;
using Microsoft.Extensions.Caching.Memory;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace SmsGatewaySystem.Repository
{
    public class SmsRepository : ISmsRepository
    {
        private readonly DapperContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmsRepository> _logger;
        private readonly ISmsQueueHelper _messagesend;
        private readonly IMemoryCache _memoryCache;



        public SmsRepository(DapperContext context, IConfiguration configuration, ILogger<SmsRepository> logger, ISmsQueueHelper messagesend, IMemoryCache memoryCache)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _messagesend = messagesend;
            _memoryCache = memoryCache;
        }

        public async Task<bool> SaveSmsInfo(SmsTranData model)
        {
           var data=new SmsTranResponse();
            
            try
            {
                
                var query = "INSERT INTO mnouser.smstrandata(refid, trandate, csmsid, serviceid, mobileno, message, status, statusmsg, smstype, mnocode, mnoprefix, priorityid)"+
                                                             "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                var result = 0; var messg = MaskNumbers(model.Message);
                using (var connection = _context.CreateConnection())
                {
                    result = await connection.ExecuteAsync(query, model);
                }

                if (result > 0) {
                    data.Msisdn = model.MobileNo;
                    data.RefId = model.RefId;
                    data.CsmsId = model.CsmsId;
                }

            }
            catch (Exception es)
            {
                _logger.LogInformation(es.StackTrace);
            }


            return data;
        }

        public async Task<bool> UpdateSmsInfo(SmsTranData model, string statusmsg, string status)
        {
            bool opstat = false;

            try
            {
                if (string.IsNullOrEmpty(statusmsg)) statusmsg = "";

                var query = "UPDATE TBL_SMS_DATA SET STATUS=:status, STATUS_MSG=:statusmsg WHERE SMSID=:csms_id and  PHONE_NO=:msisdn ";

                var result = 0;
                using (var connection = _context.CreateConnection())
                {
                    result = await connection.ExecuteAsync(query, new { model.CsmsId, statusmsg, model.StatusMsg, status });
                }

                if (result > 0) opstat = true;
                else opstat = false;

            }
            catch (Exception es)
            {
                opstat = false;
                _logger.LogInformation(es.StackTrace);
            }


            return opstat;
        }

        public async Task<bool> CheckMobileNumber(string number)
        {
            bool res = false;
            string pattern = @"\d+";
            Regex regex = new Regex(pattern);
            if (regex.IsMatch(number) && number.Length > 9 && number.Length < 20)
            {
                res = true;
            }
            return res;
        }

        internal static string MaskNumbers(string input)
        {
            return Regex.Replace(input, @"\d", "*");
        }

        public async Task<List<MNOInfo>> FuncReturnMnoInfo()
        {
            var key = "MnoData_Key";
            string cachedData = "";

            var dataList = new List<MNOInfo>();
            var query = "SELECT autoid, mnocode, mnoprefix, mnodesc, status, entrydate FROM mnouser.mnolist where status=1";
            
            try
            {

                if (!_memoryCache.TryGetValue(key, out cachedData))
                {
                    // Simulate fetching data
                    //cachedData = $"Data fetched at {DateTime.UtcNow}";
                    using (var connection = _context.CreateSmsDbConnection())
                    {
                        dataList = (await connection.QueryAsync<MNOInfo>(query)).ToList();

                        cachedData = JsonConvert.SerializeObject(dataList, Formatting.Indented); 
                    }

                    // Set cache options
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1700),
                        SlidingExpiration = TimeSpan.FromMinutes(1)
                    };

                    // Save data in cache
                    _memoryCache.Set(key, cachedData, cacheOptions);
                }
                else
                {
                    if(!string.IsNullOrEmpty(cachedData)) 
                      dataList = JsonConvert.DeserializeObject<List<MNOInfo>>(cachedData); 
                }
                

            }
            catch (Exception ex)
            {

                _logger.LogInformation("FuncGetAuthInfo :" + ex.StackTrace);
            }
            finally
            {

            }

            return dataList;
        }

        public async Task<List<ServiceType>> FuncReturnServiceInfo()
        {
            var key = "ServiceInfo_Key";
            string cachedData = "";

            var dataList = new List<ServiceType>();
            var query = "SELECT autoid, serviceid, smstype, priorityid FROM mnouser.servicetype";

            try
            {                

                if (!_memoryCache.TryGetValue(key, out cachedData))
                {
                    // Simulate fetching data
                    using (var connection = _context.CreateSmsDbConnection())
                    {
                        dataList = (await connection.QueryAsync<ServiceType>(query)).ToList();

                        cachedData = JsonConvert.SerializeObject(dataList, Formatting.Indented);
                    }

                    // Set cache options
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1700),
                        SlidingExpiration = TimeSpan.FromMinutes(1)
                    };

                    // Save data in cache
                    _memoryCache.Set(key, cachedData, cacheOptions);
                }
                else
                {
                    if (!string.IsNullOrEmpty(cachedData))
                        dataList = JsonConvert.DeserializeObject<List<ServiceType>>(cachedData);
                }


            }
            catch (Exception ex)
            {

                _logger.LogInformation("FuncGetAuthInfo :" + ex.StackTrace);
            }
            finally
            {

            }

            return dataList;
        }

    }
}
