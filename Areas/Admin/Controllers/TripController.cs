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
			ViewBag.ClientUsers = new List<ClientUser>();

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

			trip.Status = "Pending";
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
				.Include(t => t.Vehicle)
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
				.Include(t => t.Driver)
				.Include(t => t.Vehicle)
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

			// ✅ Safe updates
			trip.ClientId = model.ClientId;
			trip.DriverId = model.DriverId;
			trip.VehicleId = model.VehicleId;
			trip.RateCardId = model.RateCardId;

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

		// =======================
		// 🔁 COMMON DROPDOWN LOADER
		// =======================
		private async Task LoadDropdowns()
		{
			ViewBag.Clients = _context.Clients.ToList();
			ViewBag.Vehicles = _context.Vehicles.ToList();
			ViewBag.RateCards = _context.RateCards.ToList();

			// ✅ Only drivers
			ViewBag.Drivers = await _userManager.GetUsersInRoleAsync("Driver");
		}
	}
}