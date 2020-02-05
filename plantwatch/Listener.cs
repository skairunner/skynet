using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using InfluxDB.Collector;

namespace plantwatch
{
    public static class Listener
    {
        public static IManagedMqttClient listener;
        public static MetricsCollector connection; // to InfluxDB

        public static async Task StartListener()
        {
            connection = new CollectorConfiguration()
                .Tag.With("host", "client1")
                .Batch.AtInterval(TimeSpan.FromSeconds(2))
                .WriteTo.InfluxDB("http://localhost:8086", "farmdb")
                .CreateCollector();
            
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
                Payload payload = null;
                var type = PayloadUtilities.FindBinaryPayloadType(context.ApplicationMessage.Payload);
                switch (type)
                {
                    case PayloadTypes.Generic:
                        payload = new Payload();
                        payload.FromBytes(context.ApplicationMessage.Payload);
                        break;
                    case PayloadTypes.Moisture:
                        payload = new MoisturePayload();
                        payload.FromBytes(context.ApplicationMessage.Payload);
                        connection.Write($"plant{payload.UID}",
                            new Dictionary<string, object>
                            {
                                {"moisture", (payload as MoisturePayload).MoistureFraction}
                            });
                        break;
                    default:
                        Console.WriteLine($"Received packet type '{type.ToString()}' with no handler.");
                        return;
                }
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