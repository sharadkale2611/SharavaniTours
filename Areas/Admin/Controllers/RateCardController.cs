using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
			var rates = _context.RateCards.ToList(); // Soft delete filter applied
			return View(rates);
		}

		// =======================
		// CREATE GET
		// =======================
		public IActionResult Create()
		{
			return View();
		}

		// =======================
		// CREATE POST
		// =======================
		[HttpPost]
		public IActionResult Create(RateCard model)
		{
			if (!ModelState.IsValid)
				return View(model);

			_context.RateCards.Add(model);
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		// =======================
		// EDIT GET
		// =======================
		public IActionResult Edit(int id)
		{
			var rate = _context.RateCards.FirstOrDefault(x => x.Id == id);

			if (rate == null)
				return NotFound();

			return View(rate);
		}

		// =======================
		// EDIT POST
		// =======================
		[HttpPost]
		public IActionResult Edit(RateCard model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var existing = _context.RateCards.FirstOrDefault(x => x.Id == model.Id);

			if (existing == null)
				return NotFound();

			// ✅ Update fields safely
			existing.Name = model.Name;

			existing.BaseKM = model.BaseKM;
			existing.BaseHours = model.BaseHours;
			existing.BasePrice = model.BasePrice;

			existing.ExtraKMRate = model.ExtraKMRate;
			existing.ExtraHourRate = model.ExtraHourRate;

			existing.OutstationPerDayKM = model.OutstationPerDayKM;
			existing.OutstationRatePerDay = model.OutstationRatePerDay;

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

			// ✅ Soft delete
			rate.IsDeleted = true;

			_context.SaveChanges();

			return RedirectToAction("Index");
		}
	}
}