using Rebus.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SenderReceiverConsole
{
    class StringMessageHandler : IHandleMessages<string>
    {
        public async Task Handle(string message)
        {
            Console.WriteLine($"Received Message : {message}");
            await Task.CompletedTask;
        }
    }
}
