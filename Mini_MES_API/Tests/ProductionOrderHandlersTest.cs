/*using Microsoft.EntityFrameworkCore;
using Mini_MES_API.Data;
using Mini_MES_API.Dto;
using Mini_MES_API.Enums;
using Mini_MES_API.Handlers;
using Mini_MES_API.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Mini_MES_API.Helpers;
using Xunit;

namespace Mini_MES_API.Tests
{
    public class ProductionOrderHandlersTests
    {
        private DbContextOptions<DataContext> GetDbContextOptions()
        {
            return new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetAllProductionOrders_ReturnsAllOrders()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();

            context.ProductionOrders.AddRange(
                new ProductionOrder { ProductSKU = "SKU1", Quantity = 10 },
                new ProductionOrder { ProductSKU = "SKU2", Quantity = 20 }
            );
            await context.SaveChangesAsync();

            var result = await handler.GetAllProductionOrders(context);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.ProductSKU == "SKU1");
            Assert.Contains(result, p => p.ProductSKU == "SKU2");
        }

        [Fact]
        public async Task GetProductionOrderById_ExistingId_ReturnsOrder()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();

            var productionOrder = new ProductionOrder { ProductSKU = "SKU1", Quantity = 10 };
            context.ProductionOrders.Add(productionOrder);
            await context.SaveChangesAsync();

            var result = await handler.GetProductionOrderById(context, productionOrder.Id);

            Assert.Equal(Results.Ok(productionOrder).GetType(), result.GetType());
        }

        [Fact]
        public async Task GetProductionOrderById_NonExistingId_ReturnsNotFound()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();

            var result = await handler.GetProductionOrderById(context, 999);

            Assert.Equal(Results.NotFound($"Sorry, no production order with id: 999 found.").GetType(),
                result.GetType());
        }

        [Fact]
        public async Task GetWorkOrderByProductionOrderId_WithWorkOrders_ReturnsOk()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);

            var productionOrder = new ProductionOrder { Id = 1, ProductSKU = "ENG-V8" };
            context.ProductionOrders.Add(productionOrder);

            var workOrders = new List<WorkOrder>
            {
                new()
                {
                    Id = 1, ProductionOrderId = 1, Description = "Desc", StepName = "Casting", DurationInMinutes = 240
                },
                new()
                {
                    Id = 2, ProductionOrderId = 1, Description = "Desc", StepName = "Machining", DurationInMinutes = 360
                }
            };
            context.WorkOrders.AddRange(workOrders);
            await context.SaveChangesAsync();

            var handler = new ProductionOrderHandlers();

            // Act
            var result = await handler.GetWorkOrderByProductionOrderId(context, 1);

            // Assert
            Assert.IsType<Ok<List<WorkOrder>>>(result);
            var okResult = (Ok<List<WorkOrder>>)result;
            Assert.Equal(2, okResult.Value.Count);
        }

        [Fact]
        public async Task GetWorkOrderByProductionOrderId_NoWorkOrders_ReturnsNotFound()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);

            var handler = new ProductionOrderHandlers();

            // Act
            var result = await handler.GetWorkOrderByProductionOrderId(context, 999);

            // Assert
            Assert.IsType<NotFound<string>>(result);
            var notFoundResult = (NotFound<string>)result;
            Assert.Equal("No work orders found for production order 999.", notFoundResult.Value);
        }

        [Fact]
        public async Task CreateProductionOrder_ValidDto_CreatesOrder()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();

            var dto = new CreateProductionOrderDto
            {
                ProductSKU = "SKU1",
                Quantity = 10,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(2),
                PlannedEndTime = DateTime.Now.AddHours(1),
                DefectCount = 0,
                IdealCycleTimeMinutes = 5,
                Status = Status.Draft
            };

            var result = await handler.CreateProductionOrder(context, dto);

            Assert.Equal(1, await context.ProductionOrders.CountAsync());
            var savedOrder = await context.ProductionOrders.FirstAsync();
            Assert.Equal(dto.ProductSKU, savedOrder.ProductSKU);
            Assert.Equal(dto.Quantity, savedOrder.Quantity);
        }

        [Fact]
        public async Task SetProductionOrderStatusInProgress_ExistingOrder_UpdatesStatus()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();

            var productionOrder = new ProductionOrder { ProductSKU = "SKU1", Quantity = 10, Status = Status.Scheduled };
            context.ProductionOrders.Add(productionOrder);
            await context.SaveChangesAsync();

            await handler.SetProductionOrderStatusInProgress(context, productionOrder.Id);

            var updatedOrder = await context.ProductionOrders.FindAsync(productionOrder.Id);
            Assert.Equal(Status.InProgress, updatedOrder.Status);
            Assert.NotNull(updatedOrder.StartTime);
        }
        
        [Fact]
        public async Task SetProductionOrderStatusInProgress_OrderDoesNotExist_ReturnsNotFound()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();

            var nonExistentId = 999;
            
            var result = await handler.SetProductionOrderStatusInProgress(context, nonExistentId);
            
            Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>(result);
            var notFoundResult = result as Microsoft.AspNetCore.Http.HttpResults.NotFound<string>;
            Assert.NotNull(notFoundResult);
            Assert.Equal($"Sorry, order could not be started because no production order with id: {nonExistentId} found.", notFoundResult.Value);
        }
        
        [Fact]
        public async Task SetProductionOrderScheduled_ExistingOrder_UpdatesStatus()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();

            var productionOrder = new ProductionOrder { ProductSKU = "SKU1", Quantity = 10, Status = Status.Draft };
            context.ProductionOrders.Add(productionOrder);
            await context.SaveChangesAsync();

            await handler.SetProductionOrderStatusScheduled(context, productionOrder.Id);

            var updatedOrder = await context.ProductionOrders.FindAsync(productionOrder.Id);
            Assert.Equal(Status.Scheduled, updatedOrder.Status);
        }
        
        [Fact]
        public async Task SetProductionOrderStatusScheduled_OrderDoesNotExist_ReturnsNotFound()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();

            var nonExistentId = 999;
            
            var result = await handler.SetProductionOrderStatusScheduled(context, nonExistentId);
            
            Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>(result);
            var notFoundResult = result as Microsoft.AspNetCore.Http.HttpResults.NotFound<string>;
            Assert.NotNull(notFoundResult);
            Assert.Equal($"Sorry, order could not be scheduled because no production order with id: {nonExistentId} found.", notFoundResult.Value);
        }
        
        [Fact]
        public async Task SetProductionOrderCompleted_ExistingOrder_UpdatesStatus()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();

            var productionOrder = new ProductionOrder { ProductSKU = "SKU1", Quantity = 10, Status = Status.InProgress };
            context.ProductionOrders.Add(productionOrder);
            await context.SaveChangesAsync();

            await handler.SetProductionOrderStatusCompleted(context, productionOrder.Id);

            var updatedOrder = await context.ProductionOrders.FindAsync(productionOrder.Id);
            Assert.Equal(Status.Completed, updatedOrder.Status);
            Assert.NotNull(updatedOrder.EndTime);
        }
        
        [Fact]
        public async Task SetProductionOrderStatusCompleted_OrderDoesNotExist_ReturnsNotFound()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();

            var nonExistentId = 999;
            
            var result = await handler.SetProductionOrderStatusCompleted(context, nonExistentId);
            
            Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>(result);
            var notFoundResult = result as Microsoft.AspNetCore.Http.HttpResults.NotFound<string>;
            Assert.NotNull(notFoundResult);
            Assert.Equal($"Sorry, order could not be completed because no production order with id: {nonExistentId} found.", notFoundResult.Value);
        }
        
        [Fact]
        public async Task SetProductionOrderCancelled_ExistingOrder_UpdatesStatus()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();

            var productionOrder = new ProductionOrder { ProductSKU = "SKU1", Quantity = 10, Status = Status.Scheduled };
            context.ProductionOrders.Add(productionOrder);
            await context.SaveChangesAsync();

            await handler.SetProductionOrderStatusCancelled(context, productionOrder.Id);

            var updatedOrder = await context.ProductionOrders.FindAsync(productionOrder.Id);
            Assert.Equal(Status.Cancelled, updatedOrder.Status);
            Assert.NotNull(updatedOrder.EndTime);
        }
        
        [Fact]
        public async Task SetProductionOrderStatusCancelled_OrderDoesNotExist_ReturnsNotFound()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();

            var nonExistentId = 999;
            
            var result = await handler.SetProductionOrderStatusCancelled(context, nonExistentId);
            
            Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>(result);
            var notFoundResult = result as Microsoft.AspNetCore.Http.HttpResults.NotFound<string>;
            Assert.NotNull(notFoundResult);
            Assert.Equal($"Sorry, order could not be cancelled because no production order with id: {nonExistentId} found.", notFoundResult.Value);
        }
        
        [Fact]
        public async Task GetOeeCalculationsByProductionOrderId_OrderDoesNotExist_ReturnsNotFound()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();

            var nonExistentId = 999;
            
            var result = await handler.GetOeeCalculationsByProductionOrderId(context, nonExistentId);
            
            Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>(result);
            var notFoundResult = result as Microsoft.AspNetCore.Http.HttpResults.NotFound<string>;
            Assert.NotNull(notFoundResult);
            Assert.Equal($"No production order with id: {nonExistentId} found.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetOeeCalculationsByProductionOrderId_OrderNotCompleted_ReturnsBadRequest()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();
            var productionOrder = new ProductionOrder
            {
                Id = 1,
                ProductSKU = "SKU1",
                Status = Status.InProgress,
                StartTime = DateTime.Now.AddHours(-1),
                EndTime = DateTime.Now
            };
            context.ProductionOrders.Add(productionOrder);
            await context.SaveChangesAsync();
            
            var result = await handler.GetOeeCalculationsByProductionOrderId(context, productionOrder.Id);
            
            Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>>(result);
            var badRequestResult = result as Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>;
            Assert.NotNull(badRequestResult);
            Assert.Equal($"Order {productionOrder.Id} is not completed. OEE unavailable.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetOeeCalculationsByProductionOrderId_Success_ReturnsOkWithOee()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();
            var productionOrder = new ProductionOrder
            {
                Id = 1,
                ProductSKU = "SKU1",
                Status = Status.Completed,
                StartTime = DateTime.Now.AddHours(-2),
                EndTime = DateTime.Now,
                IdealCycleTimeMinutes = 1,
                Quantity = 100,
                DefectCount = 5
            };
            var workOrder = new WorkOrder
            {
                ProductionOrderId = productionOrder.Id,
                DurationInMinutes = 120,
                Description = "String",
                StepName = "String"
            };
            context.ProductionOrders.Add(productionOrder);
            context.WorkOrders.Add(workOrder);
            await context.SaveChangesAsync();
            
            var result = await handler.GetOeeCalculationsByProductionOrderId(context, productionOrder.Id);
            
            Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<string>>(result);
            var okResult = result as Microsoft.AspNetCore.Http.HttpResults.Ok<string>;
            Assert.NotNull(okResult);
            
            var expectedOeeResult = OeeCalculationHelper.CalculateOee(productionOrder, workOrder).ToFormattedString();
            Assert.Equal(expectedOeeResult, okResult.Value);
        }
        
        [Fact]
        public async Task CreateWorkOrder_ProductionOrderNotFound_ReturnsNotFound()
        {

            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();

            var nonExistentProductionOrderId = 999;
            var dto = new CreateWorkOrderDto
            {
                StepName = "Assembly",
                Description = "Assembly process for the product",
                DurationInMinutes = 120
            };
            
            var result = await handler.CreateWorkOrder(context, nonExistentProductionOrderId, dto);
            
            Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>(result);
            var notFoundResult = result as Microsoft.AspNetCore.Http.HttpResults.NotFound<string>;
            Assert.NotNull(notFoundResult);
            Assert.Equal($"Sorry, Production order with id: {nonExistentProductionOrderId} not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task CreateWorkOrder_Success_ReturnsCreated()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();
            
            var productionOrder = new ProductionOrder
            {
                ProductSKU = "SKU123",
                Quantity = 100,
                StartTime = DateTime.Now.AddHours(-1),
                EndTime = DateTime.Now,
                PlannedEndTime = DateTime.Now.AddHours(2),
                DefectCount = 10,
                IdealCycleTimeMinutes = 1.5,
                Status = Status.Scheduled
            };
            context.ProductionOrders.Add(productionOrder);
            await context.SaveChangesAsync();

            var dto = new CreateWorkOrderDto
            {
                StepName = "Assembly",
                Description = "Assembly process for the product",
                DurationInMinutes = 120
            };
            
            var result = await handler.CreateWorkOrder(context, productionOrder.Id, dto);
            
            Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Created<WorkOrder>>(result);
            var createdResult = result as Microsoft.AspNetCore.Http.HttpResults.Created<WorkOrder>;
            Assert.NotNull(createdResult);
            Assert.NotNull(createdResult.Value);
            Assert.Equal($"/work-orders/{createdResult.Value.Id}", createdResult.Location);

            
            var storedWorkOrder = await context.WorkOrders.FindAsync(createdResult.Value.Id);
            Assert.NotNull(storedWorkOrder);
            Assert.Equal(productionOrder.Id, storedWorkOrder.ProductionOrderId);
            Assert.Equal(dto.StepName, storedWorkOrder.StepName);
            Assert.Equal(dto.Description, storedWorkOrder.Description);
            Assert.Equal(dto.DurationInMinutes, storedWorkOrder.DurationInMinutes);
        }
        
        [Fact]
        public async Task DeleteProductionOrder_OrderDoesNotExist_ReturnsNotFound()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();

            var nonExistentId = 999; 
            
            var result = await handler.DeleteProductionOrder(context, nonExistentId);
            
            Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>(result);
            var notFoundResult = result as Microsoft.AspNetCore.Http.HttpResults.NotFound<string>;
            Assert.NotNull(notFoundResult);
            Assert.Equal($"Sorry, order could not be deleted because no production order with id: {nonExistentId} found.", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteProductionOrder_OrderNotDraft_ReturnsBadRequest()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();
            
            var productionOrder = new ProductionOrder
            {
                ProductSKU = "SKU123",
                Quantity = 100,
                Status = Status.Scheduled,
                StartTime = DateTime.Now.AddHours(-1),
                EndTime = DateTime.Now
            };
            context.ProductionOrders.Add(productionOrder);
            await context.SaveChangesAsync();
            
            var result = await handler.DeleteProductionOrder(context, productionOrder.Id);
            
            Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>>(result);
            var badRequestResult = result as Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>;
            Assert.NotNull(badRequestResult);
            Assert.Equal($"Sorry, order could not be deleted because production order with id: {productionOrder.Id} is not a draft.", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteProductionOrder_Success_ReturnsOk()
        {
            var options = GetDbContextOptions();
            using var context = new DataContext(options);
            var handler = new ProductionOrderHandlers();
            
            var productionOrder = new ProductionOrder
            {
                ProductSKU = "SKU123",
                Quantity = 100,
                Status = Status.Draft,
                StartTime = DateTime.Now.AddHours(-1),
                EndTime = DateTime.Now
            };
            context.ProductionOrders.Add(productionOrder);
            await context.SaveChangesAsync();


            var result = await handler.DeleteProductionOrder(context, productionOrder.Id);
            
            Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<string>>(result);
            var okResult = result as Microsoft.AspNetCore.Http.HttpResults.Ok<string>;
            Assert.NotNull(okResult);
            Assert.Equal($"Production order with id: {productionOrder.Id} deleted.", okResult.Value);
            
            var deletedOrder = await context.ProductionOrders.FindAsync(productionOrder.Id);
            Assert.Null(deletedOrder);
        }
    }
}*/