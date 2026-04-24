using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharavaniTours.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SharavaniTours.Models;
using SharavaniTours.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SharavaniTours.Areas.Driver.Controllers
{
	[Area("Driver")]
	[Authorize(Roles = "Driver,Admin")]
	public class DriverController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly BillingService _billing;
		private readonly UserManager<ApplicationUser> _userManager;


		public DriverController(ApplicationDbContext context, BillingService billing, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_billing = billing;
			_userManager = userManager;
		}

		// =======================
		// 🧾 DRIVER TRIPS
		// =======================
		public IActionResult MyTrips()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var trips = _context.Trips
				.Where(t => t.DriverId == userId)
				.Include(t => t.Client)
				.Include(t => t.DutySlip)
				.Include(t => t.RequestedVehicle)
				.Include(t => t.SentVehicle)
					.ThenInclude(v => v.VehicleType) // 🔥 FIX
				.ToList();

			ViewBag.Vehicles = _context.Vehicles
				.Include(v => v.VehicleType) // 🔥 FIX
				.ToList();

			return View(trips);
		}

		// =======================
		// DETAILS
		// =======================
		public IActionResult Details(int id)
		{
			var trip = _context.Trips
				.Include(t => t.Client)
				.Include(t => t.ClientUser)
				.Include(t => t.Driver)
				.Include(t => t.RequestedVehicle)
				.Include(t => t.SentVehicle)
					.ThenInclude(v => v.VehicleType)
				.Include(t => t.RateCard)
				.Include(t => t.DutySlip)
				.FirstOrDefault(t => t.Id == id);

			if (trip == null)
				return NotFound();

			return View(trip);
		}

		// =======================
		// 🚀 START TRIP (GET)
		// =======================
		public IActionResult StartTrip(int id)
		{
			var trip = _context.Trips
				.Include(t => t.DutySlip)
				.Include(t => t.RequestedVehicle)
				.FirstOrDefault(t => t.Id == id);

			if (trip == null) return NotFound();

			if (trip.Status == TripStatus.Started || trip.Status == TripStatus.Completed)
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
		public IActionResult StartTrip(int id, int startKM, int? sentVehicleId)
		{
			var trip = _context.Trips
				.Include(t => t.DutySlip)
				.Include(t => t.RequestedVehicle)
				.FirstOrDefault(t => t.Id == id);

			if (trip == null) return NotFound();

			if (trip.Status == TripStatus.Started || trip.Status == TripStatus.Completed)
			{
				TempData["Error"] = "Trip already started!";
				return RedirectToAction("MyTrips");
			}

			int finalVehicleId;

			if (sentVehicleId.HasValue)
			{
				finalVehicleId = sentVehicleId.Value;
			}
			else if (trip.RequestedVehicleId.HasValue)
			{
				finalVehicleId = trip.RequestedVehicleId.Value;
			}
			else
			{
				TempData["Error"] = "No vehicle available!";
				return RedirectToAction("MyTrips");
			}

			// ✅ Type validation
			if (trip.RequestedVehicleId != null)
			{
				var requested = trip.RequestedVehicle;
				var selected = _context.Vehicles.Find(finalVehicleId);

				if (requested != null && selected != null &&
					requested.VehicleTypeId != selected.VehicleTypeId)
				{
					TempData["Error"] = "Vehicle type mismatch!";
					return RedirectToAction("MyTrips");
				}
			}

			trip.SentVehicleId = finalVehicleId;

			// ✅ Ensure DutySlip exists
			if (trip.DutySlip == null)
			{
				trip.DutySlip = new DutySlip
				{
					TripId = trip.Id,
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

			trip.Status = TripStatus.Started;

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

			if (trip.Status != TripStatus.Started)
			{
				TempData["Error"] = "Trip must be started first!";
				return RedirectToAction("MyTrips");
			}

			if (trip.DutySlip == null)
			{
				TempData["Error"] = "Duty slip missing!";
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

			if (trip.Status != TripStatus.Started)
			{
				TempData["Error"] = "Start trip first!";
				return RedirectToAction("MyTrips");
			}

			if (trip.DutySlip == null)
			{
				TempData["Error"] = "Duty slip missing!";
				return RedirectToAction("MyTrips");
			}

			if (endKM <= trip.DutySlip.StartKM)
			{
				TempData["Error"] = "End KM must be greater!";
				return RedirectToAction("EndTrip", new { id });
			}

			trip.DutySlip.EndKM = endKM;
			trip.DutySlip.EndTime = DateTime.Now;
			trip.DutySlip.TollCharges = tollCharges;
			trip.DutySlip.ParkingCharges = parkingCharges;

			var rate = _context.RateCards.Find(trip.RateCardId);

			if (rate == null)
			{
				TempData["Error"] = "Rate card missing!";
				return RedirectToAction("MyTrips");
			}

			// 💰 Billing
			trip.TotalAmount = _billing.Calculate(trip, rate);

			trip.Status = TripStatus.Completed;

			_context.SaveChanges();

			return RedirectToAction("Invoice", new { id = trip.Id });
		}

		// =======================
		// 🧾 INVOICE
		// =======================
		public IActionResult Invoice(int id)
		{
			var trip = _context.Trips
				.Include(x => x.DutySlip)
				.Include(x => x.SentVehicle)
					.ThenInclude(v => v.VehicleType) // 🔥 FIX
				.Include(x => x.Driver)
				.Include(x => x.Client)
				.Include(x => x.ClientUser)
				.Include(x => x.RateCard)
				.FirstOrDefault(t => t.Id == id);

			if (trip == null) return NotFound();

			return View(trip);
		}

		// =======================
		// 📊 REPORT
		// =======================

		public async Task<IActionResult> InvoiceReport(InvoiceFilterVM filter)
		{
			var query = _context.Trips
				.Include(x => x.DutySlip)
				.Include(x => x.SentVehicle).ThenInclude(v => v.VehicleType)
				.Include(x => x.Driver)
				.Include(x => x.Client)
				.Include(x => x.ClientUser)
				.Include(x => x.RateCard)
				.Where(x => x.Status == TripStatus.Completed);

			// ✅ Filters
			if (filter.FromDate.HasValue)
				query = query.Where(x => x.BookedDate >= filter.FromDate);

			if (filter.ToDate.HasValue)
				query = query.Where(x => x.BookedDate <= filter.ToDate);

			if (!string.IsNullOrEmpty(filter.DriverId))
				query = query.Where(x => x.DriverId == filter.DriverId);

			if (filter.VehicleId.HasValue)
				query = query.Where(x => x.SentVehicleId == filter.VehicleId);

			if (filter.ClientUserId.HasValue)
				query = query.Where(x => x.ClientUserId == filter.ClientUserId);

			// ✅ Data
			filter.Trips = await query
				.OrderByDescending(x => x.BookedDate)
				.ToListAsync();

			// =========================
			// 🔽 DROPDOWNS
			// =========================

			// ✅ Drivers (SAFE FIX)
			var drivers = await _userManager.GetUsersInRoleAsync("Driver");

			filter.Drivers = drivers.Select(u => new SelectListItem
			{
				Value = u.Id,
				Text = u.FullName
			});

			// ✅ Vehicles
			filter.Vehicles = await _context.Vehicles
				.Select(v => new SelectListItem
				{
					Value = v.Id.ToString(),
					Text = v.VehicleNo
				})
				.ToListAsync();

			// ✅ Client Users
			filter.ClientUsers = await _context.ClientUsers
				.Select(c => new SelectListItem
				{
					Value = c.Id.ToString(),
					Text = c.FullName
				})
				.ToListAsync();

			return View(filter);
		}



	}
}