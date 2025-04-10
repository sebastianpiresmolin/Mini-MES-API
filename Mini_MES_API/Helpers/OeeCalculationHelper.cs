using Mini_MES_API.Dto;
using Mini_MES_API.Models;

namespace Mini_MES_API.Helpers;

public static class OeeCalculationHelper
{
    public static OeeResultDto CalculateOee(ProductionOrder order, WorkOrder workOrder)
    {
        var runTime = (order.EndTime - order.StartTime).TotalMinutes;
        var plannedProductionTime = workOrder.DurationInMinutes;

        double availability = Math.Min(1.0, runTime / plannedProductionTime);
        double performance = Math.Min(1.0, (order.IdealCycleTimeMinutes * order.Quantity) / runTime);
        double quality = Math.Min(1.0, (double)(order.Quantity - order.DefectCount) / order.Quantity);
        double oee = availability * performance * quality;

        return new OeeResultDto
        {
            Availability = availability,
            Performance = performance,
            Quality = quality,
            Oee = oee
        };
    }
}