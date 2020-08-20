using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dotenv.net;
using dotenv.net.Utilities;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using InfluxDB.Collector;
using InfluxDB.Collector.Diagnostics;

namespace plantwatch
{
    public static class Listener
    {
        public static IManagedMqttClient listener;
        public static MetricsCollector connection; // to InfluxDB

        public static void Initialize()
        {
            DotEnv.Config();
            var envReader = new EnvReader();
            var address = envReader.GetStringValue("INFLUX_ADDRESS");
            var dbname = envReader.GetStringValue("INFLUX_DBNAME");
            var username = envReader.GetStringValue("INFLUX_USERNAME");
            var password = envReader.GetStringValue("INFLUX_PASSWORD");
            connection = new CollectorConfiguration()
                .Tag.With("host", "client1")
                .Batch.AtInterval(TimeSpan.FromSeconds(5))
                .WriteTo.InfluxDB(address, dbname, username, password)
                .CreateCollector();
            CollectorLog.RegisterErrorHandler((m, e) =>
            {
                Console.WriteLine($"{m}: {e}");
            });
        }
        
        public static async Task StartListener()
        {          
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId("server")
                .WithTcpServer("localhost", 1884)
                .Build();

            listener = new MqttFactory().CreateManagedMqttClient();
            
            var opt = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(mqttClientOptions)
                .Build();
            
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