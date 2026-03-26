namespace Service.DTOs.DashBoardDto
{
    public class DiseaseHeatmapDto
    {
        public string DiseaseName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public int Count { get; set; }
    }
}
