using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Server;

namespace plantwatch
{
    static class Broker
    {
        public static async Task StartBroker()
        {
            Console.WriteLine("\nSkyNet - Plantwatch Broker");

            var mqttserver = new MqttFactory().CreateMqttServer();
            var opt = new MqttServerOptionsBuilder()
                .WithDefaultEndpointPort(1884)
                .WithApplicationMessageInterceptor(context =>
                {
                    // Add timestamps to messages, so that clients dont need to know their time
                    var payload = context.ApplicationMessage.Payload;
                    var editedPayload = new List<byte>(payload);
                    editedPayload.Add(Convert.ToByte(DateTime.Now.ToFileTimeUtc()));
                    context.ApplicationMessage.Payload = editedPayload.ToArray();
                })
                .WithClientId("broker")
                .Build();
            mqttserver.UseApplicationMessageReceivedHandler(context =>
            {
                Console.WriteLine($"{context.ApplicationMessage.Topic}");
            });
            mqttserver.UseClientConnectedHandler(context =>
                {
                    Console.WriteLine($"Client {context.ClientId} connected.");
                });
            await mqttserver.StartAsync(opt);
            
            
            Console.WriteLine("Listening on port 1884.");
        }
    }
}