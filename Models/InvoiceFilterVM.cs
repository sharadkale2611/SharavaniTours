using Microsoft.AspNetCore.Mvc.Rendering;

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

		// Dropdowns
		public IEnumerable<SelectListItem>? Drivers { get; set; }
		public IEnumerable<SelectListItem>? Vehicles { get; set; }
		public IEnumerable<SelectListItem>? ClientUsers { get; set; }
	}
}
