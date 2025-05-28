namespace SmartRide.Models
{
    public class Report
    {
        public int ReportId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalRides { get; set; }
        public decimal TotalRevenue { get; set; }
        public DateTime GenerateDate { get; set; }
    }
}