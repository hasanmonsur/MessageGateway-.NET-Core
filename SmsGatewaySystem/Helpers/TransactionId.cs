namespace SmsGatewaySystem.Helpers
{
    public class TransactionUniqueId
    {
        //Random _rdm = new Random();
        public string Next()
        {

            //string first = DateTime.Now.ToString("yyMMddHHmmssff");
            //string middle = _rdm.Next(1, 999).ToString("D3"); // _rdm.Next(100000, 999999).ToString();
            //string last = _rdm.Next(1, 999).ToString("D3"); // _rdm.Next(100000, 999999).ToString();
            //return first + middle + last;

            Guid guid = Guid.NewGuid();

            //Console.WriteLine(guid.ToString());            // Default format
            //Console.WriteLine(guid.ToString("D"));        // 32 digits separated by hyphens (default)
            //Console.WriteLine(guid.ToString("N"));
            return guid.ToString("N");
        }
    }
}
