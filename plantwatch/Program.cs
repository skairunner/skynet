using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace plantwatch
{
    public static class Program
    {
        static async Task Heartbeat(TimeSpan interval, CancellationToken cancellationToken = default)
        {
            while (true)
            {
                Listener.connection.Write("heartbeat", new Dictionary<string, object>
                    {
                        { "heartbeat", 1 }   
                    });
                await Task.Delay(interval, cancellationToken);
            }
        }
        
        public static async Task Main(string[] args)
        {
            await Broker.StartBroker();
            Console.WriteLine("Plantwatch started.");

            Listener.Initialize();
            await Listener.StartListener();
            Console.WriteLine("Plantwatch server started.");

            var task = Task.Run(async () => { await Heartbeat(TimeSpan.FromSeconds(10)); });

            Console.ReadLine();
        }
    }
}