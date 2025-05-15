using Mini_MES_API.Dto;
using Mini_MES_API.Models;
using Mini_MES_API.Handlers;

namespace Mini_MES_API.Api;

public static class ProductionOrderEndpoints
{
    public static void MapProductionOrderEndPoints(this WebApplication app)
    {
        app.MapGet("/production-orders", async (ProductionOrderHandlers handlers) => await handlers.GetAllProductionOrders())
            .WithDescription("Get all production orders.")
            .WithOpenApi();

        app.MapGet("/production-orders/{id:int}", async (int id, ProductionOrderHandlers handlers) => await handlers.GetProductionOrderById(id))
            .WithDescription("Get production order by id.")
            .WithOpenApi();

        app.MapGet("/production-orders/{id:int}/instructions", async (int id, ProductionOrderHandlers handlers) => await handlers.GetWorkOrderByProductionOrderId(id))
            .WithDescription("Get Work Orders by production order.")
            .WithOpenApi();

        app.MapGet("/production-orders/{id:int}/oee", async (int id, ProductionOrderHandlers handlers) => await handlers.GetOeeCalculationsByProductionOrderId(id))
            .WithDescription("Calculates OEE from COMPLETED production orders.")
            .WithOpenApi();


        app.MapPost("/production-orders", 
                async (CreateProductionOrderDto dto, ProductionOrderHandlers handlers) => 
                    await handlers.CreateProductionOrder(dto))
            .ProducesValidationProblem()
            .Produces<ProductionOrder>(StatusCodes.Status201Created)
            .WithDescription("Create a production order.")
            .WithOpenApi();

        app.MapPost("/production-orders/{id:int}/instructions", 
                async (int id, CreateWorkOrderDto dto, ProductionOrderHandlers handlers) => 
                    await handlers.CreateWorkOrder(id, dto))
            .ProducesValidationProblem()
            .Produces<WorkOrder>(StatusCodes.Status201Created)
            .WithDescription("Create a new work order.")
            .WithOpenApi();

        app.MapPut("/production-orders/{id:int}/schedule", 
                async (int id, ProductionOrderHandlers handlers) => 
                    await handlers.SetProductionOrderStatusScheduled(id))
            .WithDescription("Set production order status to Scheduled.")
            .WithOpenApi();

        app.MapPut("/production-orders/{id:int}/start", 
                async (int id, ProductionOrderHandlers handlers) => 
                    await handlers.SetProductionOrderStatusInProgress(id))
            .WithDescription("Set production order status to InProgress.")
            .WithOpenApi();

        app.MapPut("/production-orders/{id:int}/complete", 
                async (int id, ProductionOrderHandlers handlers) => 
                    await handlers.SetProductionOrderStatusCompleted(id))
            .WithDescription("Set production order status to Completed.")
            .WithOpenApi();

        app.MapPut("/production-orders/{id:int}/cancel", 
                async (int id, ProductionOrderHandlers handlers) => 
                    await handlers.SetProductionOrderStatusCancelled(id))
            .WithDescription("Set production order status to Cancelled.")
            .WithOpenApi();

        app.MapDelete("/production-orders/{id:int}", 
                async (int id, ProductionOrderHandlers handlers) => 
                    await handlers.DeleteProductionOrder(id))
            .WithDescription("Delete a production order.")
            .WithOpenApi();
    }
}