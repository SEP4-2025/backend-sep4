using System.Text;
using MQTTnet;

namespace SenderService;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("MQTT Publisher Test Tool starting...");

        var server = Environment.GetEnvironmentVariable("MQTT_BROKER_HOST") ?? "localhost";
        var port = int.TryParse(Environment.GetEnvironmentVariable("MQTT_BROKER_PORT"), out var p)
            ? p
            : 1883;
        var clientId = $"CSharpPublisherClient_{Guid.NewGuid().ToString("N")}";

        bool runningInContainer =
            Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

        Console.WriteLine($"Connecting to MQTT broker at {server}:{port}");
        Console.WriteLine($"Running in container: {runningInContainer}");

        var mqttFactory = new MqttClientFactory();
        using (var mqttClient = mqttFactory.CreateMqttClient())
        {
            SenderUtil.ConfigureMqttClientEvents(mqttClient);

            try
            {
                await SenderUtil.ConnectMqttClient(mqttClient, server, port, clientId);
                Console.WriteLine("Connected successfully.");

                if (runningInContainer)
                {
                    // In container mode, wait for signals and respond to specific environment triggers
                    Console.WriteLine("Running in container mode. Waiting for 5 seconds...");
                    // Send a test message on startup
                    await SenderUtil.PublishMessage(
                        mqttClient,
                        "iot/test",
                        $"Test message from container at {DateTime.UtcNow:o}"
                    );
                    Console.WriteLine(
                        "Test message sent. Service will stay connected and wait for external triggers."
                    );

                    // Example of waiting for specific signals
                    await Task.Delay(TimeSpan.FromMinutes(5)); // Stay alive for 5 minutes
                }
                else
                {
                    // Interactive mode for development
                    Console.WriteLine("Starting interactive test sequence...");
                    bool keepRunning = true;
                    while (keepRunning)
                    {
                        Console.WriteLine("\nSelect a topic to publish to:");
                        Console.WriteLine("1. iot/waterSensor");
                        Console.WriteLine("2. iot/lightSensor");
                        Console.WriteLine("3. iot/temperatureSensor");
                        Console.WriteLine("4. Custom topic");
                        Console.WriteLine("q. Quit");

                        var choice = Console.ReadLine()?.ToLower();

                        if (choice == "q")
                        {
                            keepRunning = false;
                            continue;
                        }

                        string topic;
                        switch (choice)
                        {
                            case "1":
                                topic = "iot/waterSensor";
                                break;
                            case "2":
                                topic = "iot/lightSensor";
                                break;
                            case "3":
                                topic = "iot/temperatureSensor";
                                break;
                            case "4":
                                Console.Write("Enter custom topic: ");
                                topic = Console.ReadLine() ?? "test/topic";
                                break;
                            default:
                                Console.WriteLine("Invalid choice. Using default topic.");
                                topic = "test/topic";
                                break;
                        }

                        Console.Write($"Enter message payload for {topic}: ");
                        var payload = Console.ReadLine() ?? DateTime.UtcNow.ToString("o");

                        await SenderUtil.PublishMessage(mqttClient, topic, payload);
                        Console.WriteLine($"Message sent to {topic}");
                    }
                }
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
