using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SharavaniTours.Models
{

	public class Trip : BaseEntity
	{
		public int Id { get; set; }

		// 🔹 Client Info
		public int ClientId { get; set; }

		[ValidateNever]
		public Client Client { get; set; }

		// 🔹 Driver Info
		public string DriverId { get; set; }

		[ValidateNever]
		public ApplicationUser Driver { get; set; }

		// 🔹 Vehicle Info
		public int VehicleId { get; set; }

		[ValidateNever]
		public Vehicle Vehicle { get; set; }

		// 🔹 Rate Info
		public int RateCardId { get; set; }

		[ValidateNever]
		public RateCard RateCard { get; set; }

		// 🔹 Booking Details
		[ValidateNever]
		public string ItineraryCode { get; set; }
		public string BookedBy { get; set; }
		public DateTime BookedDate { get; set; } = DateTime.Now;

		// 🔹 Optional: Client Contact Person
		public int? ClientUserId { get; set; }

		[ValidateNever]
		public ClientUser ClientUser { get; set; }

		// 🔹 Pricing
		public decimal DriverAllowance { get; set; }
		public decimal TotalAmount { get; set; }

		// 🔹 Status
		public string Status { get; set; } = "Pending";

		// 🔹 Navigation
		[ValidateNever]
		public DutySlip DutySlip { get; set; }
	}

}