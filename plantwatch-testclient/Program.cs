﻿using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using plantwatch;

namespace plantwatch_testclient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId("client1")
                .WithTcpServer("localhost", 1884)
                .Build();
            var opt = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(mqttClientOptions)
                .Build();

            var client = new MqttFactory().CreateManagedMqttClient();
            await client.StartAsync(opt);
            var generic_msg = new MqttApplicationMessageBuilder()
                .WithTopic("farm/client1")
                .WithPayload(new Payload().ToBytes(false))
                .WithAtLeastOnceQoS()
                .Build();
            var rng = new Random();
            var prevValue = 0.0;
            for (var i = 0; i < 1000000; i++)
            {
                prevValue += rng.NextDouble() * 0.01;
                await client.PublishAsync(generic_msg);
                var moistureMsg = new MqttApplicationMessageBuilder()
                    .WithTopic("farm/client1/moisture")
                    .WithPayload(new MoisturePayload{ MoistureFraction = prevValue}.ToBytes(false))
                    .WithAtLeastOnceQoS()
                    .Build();
                await client.PublishAsync(moistureMsg);
                Console.WriteLine("Sent.");
                await Task.Delay(1000);
            }

            Console.ReadLine();
        }
    }
}