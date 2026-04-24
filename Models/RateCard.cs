
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SharavaniTours.Models
{
	public enum TripType
	{
		Local = 1,
			Outstation = 2
		}

		public class RateCard : BaseEntity
		{
			public int Id { get; set; }

			// 🔹 Basic Info
			public string Name { get; set; }

			// ✅ Vehicle Type (NOT Vehicle)
			public int VehicleTypeId { get; set; }

			[ValidateNever]
			public VehicleType VehicleType { get; set; }

		// 🔹 Trip Type (Local / Outstation)
		public TripType TripType { get; set; }

			// ================= LOCAL PRICING =================
			public int? BaseKM { get; set; }          // e.g. 80 KM
			public int? BaseHours { get; set; }       // e.g. 8 hours
			public decimal? BasePrice { get; set; }   // e.g. ₹2000

			public decimal? ExtraKMRate { get; set; }     // ₹ per extra KM
			public decimal? ExtraHourRate { get; set; }   // ₹ per extra hour

			// ================= OUTSTATION PRICING =================
			public decimal? OutstationPerDayKM { get; set; } = 300;   // Min KM per day
			public decimal? OutstationRatePerDay { get; set; }        // ₹ per KM OR per day

		// 🔹 Optional Extras (VERY USEFUL IN REAL SYSTEM)
		public decimal? NightCharges { get; set; } = 0;
			public decimal? DriverAllowancePerDay { get; set; }

			// 🔹 Status
			public bool IsActive { get; set; } = true;
		}
	}