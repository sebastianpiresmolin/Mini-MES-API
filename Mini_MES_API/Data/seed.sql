BEGIN TRANSACTION;

-- Insert 50 Production Orders (Car Parts Manufacturing)
INSERT INTO ProductionOrders (ProductSKU, Quantity, StartTime, EndTime, Status)
VALUES
    -- Original 3 orders
    ('ENG-V8-2023', 50, '2023-10-01 08:00', '2023-10-05 17:00', 'Draft'),
    ('TRANS-9AT-2024', 100, GETDATE(), DATEADD(day, 7, GETDATE()), 'Scheduled'),
    ('BAT-LI-ION-50K', 200, '2023-10-10 07:30', '2023-10-12 15:00', 'InProgress'),

    -- Additional 47 orders
    ('ENG-V6-2024', 75, '2024-01-15 07:00', '2024-01-20 16:00', 'Scheduled'),
    ('ENG-I4-TURBO', 120, '2024-01-18 08:30', '2024-01-25 15:30', 'Draft'),
    ('TRANS-6MT-2024', 80, '2024-01-22 07:00', '2024-01-28 17:00', 'Scheduled'),
    ('BRAKE-DISC-F150', 500, '2024-01-10 06:00', '2024-01-15 14:00', 'InProgress'),
    ('SUSP-STRUT-FRT', 300, '2024-01-12 08:00', '2024-01-18 12:00', 'Completed'),
    ('STEER-RACK-SUV', 150, '2024-01-20 07:30', '2024-01-27 16:30', 'Draft'),
    ('ALT-12V-150A', 400, '2024-01-25 08:00', '2024-01-30 17:00', 'Scheduled'),
    ('SEAT-FRAME-LUX', 200, '2024-02-01 07:00', '2024-02-08 15:00', 'Draft'),
    ('DASHBOARD-2024', 100, '2024-02-05 08:30', '2024-02-12 16:30', 'Scheduled'),
    ('EXHAUST-V8-PERF', 80, '2024-02-10 07:00', '2024-02-15 15:00', 'InProgress'),
    ('FUEL-PUMP-2024', 350, '2024-02-12 08:00', '2024-02-18 17:00', 'Draft'),
    ('RAD-ALUM-SPORT', 250, '2024-02-15 07:30', '2024-02-20 16:30', 'Scheduled'),
    ('A/C-COMP-2024', 200, '2024-02-18 08:00', '2024-02-25 17:00', 'Draft'),
    ('OIL-PUMP-V8', 300, '2024-02-20 07:00', '2024-02-25 15:00', 'InProgress'),
    ('CAM-SHAFT-VVTI', 150, '2024-02-22 08:30', '2024-02-28 16:30', 'Scheduled'),
    ('CRANK-FORGED', 100, '2024-02-25 07:00', '2024-03-02 15:00', 'Draft'),
    ('TURBO-TWIN-V6', 75, '2024-02-28 08:00', '2024-03-05 17:00', 'Scheduled'),
    ('WHEEL-18-ALLOY', 600, '2024-03-01 07:30', '2024-03-07 16:30', 'InProgress'),
    ('TIRE-SUV-ALL-TERR', 800, '2024-03-05 08:00', '2024-03-12 17:00', 'Draft'),
    ('BELT-TIMING-V8', 400, '2024-03-08 07:00', '2024-03-13 15:00', 'Scheduled'),
    ('AIRBAG-DRIVER', 500, '2024-03-10 08:30', '2024-03-15 16:30', 'InProgress'),
    ('ECU-2024-SPORT', 250, '2024-03-12 07:00', '2024-03-18 15:00', 'Draft'),
    ('WIPER-BLADE-24', 1000, '2024-03-15 08:00', '2024-03-18 17:00', 'Scheduled'),
    ('DOOR-PANEL-LUX', 200, '2024-03-18 07:30', '2024-03-25 16:30', 'InProgress'),
    ('HEADLIGHT-LED', 300, '2024-03-20 08:00', '2024-03-26 17:00', 'Draft'),
    ('TAILLIGHT-MATRIX', 300, '2024-03-22 07:00', '2024-03-28 15:00', 'Scheduled'),
    ('MUFFLER-SPORT', 150, '2024-03-25 08:30', '2024-03-30 16:30', 'InProgress'),
    ('THROTTLE-BODY-ELE', 200, '2024-03-28 07:00', '2024-04-03 15:00', 'Draft'),
    ('INJECTOR-DIRECT', 500, '2024-04-01 08:00', '2024-04-07 17:00', 'Scheduled'),
    ('SPARK-PLUG-IRID', 2000, '2024-04-03 07:30', '2024-04-08 16:30', 'InProgress'),
    ('DIFF-LSD-SPORT', 100, '2024-04-05 08:00', '2024-04-12 17:00', 'Draft'),
    ('AXLE-REAR-TRUCK', 200, '2024-04-08 07:00', '2024-04-15 15:00', 'Scheduled'),
    ('STEERING-WHEEL-M', 300, '2024-04-10 08:30', '2024-04-16 16:30', 'InProgress'),
    ('CLUTCH-DUAL-MASS', 150, '2024-04-12 07:00', '2024-04-18 15:00', 'Draft'),
    ('GASKET-HEAD-V8', 400, '2024-04-15 08:00', '2024-04-20 17:00', 'Scheduled'),
    ('PISTON-FORGED', 250, '2024-04-18 07:30', '2024-04-25 16:30', 'InProgress'),
    ('VALVE-INTAKE-TI', 500, '2024-04-20 08:00', '2024-04-26 17:00', 'Draft'),
    ('CAT-CONVERTER-EU6', 200, '2024-04-22 07:00', '2024-04-28 15:00', 'Scheduled'),
    ('O2-SENSOR-WIDEBAND', 300, '2024-04-25 08:30', '2024-04-30 16:30', 'InProgress'),
    ('ABS-MODULE-2024', 150, '2024-04-28 07:00', '2024-05-05 15:00', 'Draft'),
    ('CONTROL-ARM-FRONT', 250, '2024-05-01 08:00', '2024-05-08 17:00', 'Scheduled'),
    ('BUSHING-POLY-SPORT', 400, '2024-05-03 07:30', '2024-05-08 16:30', 'InProgress'),
    ('COIL-SPRING-SUV', 300, '2024-05-05 08:00', '2024-05-12 17:00', 'Draft'),
    ('SHOCK-ADJ-SPORT', 200, '2024-05-08 07:00', '2024-05-15 15:00', 'Scheduled'),
    ('SWAY-BAR-REAR', 150, '2024-05-10 08:30', '2024-05-16 16:30', 'InProgress'),
    ('BATTERY-AGM-95', 300, '2024-05-12 07:00', '2024-05-18 15:00', 'Draft'),
    ('WIRING-HARNESS-ENG', 100, '2024-05-15 08:00', '2024-05-22 17:00', 'Scheduled');

-- Get the first ProductionOrder ID for reference
DECLARE @FirstId INT;
SELECT @FirstId = MIN(Id) FROM ProductionOrders;

-- Insert Work Orders for the first 3 existing Production Orders (unchanged)
DECLARE @EngineV8Id INT = @FirstId;
DECLARE @TransmissionId INT = @FirstId + 1;
DECLARE @BatteryId INT = @FirstId + 2;

-- Insert Work Orders for Engine V8 Production
INSERT INTO WorkOrders (ProductionOrderId, StepName, Description, DurationInMinutes)
VALUES
    (@EngineV8Id, 'Engine Block Machining', 'CNC machining of aluminum engine block', 240),
    (@EngineV8Id, 'Piston Assembly', 'Install forged pistons and connecting rods', 90),
    (@EngineV8Id, 'Quality Testing', 'Dyno testing and leak checks', 180);

-- Insert Work Orders for 9-Speed Transmission
INSERT INTO WorkOrders (ProductionOrderId, StepName, Description, DurationInMinutes)
VALUES
    (@TransmissionId, 'Gear Cutting', 'Precision cutting of helical gears', 300),
    (@TransmissionId, 'Hydraulic Assembly', 'Install valve body and torque converter', 150),
    (@TransmissionId, 'Final Inspection', 'Shift quality and pressure testing', 120);

-- Insert Work Orders for Lithium Batteries
INSERT INTO WorkOrders (ProductionOrderId, StepName, Description, DurationInMinutes)
VALUES
    (@BatteryId, 'Cell Stacking', 'Automated stacking of 18650 battery cells', 180),
    (@BatteryId, 'Welding', 'Laser welding of cell connections', 90),
    (@BatteryId, 'Encapsulation', 'Thermal management system installation', 210);

-- Add 41 more Work Orders for other Production Orders (to reach 50 total)
INSERT INTO WorkOrders (ProductionOrderId, StepName, Description, DurationInMinutes)
VALUES
    (@FirstId + 3, 'Block Casting', 'V6 engine block casting in aluminum foundry', 360),
    (@FirstId + 3, 'Cylinder Honing', 'Precision cylinder wall honing', 120),
    (@FirstId + 3, 'Camshaft Installation', 'Variable valve timing camshaft placement', 90),

    (@FirstId + 4, 'Turbocharger Balancing', 'High-speed balancing of turbo impeller', 180),
    (@FirstId + 4, 'Intercooler Assembly', 'Air-to-liquid intercooler assembly', 120),

    (@FirstId + 5, 'Gear Forging', 'High-strength gear forging for manual transmission', 240),
    (@FirstId + 5, 'Synchronizer Assembly', 'Precision assembly of synchronizer rings', 180),

    (@FirstId + 6, 'Disc Casting', 'High-carbon brake disc casting process', 200),
    (@FirstId + 6, 'Surface Grinding', 'Precision surface finish grinding', 120),

    (@FirstId + 7, 'Strut Tube Forming', 'Cold forming of suspension strut tubes', 150),
    (@FirstId + 7, 'Damper Assembly', 'Oil filling and gas pressurization', 90),

    (@FirstId + 8, 'Rack Housing Machining', 'CNC machining of steering rack housing', 240),
    (@FirstId + 8, 'Pinion Gear Installation', 'Precision gear meshing and adjustment', 120),

    (@FirstId + 9, 'Stator Winding', 'Copper wire winding for alternator stator', 180),
    (@FirstId + 9, 'Voltage Regulator Calibration', 'Electronic calibration of voltage regulator', 60),

    (@FirstId + 10, 'Frame Welding', 'Robotic welding of seat frame components', 210),
    (@FirstId + 10, 'Padding Installation', 'Foam padding installation and shaping', 120),

    (@FirstId + 11, 'Injection Molding', 'Dashboard main panel injection molding', 270),
    (@FirstId + 11, 'Component Integration', 'Electronics and display integration', 240),

    (@FirstId + 12, 'Pipe Bending', 'CNC pipe bending for performance exhaust', 150),
    (@FirstId + 12, 'Catalytic Coating', 'Precious metal coating of catalytic converter', 180),

    (@FirstId + 13, 'Motor Assembly', 'Electric motor assembly for fuel pump', 120),
    (@FirstId + 13, 'Pressure Testing', 'Flow rate and pressure validation', 90),

    (@FirstId + 14, 'Core Assembly', 'Aluminum radiator core assembly', 240),
    (@FirstId + 14, 'Pressure Testing', 'Leak detection and pressure validation', 120),

    (@FirstId + 15, 'Clutch Plate Assembly', 'A/C compressor clutch plate balancing', 150),
    (@FirstId + 15, 'Refrigerant Fitting', 'High-pressure fitting installation', 90),

    (@FirstId + 16, 'Precision Machining', 'CNC machining of oil pump housing', 180),
    (@FirstId + 16, 'Pressure Relief Calibration', 'Calibration of pressure relief valve', 90),

    (@FirstId + 17, 'Lobe Profiling', 'CNC grinding of camshaft lobes', 240),
    (@FirstId + 17, 'Heat Treatment', 'Induction hardening of cam surfaces', 180),

    (@FirstId + 18, 'Forging Process', 'Hot forging of crankshaft blank', 300),
    (@FirstId + 18, 'Balancing', 'Precision dynamic balancing', 150),

    (@FirstId + 19, 'Turbine Wheel Casting', 'Investment casting of turbine wheels', 210),
    (@FirstId + 19, 'Shaft Balancing', 'High-speed dynamic balancing of turbo assembly', 180),

    (@FirstId + 20, 'Rim Casting', 'Aluminum alloy wheel rim casting', 270),
    (@FirstId + 20, 'CNC Machining', 'Precision face machining and drilling', 180),

    (@FirstId + 21, 'Rubber Compounding', 'Specialty rubber compounding for all-terrain performance', 240),
    (@FirstId + 21, 'Tread Molding', 'Precision tread pattern molding', 210),

    (@FirstId + 22, 'Belt Teeth Forming', 'Precision molding of timing belt teeth', 150),
    (@FirstId + 22, 'Tensile Testing', 'Strength and elongation testing', 90),

    (@FirstId + 23, 'Fabric Cutting', 'Automated cutting of airbag fabric', 120),
    (@FirstId + 23, 'Inflator Assembly', 'Gas generator assembly and testing', 180),

    (@FirstId + 24, 'Circuit Board Assembly', 'SMT assembly of ECU main board', 240),
    (@FirstId + 24, 'Firmware Loading', 'Calibration firmware loading and testing', 120);

COMMIT TRANSACTION;