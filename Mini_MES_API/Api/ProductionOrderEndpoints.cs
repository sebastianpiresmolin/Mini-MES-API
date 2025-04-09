using Microsoft.AspNetCore.Mvc;
using Mini_MES_API.Data;
using Mini_MES_API.Models;
using Mini_MES_API.Dto;
using Microsoft.EntityFrameworkCore;
using Mini_MES_API.Enums;
using Mini_MES_API.Helpers;

namespace Mini_MES_API.Api;

public static class ProductionOrderEndpoints
{
    public static void MapProductionOrderEndPoints(this WebApplication app)
    {
        app.MapGet("/production-orders", async (DataContext context) =>
            await context.ProductionOrders.ToListAsync())
            .WithDescription("Get all production orders.")
            .WithOpenApi();
        
        app.MapGet("/production-orders/{id:int}", async (DataContext context, int id) =>
            await context.ProductionOrders.FindAsync(id) is { } productionOrder
                ? Results.Ok(productionOrder)
                : Results.NotFound($"Sorry, no production order with id: {id} found."))
            .WithDescription("Get production order by id.")
            .WithOpenApi();

        app.MapGet("/production-orders/{id:int}/instructions", async (DataContext context, int id) =>
        {
            var workOrders = await context.WorkOrders
                .Where(w => w.ProductionOrderId == id)
                .ToListAsync();

            return workOrders.Count > 0 
                ? Results.Ok(workOrders) 
                : Results.NotFound($"No work orders found for production order {id}.");
        })
        .WithDescription("Get Work Orders by production order.")
        .WithOpenApi();

        app.MapGet("/production-orders/{id}/oee", async (DataContext context, int id) =>
            {
                var productionOrder = await context.ProductionOrders.FindAsync(id);
                if (productionOrder == null)
                    return Results.NotFound($"No production order with id: {id} found.");

                if (productionOrder.Status != Status.Completed)
                    return Results.BadRequest($"Order {id} is not completed. OEE unavailable.");

                var workOrder = await context.WorkOrders
                    .FirstOrDefaultAsync(w => w.ProductionOrderId == id);

                var oeeResult = OeeCalculationHelper.CalculateOee(productionOrder, workOrder);
                return Results.Ok(oeeResult.ToFormattedString());
            })
            .WithDescription("Calculates OEE from COMPLETED production orders.")
            .WithOpenApi();
        
        
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
            .ProducesValidationProblem()
            .WithDescription("Create a production order.")
            .WithOpenApi();
        
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
            .Produces<WorkOrder>(StatusCodes.Status201Created)
            .WithDescription("Create a new work order.")
            .WithOpenApi();
        
        app.MapPut("/production-orders/{id:int}/schedule", async (DataContext context, int id) =>
        {
            var productionOrder = await context.ProductionOrders.FindAsync(id);
            if (productionOrder is null)
                return Results.NotFound(
                    $"Sorry, order could not be started because no production order with id: {id} found.");
                
            productionOrder.Status = Status.Scheduled;
            await context.SaveChangesAsync();
                
            return Results.Ok(await context.ProductionOrders.FindAsync(id));
        })
        .WithDescription("Set production order status to Scheduled.")
        .WithOpenApi();

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
            })
            .WithDescription("Set production order status to InProgress.")
            .WithOpenApi();
        
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
            })
            .WithDescription("Set production order status to Completed.")
            .WithOpenApi();
        
        app.MapPut("/production-orders/{id:int}/cancel", async (DataContext context, int id) =>
        {
            var productionOrder = await context.ProductionOrders.FindAsync(id);
            if (productionOrder is null)
                return Results.NotFound(
                    $"Sorry, order could not be started because no production order with id: {id} found.");
                
            productionOrder.Status = Status.Cancelled;
            await context.SaveChangesAsync();
                
            return Results.Ok(await context.ProductionOrders.FindAsync(id));
        })
        .WithDescription("Set production order status to Cancelled.")
        .WithOpenApi();

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
            })
            .WithDescription("Delete a production order.")
            .WithOpenApi();
    }
}