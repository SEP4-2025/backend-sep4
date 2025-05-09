Coverage-badge-placeholder
# GrowMate Backend 
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
The GrowMate backend is a .NET 8-based microservice architecture that powers the GrowMate plant monitoring and management system. It enables data collection from sensors, data processing, and provides a REST API for client applications.
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
## Project Overview
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
The backend is structured as a collection of microservices:
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
- **WebAPI**: REST API service exposing endpoints for client applications to access plant data, sensor readings, and issue commands
- **ReceiverService**: Service that receives and processes MQTT messages from IoT devices and sensors
- **SenderService**: Service for sending commands to IoT devices through MQTT
- **Database**: PostgreSQL database service with Entity Framework Core for data persistence
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
## API Endpoints
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
The GrowMate backend provides the following RESTful API endpoints:
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
### Authentication
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
| Method | Endpoint                | Description                      |
| ------ | ----------------------- | -------------------------------- |
| POST   | `/auth/login`           | Login with username and password |
| PATCH  | `/auth/change-password` | Change user password             |
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
### Notification
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
| Method | Endpoint                | Description                      |
| ------ | ----------------------- | -------------------------------- |
| POST   | `/notification/trigger` | Send notification to frontend    |
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
### Gardener
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
| Method | Endpoint         | Description           |
| ------ | ---------------- | --------------------- |
| GET    | `/gardener`      | Get all gardeners     |
| GET    | `/gardener/{id}` | Get gardener by ID    |
| POST   | `/gardener`      | Create a new gardener |
| PATCH  | `/gardener`      | Update a gardener     |
| DELETE | `/gardener/{id}` | Delete a gardener     |
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
### Greenhouse
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
| Method | Endpoint                            | Description                   |
| ------ | ----------------------------------- | ----------------------------- |
| GET    | `/greenhouse/{id}`                  | Get greenhouse by ID          |
| GET    | `/greenhouse/gardener/{gardenerId}` | Get greenhouse by gardener ID |
| GET    | `/greenhouse/name/{name}`           | Get greenhouse by name        |
| POST   | `/greenhouse`                       | Create a new greenhouse       |
| PUT    | `/greenhouse`                       | Update greenhouse name        |
| DELETE | `/greenhouse/{id}`                  | Delete a greenhouse           |
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
### Plant
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
| Method | Endpoint           | Description        |
| ------ | ------------------ | ------------------ |
| GET    | `/plant/{plantId}` | Get plant by ID    |
| POST   | `/plant`           | Create a new plant |
| PUT    | `/plant`           | Update plant name  |
| DELETE | `/plant/{id}`      | Delete a plant     |
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
### Picture
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
| Method | Endpoint             | Description              |
| ------ | -------------------- | ------------------------ |
| GET    | `/picture/{plantId}` | Get pictures by plant ID |
| POST   | `/picture`           | Add a new picture        |
| PUT    | `/picture`           | Update picture note      |
| DELETE | `/picture/{id}`      | Delete a picture         |
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
### Sensor
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
| Method | Endpoint       | Description         |
| ------ | -------------- | ------------------- |
| GET    | `/sensor`      | Get all sensors     |
| GET    | `/sensor/{id}` | Get sensor by ID    |
| POST   | `/sensor`      | Create a new sensor |
| PATCH  | `/sensor`      | Update a sensor     |
| DELETE | `/sensor/{id}` | Delete a sensor     |
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
### Sensor Reading
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
| Method | Endpoint                           | Description               |
| ------ | ---------------------------------- | ------------------------- |
| GET    | `/sensorreading`                   | Get all sensor readings   |
| GET    | `/sensorreading/{id}`              | Get sensor reading by ID  |
| GET    | `/sensorreading/sensor/{sensorId}` | Get readings by sensor ID |
| GET    | `/sensorreading/date/{date}`       | Get readings by date      |
| POST   | `/sensorreading`                   | Add a new sensor reading  |
| DELETE | `/sensorreading/{id}`              | Delete a sensor reading   |
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
### Water Pump
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
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
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
### Prediction
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
| Method | Endpoint                  | Description             |
| ------ | ------------------------- | ----------------------- |
| GET    | `/prediction`             | Get all predictions     |
| GET    | `/prediction/{id}`        | Get prediction by ID    |
| GET    | `/prediction/date/{date}` | Get predictions by date |
| POST   | `/prediction`             | Add a new prediction    |
| DELETE | `/prediction/{id}`        | Delete a prediction     |
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
### Log
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
| Method | Endpoint           | Description      |
| ------ | ------------------ | ---------------- |
| GET    | `/log`             | Get all logs     |
| GET    | `/log/date/{date}` | Get logs by date |
| DELETE | `/log/{id}`        | Delete a log     |
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
## Project Structure
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
The solution is organized into several projects:
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
### Server
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
- **Entities**: Contains data models like Plant, Sensor, Greenhouse, etc.
- **LogicInterfaces**: Defines interfaces for business logic operations
- **LogicImplements**: Implements the business logic interfaces
- **WebAPI**: REST API for client applications with controllers and endpoints
- **ReceiverService**: MQTT client service for receiving sensor data
- **SenderService**: MQTT client service for sending commands to devices
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
### Shared
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
- **Database**: Database context and configuration using Entity Framework Core
- **DTOs**: Data Transfer Objects for API communication
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
## Technologies
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
- .NET 8
- Entity Framework Core
- MQTT for IoT communication (Eclipse Mosquitto)
- PostgreSQL for data storage
- Docker for containerization
- GitHub Actions for CI/CD
- Google Cloud Run for deployment
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
## Local Development
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
The project includes Docker containerization for local development and testing:
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
```bash
# Navigate to the code directory
cd backend/code
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
# Start the Docker containers
docker-compose up
```
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
This will start:
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
1. A Mosquitto MQTT broker
2. The WebAPI service
3. The ReceiverService for processing MQTT messages
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
## CI/CD Pipeline
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
### GitHub Actions
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
The project uses GitHub Actions for continuous integration:
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
1. **C#-linter**: Verifies code style and formatting
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
   - Runs `dotnet format` to ensure code meets style guidelines
   - Triggers on PRs to master branch
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
2. **Dockerization validation & build test**:
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
   - Tests Docker builds for all services
   - Verifies container functionality
   - Validates cloudbuild.yaml structure
   - Triggers on PRs to master branch
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
3. **Trigger Cloud Build**:
   - Automatically triggers Google Cloud Build pipeline
   - Authenticates with Google Cloud Platform using service account
   - Submits build configuration to GCP with commit SHA
   - Runs on both PR creation and merges to master branch
   - Can also be triggered manually via workflow_dispatch
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
### Google Cloud Build
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
The project uses Google Cloud Build for continuous deployment:
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
- The `cloudbuild.yaml` file defines the build and deployment pipeline
- Builds Docker images for WebAPI, ReceiverService, and MQTT broker
- Pushes images to Google Container Registry
- Deploys services to Google Cloud Run in the europe-north2 region
- Sets up necessary networking and permissions
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
## Deployment
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
When code is merged to the master branch, it is automatically deployed using the Google Cloud Build pipeline defined in `cloudbuild.yaml`. The services are deployed as independent containers on Google Cloud Run, providing scalability and reliability.
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
## Project Dependencies
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
The GrowMate backend follows a layered architecture with clear dependencies between projects:
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
```
WebAPI
├── LogicInterfaces
├── LogicImplements
├── DTOs
└── Database
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
ReceiverService
├── LogicInterfaces
├── LogicImplements
├── DTOs
└── Database
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
LogicImplements
├── LogicInterfaces
├── DTOs
└── Database
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
LogicInterfaces
├── Entities
└── DTOs
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
Database
└── Entities
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
DTOs
└── (No dependencies)
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
Entities
└── (No dependencies)
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
SenderService
└── (Only external MQTTnet dependency)
```
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
### Key Dependencies
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
1. **WebAPI** depends on:
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
   - LogicInterfaces (for business logic contracts)
   - LogicImplements (for business logic implementation)
   - Database (for data access)
   - DTOs (for data transfer objects)
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
2. **ReceiverService** depends on:
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
   - LogicInterfaces
   - LogicImplements
   - Database
   - DTOs
   - External MQTT library for IoT communication
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
3. **LogicImplements** depends on:
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
   - LogicInterfaces (implements these interfaces)
   - Database (for data access)
   - DTOs (for data transformation)
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
4. **LogicInterfaces** depends on:
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
   - Entities (for domain models)
   - DTOs (for data transfer contracts)
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
5. **Database** depends on:
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
   - Entities (for domain models to persist)
   - Entity Framework Core (external)
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
6. **SenderService** is relatively independent and primarily depends on the external MQTT library.
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
This layered architecture facilitates separation of concerns and allows for easier testing and maintenance.
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
## License
![Coverage](https://img.shields.io/badge/coverage-0.0%25-red)
This project is licensed under the MIT License - see the LICENSE file for details.
