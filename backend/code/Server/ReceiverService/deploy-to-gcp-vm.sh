#!/bin/bash

# Deployment script for SensorReceiverService on Google Cloud VM

# Variables (replace these with your actual values)
PROJECT_ID="warm-rock-458712-a1"
VM_NAME="sensor-receiver-service-vm"
ZONE="europe-north2-a"  # Updated to use a specific zone within europe-north2 region
MACHINE_TYPE="e2-medium"  # Adjust based on your needs

# Create VM instance
echo "Creating VM instance $VM_NAME..."
gcloud compute instances create $VM_NAME \
  --project=$PROJECT_ID \
  --zone=$ZONE \
  --machine-type=$MACHINE_TYPE \
  --subnet=default \
  --network-tier=PREMIUM \
  --maintenance-policy=MIGRATE \
  --scopes=https://www.googleapis.com/auth/cloud-platform \
  --tags=http-server,https-server \
  --image=ubuntu-2004-focal-v20250508a \
  --image-project=ubuntu-os-cloud \
  --boot-disk-size=10GB \
  --boot-disk-type=pd-balanced \
  --boot-disk-device-name=$VM_NAME

# Wait for VM to be ready
sleep 30

# Install Docker on the VM
echo "Installing Docker on the VM..."
gcloud compute ssh $VM_NAME --zone=$ZONE --command='
sudo apt-get update && \
sudo apt-get install -y apt-transport-https ca-certificates curl gnupg lsb-release && \
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg && \
echo "deb [arch=amd64 signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null && \
sudo apt-get update && \
sudo apt-get install -y docker-ce docker-ce-cli containerd.io && \
sudo usermod -aG docker $USER
'

# Create an appsettings.json file with appropriate values
echo "Creating appsettings.json file..."
cat > appsettings.json << EOL
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  "ConnectionStrings": {
    "DefaultConnection": "Host=35.228.249.205;Port=5432;Database=GrowMate;Username=postgres;Password=postgres;SearchPath=GrowMate"
  },
  "AllowedHosts": "*",

  "WebApiEndpoint": "http://localhost:8080/notification/trigger",
  "CloudWebApiEndpoint": "https://webapi-service-68779328892.europe-north2.run.app/notification/trigger",
  
  "MqttSettings": {
    "Server": "34.27.128.90",
    "Port": 1883
  }
}
EOL

# Copy appsettings.json to VM
gcloud compute scp appsettings.json $VM_NAME:~/ --zone=$ZONE

# Transfer Dockerfile and needed files directly to VM and build there
echo "Transferring files and building Docker image on the VM..."

# Create a temporary directory for Docker build context
gcloud compute ssh $VM_NAME --zone=$ZONE --command="mkdir -p ~/sensor-receiver-build"

# Transfer the Dockerfile to the VM
gcloud compute scp /Users/maciej/growmate/backend-sep4/backend/code/Server/ReceiverService/Dockerfile $VM_NAME:~/sensor-receiver-build/ --zone=$ZONE

# Create an archive of the code directory and transfer it
echo "Creating code archive for transfer..."
cd /Users/maciej/growmate/backend-sep4/backend/code
# Use --exclude to ignore macOS metadata files
tar --exclude="._*" --exclude=".DS_Store" -czf /tmp/code.tar.gz .
gcloud compute scp /tmp/code.tar.gz $VM_NAME:~/sensor-receiver-build/ --zone=$ZONE

# Extract the code and build the Docker image on the VM
gcloud compute ssh $VM_NAME --zone=$ZONE --command="
cd ~/sensor-receiver-build && 
tar -xzf code.tar.gz &&
echo 'Cleaning up any macOS metadata files that might have been transferred...' &&
find . -name '._*' -type f -delete &&
sudo docker build -t sensor-receiver:latest -f ./Server/ReceiverService/Dockerfile ."
gcloud compute ssh $VM_NAME --zone=$ZONE --command="
# Remove any existing container with the same name
sudo docker rm -f sensor-receiver 2>/dev/null || true && \
sudo mkdir -p /app/config && \
sudo cp ~/appsettings.json /app/config/ && \
sudo docker run -d --name sensor-receiver \
  -p 80:80 \
  -v /app/config/appsettings.json:/app/appsettings.json \
  --restart=always \
  sensor-receiver:latest
"

echo "Deployment complete! The SensorReceiverService is now running on the VM."
echo "You can access the VM using: gcloud compute ssh $VM_NAME --zone=$ZONE"
