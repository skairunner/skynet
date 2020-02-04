using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Server;

namespace plantwatch
{
    static class Broker
    {
        public static IMqttServer broker = null;
        
        public static async Task StartBroker()
        {
            Console.WriteLine("\nSkyNet - Plantwatch Broker");

            broker = new MqttFactory().CreateMqttServer();
            var opt = new MqttServerOptionsBuilder()
                .WithDefaultEndpointPort(1884)
                .WithApplicationMessageInterceptor(context =>
                {
                    // Add timestamps to messages, so that clients dont need to know their time
                    var payload = context.ApplicationMessage.Payload;
                    var timestamp = BitConverter.GetBytes(UnixTime.ToUnix(DateTimeOffset.Now));
                    var editedPayload = new List<byte>(payload);
                    editedPayload.AddRange(timestamp);
                    context.ApplicationMessage.Payload = editedPayload.ToArray();
                })
                .Build();
            broker.UseClientConnectedHandler(context =>
                {
                    Console.WriteLine($"Client {context.ClientId} connected.");
                });
            await broker.StartAsync(opt);
            
            
            Console.WriteLine("Listening on port 1884.");
        }
    }
}