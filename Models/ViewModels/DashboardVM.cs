namespace SharavaniTours.Models.ViewModels
{
    public class DashboardVM
    {
        // TRIPS
        public int TotalTrips { get; set; }
        public int PendingTrips { get; set; }
        public int CompletedTrips { get; set; }
        public int CancelledTrips { get; set; }

        // BILLING
        public decimal TotalBillAmount { get; set; }
        public decimal TotalReceivedAmount { get; set; }
        public decimal PendingAmount { get; set; }

        // NEW
        public int TotalBills { get; set; }
        public int CompletedBills { get; set; }
        public int PendingBills { get; set; }

        // OTHER
        public int TotalClients { get; set; }
        public int TotalVehicles { get; set; }
        public int TotalDrivers { get; set; }
    }
}