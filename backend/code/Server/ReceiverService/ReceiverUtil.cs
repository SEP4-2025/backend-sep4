using System.Text;
using System.Text.Json;
using DTOs;
using Entities;
using LogicImplements;
using LogicInterfaces;
using MQTTnet;

namespace ReceiverService;

public static class ReceiverUtil
{
    public static void ConfigureMqttClientEvents(
        IMqttClient mqttClient,
        ILogger logger,
        ISensorReadingInterface sensorReadingLogic,
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

            // Get the message payload as a string
            var messagePayload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            logger.LogInformation("Message = {Message}", messagePayload);

            logger.LogInformation("Adding received reading to database...");

            try
            {
                // Try to parse the message as JSON directly into SensorReadingDTO
                var newSensorReading = JsonSerializer.Deserialize<SensorReadingDTO>(messagePayload);

                if (newSensorReading != null)
                {
                    // If TimeStamp wasn't set in the JSON, use current time
                    if (newSensorReading.TimeStamp == default)
                    {
                        newSensorReading.TimeStamp = DateTime.UtcNow;
                    }

                    logger.LogInformation(
                        "Parsed sensor reading: Value={Value}, SensorId={SensorId}, ThresholdValue={ThresholdValue}",
                        newSensorReading.Value,
                        newSensorReading.SensorId,
                        newSensorReading.ThresholdValue
                    );

                    sensorReadingLogic.AddSensorReadingAsync(newSensorReading).Wait();
                }
                else
                {
                    logger.LogWarning("Failed to parse message as JSON: {Message}", messagePayload);
                }
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Failed to parse message as JSON: {Message}", messagePayload);

                // Fallback to the original behavior when JSON parsing fails
                var newSensorReading = new SensorReadingDTO
                {
                    Value = int.TryParse(messagePayload, out var value) ? value : 0,
                    TimeStamp = DateTime.UtcNow,
                    ThresholdValue = 0,
                    SensorId = 1,
                };

                sensorReadingLogic.AddSensorReadingAsync(newSensorReading).Wait();
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
