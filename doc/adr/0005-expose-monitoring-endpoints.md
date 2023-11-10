# 5. Expose monitoring endpoints

Date: 2023-11-10

## Status

Status: Accepted on 2023-11-10


## Context

When deploying and monitoring the application it is common practice to expose a health-endpoint with a simple response indicating the health of the application. In fact, several monitoring endpoints are quite common:

| Endpoint | Description | Notes |
| --- | --- | --- |
| /healthz | A simple response indicating the health of the application, 200 OK if healthy, 500 otherwise | |
| /readyz | Indicating the readiness of the application, intended for load-balancers and orchestrators |  |
| /version | The version of the application |  |
| /metrics | Metrics of the application, usually in prometheus format | |
| /statusz | A more detailed status of the application, including the health of the application and other components it depends on | |
| /debugz | A debug endpoint, usually with a lot of information about the application, including the health of the application and other components it depends on. | not in production |
| /info | Information about the application, usually in json format | not in production|
| /configz | Configuration of the application, usually in json format. | sensitive |
| /envz | Environment variables of the application, usually in json format. | sensitive |

## Decision

The minimum of monitoring endpoints, and ones that are not security-sensitive to expose are:

| Endpoint | Description | Notes |
| --- | --- | --- |
| /healthz | A simple response indicating the health of the application, 200 OK if healthy, 500 otherwise | Use HealthCheck |
| /readyz | Indicating the readiness of the application, intended for load-balancers and orchestrators | Use HealthCheck |
| /version | The version of the application | We will use the Assembly-version of the application, and depend on the version-setting strategy to set the version correctly for the docker-image -tag and the compiled application. |

The other endpoints are optional and can be added as needed.

## Consequences

We need to expose the three monitoring endpoints and implement the health-checks. We also need a versioning-strategy that sets the version of the application correctly in the docker-image -tag and the compiled application.
