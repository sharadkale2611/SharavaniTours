namespace SharavaniTours.Models
{
	public class DutySlip : BaseEntity
	{
		public int Id { get; set; }

		public int TripId { get; set; }
		public Trip Trip { get; set; }

		// 🔹 Trip execution
		public int StartKM { get; set; }
		public int EndKM { get; set; }

		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }

		// 🔹 Charges
		public decimal TollCharges { get; set; }
		public decimal ParkingCharges { get; set; }

		// 🔹 Duty Details
		public DateTime ReportingTime { get; set; }
		public string ReportingAddress { get; set; }

		public string DutyType { get; set; }
		public string PaymentMode { get; set; }

		public string NextDayInstruction { get; set; }
	}
}
