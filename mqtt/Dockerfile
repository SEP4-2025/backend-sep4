FROM eclipse-mosquitto:2.0.21

RUN apk update && apk add --no-cache fuse gcsfuse

RUN mkdir /mnt/data

COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]

