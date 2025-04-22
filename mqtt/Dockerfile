FROM eclipse-mosquitto:2.0.21

# Copy the configuration file into the container
COPY ./mosquitto.conf /mosquitto/config/mosquitto.conf
# Copy the data directory into the container
COPY ./mosquitto.db /mosquitto/data

ENTRYPOINT ["/usr/bin/mosquitto", "-c", "/mosquitto/config/mosquitto.conf"]
