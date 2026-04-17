using Microsoft.AspNetCore.Mvc;
using SharavaniTours.Services;

namespace SharavaniTours.Controllers
{
	public class SystemController : Controller
	{
		private readonly DatabaseResetService _resetService;

		public SystemController(DatabaseResetService resetService)
		{
			_resetService = resetService;
		}

		public IActionResult Reset()
		{
			return View(); // confirmation page
		}

		[HttpPost]
		public async Task<IActionResult> ResetConfirmed()
		{
			await _resetService.ResetDatabaseAsync();

			return RedirectToAction("Index", "Home");
		}
	}
}
