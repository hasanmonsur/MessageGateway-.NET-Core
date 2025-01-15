using MassTransit;

namespace SmsGatewaySystem.Helpers
{
    public class OtpGPConsumer : IConsumer<OtpGPMessage>
    {
        public async Task Consume(ConsumeContext<OtpGPMessage> context)
        {
            var sms = context.Message;
            Console.WriteLine($"SMS GP Sending....");
        }
    }

    public class OtpRBConsumer : IConsumer<OtpRBMessage>
    {
        public async Task Consume(ConsumeContext<OtpRBMessage> context)
        {
            var email = context.Message;
            Console.WriteLine($"SMS RB Sending....");
        }
    }

    public class OtpBLConsumer : IConsumer<OtpBLMessage>
    {
        public async Task Consume(ConsumeContext<OtpBLMessage> context)
        {
            var email = context.Message;
            Console.WriteLine($"SMS BL Sending....");
        }
    }

    public class OtpTTConsumer : IConsumer<OtpTTMessage>
    {
        public async Task Consume(ConsumeContext<OtpTTMessage> context)
        {
            var email = context.Message;
            Console.WriteLine($"SMS TT Sending....");
        }
    }
}
