namespace SharavaniTours.Models
{
	public class InvoiceFilterVM
	{
		public DateTime? FromDate { get; set; }
		public DateTime? ToDate { get; set; }

		public string? DriverId { get; set; }
		public int? VehicleId { get; set; }
		public int? ClientUserId { get; set; }

		public List<Trip>? Trips { get; set; }
	}
}
