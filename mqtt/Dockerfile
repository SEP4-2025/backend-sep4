FROM eclipse-mosquitto:2.0.21

RUN apt-get update && apt-get install -y --no-install-recommends \
    fuse \
    wget \
    gnupg

RUN export GCSFUSE_REPO=gcsfuse-`lsb_release -c -s`
RUN echo "deb http://packages.cloud.google.com/apt $GCSFUSE_REPO main" | tee /etc/apt/sources.list.d/gcsfuse.list
RUN curl https://packages.cloud.google.com/apt/doc/apt-key.gpg | apt-key add -

RUN apt-get update -y
RUN apt-get install -y gcsfuse

RUN mkdir /mnt/data

COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]
