using Microsoft.EntityFrameworkCore;
using Mini_MES_API.Data;
using Mini_MES_API.Dto;
using Mini_MES_API.Enums;
using Mini_MES_API.Handlers;
using Mini_MES_API.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
    }
}