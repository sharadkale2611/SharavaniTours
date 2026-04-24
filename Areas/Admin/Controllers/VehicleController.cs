using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
		private readonly UserManager<ApplicationUser> _userManager;

		public VehicleController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		// =======================
		// LIST
		// =======================
		public IActionResult Index()
		{
			var vehicles = _context.Vehicles
				.Include(t => t.VehicleType)
				.Include(v => v.Driver)
				.ToList();

			return View(vehicles);
		}

		// =======================
		// CREATE (GET)
		// =======================
		public async Task<IActionResult> Create()
		{
			await LoadDrivers();
			return View();
		}

		// =======================
		// CREATE (POST)
		// =======================
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Vehicle vehicle)
		{
			if (!ModelState.IsValid)
			{
				await LoadDrivers();
				return View(vehicle);
			}

			_context.Vehicles.Add(vehicle);
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		// =======================
		// EDIT (GET)
		// =======================
		public async Task<IActionResult> Edit(int id)
		{
			var vehicle = _context.Vehicles.Find(id);

			if (vehicle == null)
				return NotFound();

			await LoadDrivers(vehicle.DriverId);

			return View(vehicle);
		}

		// =======================
		// EDIT (POST)
		// =======================
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Vehicle model)
		{
			if (!ModelState.IsValid)
			{
				await LoadDrivers(model.DriverId);
				return View(model);
			}

			var vehicle = _context.Vehicles.FirstOrDefault(x => x.Id == model.Id);

			if (vehicle == null)
				return NotFound();

			// ✅ Safe update
			vehicle.VehicleNo = model.VehicleNo;
			vehicle.VehicleTypeId = model.VehicleTypeId;
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
		private async Task LoadDrivers(string? selectedDriverId = null)
		{
			var driverRoleId = await _context.Roles
				.Where(r => r.Name == "Driver")
				.Select(r => r.Id)
				.FirstOrDefaultAsync();

			var drivers = await (from user in _context.Users
								 join userRole in _context.UserRoles
									 on user.Id equals userRole.UserId
								 where userRole.RoleId == driverRoleId
								 select user)
								 .ToListAsync();

			ViewBag.DriverList = new SelectList(
				drivers,
				"Id",
				"FullName",
				selectedDriverId
			);

			ViewBag.VehicleTypes = new SelectList(
					await _context.VehicleTypes.ToListAsync(),
					"Id",
					"Name"
				);
		}
	}
}