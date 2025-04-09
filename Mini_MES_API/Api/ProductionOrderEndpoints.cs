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
        
        app.MapPost("/production-orders", async (DataContext context, [FromBody] CreateProductionOrderDto dto) =>
            {
                var productionOrder = new ProductionOrder
                {
                    ProductSKU = dto.ProductSKU,
                    Quantity = dto.Quantity,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
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