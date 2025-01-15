using MassTransit;
using ModelsLibrary;
using System.Net.Mail;

namespace SmsGatewaySystem.Helpers
{
    public class QueueNameGenerator
    {

        public static string GetQueueName(string serviceName)
        {
            return $"{serviceName}-queue";
        }
    }


    public record SmsGPMessage(SmsTranData smsData);
    public record SmsRBMessage(SmsTranData smsData);
    public record SmsBLMessage(SmsTranData smsData);
    public record SmsTTMessage(SmsTranData smsData);

    public record OtpGPMessage(SmsTranData smsData);
    public record OtpRBMessage(SmsTranData smsData);
    public record OtpBLMessage(SmsTranData smsData);
    public record OtpTTMessage(SmsTranData smsData); 


}
