FROM eclipse-mosquitto:latest  # Or a specific version if preferred

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
