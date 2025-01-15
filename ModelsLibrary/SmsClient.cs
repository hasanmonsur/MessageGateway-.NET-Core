namespace ModelsLibrary
{
    public class SmsClientRequest
    {
        public string CsmsId { get; set; }
        public string Sid { get; set; }
        public string Msisdn { get; set; }
        public string Sms { get; set; }
    }


    public class SmsClientResponse
    {
        public int StatusCode { get; set; }
        public string StatusMsg { get; set; }
        public List<SmsClientInfo> SmsInfo { get; set; }
    }

    public class SmsClientInfo
    {
        public string CsmsId { get; set; }
        public string RefId { get; set; }
        public string Msisdn { get; set; }
    }

}
