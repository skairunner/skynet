using System;
using System.Threading.Tasks;

namespace plantwatch
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await Broker.StartBroker();
            Console.WriteLine("Plantwatch started.");

            await Listener.StartListener();
            Console.WriteLine("Plantwatch server started.");

            Console.ReadLine();
        }
    }
}