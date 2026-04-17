using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SharavaniTours.Data;
using SharavaniTours.Models;

namespace SharavaniTours.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class VehicleController : Controller
	{
		private readonly ApplicationDbContext _context;

		public VehicleController(ApplicationDbContext context)
		{
			_context = context;
		}

		// =======================
		// LIST
		// =======================
		public IActionResult Index()
		{
			var vehicles = _context.Vehicles
				.Include(v => v.Driver)
				.ToList();

			return View(vehicles);
		}

		// =======================
		// CREATE (GET)
		// =======================
		public IActionResult Create()
		{
			LoadDrivers();
			return View();
		}

		// =======================
		// CREATE (POST)
		// =======================
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(Vehicle vehicle)
		{
			if (!ModelState.IsValid)
			{
				LoadDrivers();
				return View(vehicle);
			}

			_context.Vehicles.Add(vehicle);
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		// =======================
		// EDIT (GET)
		// =======================
		public IActionResult Edit(int id)
		{
			var vehicle = _context.Vehicles.Find(id);

			if (vehicle == null)
				return NotFound();

			LoadDrivers(vehicle.DriverId);

			return View(vehicle);
		}

		// =======================
		// EDIT (POST)
		// =======================
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(Vehicle model)
		{
			if (!ModelState.IsValid)
			{
				LoadDrivers(model.DriverId);
				return View(model);
			}

			var vehicle = _context.Vehicles.FirstOrDefault(x => x.Id == model.Id);

			if (vehicle == null)
				return NotFound();

			// ✅ Safe update
			vehicle.VehicleNo = model.VehicleNo;
			vehicle.Type = model.Type;
			vehicle.DriverId = model.DriverId;

			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		// =======================
		// DELETE (SOFT DELETE)
		// =======================
		[HttpPost]
		public IActionResult Delete(int id)
		{
			var vehicle = _context.Vehicles.FirstOrDefault(x => x.Id == id);

			if (vehicle == null)
				return NotFound();

			// ✅ Soft delete (BaseEntity)
			vehicle.IsDeleted = true;

			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		// =======================
		// DETAILS (OPTIONAL BUT USEFUL)
		// =======================
		public IActionResult Details(int id)
		{
			var vehicle = _context.Vehicles
				.Include(v => v.Driver)
				.FirstOrDefault(x => x.Id == id);

			if (vehicle == null)
				return NotFound();

			return View(vehicle);
		}

		// =======================
		// 🔁 COMMON DRIVER DROPDOWN
		// =======================
		private void LoadDrivers(string? selectedDriverId = null)
		{
			var drivers = _context.Users.ToList();

			ViewBag.DriverList = new SelectList(
				drivers,
				"Id",
				"Email",
				selectedDriverId
			);
		}
	}
}