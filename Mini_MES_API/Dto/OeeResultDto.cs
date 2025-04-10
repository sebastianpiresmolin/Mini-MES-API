namespace Mini_MES_API.Dto;

public class OeeResultDto
{
    public double Availability { get; set; }
    public double Performance { get; set; }
    public double Quality { get; set; }
    public double Oee { get; set; }

    public string ToFormattedString() => 
        $@"Availability: {Availability * 100:F2}%
        Performance: {Performance * 100:F2}%
        Quality: {Quality * 100:F2}%

        Overall OEE: {Oee * 100:F2}%";
}