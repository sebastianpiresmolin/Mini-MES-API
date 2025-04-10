using System.Net;
using Mini_MES_API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Mini_MES_API.Dto;
using Mini_MES_API.Enums;
using Xunit;

namespace Mini_MES_API.Tests;

public class ProductionOrderEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductionOrderEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllProductionOrders_ReturnsOk()
    {
        var response = await _client.GetAsync("/production-orders");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var orders = await response.Content.ReadFromJsonAsync<List<ProductionOrder>>();
        Assert.NotNull(orders);
    }

    [Fact]
    public async Task GetProductionOrderById_ValidId_ReturnsOk()
    {
        var productionOrderId = 1;
        
        var response = await _client.GetAsync($"/production-orders/{productionOrderId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var order = await response.Content.ReadFromJsonAsync<ProductionOrder>();
        Assert.NotNull(order);
        Assert.Equal(productionOrderId, order.Id);
    }

    [Fact]
    public async Task GetProductionOrderById_InvalidId_ReturnsNotFound()
    {
        var invalidId = 9999;
        
        var response = await _client.GetAsync($"/production-orders/{invalidId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains($"Sorry, no production order with id: {invalidId} found.", errorMessage);
    }

    [Fact]
    public async Task CreateProductionOrder_ValidDto_ReturnsCreated()
    {
   
        var newOrder = new CreateProductionOrderDto
        {
            ProductSKU = "SKU123",
            Quantity = 100,
            StartTime = DateTime.Now.AddHours(-1),
            EndTime = DateTime.Now,
            PlannedEndTime = DateTime.Now.AddHours(2),
            DefectCount = 0,
            IdealCycleTimeMinutes = 1.5,
            Status = Status.Draft
        };
        
        var response = await _client.PostAsJsonAsync("/production-orders", newOrder);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdOrder = await response.Content.ReadFromJsonAsync<ProductionOrder>();
        Assert.NotNull(createdOrder);
        Assert.Equal(newOrder.ProductSKU, createdOrder.ProductSKU);
        Assert.Equal(newOrder.Quantity, createdOrder.Quantity);
    }

    [Fact]
    public async Task DeleteProductionOrder_ValidId_ReturnsOk()
    {
        var productionOrderId = 1;
        
        var response = await _client.DeleteAsync($"/production-orders/{productionOrderId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var message = await response.Content.ReadAsStringAsync();
        Assert.Contains($"Production order with id: {productionOrderId} deleted.", message);
    }

    [Fact]
    public async Task DeleteProductionOrder_InvalidId_ReturnsNotFound()
    {
        var invalidId = 9999;
        
        var response = await _client.DeleteAsync($"/production-orders/{invalidId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains($"Sorry, order could not be deleted because no production order with id: {invalidId} found.", errorMessage);
    }
}