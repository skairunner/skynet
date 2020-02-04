using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;

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
                var payload = new Payload();
                payload.FromBytes(context.ApplicationMessage.Payload);
                Console.WriteLine(payload.ToString());
            });
            listener.UseConnectedHandler(async e =>
            {
                await listener.SubscribeAsync(new TopicFilterBuilder().WithTopic("#").Build());
            });
            await listener.StartAsync(opt);
        }
    }
}