using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mini_MES_API.Data;
using Mini_MES_API.Dto;
using Mini_MES_API.Enums;
using Mini_MES_API.Helpers;
using Mini_MES_API.Models;

namespace Mini_MES_API.Handlers;

public class ProductionOrderHandlers
{
    public async Task<List<ProductionOrder>> GetAllProductionOrders(DataContext context)
    {
        return await context.ProductionOrders.ToListAsync();
    }
    
    public async Task<IResult> GetProductionOrderById(DataContext context, int id)
    {
        var productionOrder = await context.ProductionOrders.FindAsync(id);
        return productionOrder is not null 
            ? Results.Ok(productionOrder)
            : Results.NotFound($"Sorry, no production order with id: {id} found.");
    }

    public async Task<IResult> GetWorkOrderByProductionOrderId(DataContext context, int id)
    {
        var workOrders = await context.WorkOrders
            .Where(w => w.ProductionOrderId == id)
            .ToListAsync();

        return workOrders.Count > 0 
            ? Results.Ok(workOrders) 
            : Results.NotFound($"No work orders found for production order {id}.");
    }

    public async Task<IResult> GetOeeCalculationsByProductionOrderId(DataContext context, int id)
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
    }

    public async Task<IResult> CreateProductionOrder(DataContext context, [FromBody] CreateProductionOrderDto dto)
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
    }

    public async Task<IResult> CreateWorkOrder(DataContext context, int id, [FromBody] CreateWorkOrderDto dto)
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
    }

    public async Task<IResult> SetProductionOrderStatusScheduled(DataContext context, int id)
    {
        var productionOrder = await context.ProductionOrders.FindAsync(id);
        if (productionOrder is null)
            return Results.NotFound(
                $"Sorry, order could not be started because no production order with id: {id} found.");
                
        productionOrder.Status = Status.Scheduled;
        await context.SaveChangesAsync();
                
        return Results.Ok(await context.ProductionOrders.FindAsync(id));
    }

    public async Task<IResult> SetProductionOrderStatusInProgress(DataContext context, int id)
    {
        var productionOrder = await context.ProductionOrders.FindAsync(id);
        if (productionOrder is null)
            return Results.NotFound(
                $"Sorry, order could not be started because no production order with id: {id} found.");
                
        productionOrder.Status = Status.InProgress;
        productionOrder.StartTime = DateTime.Now;
        await context.SaveChangesAsync();
                
        return Results.Ok(await context.ProductionOrders.FindAsync(id));
    }

    public async Task<IResult> SetProductionOrderStatusCompleted(DataContext context, int id)
    {
        var productionOrder = await context.ProductionOrders.FindAsync(id);
        if (productionOrder is null)
            return Results.NotFound(
                $"Sorry, order could not be completed because no production order with id: {id} found.");
                    
        productionOrder.Status = Status.Completed;
        productionOrder.EndTime = DateTime.Now;
        await context.SaveChangesAsync();
                    
        return Results.Ok(await context.ProductionOrders.FindAsync(id));
    }

    public async Task<IResult> SetProductionOrderStatusCancelled(DataContext context, int id)
    {
        var productionOrder = await context.ProductionOrders.FindAsync(id);
        if (productionOrder is null)
            return Results.NotFound(
                $"Sorry, order could not be started because no production order with id: {id} found.");
                
        productionOrder.Status = Status.Cancelled;
        await context.SaveChangesAsync();
                
        return Results.Ok(await context.ProductionOrders.FindAsync(id));
    }

    public async Task<IResult> DeleteProductionOrder(DataContext context, int id)
    {
        var productionOrder = await context.ProductionOrders.FindAsync(id);
        if (productionOrder is null)
            return Results.NotFound($"Sorry, order could not be deleted because no production order with id: {id} found.");
        if (productionOrder.Status != 0) // 0 == Draft
            return Results.BadRequest($"Sorry, order could not be deleted because production order with id: {id} is not a draft.");
                
        context.ProductionOrders.Remove(productionOrder);
        await context.SaveChangesAsync();
                
        return Results.Ok($"Production order with id: {id} deleted.");
    }
}