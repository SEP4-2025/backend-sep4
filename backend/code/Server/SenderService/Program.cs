using System.Text;
using MQTTnet;

namespace SenderService;

class Program
{
    private static int port = 1883;
    private static string server = "localhost";
    private static string clientId = "CSharpPublisherClient_" + Guid.NewGuid().ToString("N");

    static async Task Main(string[] args)
    {
        Console.WriteLine("MQTT Publisher starting...");

        var mqttFactory = new MqttClientFactory();
        using (var mqttClient = mqttFactory.CreateMqttClient())
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(server, port)
                .WithClientId("CSharpPublisherClient_" + Guid.NewGuid().ToString("N"))
                .WithCleanSession()
                .Build();

            SenderUtil.ConfigureMqttClientEvents(mqttClient);

            try
            {
                await SenderUtil.ConnectMqttClient(mqttClient, server, port, clientId);

                await SenderUtil.PublishMessage(mqttClient, "iot/waterSensor", "Water sensor data");
                await SenderUtil.PublishMessage(mqttClient, "iot/lightSensor", "Light sensor data");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                if (mqttClient.IsConnected)
                {
                    Console.WriteLine("Disconnecting...");
                    await SenderUtil.DisconnectMqttClient(mqttClient);
                }
            }
        }
    }
}
