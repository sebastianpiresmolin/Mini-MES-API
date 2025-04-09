using Microsoft.AspNetCore.Mvc;
using Mini_MES_API.Data;
using Mini_MES_API.Models;
using Mini_MES_API.Dto;
using Microsoft.EntityFrameworkCore;
using Mini_MES_API.Enums;


namespace Mini_MES_API.Api;

public static class ProductionOrderEndpoints
{
    public static void MapProductionOrderEndPoints(this WebApplication app)
    {
        app.MapGet("/production-orders", async (DataContext context) =>
            await context.ProductionOrders.ToListAsync());
        
        app.MapGet("/production-orders/{id:int}", async (DataContext context, int id) =>
            await context.ProductionOrders.FindAsync(id) is { } productionOrder
                ? Results.Ok(productionOrder)
                : Results.NotFound($"Sorry, no production order with id: {id} found."));

        app.MapGet("/production-orders/{id:int}/instructions", async (DataContext context, int id) =>
        {
            var workOrders = await context.WorkOrders
                .Where(w => w.ProductionOrderId == id)
                .ToListAsync();

            return workOrders.Count > 0 
                ? Results.Ok(workOrders) 
                : Results.NotFound($"No work orders found for production order {id}.");
        });
        
        app.MapGet("/production-orders/{id}/oee", async (DataContext context, int id) =>  
        {  
           var productionOrder = await context.ProductionOrders.FindAsync(id);

           if (productionOrder == null)
           {
               return Results.NotFound($"Sorry, no production order with id: {id} found.");
           }

           var workOrder = await context.WorkOrders
               .Where(w => w.ProductionOrderId == id)
               .FirstOrDefaultAsync();

           if (workOrder == null)
           {
               return Results.NotFound($"No work orders for production order with id: {id} found.");
           }

           if (productionOrder.Status != Status.Completed)
           {
               return Results.BadRequest($"The production order with id: {id} is not completed. OEE calculation unavailable.");
           }
           
           var difference = productionOrder.EndTime - productionOrder.StartTime;
           int totalMinutes = (int)difference.TotalMinutes;

           var runTime = totalMinutes;
           var plannedProductionTime = workOrder.DurationInMinutes;

           double availability = Math.Min(1.0, (double)runTime / plannedProductionTime);
           double performance = Math.Min(1.0, (productionOrder.IdealCycleTimeMinutes * productionOrder.Quantity) / runTime);
           double quality = Math.Min(1.0, (double)(productionOrder.Quantity - productionOrder.DefectCount) / productionOrder.Quantity);

           double oee = availability * performance * quality;

           return Results.Ok($@"Availability: {availability * 100:F2}%
            Performance: {performance * 100:F2}%
            Quality: {quality * 100:F2}%

            Overall OEE: {oee * 100:F2}%");
        });
        
        
        app.MapPost("/production-orders", async (DataContext context, [FromBody] CreateProductionOrderDto dto) =>
            {
                var productionOrder = new ProductionOrder
                {
                    ProductSKU = dto.ProductSKU,
                    Quantity = dto.Quantity,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    PlannedEndTime = dto.PlannedEndTime,
                    DefectCount = dto.DefectCount,
                    IdealCycleTimeMinutes = dto.IdealCycleTimeMinutes,
                    Status = dto.Status
                };

                context.ProductionOrders.Add(productionOrder);
                await context.SaveChangesAsync();
    
                return Results.Created($"/production-orders/{productionOrder.Id}", productionOrder);
            })
            .ProducesValidationProblem();
        
        app.MapPost("/production-orders/{id:int}/instructions",
                async (DataContext context, int id, [FromBody] CreateWorkOrderDto dto) =>
                {
                    var productionOrderExists = await context.ProductionOrders.AnyAsync(p => p.Id == id);
                    if (!productionOrderExists)
                    {
                        return Results.NotFound($"Sorry, Production order with id: {id} not found.");
                    }
                    
                    var workOrder = new WorkOrder
                    {
                        ProductionOrderId = id,
                        StepName = dto.StepName,
                        Description = dto.Description,
                        DurationInMinutes = dto.DurationInMinutes
                    };

                    context.WorkOrders.Add(workOrder);
                    await context.SaveChangesAsync();

                    return Results.Created($"/work-orders/{workOrder.Id}", workOrder);
                })
            .ProducesValidationProblem()
            .Produces<WorkOrder>(StatusCodes.Status201Created);
        
        app.MapPut("/production-orders/{id:int}/schedule", async (DataContext context, int id) =>
        {
            var productionOrder = await context.ProductionOrders.FindAsync(id);
            if (productionOrder is null)
                return Results.NotFound(
                    $"Sorry, order could not be started because no production order with id: {id} found.");
                
            productionOrder.Status = Status.Scheduled;
            await context.SaveChangesAsync();
                
            return Results.Ok(await context.ProductionOrders.FindAsync(id));
        });

        app.MapPut("/production-orders/{id:int}/start", async (DataContext context, int id) =>
            {
                var productionOrder = await context.ProductionOrders.FindAsync(id);
                if (productionOrder is null)
                    return Results.NotFound(
                        $"Sorry, order could not be started because no production order with id: {id} found.");
                
                productionOrder.Status = Status.InProgress;
                productionOrder.StartTime = DateTime.Now;
                await context.SaveChangesAsync();
                
                return Results.Ok(await context.ProductionOrders.FindAsync(id));
            });
        
        app.MapPut("/production-orders/{id:int}/complete", async (DataContext context, int id) =>
            {
                var productionOrder = await context.ProductionOrders.FindAsync(id);
                if (productionOrder is null)
                    return Results.NotFound(
                        $"Sorry, order could not be completed because no production order with id: {id} found.");
                    
                productionOrder.Status = Status.Completed;
                productionOrder.EndTime = DateTime.Now;
                await context.SaveChangesAsync();
                    
                return Results.Ok(await context.ProductionOrders.FindAsync(id));
            });
        
        app.MapPut("/production-orders/{id:int}/cancel", async (DataContext context, int id) =>
        {
            var productionOrder = await context.ProductionOrders.FindAsync(id);
            if (productionOrder is null)
                return Results.NotFound(
                    $"Sorry, order could not be started because no production order with id: {id} found.");
                
            productionOrder.Status = Status.Cancelled;
            await context.SaveChangesAsync();
                
            return Results.Ok(await context.ProductionOrders.FindAsync(id));
        });

        app.MapDelete("/production-orders/{id:int}", async (DataContext context, int id) =>
            {
                var productionOrder = await context.ProductionOrders.FindAsync(id);
                if (productionOrder is null)
                    return Results.NotFound($"Sorry, order could not be deleted because no production order with id: {id} found.");
                if (productionOrder.Status != 0) // 0 == Draft
                    return Results.BadRequest($"Sorry, order could not be deleted because production order with id: {id} is not a draft.");
                
                context.ProductionOrders.Remove(productionOrder);
                await context.SaveChangesAsync();
                
                return Results.Ok($"Production order with id: {id} deleted.");
            });
    }
    
}