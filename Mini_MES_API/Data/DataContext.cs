using Microsoft.EntityFrameworkCore;
using Mini_MES_API.Models;

namespace Mini_MES_API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }
    
    public DbSet<ProductionOrder> ProductionOrders => Set<ProductionOrder>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkOrder>()
            .HasOne(w => w.ProductionOrder)
            .WithMany(p => p.WorkOrders)
            .HasForeignKey(w => w.ProductionOrderId);
    }
}