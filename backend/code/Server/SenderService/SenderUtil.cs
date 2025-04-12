using System.Text;
using MQTTnet;

namespace SenderService;

public static class SenderUtil
{
    public static async Task PublishMessage(IMqttClient mqttClient, string topic, string payload)
    {
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(Encoding.UTF8.GetBytes(payload))
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce) // QoS Level 1
            .WithRetainFlag(false)
            .Build();

        await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
    }

    public static async Task DisconnectMqttClient(IMqttClient mqttClient)
    {
        await mqttClient.DisconnectAsync();
        Console.WriteLine("Disconnected from MQTT broker.");
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
    }
}
