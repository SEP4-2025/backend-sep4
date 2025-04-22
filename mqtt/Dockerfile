FROM ansi/mosquitto
USER root

ARG GCSFUSE_VERSION=2.4.2
ARG ARCH=amd64
RUN apt-get update && apt-get install gcsfuse

RUN mkdir /mnt/data

COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]
