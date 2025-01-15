using SmsGatewaySystem.Contracts;

namespace SmsGatewaySystem.Helpers
{
    public class CommHelper : ICommHelper
    {
        private readonly IConfiguration _configuration;
        public CommHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<CommModels> FuncRetunCommData()
        {
            var objCommModels = new CommModels();

            objCommModels.SslsidSms = _configuration["SmsSID:SslsidSms"];
            objCommModels.FT_SslsidSms = _configuration["SmsSID:FT_SslsidSms"];
            objCommModels.SslsidOtp = _configuration["SmsSID:SslsidOtp"];
            objCommModels.FT_SslsidOtp = _configuration["SmsSID:FT_SslsidOtp"];
            objCommModels.SMSUrl = _configuration["SmsSID:SMSUrl"];
            objCommModels.api_token = _configuration["SmsSID:api_token"];


            return objCommModels;
        }
    }


    public class CommModels
    {
        public string? SslsidSms { get; set; }

        public string? FT_SslsidSms { get; set; }

        public string? SslsidOtp { get; set; }
        public string? FT_SslsidOtp { get; set; }
        public string SMSUrl { get; set; }
        public string api_token { get; set; }
    }

}

