using System.Text;
using MQTTnet;

namespace ReceiverService;

public static class ReceiverUtil
{
    public static void ConfigureMqttClientEvents(IMqttClient mqttClient)
    {
        mqttClient.ConnectedAsync += e =>
        {
            Console.WriteLine("### CONNECTED TO BROKER ###");
            return Task.CompletedTask;
        };

        mqttClient.DisconnectedAsync += e =>
        {
            Console.WriteLine("### DISCONNECTED FROM BROKER ###");
            return Task.CompletedTask;
        };

        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            Console.WriteLine("### RECEIVED MESSAGE ###");
            Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
            Console.WriteLine(
                $"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}"
            );
            Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
            Console.WriteLine();

            return Task.CompletedTask;
        };
    }

    public static async Task ConnectMqttClient(
        IMqttClient mqttClient,
        string server,
        int port,
        string clientId
    )
    {
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(server, port)
            .WithClientId(clientId)
            .WithCleanSession()
            .Build();

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        Console.WriteLine("Connected to MQTT broker.");
    }

    public static async Task DisconnectMqttClient(IMqttClient mqttClient)
    {
        var disconnectOptions = new MqttClientDisconnectOptionsBuilder()
            .WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection)
            .Build();

        await mqttClient.DisconnectAsync(disconnectOptions, CancellationToken.None);
        Console.WriteLine("Disconnected from MQTT broker.");
    }

    public static async Task SubscribeToTopics(
        IMqttClient mqttClient,
        MqttClientFactory mqttFactory,
        List<string> topics
    )
    {
        var subscribeOptionsBuilder = mqttFactory.CreateSubscribeOptionsBuilder();

        foreach (var topic in topics)
        {
            subscribeOptionsBuilder.WithTopicFilter(f =>
                f.WithTopic(topic)
                    .WithQualityOfServiceLevel(
                        MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce
                    )
            );
        }

        var mqttSubscribeOptions = subscribeOptionsBuilder.Build();

        var subResult = await mqttClient.SubscribeAsync(
            mqttSubscribeOptions,
            CancellationToken.None
        );

        Console.WriteLine("### SUBSCRIPTION RESULT ###");
        foreach (var subscription in subResult.Items)
        {
            Console.WriteLine(
                $"+ Topic Filter: {subscription.TopicFilter.Topic}, Result Code: {subscription.ResultCode}"
            );
        }
    }
}
