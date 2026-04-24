using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;


namespace SharavaniTours.Models
{
	public enum TripStatus
	{
		Pending,
		Assigned,
		Started,
		Completed,
		Cancelled
	}

	public class Trip : BaseEntity
	{
		public int Id { get; set; }

		// 🔹 Client
		[Required(ErrorMessage = "Please select client")]
		public int ClientId { get; set; }

		[ValidateNever]
		public Client? Client { get; set; }

		// 🔹 Driver & Vehicle
		[Required(ErrorMessage = "Please select driver")]
		public string DriverId { get; set; }

		[ValidateNever]
		public ApplicationUser? Driver { get; set; }

		// 🔹 Rate
		[Required(ErrorMessage = "Please select rate card")]
		public int RateCardId { get; set; }

		[ValidateNever]
		public RateCard? RateCard { get; set; }

		public TripType TripType { get; set; }

		public int? RequestedVehicleId { get; set; }

		[ValidateNever]
		public Vehicle? RequestedVehicle { get; set; }

		public int? SentVehicleId { get; set; }

		[ValidateNever]
		public Vehicle? SentVehicle { get; set; }

		// 🔹 Locations
		[Required(ErrorMessage = "Pickup location is required")]
		public string PickupLocation { get; set; }

		[Required(ErrorMessage = "Drop location is required")]
		public string DropLocation { get; set; }


		// 🔹 Timing
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		// 🔹 Booking
		public string BookedBy { get; set; }
		public DateTime BookedDate { get; set; } = DateTime.Now;

		// 🔹 Pricing
		public decimal DriverAllowance { get; set; }
		public decimal TotalAmount { get; set; }

		public TripStatus Status { get; set; } = TripStatus.Pending;

		// 🔹 Navigation (DO NOT VALIDATE)
		[ValidateNever]
		public DutySlip? DutySlip { get; set; }

		// 🔹 Optional
		public int? ClientUserId { get; set; }

		[ValidateNever]
		public ClientUser? ClientUser { get; set; }

		// 🔹 System Generated
		[ValidateNever]
		public string? ItineraryCode { get; set; }
	}
}