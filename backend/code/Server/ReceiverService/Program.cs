using System.Text;
using MQTTnet;

namespace ReceiverService
{
    class Program
    {
        private static int port = 1883;
        private static string server = "localhost";
        private static string clientId = "CSharpSubscriberClient_" + Guid.NewGuid().ToString("N");
        private static List<string> topics = new List<string>
        {
            "iot/waterSensor",
            "iot/lightSensor",
        };

        static async Task Main(string[] args)
        {
            Console.WriteLine("MQTT Subscriber starting...");

            var mqttFactory = new MqttClientFactory();
            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                ReceiverUtil.ConfigureMqttClientEvents(mqttClient);

                try
                {
                    await ReceiverUtil.ConnectMqttClient(mqttClient, server, port, clientId);

                    await ReceiverUtil.SubscribeToTopics(mqttClient, mqttFactory, topics);

                    Console.WriteLine("Connection successful. Waiting for messages...");
                    Console.WriteLine("Press Enter to exit.");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.WriteLine("Press Enter to exit.");
                    Console.ReadLine();
                }
                finally
                {
                    // Gracefully disconnect
                    if (mqttClient.IsConnected)
                    {
                        Console.WriteLine("Disconnecting...");
                        await ReceiverUtil.DisconnectMqttClient(mqttClient);
                    }
                    Console.WriteLine("Subscriber finished.");
                }
            } // MqttClient is disposed here
        }
    }
}
