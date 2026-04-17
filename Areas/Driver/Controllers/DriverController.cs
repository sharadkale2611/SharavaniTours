using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharavaniTours.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SharavaniTours.Models;
using SharavaniTours.Services;

namespace SharavaniTours.Areas.Driver.Controllers
{
	[Area("Driver")]
	[Authorize(Roles = "Driver,Admin")]
	public class DriverController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly BillingService _billing;

		public DriverController(ApplicationDbContext context, BillingService billing)
		{
			_context = context;
			_billing = billing;
		}

		// =======================
		// 🧾 DRIVER TRIPS
		// =======================
		public IActionResult MyTrips()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var trips = _context.Trips
				.Where(t => t.DriverId == userId
					  //&& (t.Status == "Pending" || t.Status == "Running")
					  )
				.Include(t => t.Client)
				.Include(t => t.DutySlip)
				.ToList();

			return View(trips);
		}

		// =======================
		// 🚀 START TRIP (GET)
		// =======================
		public IActionResult StartTrip(int id)
		{
			var trip = _context.Trips
				.Include(t => t.DutySlip)
				.FirstOrDefault(t => t.Id == id);

			if (trip == null) return NotFound();

			if (trip.Status == "Running" || trip.Status == "Completed")
			{
				TempData["Error"] = "Trip already started or completed!";
				return RedirectToAction("MyTrips");
			}

			return View(trip);
		}

		// =======================
		// 🚀 START TRIP (POST)
		// =======================
		[HttpPost]
		public IActionResult StartTrip(int id, int startKM)
		{
			var trip = _context.Trips
				.Include(t => t.DutySlip)
				.FirstOrDefault(t => t.Id == id);

			if (trip == null) return NotFound();

			if (trip.Status == "Running" || trip.Status == "Completed")
			{
				TempData["Error"] = "Trip already started!";
				return RedirectToAction("MyTrips");
			}

			// 🔥 Create DutySlip if not exists
			if (trip.DutySlip == null)
			{
				trip.DutySlip = new DutySlip
				{
					TripId = trip.Id,

					// ✅ DEFAULT VALUES (FIX FOR NULL ERROR)
					DutyType = "Local",
					PaymentMode = "Cash",
					ReportingTime = DateTime.Now,
					ReportingAddress = "N/A",
					NextDayInstruction = "N/A"
				};

				_context.DutySlips.Add(trip.DutySlip);
			}

			trip.DutySlip.StartKM = startKM;
			trip.DutySlip.StartTime = DateTime.Now;

			trip.Status = "Running";

			_context.SaveChanges();

			return RedirectToAction("MyTrips");
		}

		// =======================
		// 🛑 END TRIP (GET)
		// =======================
		public IActionResult EndTrip(int id)
		{
			var trip = _context.Trips
				.Include(t => t.DutySlip)
				.FirstOrDefault(t => t.Id == id);

			if (trip == null) return NotFound();

			if (trip.Status != "Running")
			{
				TempData["Error"] = "Trip must be started first!";
				return RedirectToAction("MyTrips");
			}

			return View(trip);
		}

		// =======================
		// 🛑 END TRIP (POST)
		// =======================
		[HttpPost]
		public IActionResult EndTrip(int id, int endKM, decimal tollCharges, decimal parkingCharges)
		{
			var trip = _context.Trips
				.Include(t => t.DutySlip)
				.FirstOrDefault(t => t.Id == id);

			if (trip == null) return NotFound();

			if (trip.Status != "Running")
			{
				TempData["Error"] = "Start trip first!";
				return RedirectToAction("MyTrips");
			}

			// 🔥 Validation
			if (endKM <= trip.DutySlip.StartKM)
			{
				TempData["Error"] = "End KM must be greater than Start KM!";
				return RedirectToAction("EndTrip", new { id });
			}

			trip.DutySlip.EndKM = endKM;
			trip.DutySlip.EndTime = DateTime.Now;
			trip.DutySlip.TollCharges = tollCharges;
			trip.DutySlip.ParkingCharges = parkingCharges;

			var rate = _context.RateCards.Find(trip.RateCardId);

			// 💰 Billing Calculation
			trip.TotalAmount = _billing.Calculate(trip, rate);

			trip.Status = "Completed";

			_context.SaveChanges();

			return RedirectToAction("Invoice", new { id = trip.Id });
		}

		// =======================
		// 🧾 INVOICE
		// =======================
		public IActionResult Invoice(int id)
		{
			var trip = _context.Trips
				.Include(t => t.Client)
				.Include(t => t.Vehicle)
				.Include(t => t.RateCard)
				.Include(t => t.DutySlip)
				.FirstOrDefault(t => t.Id == id);

			if (trip == null) return NotFound();

			return View(trip);
		}
	}
}