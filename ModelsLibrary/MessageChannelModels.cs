namespace ModelsLibrary
{
    

    //--------------SSL Wireless---------
    public class SmsSslRequest
    {
        public SmsSslRequest() { }
        public string api_token { get; set; }
        public string sid { get; set; }
        public string msisdn { get; set; }
        public string sms { get; set; }
        public string csms_id { get; set; }
    }

    public class SmsSslResponse
    {
        public SmsSslResponse()
        {
            smsinfo = new List<SmsSslinfo>();
        }
        public string status { get; set; }
        public int status_code { get; set; }
        public string error_message { get; set; }
        public List<SmsSslinfo> smsinfo { get; set; }
    }

    public class SmsSslinfo
    {
        public SmsSslinfo()
        {

        }
        public string sms_status { get; set; }
        public string status_message { get; set; }
        public string msisdn { get; set; }
        public string sms_type { get; set; }
        public string sms_body { get; set; }
        public string csms_id { get; set; }
        public string reference_id { get; set; }
    }


    //--------------SSL Wireless---------




}
