
CREATE SCHEMA IF NOT EXISTS "GrowMate";
SET SCHEMA 'GrowMate';



CREATE TABLE "Gardener"
(
    id       SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    password VARCHAR(50) NOT NULL
);


CREATE TABLE "Greenhouse"
(
    id         SERIAL PRIMARY KEY,
    name       VARCHAR(50) NOT NULL,
    gardenerId INT         NOT NULL,
    FOREIGN KEY (gardenerId) REFERENCES "Gardener" (id) ON UPDATE CASCADE
);

CREATE TABLE "Plant"
(
    id           SERIAL PRIMARY KEY,
    name         VARCHAR(50) NOT NULL,
    species      VARCHAR(50) NOT NULL,
    greenhouseId INT         NOT NULL,
    FOREIGN KEY (greenhouseId) REFERENCES "Greenhouse" (id) ON UPDATE CASCADE
);

CREATE TABLE "Picture"
(
    id SERIAL PRIMARY KEY,
    date TIMESTAMP NOT NULL,
    note VARCHAR(1024) NOT NULL,
    url VARCHAR(1024) NOT NULL,
    plantId INT         NOT NULL,
    FOREIGN KEY (plantId) REFERENCES "Plant" (id) ON UPDATE CASCADE
);


CREATE TABLE "Sensor"
(
    id INT PRIMARY KEY,
    type VARCHAR(50) NOT NULL,
    metricUnit VARCHAR(50) NOT NULL,
    greenhouseId INT         NOT NULL,
    FOREIGN KEY (greenhouseId) REFERENCES "Greenhouse" (id) ON UPDATE CASCADE
);



CREATE TABLE "SensorReading"
(
    id SERIAL PRIMARY KEY,
    date TIMESTAMP NOT NULL,
    value INT NOT NULL,
    threshold INT NOT NULL,
    sensorId INT         NOT NULL,
    FOREIGN KEY (sensorId) REFERENCES "Sensor" (id) ON UPDATE CASCADE
);


CREATE TABLE "Prediction"
(
    id SERIAL PRIMARY KEY,
    optimalTemp INT,
    optimalLight INT,
    optimalHumidity INT,
    optimalWaterLevel INT,
    date TIMESTAMP NOT NULL,
    greenhouseId INT         NOT NULL,
    sensorReadingId INT         NOT NULL,
    FOREIGN KEY (greenhouseId) REFERENCES "Greenhouse" (id) ON UPDATE CASCADE,
    FOREIGN KEY (sensorReadingId) REFERENCES "SensorReading" (id) ON UPDATE CASCADE
);



CREATE TABLE "WaterPump"
(
    id SERIAL PRIMARY KEY,
    lastWatered TIMESTAMP NOT NULL,
    lastWaterAmount INT NOT NULL,
    autoWatering BOOLEAN NOT NULL,
    waterTankCapacity INT NOT NULL,
    currentWaterLevel INT NOT NULL,
    thresholdValue INT NOT NULL
);

CREATE TABLE "Log"
(
    id SERIAL PRIMARY KEY,
    date TIMESTAMP NOT NULL,
    message VARCHAR(1024) NOT NULL,
   sensorReadingId INT         NOT NULL,
   waterPumpId INT         NOT NULL,
    greenhouseId INT         NOT NULL,
    FOREIGN KEY (sensorReadingId) REFERENCES "SensorReading" (id) ON UPDATE CASCADE,
    FOREIGN KEY (waterPumpId) REFERENCES "WaterPump" (id) ON UPDATE CASCADE,
    FOREIGN KEY (greenhouseId) REFERENCES "Greenhouse" (id) ON UPDATE CASCADE

);


CREATE TABLE "Notification"
(
    id SERIAL PRIMARY KEY,
    type VARCHAR(50) NOT NULL,
    message VARCHAR(1024) NOT NULL,
    date TIMESTAMP NOT NULL,
    isRead BOOLEAN NOT NULL,
    waterPumpId INT         NOT NULL,
    sensorReadingId INT         NOT NULL,
    FOREIGN KEY (waterPumpId) REFERENCES "WaterPump" (id) ON UPDATE CASCADE,
    FOREIGN KEY (sensorReadingId) REFERENCES "SensorReading" (id) ON UPDATE CASCADE
);


CREATE TABLE "NotificationPreference"
(
    gardenerId INT PRIMARY KEY REFERENCES "Gardener" (id) ON UPDATE CASCADE,
    isEnabled BOOLEAN NOT NULL
);

INSERT INTO "Gardener" (username, password) VALUES
    ('test', 'test');


INSERT INTO "Greenhouse" (name, gardenerId) VALUES
    ('gh1', 1);


INSERT INTO "Plant" (name, species, greenhouseId) VALUES

    ('Basil', 'basil', 1);




INSERT INTO "Sensor" (id, type, metricUnit, greenhouseId) VALUES
                                                              (101, 'Temperature', '°C', 1),
                                                              (102, 'Humidity', '%', 1);


INSERT INTO "SensorReading" (date, value, threshold, sensorId) VALUES
                                                                  ('2025-04-20 08:00:00', 21, 24, 101),
                                                                  ('2025-04-20 09:00:00', 26, 24, 101),
                                                                  ('2025-04-20 10:00:00', 23, 24, 101),
                                                                  ('2025-04-20 08:00:00', 55, 60, 102),
                                                                  ('2025-04-20 09:00:00', 67, 60, 102);



