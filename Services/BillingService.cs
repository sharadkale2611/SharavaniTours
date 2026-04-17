using SharavaniTours.Models;

namespace SharavaniTours.Services
{
	public class BillingService
	{
		public decimal Calculate(Trip trip, RateCard rate)
		{
			if (trip.DutySlip == null)
				return 0;

			// 🔹 KM Calculation
			var totalKm = trip.DutySlip.EndKM - trip.DutySlip.StartKM;

			// 🔹 Time Calculation
			var totalHours = (decimal)(trip.DutySlip.EndTime - trip.DutySlip.StartTime).TotalHours;

			// 🔹 Extra Calculations
			var extraKm = Math.Max(0, totalKm - rate.BaseKM);
			var extraHours = Math.Max(0, totalHours - rate.BaseHours);

			// 🔹 Base Billing
			var total =
				rate.BasePrice +
				(extraKm * rate.ExtraKMRate) +
				(extraHours * rate.ExtraHourRate);

			// 🔥 ADDITIONAL CHARGES
			total += trip.DriverAllowance;                       // From Trip (Admin)
			total += trip.DutySlip.TollCharges;                  // From Driver
			total += trip.DutySlip.ParkingCharges;               // From Driver

			return total;
		}
	}
}