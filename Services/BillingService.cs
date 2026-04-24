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

			// 🔹 Safe defaults for nullable values
			var baseKm = rate.BaseKM ?? 0;
			var baseHours = rate.BaseHours ?? 0;
			var basePrice = rate.BasePrice ?? 0;

			var extraKmRate = rate.ExtraKMRate ?? 0;
			var extraHourRate = rate.ExtraHourRate ?? 0;

			// 🔹 Extra Calculations
			var extraKm = Math.Max(0, totalKm - baseKm);
			var extraHours = Math.Max(0, totalHours - baseHours);

			// 🔹 Base Billing
			var total =
				basePrice +
				(extraKm * extraKmRate) +
				(extraHours * extraHourRate);

			// 🔥 ADDITIONAL CHARGES
			total += trip.DriverAllowance;
			total += trip.DutySlip.TollCharges;
			total += trip.DutySlip.ParkingCharges;

			return total;
		}
	}
}