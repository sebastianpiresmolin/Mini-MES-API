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

- Clone repository
- open the project and cd into `Mini_MES_API`
- `dotnet restore` to download all the NuGet packages
- `dotnet run` to start the application. This will update and seed a localdb Microsoft SQL Server. The connection string is already set in appsettings.json.
- navigate to `http://localhost:5195/swagger` to interact with the API and see the API documentation. Or you can use any API agent of your choosing.


# What is Mini MES API?

Originally it was just supposed to be a tutorial project to teach myself how to work with MinimalAPI and Handlers. But at this time I was also diving deeper into [ISA-95](https://www.isa.org/standards-and-publications/isa-standards/isa-95-standard) and more specifically level three - Manufacturing Execution Systems (MES). So the result became a project that was now both a platform to help me hone my API skills, but also shaped as a miniature MES-API. To make the MES-API have some sort of interactivity I let the SwaggerUI act as a small level 4 (Business Layer) by enabling POST/PUT requests.

![ISA-95-Model](https://github.com/user-attachments/assets/229f4401-9015-4783-890e-8572f4a6cdaf)
*The ISA-95 Modell showing the different layers. This API lives in the third level while mimicking some communication coming in from the forth level*
*Source:* [ISA-95](https://www.isa.org/standards-and-publications/isa-standards/isa-95-standard)

# Features

The Mini MES API has three features, two of these are rather plain and the first is the ability to perform CRUD operations on Work Orders and Production Orders. Work Orders are attached to Production Orders in a one-to-many-relationship, which means that Production Orders can have multiple Work Orders attached to them. This simulated how industries often queue multiple jobs (Work Orders) to the same production line (Production Order in Mini MES API).

The second of the rather basic features is the status of Production Orders which right now is set by the same CRUD I mentioned above, But I envision that the status would be part of an Event Driven design using a tool like MediatR in the future. In its current implementation it doesn't effect much other than that you can't delete Production Orders that isn't Cancelled or a Draft (To maintain documentation a soft deletion is preferred). But it is a part of the last feature which is a bit special and related to LEAN standards, namely *OEE* calculations which you can only do on Production Orders with status Completed in the API.

![image](https://github.com/user-attachments/assets/9d0b8a44-1a52-4e13-8307-7bd87e1caf6b)

*The OEE Calculation*                                                                                                                                                                   
*Source:* [OEE](https://www.leanproduction.com/oee/)


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
