# Mini MES API

**Keywords**
- .NET
- Entity Framework
- LINQ
- SQL
- MinimalAPI
- Xunit
- OEE

**Current Test Coverage: 90%**

![image](https://github.com/user-attachments/assets/ebce0a84-7dc5-4ddf-9f7f-fdc43871623b)


# How to run

- Go to ![link](https://github.com/sebastianpiresmolin/Smart-Factory-Sim) and follow instructions

- Clone repository
- open the project and cd into `Mini_MES_API`
- `docker compose up --build` to build the docker image
- Make sure that Smart-Factory-Sim is running
- `docker compose up` to start all of the containers.
- Navigate to `http://localhost:5000/swagger` to interact with the API and see the API documentation. Or you can use any API agent of your choosing.


# What is Mini MES API?
Originally it was just supposed to be a tutorial project to teach myself how to work with MinimalAPI and Handlers. But at this time I was also diving deeper into ISA-95 and more specifically level three - Manufacturing Execution Systems (MES). So the result became a project that was both a platform to help me hone my API skills, but also shaped as a miniature MES-API. To make the MES-API have some sort of interactivity I let the SwaggerUI act as a small level 4 (Business Layer) by enabling POST/PUT requests.

**New: Factory Simulator Integration**

The project now includes integration with a C++ factory simulator that provides real-time data about machines, sensors, and production metrics via MQTT. This adds a significant level of realism to the MES by:

Providing real-time machine state monitoring (temperatures, vibration, operational status)
Tracking production and material loss metrics from simulated equipment
Publishing factory state snapshots through MQTT for consumption by the MES-API
Creating a more complete ISA-95 implementation with actual Level 1-2 data flowing up to Level 3
ISA-95-Model The ISA-95 Model showing the different layers. This API lives in the third level, receiving data from the factory simulation (Level 1-2) while also mimicking communication from the fourth level Source: ISA-95

*Features*

Work Order & Production Order Management
The Mini MES API provides CRUD operations on Work Orders and Production Orders. Work Orders are attached to Production Orders in a one-to-many-relationship, which means that Production Orders can have multiple Work Orders attached to them. This simulates how industries often queue multiple jobs (Work Orders) to the same production line (Production Order).

*Production Order Status Tracking*

The status of Production Orders can be updated through the API. In its current implementation, the status affects operations like deletion (you can't delete Production Orders that aren't Cancelled or a Draft). This status is also used for OEE calculations, which can only be performed on Production Orders with status "Completed".

*Factory Simulation Monitoring*

The newest feature integrates real-time factory floor simulation data:

Machine state monitoring (running/stopped)
Sensor data visualization (temperature, vibration)
Production metrics tracking (items produced, materials lost)
Live factory state snapshots via MQTT integration
OEE Calculation
For completed Production Orders, the API can calculate OEE (Overall Equipment Effectiveness), a key performance indicator in manufacturing that combines availability, performance, and quality metrics.

*Technical Implementation*

The project now incorporates a multi-container architecture:

.NET Minimal API for the MES functionality
C++ factory simulator generating realistic production data
MQTT broker (Mosquitto) for message passing between components
Node-RED for workflow visualization and testing
This setup provides a more comprehensive demonstration of modern manufacturing software architecture, closely mimicking the ISA-95 model with actual data flowing between layers.


*OEE Calculation*
Overall Equipment Effectiveness (OEE) is a metric that identifies the percentage of planned production time that is truly productive. An OEE score of 100% represents perfect production: manufacturing only good parts, as fast as possible, with no downtime. The Mini MES API has a feature to do this calculation, and also display each part calculation as well. These parts are: 
- **Availability** that takes into account *Availability Loss*, which includes all events that stop planned production for an appreciable length of time (typically several minutes or longer). Availability Loss includes Unplanned Stops (such as equipment failures and material shortages), and Planned Stops (such as changeover time). Although the Mini MES API only uses start, stop and expected time discrepancies for this calculation.
- **Quality** that takes into account *Quality Loss*, which factors out manufactured pieces that do not meet quality standards, including pieces that are later reworked.
- **Performance** that takes into account *Performance Loss*, which includes all factors that cause the production asset to operate at less than the maximum possible speed when running (including Slow Cycles and Small Stops). This calculation, in contrast to *Availability*, factors in the *Ideal Cycle Time* which basically means how many minutes it ideally should take to manufacture one unit instead of looking at the total expected time for the order.

Since we have have these three variables calculated we can now get the OEE-value of our Product Order!

![image](https://github.com/user-attachments/assets/b495a034-18f9-4b82-87e3-5eddec565093)

*Example response from the ../oee endpoint in Swagger*

# Example Scenario

**Create Production Order:**

POST /production-orders

`
{
  "productSKU": "string",
  "quantity": 2147483647,
  "startTime": "",
  "endTime": "",
  "plannedEndTime": "2025-04-10T07:04:09.321Z",
  "defectCount": 0,
  "idealCycleTimeMinutes": 0,
  "status": 0
}`


**Start the order:**

This will set the startTime to Now();

PUT /production-orders/{id}/start


`{}`


**Add Work Order**

POST /production-orders/{id}/instructions

`
{
  "stepName": "string",
  "description": "string",
  "durationInMinutes": 250
}`


**Complete Production Order:**

This will set endTime to Now();

PUT /production-orders/{id}/complete

`{}`
