using System.Text;
using DTOs;
using Entities;
using MQTTnet;

namespace ReceiverService;

public static class ReceiverUtil
{
    // Define a delegate for handling sensor readings
    public delegate Task SensorReadingHandlerDelegate(SensorReadingDTO sensorReading);

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
            logger.LogInformation("Topic = {Topic}", e.ApplicationMessage.Topic);

            var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            logger.LogInformation("Message = {Message}", message);
            logger.LogInformation("Parsing received string message...");

            try
            {
                var parts = message.Split('|');

                if (parts.Length >= 2)
                {
                    if (
                        int.TryParse(parts[0], out var sensorId) // TODO: Delete it, not needed
                        && int.TryParse(parts[1], out var value)
                    )
                    {
                        var newSensorReading = new SensorReadingDTO
                        {
                            SensorId = 3, // Default for light sensor
                            Value = value,
                            TimeStamp = DateTime.UtcNow,
                            ThresholdValue = 0, // TODO: This one needs to be in sensor entity
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
                            "Failed to parse one or more components of the message: {Message}",
                            message
                        );
                    }
                }
                else
                {
                    logger.LogWarning(
                        "Message doesn't match expected format (sensorId,Value): {Message}",
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
