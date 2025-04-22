#!/bin/bash
# entrypoint.sh

BUCKET_NAME="growmate-bucket"
MOUNT_POINT="/mnt/mqtt"

gcsfuse --implicit-dirs "$BUCKET_NAME" "$MOUNT_POINT"

MOSQUITTO_DATA_DIR="/mosquitto/data"

if [ -z "$(ls -A $MOSQUITTO_DATA_DIR)" ]; then
    echo "Copying initial MQTT data from GCS bucket..."
    cp -r $MOUNT_POINT/* $MOSQUITTO_DATA_DIR/
fi

exec /usr/sbin/mosquitto -c /mosquitto/config/mosquitto.conf  # Adjust path if needed
