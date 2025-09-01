# Jobs Processor API

A **background jobs processing API** built with **.NET 9**, **MongoDB**, and **RabbitMQ**, supporting asynchronous task processing, retries, and observability via **OpenTelemetry**.

---

## Features

* Exposes an API to **create jobs** with type, content, sender, and receiver.
* Stores jobs in **MongoDB** for persistence.
* Processes jobs asynchronously using **RabbitMQ** and **MassTransit**.
* Supports **job status tracking**: `Pending`, `Processing`, `Completed`, `Failed`.
* Handles **retry for transient errors**.
* Observability using **OpenTelemetry** (traces and logs).
* Fully containerized via **Docker Compose**.

---

## Tech Stack

| Component             | Technology              |
|-----------------------|-------------------------|
| API Framework         | .NET 9 / ASP.NET Core   |
| Message Broker        | RabbitMQ                |
| Database              | MongoDB (NoSQL)         |
| Background Processing | MassTransit             |
| Observability         | OpenTelemetry (OTLP)    |
| Containerization      | Docker / Docker Compose |

---

## Environment Variables

Create a `.env` file based on the `.env.example`:

```bash
cp .env.example .env
```

```env
# Message Broker
DATABASE__MONGO__CONNECTION_URI="mongodb://admin:examplepassword@mongodb:27017/"
DATABASE__MONGO__DATABASE_NAME=""
DATABASE__MONGO__TABLE__JOBS_COLLECTION_NAME=""

# Message Broker
MESSAGE_BROKER__HOST="amqp://rabbit-mq:5672"
MESSAGE_BROKER__USERNAME="admin"
MESSAGE_BROKER__PASSWORD="examplepassword"

# OpenTelemetry
OTEL_SERVICE_NAME="F360.JobsProcessor.API"
OTEL_EXPORTER_OTLP_ENDPOINT="http://aspire-dashboard:4318"
```

> ⚠ Use **service names from Docker Compose**, not `localhost`, for container-to-container connections.

---

## Running the Application

All services are configured with Docker Compose. The setup includes:

* API container
* RabbitMQ container
* MongoDB
* Mongo Express
* OpenTelemetry collector

---

### Step 1: Start services

```bash
docker-compose up -d
```

This will:

* Start the **MongoDB** with initialization scripts.
* Start **RabbitMQ** with default user credentials.
* Start the **API** container.
* Start the **OTLP collector** for telemetry.

---

### Step 2: Verify services

* RabbitMQ Management UI: [http://localhost:15672](http://localhost:15672)

    * Username: `admin`
    * Password: `examplepassword`

* Mongo Express (optional, for DB viewing): [http://localhost:8887](http://localhost:8887)

    * Username: `admin`
    * Password: `pass`

---

### Step 3: Create a Job

Send a POST request to the API endpoint:

```http
POST /api/jobs
Content-Type: application/json

{
  "type": "SendEmail",
  "sender": "from@example.com",
  "to": "to@example.com",
  "content": "This is a test job"
}
```

* The job will be stored in MongoDB.
* A consumer will pick up the job from RabbitMQ for processing.
* Job status updates will be persisted in MongoDB (`Pending → Processing → Completed`).

If you want to see all the endpoints access this route:
> `http://localhost:5079/scalar`

---

### Step 4: Create a Job

Send a GET request to the API endpoint:

```http
GET /api/jobs/{jobId}
```

If you want to see all the endpoints access this route:
> `http://localhost:5079/scalar`

---

### Step 5: Monitoring

* OpenTelemetry traces and logs are sent to the OTLP collector.
* Retry and error messages can be seen in RabbitMQ’s `_error` queue.
* You can observe MongoDB collections via Mongo Express.

---

## Notes

* The **API** and **workers** are already configured to handle **concurrent job processing**.
* Retry policies are defined in MassTransit, with logs for failures.

---

### Step 6: Stop services

```bash
docker-compose down
```

This will stop and remove all containers while keeping volumes for MongoDB and RabbitMQ intact.

## Load Testing the Jobs Endpoint

You can test how the API handles concurrent job creation using **k6**.

### Step 1: Install k6

* macOS:

```bash
brew install k6
```

* Windows (via Chocolatey):

```bash
choco install k6
```

* Linux:

```bash
sudo apt install k6
```

Or check the [official docs](https://k6.io/docs/getting-started/installation/) for other options.

### Step 2: Run the load test

```bash
k6 run Tests/create_jobs_test.js
```

* This will simulate **10 virtual users** sending job requests for **30 seconds**.
* You will see real-time metrics for requests per second, response time, and error rate.

---