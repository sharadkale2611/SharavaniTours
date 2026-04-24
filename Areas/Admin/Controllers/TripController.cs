using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharavaniTours.Data;
using SharavaniTours.Models;

namespace SharavaniTours.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class TripController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public TripController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}


		// =======================
		// CREATE (GET)
		// =======================
		public async Task<IActionResult> Create()
		{
			await LoadDropdowns();


			// ✅ Prevent null error
			//ViewBag.ClientUsers = new List<ClientUser>();

			return View();
		}

		// =======================
		// CREATE (POST)
		// =======================
		[HttpPost]
		public async Task<IActionResult> Create(Trip trip)
		{
			if (!ModelState.IsValid)
			{
				await LoadDropdowns();

				ViewBag.ClientUsers = _context.ClientUsers
					.Where(x => x.ClientId == trip.ClientId)
					.ToList();

				return View(trip);
			}

			trip.Status = TripStatus.Pending;
			trip.BookedDate = DateTime.Now;
			trip.ItineraryCode = "ITN-" + DateTime.Now.Ticks;

			_context.Trips.Add(trip);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		// =======================
		// LIST
		// =======================
		public IActionResult Index()
		{
			var trips = _context.Trips
				.Include(t => t.Client)
				.Include(t => t.Driver)
				.Include(t => t.RequestedVehicle)
				.Include(t => t.SentVehicle)
				.Include(t => t.RateCard)
				.Include(t => t.DutySlip)
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
				.Include(t => t.RateCard)
				.Include(t => t.DutySlip)
				.FirstOrDefault(t => t.Id == id);

			if (trip == null)
				return NotFound();

			return View(trip);
		}

		// =======================
		// EDIT (GET)
		// =======================
		public async Task<IActionResult> Edit(int id)
		{
			var trip = _context.Trips.FirstOrDefault(x => x.Id == id);

			if (trip == null)
				return NotFound();

			await LoadDropdowns();

			// ✅ Load ClientUsers based on selected client
			ViewBag.ClientUsers = _context.ClientUsers
				.Where(x => x.ClientId == trip.ClientId)
				.ToList();

			return View(trip);
		}

		// =======================
		// EDIT (POST)
		// =======================
		[HttpPost]
		public async Task<IActionResult> Edit(Trip model)
		{
			if (!ModelState.IsValid)
			{
				await LoadDropdowns();

				ViewBag.ClientUsers = _context.ClientUsers
					.Where(x => x.ClientId == model.ClientId)
					.ToList();

				return View(model);
			}

			var trip = _context.Trips.FirstOrDefault(x => x.Id == model.Id);

			if (trip == null)
				return NotFound();

			// ✅ Allowed updates
			trip.ClientId = model.ClientId;
			trip.DriverId = model.DriverId;
			trip.RateCardId = model.RateCardId;

			// ✅ Only requested vehicle editable
			trip.RequestedVehicleId = model.RequestedVehicleId;

			// ❌ DO NOT TOUCH SentVehicleId here

			trip.ClientUserId = model.ClientUserId;
			trip.BookedBy = model.BookedBy;

			trip.DriverAllowance = model.DriverAllowance;
			trip.TotalAmount = model.TotalAmount;

			trip.Status = model.Status;

			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}
		// =======================
		// DELETE (SOFT DELETE)
		// =======================
		[HttpPost]
		public IActionResult Delete(int id)
		{
			var trip = _context.Trips.FirstOrDefault(x => x.Id == id);

			if (trip == null)
				return NotFound();

			trip.IsDeleted = true;

			_context.SaveChanges();

			return RedirectToAction("Index");
		}


		[HttpGet]
		public IActionResult GetRateCardsByVehicle(int vehicleId)
		{
			var vehicle = _context.Vehicles
				.FirstOrDefault(v => v.Id == vehicleId);

			if (vehicle == null)
				return Json(new List<object>());

			var rateCards = _context.RateCards
				.Where(r => r.VehicleTypeId == vehicle.VehicleTypeId && !r.IsDeleted)
				.Select(r => new
				{
					r.Id,
					r.Name,
					r.TripType,
					Price = r.TripType == TripType.Local
						? r.BasePrice
						: r.OutstationRatePerDay,
					DriverAllowance = r.DriverAllowancePerDay // ✅ ADD THIS
				})
				.ToList();

			return Json(rateCards);
		}

		// =======================
		// 🔁 COMMON DROPDOWN LOADER
		// =======================
		private async Task LoadDropdowns()
		{
			ViewBag.Clients = _context.Clients.ToList();

			ViewBag.ClientUsers = _context.ClientUsers.Any()
				? _context.ClientUsers.ToList()
				: new List<ClientUser>();

			ViewBag.Vehicles = _context.Vehicles
				.Include(v => v.VehicleType) // 🔥 MUST
				.ToList();

			ViewBag.RateCards = new List<RateCard>(); // ❌ not preloaded anymore

			ViewBag.Drivers = await _userManager.GetUsersInRoleAsync("Driver");
		}
	}
}