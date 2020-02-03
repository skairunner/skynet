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

            Console.ReadLine();
        }
    }
}