# Mini MES API

**Keywords**
- .NET
- Entity Framework
- LINQ
- SQL
- MinimalAPI
- Xunit
- OEE

***What is Mini MES API?***

Originally it was just supposed to be a tutorial project to teach myself how to work with MinimalAPI and Handlers. But at this time I was also diving deeper into [ISA-95](https://www.isa.org/standards-and-publications/isa-standards/isa-95-standard) and more specifically level three - Manufacturing Execution Systems (MES). So the result became a project that was now both a platform to help me hone my API skills, but also shaped as a miniature MES-API. This will be explained further upon as you go on reading.


![ISA-95-Model](https://github.com/user-attachments/assets/229f4401-9015-4783-890e-8572f4a6cdaf)
*The ISA-95 Modell showing the different layers. This API lives in the third level while mimicing some communication coming in from the forth level*
*Source:* [ISA-95](https://www.isa.org/standards-and-publications/isa-standards/isa-95-standard)

***Features***

The Mini MES API has three features, two of these are rather plain and the first is the ability to perform CRUD operations on Work Orders and Production Orders. Work Orders are attached to Production Orders in a one-to-many-relationship, which means that Production Orders can have multiple Work Orders attached to them. This simulated how industries often queue multiple jobs (Work Orders) to the same production line (Production Order in Mini MES API).

The second of the rather basic features is the ability to use the API to set the status of Production Orders through PUT-requests. In its current implementation it doesn't effect much other than that you can't delete Production Orders that isn't Cancelled or a Draft (To maintain documentation a soft deletion is preferred). But it is a part of the last feature which is a bit special and related to industrial standards, namely *OEE* calculations which you can only do on Production Orders with status Completed in the API.

![image](https://github.com/user-attachments/assets/557e3efd-9d5f-4930-a29b-ce286267549e)

*The OEE Calculation*                                                                                                                                                                   
*Source:* [OEE](https://www.leanproduction.com/oee/)


*OEE Calculation*
Overall Equipment Effectiveness (OEE) is a metric that identifies the percentage of planned production time that is truly productive. An OEE score of 100% represents perfect production: manufacturing only good parts, as fast as possible, with no downtime. The Mini MES API has a feature to do this calculation, and also display each part calculation as well. These parts are: 
- **Availability** that takes into account *Availability Loss*, which includes all events that stop planned production for an appreciable length of time (typically several minutes or longer). Availability Loss includes Unplanned Stops (such as equipment failures and material shortages), and Planned Stops (such as changeover time). Although the Mini MES API only uses start, stop and expected time discrepancies for this calculation.
- **Quality** that takes into account *Quality Loss*, which factors out manufactured pieces that do not meet quality standards, including pieces that are later reworked.
- **Performance** that takes into account *Performance Loss*, which includes all factors that cause the production asset to operate at less than the maximum possible speed when running (including Slow Cycles and Small Stops). This calculation, in contrast to *Availability*, factors in the *Ideal Cycle Time* which basically means how many minutes it ideally should take to manufacture one unit instead of looking at the total expected time for the order.

When you have these three variables calculated you can now 

