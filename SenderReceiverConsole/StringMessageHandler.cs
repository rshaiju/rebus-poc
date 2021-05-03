using Rebus.Bus;
using Rebus.Exceptions;
using Rebus.Handlers;
using Rebus.Messages;
using Rebus.Retry.Simple;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SenderReceiverConsole
{
    class StringMessageHandler : IHandleMessages<string>, IHandleMessages<IFailed<string>>
    {
        private readonly IBus _bus;

        public StringMessageHandler(IBus bus)
        {
            this._bus = bus;
        }

        public async Task Handle(string message)
        {
            Console.WriteLine($"Received Message : {message}");
            throw new Exception("hehe");
            await Task.CompletedTask;
        }

        public async Task Handle(IFailed<string> message)
        {
            
            const int maxDeferCount = 2;

            var deferCount = Convert.ToInt32(message.Headers.GetValueOrDefault(Headers.DeferCount));

            Console.WriteLine($"Handling failed message for the {deferCount + 1}st time");
            if (deferCount >= maxDeferCount)
            {
                Console.WriteLine($"Sending the failed message to the error queue");
                await _bus.Advanced.TransportMessage.Deadletter($"Failed after {deferCount} deferrals\n\n{message.ErrorDescription}");
                return;
            }
            await _bus.Advanced.TransportMessage.Defer(TimeSpan.FromSeconds(30));
        }
    }
}
