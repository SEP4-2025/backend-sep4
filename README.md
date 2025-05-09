Coverage-badge-placeholder

# GrowMate Backend

The GrowMate backend is a .NET 8-based microservice architecture that powers the GrowMate plant monitoring and management system. It enables data collection from sensors, data processing, and provides a REST API for client applications.

## Project Overview

The backend is structured as a collection of microservices:

- **WebAPI**: REST API service exposing endpoints for client applications to access plant data, sensor readings, and issue commands
- **ReceiverService**: Service that receives and processes MQTT messages from IoT devices and sensors
- **SenderService**: Service for sending commands to IoT devices through MQTT
- **Database**: PostgreSQL database service with Entity Framework Core for data persistence

## API Endpoints

The GrowMate backend provides the following RESTful API endpoints:

### Authentication

| Method | Endpoint                | Description                      |
| ------ | ----------------------- | -------------------------------- |
| POST   | `/auth/login`           | Login with username and password |
| PATCH  | `/auth/change-password` | Change user password             |

### Notification

| Method | Endpoint                | Description                   |
| ------ | ----------------------- | ----------------------------- |
| POST   | `/notification/trigger` | Send notification to frontend |

### Gardener

| Method | Endpoint         | Description           |
| ------ | ---------------- | --------------------- |
| GET    | `/gardener`      | Get all gardeners     |
| GET    | `/gardener/{id}` | Get gardener by ID    |
| POST   | `/gardener`      | Create a new gardener |
| PATCH  | `/gardener`      | Update a gardener     |
| DELETE | `/gardener/{id}` | Delete a gardener     |

### Greenhouse

| Method | Endpoint                            | Description                   |
| ------ | ----------------------------------- | ----------------------------- |
| GET    | `/greenhouse/{id}`                  | Get greenhouse by ID          |
| GET    | `/greenhouse/gardener/{gardenerId}` | Get greenhouse by gardener ID |
| GET    | `/greenhouse/name/{name}`           | Get greenhouse by name        |
| POST   | `/greenhouse`                       | Create a new greenhouse       |
| PUT    | `/greenhouse`                       | Update greenhouse name        |
| DELETE | `/greenhouse/{id}`                  | Delete a greenhouse           |

### Plant

| Method | Endpoint           | Description        |
| ------ | ------------------ | ------------------ |
| GET    | `/plant/{plantId}` | Get plant by ID    |
| POST   | `/plant`           | Create a new plant |
| PUT    | `/plant`           | Update plant name  |
| DELETE | `/plant/{id}`      | Delete a plant     |

### Picture

| Method | Endpoint             | Description              |
| ------ | -------------------- | ------------------------ |
| GET    | `/picture/{plantId}` | Get pictures by plant ID |
| POST   | `/picture`           | Add a new picture        |
| PUT    | `/picture`           | Update picture note      |
| DELETE | `/picture/{id}`      | Delete a picture         |

### Sensor

| Method | Endpoint       | Description         |
| ------ | -------------- | ------------------- |
| GET    | `/sensor`      | Get all sensors     |
| GET    | `/sensor/{id}` | Get sensor by ID    |
| POST   | `/sensor`      | Create a new sensor |
| PATCH  | `/sensor`      | Update a sensor     |
| DELETE | `/sensor/{id}` | Delete a sensor     |

### Sensor Reading

| Method | Endpoint                           | Description               |
| ------ | ---------------------------------- | ------------------------- |
| GET    | `/sensorreading`                   | Get all sensor readings   |
| GET    | `/sensorreading/{id}`              | Get sensor reading by ID  |
| GET    | `/sensorreading/sensor/{sensorId}` | Get readings by sensor ID |
| GET    | `/sensorreading/date/{date}`       | Get readings by date      |
| POST   | `/sensorreading`                   | Add a new sensor reading  |
| DELETE | `/sensorreading/{id}`              | Delete a sensor reading   |

### Water Pump

| Method | Endpoint                          | Description                 |
| ------ | --------------------------------- | --------------------------- |
| GET    | `/waterpump`                      | Get all water pumps         |
| GET    | `/waterpump/{id}`                 | Get water pump by ID        |
| POST   | `/waterpump`                      | Create a new water pump     |
| PATCH  | `/waterpump/{id}/auto-watering`   | Update auto watering status |
| PATCH  | `/waterpump/{id}/manual-watering` | Trigger manual watering     |
| PATCH  | `/waterpump/{id}/add-water`       | Update current water level  |
| PATCH  | `/waterpump/{id}/threshold`       | Update threshold value      |
| DELETE | `/waterpump/{id}`                 | Delete a water pump         |

### Prediction

| Method | Endpoint                  | Description             |
| ------ | ------------------------- | ----------------------- |
| GET    | `/prediction`             | Get all predictions     |
| GET    | `/prediction/{id}`        | Get prediction by ID    |
| GET    | `/prediction/date/{date}` | Get predictions by date |
| POST   | `/prediction`             | Add a new prediction    |
| DELETE | `/prediction/{id}`        | Delete a prediction     |

### Log

| Method | Endpoint           | Description      |
| ------ | ------------------ | ---------------- |
| GET    | `/log`             | Get all logs     |
| GET    | `/log/date/{date}` | Get logs by date |
| DELETE | `/log/{id}`        | Delete a log     |

## Project Structure

The solution is organized into several projects:

### Server

- **Entities**: Contains data models like Plant, Sensor, Greenhouse, etc.
- **LogicInterfaces**: Defines interfaces for business logic operations
- **LogicImplements**: Implements the business logic interfaces
- **WebAPI**: REST API for client applications with controllers and endpoints
- **ReceiverService**: MQTT client service for receiving sensor data
- **SenderService**: MQTT client service for sending commands to devices

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
