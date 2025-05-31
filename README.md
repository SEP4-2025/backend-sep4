# GrowMate Backend 

The GrowMate backend is a .NET 8-based microservice architecture that powers the GrowMate plant monitoring and management system. It enables data collection from sensors, data processing, and provides a REST API for client applications.

## Project Overview

The backend is structured as a collection of microservices:

- **WebAPI**: REST API service exposing endpoints for client applications to access plant data, sensor readings, and issue commands
- **ReceiverService**: Service that receives and processes MQTT messages from IoT devices and sensors
- **SenderService**: Service for sending commands to IoT devices through MQTT
- **Database**: PostgreSQL database service with Entity Framework Core for data persistence

## API Endpoints

### AuthController

| Method | Route                  | Description                  |
| ------ | ---------------------- | ---------------------------- |
| POST   | /Auth/login            | Login with username/password |
| PATCH  | /Auth/change-password  | Change user password         |
| POST   | /Auth/confirm-password | Confirm password             |

---

### GardenerController

| Method | Route                 | Description           |
| ------ | --------------------- | --------------------- |
| GET    | /Gardener             | Get all gardeners     |
| GET    | /Gardener/{id}        | Get gardener by ID    |
| POST   | /Gardener             | Create a new gardener |
| PATCH  | /Gardener/update/{id} | Update a gardener     |
| DELETE | /Gardener/{id}        | Delete a gardener     |

---

### GreenhouseController

| Method | Route                             | Description                   |
| ------ | --------------------------------- | ----------------------------- |
| GET    | /Greenhouse                       | Get all greenhouses           |
| GET    | /Greenhouse/{id}                  | Get greenhouse by ID          |
| GET    | /Greenhouse/gardener/{gardenerId} | Get greenhouse by gardener ID |
| GET    | /Greenhouse/name/{name}           | Get greenhouse by name        |
| POST   | /Greenhouse                       | Create a new greenhouse       |
| PUT    | /Greenhouse/update/{id}           | Update greenhouse name        |
| DELETE | /Greenhouse/{id}                  | Delete a greenhouse           |

---

### LogController

| Method | Route                 | Description                                    |
| ------ | --------------------- | ---------------------------------------------- |
| GET    | /Log                  | Get all logs                                   |
| GET    | /Log/date/{date}      | Get logs by date                               |
| GET    | /Log/{id}/water-usage | Get water usage for last 5 days for greenhouse |
| DELETE | /Log/{id}             | Delete a log                                   |

---

### NotificationController

| Method | Route                            | Description                 |
| ------ | -------------------------------- | --------------------------- |
| POST   | /Notification/trigger            | Trigger/send a notification |
| GET    | /Notification/all                | Get all notifications       |
| GET    | /Notification/byType?type={type} | Get notification by type    |

---

### NotificationPrefController

| Method | Route                          | Description                          |
| ------ | ------------------------------ | ------------------------------------ |
| GET    | /NotificationPref              | Get all notification preferences     |
| PATCH  | /NotificationPref/toggle       | Toggle notification preference       |
| GET    | /NotificationPref/{gardenerId} | Get notification prefs by gardenerId |

---

### PictureController

| Method | Route                  | Description                      |
| ------ | ---------------------- | -------------------------------- |
| POST   | /Picture/UploadPicture | Upload a new picture (multipart) |
| GET    | /Picture/{plantId}     | Get pictures by plant ID         |
| PUT    | /Picture               | Update picture note              |
| DELETE | /Picture/{Id}          | Delete a picture                 |

---

### PlantController

| Method | Route            | Description        |
| ------ | ---------------- | ------------------ |
| GET    | /Plant           | Get all plants     |
| GET    | /Plant/{plantId} | Get plant by ID    |
| POST   | /Plant           | Create a new plant |
| PUT    | /Plant/{id}      | Update plant name  |
| DELETE | /Plant/{id}      | Delete a plant     |

---

### PredictionController

| Method | Route                   | Description             |
| ------ | ----------------------- | ----------------------- |
| GET    | /Prediction             | Get all predictions     |
| GET    | /Prediction/{id}        | Get prediction by ID    |
| GET    | /Prediction/date/{date} | Get predictions by date |
| POST   | /Prediction             | Add a new prediction    |
| DELETE | /Prediction/{id}        | Delete a prediction     |

---

### SensorController

| Method | Route                  | Description                   |
| ------ | ---------------------- | ----------------------------- |
| GET    | /Sensor                | Get all sensors               |
| GET    | /Sensor/{id}           | Get sensor by ID              |
| POST   | /Sensor                | Create a new sensor           |
| PATCH  | /Sensor/update/{id}    | Update a sensor               |
| DELETE | /Sensor/{id}           | Delete a sensor               |
| PATCH  | /Sensor/{id}/threshold | Update sensor threshold value |

---

### SensorReadingController

| Method | Route                                             | Description               |
| ------ | ------------------------------------------------- | ------------------------- |
| GET    | /SensorReading                                    | Get all sensor readings   |
| GET    | /SensorReading/{id}                               | Get sensor reading by ID  |
| GET    | /SensorReading/sensor/{sensorId}                  | Get readings by sensor ID |
| GET    | /SensorReading/date/{date}                        | Get readings by date      |
| POST   | /SensorReading                                    | Add a new sensor reading  |
| DELETE | /SensorReading/{id}                               | Delete a sensor reading   |
| GET    | /SensorReading/average/{greenhouseId}/last24hours | Get avg readings last 24h |
| GET    | /SensorReading/average/{greenhouseId}/last7days   | Get avg readings last 7d  |
| GET    | /SensorReading/average/{greenhouseId}/last30days  | Get avg readings last 30d |

---

### WaterPumpController

| Method | Route                             | Description                  |
| ------ | --------------------------------- | ---------------------------- |
| GET    | /WaterPump                        | Get all water pumps          |
| GET    | /WaterPump/{id}                   | Get water pump by ID         |
| GET    | /WaterPump/{id}/water-level       | Get water pump's water level |
| POST   | /WaterPump                        | Create a new water pump      |
| PATCH  | /WaterPump/{id}/toggle-automation | Toggle automation status     |
| PATCH  | /WaterPump/{id}/manual-watering   | Trigger manual watering      |
| PATCH  | /WaterPump/{id}/add-water         | Update current water level   |
| PATCH  | /WaterPump/{id}/threshold         | Update threshold value       |
| PATCH  | /WaterPump/{id}/capacity          | Update water tank capacity   |
| DELETE | /WaterPump/{id}                   | Delete a water pump          |

## Project Structure

The solution is organized into several projects:

### Server

- **Entities**: Contains data models like Plant, Sensor, Greenhouse, etc.
- **LogicInterfaces**: Defines interfaces for business logic operations
- **LogicImplements**: Implements the business logic interfaces
- **WebAPI**: REST API for client applications with controllers and endpoints
- **ReceiverService**: MQTT client service for receiving sensor data and sending pump commands

### Shared

- **Database**: Database context and configuration using Entity Framework Core
- **DTOs**: Data Transfer Objects for API communication

## Technologies

- .NET 8
- Entity Framework Core
- MQTT for IoT communication (Eclipse Mosquitto)
- PostgreSQL for data storage
- Docker for containerization
- GitHub Actions for CI/CD
- Google Cloud Run for deployment

## Local Development

The project includes Docker containerization for local development and testing:

```bash
# Navigate to the code directory
cd backend/code

# Start the Docker containers
docker-compose up
```

This will start:

1. A Mosquitto MQTT broker
2. The WebAPI service
3. The ReceiverService for processing MQTT messages

## CI/CD Pipeline

### GitHub Actions

The project uses GitHub Actions for continuous integration:

1. **C#-linter**: Verifies code style and formatting

   - Runs `dotnet format` to ensure code meets style guidelines
   - Triggers on PRs to master branch

2. **Dockerization validation & build test**:

   - Tests Docker builds for all services
   - Verifies container functionality
   - Validates cloudbuild.yaml structure
   - Triggers on PRs to master branch

3. **Trigger Cloud Build**:
   - Automatically triggers Google Cloud Build pipeline
   - Authenticates with Google Cloud Platform using service account
   - Submits build configuration to GCP with commit SHA
   - Runs on both PR creation and merges to master branch
   - Can also be triggered manually via workflow_dispatch

### Google Cloud Build

The project uses Google Cloud Build for continuous deployment:

- The `cloudbuild.yaml` file defines the build and deployment pipeline
- Builds Docker images for WebAPI, ReceiverService, and MQTT broker
- Pushes images to Google Container Registry
- Deploys services to Google Cloud Run in the europe-north2 region
- Sets up necessary networking and permissions

## Deployment

When code is merged to the master branch, it is automatically deployed using the Google Cloud Build pipeline defined in `cloudbuild.yaml`. The services are deployed as independent containers on Google Cloud Run, providing scalability and reliability.

## Project Dependencies

The GrowMate backend follows a layered architecture with clear dependencies between projects:

```
WebAPI
├── LogicInterfaces
├── LogicImplements
├── DTOs
└── Database

ReceiverService
├── LogicInterfaces
├── LogicImplements
├── DTOs
└── Database

LogicImplements
├── LogicInterfaces
├── DTOs
└── Database

LogicInterfaces
├── Entities
└── DTOs

Database
└── Entities

DTOs
└── (No dependencies)

Entities
└── (No dependencies)

SenderService
└── (Only external MQTTnet dependency)
```

### Key Dependencies

1. **WebAPI** depends on:

   - LogicInterfaces (for business logic contracts)
   - LogicImplements (for business logic implementation)
   - Database (for data access)
   - DTOs (for data transfer objects)

2. **ReceiverService** depends on:

   - LogicInterfaces
   - LogicImplements
   - Database
   - DTOs
   - External MQTT library for IoT communication

3. **LogicImplements** depends on:

   - LogicInterfaces (implements these interfaces)
   - Database (for data access)
   - DTOs (for data transformation)

4. **LogicInterfaces** depends on:

   - Entities (for domain models)
   - DTOs (for data transfer contracts)

5. **Database** depends on:

   - Entities (for domain models to persist)
   - Entity Framework Core (external)

6. **SenderService** is relatively independent and primarily depends on the external MQTT library.

This layered architecture facilitates separation of concerns and allows for easier testing and maintenance.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
