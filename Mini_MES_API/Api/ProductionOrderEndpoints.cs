using Microsoft.AspNetCore.Mvc;
using Mini_MES_API.Data;
using Mini_MES_API.Models;
using Mini_MES_API.Dto;
using Microsoft.EntityFrameworkCore;
using Mini_MES_API.Enums;
using Mini_MES_API.Handlers;
using Mini_MES_API.Helpers;

namespace Mini_MES_API.Api;

public static class ProductionOrderEndpoints
{
    public static void MapProductionOrderEndPoints(this WebApplication app)
    {
        var handlers = new ProductionOrderHandlers();
        
        app.MapGet("/production-orders", handlers.GetAllProductionOrders)
            .WithDescription("Get all production orders.")
            .WithOpenApi();
        
        app.MapGet("/production-orders/{id:int}", handlers.GetProductionOrderById)
            .WithDescription("Get production order by id.")
            .WithOpenApi();

        app.MapGet("/production-orders/{id:int}/instructions", handlers.GetWorkOrderByProductionOrderId) 
        .WithDescription("Get Work Orders by production order.")
        .WithOpenApi();

        app.MapGet("/production-orders/{id:int}/oee", handlers.GetOeeCalculationsByProductionOrderId)
            .WithDescription("Calculates OEE from COMPLETED production orders.")
            .WithOpenApi();
        
        
        app.MapPost("/production-orders", handlers.CreateProductionOrder)
            .ProducesValidationProblem()
            .Produces<ProductionOrder>(StatusCodes.Status201Created)
            .WithDescription("Create a production order.")
            .WithOpenApi();
        
        app.MapPost("/production-orders/{id:int}/instructions", handlers.CreateWorkOrder)
            .ProducesValidationProblem()
            .Produces<WorkOrder>(StatusCodes.Status201Created)
            .WithDescription("Create a new work order.")
            .WithOpenApi();
        
        app.MapPut("/production-orders/{id:int}/schedule", handlers.SetProductionOrderStatusScheduled)
        .WithDescription("Set production order status to Scheduled.")
        .WithOpenApi();

        app.MapPut("/production-orders/{id:int}/start", handlers.SetProductionOrderStatusInProgress)
            .WithDescription("Set production order status to InProgress.")
            .WithOpenApi();
        
        app.MapPut("/production-orders/{id:int}/complete", handlers.SetProductionOrderStatusCompleted)
            .WithDescription("Set production order status to Completed.")
            .WithOpenApi();
        
        app.MapPut("/production-orders/{id:int}/cancel", handlers.SetProductionOrderStatusCancelled)
        .WithDescription("Set production order status to Cancelled.")
        .WithOpenApi();

        app.MapDelete("/production-orders/{id:int}", handlers.DeleteProductionOrder)
            .WithDescription("Delete a production order.")
            .WithOpenApi();
    }
}