using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Server;

namespace plantwatch
{
    public static class Listener
    {
        public static IManagedMqttClient listener;

        public static async Task StartListener()
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId("server")
                .WithTcpServer("localhost", 1884)
                .Build();
            var opt = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(mqttClientOptions)
                .Build();

            listener = new MqttFactory().CreateManagedMqttClient();
            listener.UseApplicationMessageReceivedHandler(context =>
            {
                Console.WriteLine($"Msg posted to {context.ApplicationMessage.Topic} received.");
            });
            listener.UseConnectedHandler(async e =>
            {
                await listener.SubscribeAsync(new TopicFilterBuilder().WithTopic("#").Build());
                Console.WriteLine("Subscribed to farm moisture");
            });
            await listener.StartAsync(opt);
        }
    }
}