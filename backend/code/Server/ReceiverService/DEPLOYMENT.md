# SensorReceiverService Deployment Guide

This document provides instructions for deploying the SensorReceiverService to Google Cloud Platform using either Google Compute Engine (VM) or Cloud Run.

## Prerequisites

1. [Google Cloud SDK](https://cloud.google.com/sdk/docs/install) installed and configured
2. [Docker](https://docs.docker.com/get-docker/) installed
3. Access to the GrowMate Google Cloud project
4. .NET 8.0 SDK installed (for local development)

## Overview

The SensorReceiverService:

- Connects to an MQTT broker at 34.27.128.90:1883
- Uses a PostgreSQL database at 35.228.249.205
- Receives sensor data and performs processing
- Makes API calls to the WebAPI at https://webapi-service-68779328892.europe-north2.run.app/notification/trigger

## Deployment Options

### Option 1: Deploy to Google Compute Engine VM (Recommended for MQTT connectivity)

This option creates a VM instance that runs the service in a Docker container.

1. Make any necessary configurations in `appsettings.json`
2. Edit the `deploy-to-gcp-vm.sh` script to update:

   - PROJECT_ID with your GCP project ID
   - ZONE with your preferred zone
   - MACHINE_TYPE based on your needs

3. Run the deployment script:

   ```bash
   chmod +x deploy-to-gcp-vm.sh
   ./deploy-to-gcp-vm.sh
   ```

4. After deployment, the service will be running on the VM and will automatically restart if the VM reboots.

### Option 2: Deploy using Docker Compose

For local testing or for deployment to a VM manually:

1. Navigate to the ReceiverService directory:

   ```bash
   cd /Users/maciej/growmate/backend-sep4/backend/code/Server/ReceiverService
   ```

2. Run with Docker Compose:

   ```bash
   docker-compose up -d
   ```

3. Check service logs:
   ```bash
   docker-compose logs -f
   ```

### Option 3: Deploy to Cloud Run

Cloud Run is a managed service, but note that MQTT connection may be limited by Cloud Run's stateless nature.

1. Update `deploy-to-cloud-run.sh` with your project ID and region.
2. Run the deployment script:
   ```bash
   chmod +x deploy-to-cloud-run.sh
   ./deploy-to-cloud-run.sh
   ```

## Configuration

The service can be configured using environment variables or `appsettings.json`. The key configuration points are:

- `MqttSettings:Server`: MQTT broker address
- `MqttSettings:Port`: MQTT broker port
- `ConnectionStrings:DefaultConnection`: Database connection string
- `CloudWebApiEndpoint`: Web API endpoint for notifications

## Monitoring

- Health checks are available at `/health`
- Cloud Logging will capture all service logs
- Set up Cloud Monitoring alerts for service availability

## Network Requirements

- Ensure firewall rules allow the VM to connect to:
  - MQTT broker (34.27.128.90:1883)
  - PostgreSQL database (35.228.249.205:5432)
  - WebAPI endpoint (https://webapi-service-68779328892.europe-north2.run.app)

## Troubleshooting

1. Check service logs:

   ```bash
   gcloud compute ssh [VM_NAME] --zone=[ZONE] --command="sudo docker logs sensor-receiver"
   ```

2. Verify connectivity to MQTT broker:

   ```bash
   gcloud compute ssh [VM_NAME] --zone=[ZONE] --command="telnet 34.27.128.90 1883"
   ```

3. Test database connectivity:
   ```bash
   gcloud compute ssh [VM_NAME] --zone=[ZONE] --command="sudo docker run --rm postgres:latest psql 'postgresql://postgres:postgres@35.228.249.205:5432/GrowMate' -c 'SELECT 1'"
   ```
