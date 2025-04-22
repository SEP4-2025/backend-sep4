FROM ansi/mosquitto
USER root

ARG GCSFUSE_VERSION=2.4.2
ARG ARCH=amd64
RUN mkdir /download
RUN wget "https://github.com/googlecloudplatform/gcsfuse/releases/download/v${GCSFUSE_VERSION}/gcsfuse_${GCSFUSE_VERSION}_${ARCH}.deb" -O /download/gcsfuse.deb
RUN dpkg -i /download/gcsfuse.deb && rm /download/gcsfuse.deb

RUN mkdir /mnt/data

COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]
