using System.Text;
using DTOs;
using Entities;
using MQTTnet;

namespace ReceiverService;

public static class ReceiverUtil
{
    public delegate Task SensorReadingHandlerDelegate(SensorReadingDTO sensorReading);

    // Only for Proof of concept, will be in .ENV file in future (??)
    private static readonly Dictionary<string, int> TopicToSensorIdMap = new()
    {
        { "light/reading", 3 },
        { "temperature/reading", 1 },
        { "humidity/reading", 2 },
        { "soil/reading", 4 },
    };

    public static void ConfigureMqttClientEvents(
        IMqttClient mqttClient,
        ILogger logger,
        SensorReadingHandlerDelegate handleSensorReading,
        Action<bool>? setHealthStatus = null
    )
    {
        mqttClient.ConnectedAsync += e =>
        {
            logger.LogInformation("### CONNECTED TO BROKER ###");
            setHealthStatus?.Invoke(true);
            return Task.CompletedTask;
        };

        mqttClient.DisconnectedAsync += e =>
        {
            logger.LogInformation("### DISCONNECTED FROM BROKER ###");
            setHealthStatus?.Invoke(false);
            return Task.CompletedTask;
        };

        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            logger.LogInformation("### RECEIVED MESSAGE ###");

            var topic = e.ApplicationMessage.Topic;
            logger.LogInformation("Topic = {Topic}", topic);

            var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            logger.LogInformation("Message = {Message}", message);
            logger.LogInformation("Parsing received string message...");

            try
            {
                if (!TopicToSensorIdMap.TryGetValue(topic, out int sensorId))
                {
                    logger.LogWarning("Unknown topic: {Topic}, cannot map to sensor ID", topic);
                    return Task.CompletedTask;
                }

                if (int.TryParse(message, out var value))
                {
                    var newSensorReading = new SensorReadingDTO
                    {
                        SensorId = sensorId,
                        Value = value,
                        TimeStamp = DateTime.UtcNow,
                       
                    };

                    logger.LogInformation(
                        "Parsed sensor reading: SensorId={SensorId}, Value={Value}, Timestamp={Timestamp}",
                        newSensorReading.SensorId,
                        newSensorReading.Value,
                        newSensorReading.TimeStamp
                    );

                    handleSensorReading(newSensorReading).Wait();
                }
                else
                {
                    logger.LogWarning(
                        "Failed to parse message as integer value: {Message}",
                        message
                    );
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error parsing message: {Message}", message);
            }

            return Task.CompletedTask;
        };
    }

    public static async Task<bool> ConnectMqttClient(
        IMqttClient mqttClient,
        string server,
        int port,
        string clientId,
        ILogger logger,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(server, port)
                .WithClientId(clientId)
                .WithCleanSession()
                .Build();

            await mqttClient.ConnectAsync(mqttClientOptions, cancellationToken);
            logger.LogInformation("Connected to MQTT broker at {Server}:{Port}", server, port);
            return true;
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Service shutdown requested during connection attempt");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to connect to MQTT broker");
            return false;
        }
    }

    public static async Task DisconnectMqttClient(
        IMqttClient mqttClient,
        ILogger logger,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (mqttClient.IsConnected)
            {
                logger.LogInformation("Disconnecting from MQTT broker...");

                var disconnectOptions = new MqttClientDisconnectOptionsBuilder()
                    .WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection)
                    .Build();

                await mqttClient.DisconnectAsync(disconnectOptions, cancellationToken);
                logger.LogInformation("Disconnected from MQTT broker.");
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error during broker disconnect");
        }
    }

    public static async Task<bool> SubscribeToTopics(
        IMqttClient mqttClient,
        MqttClientFactory mqttFactory,
        List<string> topics,
        ILogger logger,
        CancellationToken cancellationToken
    )
    {
        try
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
                cancellationToken
            );

            logger.LogInformation("### SUBSCRIPTION RESULT ###");
            foreach (var subscription in subResult.Items)
            {
                logger.LogInformation(
                    "Topic Filter: {Topic}, Result Code: {ResultCode}",
                    subscription.TopicFilter.Topic,
                    subscription.ResultCode
                );
            }

            logger.LogInformation("Receiver is now listening for sensor data...");
            return true;
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Service shutdown requested during topic subscription");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to subscribe to topics");
            return false;
        }
    }
}
