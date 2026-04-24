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
	public class RateCardController : Controller
	{
		private readonly ApplicationDbContext _context;

		public RateCardController(ApplicationDbContext context)
		{
			_context = context;
		}

		// =======================
		// LIST
		// =======================
		public IActionResult Index()
		{
			var rates = _context.RateCards
				.Include(x => x.VehicleType)
				.Where(x => !x.IsDeleted)
				.ToList();

			return View(rates);
		}

		// =======================
		// CREATE GET
		// =======================
		public async Task<IActionResult> Create()
		{
			await LoadDropdowns();
			return View();
		}

		// =======================
		// CREATE POST
		// =======================
		[HttpPost]
		public async Task<IActionResult> Create(RateCard model)
		{
			await LoadDropdowns();

			if (!ModelState.IsValid)
				return View(model);

			// 🔥 BUSINESS LOGIC: Clean irrelevant fields
			if (model.TripType == TripType.Local)
			{
				model.OutstationPerDayKM = null;
				model.OutstationRatePerDay = null;
			}
			else if (model.TripType == TripType.Outstation)
			{
				model.BaseKM = null;
				model.BaseHours = null;
				model.BasePrice = null;
				//model.ExtraKMRate = null;
				//model.ExtraHourRate = null;
			}

			_context.RateCards.Add(model);
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		// =======================
		// EDIT GET
		// =======================
		public async Task<IActionResult> Edit(int id)
		{
			await LoadDropdowns();

			var rate = _context.RateCards.FirstOrDefault(x => x.Id == id);

			if (rate == null)
				return NotFound();

			return View(rate);
		}

		// =======================
		// EDIT POST
		// =======================
		[HttpPost]
		public async Task<IActionResult> Edit(RateCard model)
		{
			await LoadDropdowns();

			if (!ModelState.IsValid)
				return View(model);

			var existing = _context.RateCards.FirstOrDefault(x => x.Id == model.Id);

			if (existing == null)
				return NotFound();

			// 🔹 BASIC
			existing.Name = model.Name;
			existing.VehicleTypeId = model.VehicleTypeId;
			existing.TripType = model.TripType;

			// 🔥 HANDLE LOCAL / OUTSTATION
			if (model.TripType == TripType.Local)
			{
				existing.BaseKM = model.BaseKM;
				existing.BaseHours = model.BaseHours;
				existing.BasePrice = model.BasePrice;

				existing.ExtraKMRate = model.ExtraKMRate;
				existing.ExtraHourRate = model.ExtraHourRate;

				// Clear outstation
				existing.OutstationPerDayKM = null;
				existing.OutstationRatePerDay = null;
			}
			else
			{
				existing.OutstationPerDayKM = model.OutstationPerDayKM;
				existing.OutstationRatePerDay = model.OutstationRatePerDay;
				existing.ExtraKMRate = model.ExtraKMRate;
				existing.ExtraHourRate = model.ExtraHourRate;

				// Clear local
				existing.BaseKM = null;
				existing.BaseHours = null;
				existing.BasePrice = null;
			}

			// 🔹 COMMON
			existing.NightCharges = model.NightCharges;
			existing.DriverAllowancePerDay = model.DriverAllowancePerDay;

			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		// =======================
		// DELETE (SOFT DELETE)
		// =======================
		[HttpPost]
		public IActionResult Delete(int id)
		{
			var rate = _context.RateCards.FirstOrDefault(x => x.Id == id);

			if (rate == null)
				return NotFound();

			rate.IsDeleted = true;

			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		// =======================
		// COMMON METHOD
		// =======================
		private async Task LoadDropdowns()
		{
			ViewBag.VehicleTypes = new SelectList(
					await _context.VehicleTypes.ToListAsync(),
					"Id",
					"Name"
				);
		}
	}
}