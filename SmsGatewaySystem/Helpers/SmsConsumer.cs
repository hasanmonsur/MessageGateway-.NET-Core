using MassTransit;

namespace SmsGatewaySystem.Helpers
{
    public class SmsGPConsumer : IConsumer<SmsGPMessage>
    {
        public async Task Consume(ConsumeContext<SmsGPMessage> context)
        {
            var sms = context.Message;
            Console.WriteLine($"SMS GP Sending....");
        }
    }

    public class SmsRBConsumer : IConsumer<SmsRBMessage>
    {
        public async Task Consume(ConsumeContext<SmsRBMessage> context)
        {
            var email = context.Message;
            Console.WriteLine($"SMS RB Sending....");
        }
    }

    public class SmsBLConsumer : IConsumer<SmsBLMessage>
    {
        public async Task Consume(ConsumeContext<SmsBLMessage> context)
        {
            var email = context.Message;
            Console.WriteLine($"SMS BL Sending....");
        }
    }

    public class SmsTTConsumer : IConsumer<SmsTTMessage>
    {
        public async Task Consume(ConsumeContext<SmsTTMessage> context)
        {
            var email = context.Message;
            Console.WriteLine($"SMS TT Sending....");
        }
    }
}
