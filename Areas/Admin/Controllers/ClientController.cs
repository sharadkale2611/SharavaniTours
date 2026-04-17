using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharavaniTours.Data;
using SharavaniTours.Models;

namespace SharavaniTours.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class ClientController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ClientController(ApplicationDbContext context)
		{
			_context = context;
		}

		// LIST
		public IActionResult Index()
		{
			var clients = _context.Clients.ToList();
			return View(clients);
		}

		// CREATE GET
		public IActionResult Create()
		{
			return View();
		}

		// CREATE POST
		[HttpPost]
		public IActionResult Create(Client client)
		{
			if (!ModelState.IsValid)
				return View(client);

			_context.Clients.Add(client);
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		// EDIT GET
		public IActionResult Edit(int id)
		{
			var client = _context.Clients.Find(id);
			if (client == null) return NotFound();

			return View(client);
		}

		// EDIT POST
		[HttpPost]
		public IActionResult Edit(Client client)
		{
			if (!ModelState.IsValid)
				return View(client);

			_context.Clients.Update(client);
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		// DELETE
		public IActionResult Delete(int id)
		{
			var client = _context.Clients.Find(id);
			if (client == null) return NotFound();
			client.IsDeleted = true;
			//_context.Clients.Remove(client);
			_context.SaveChanges();

			return RedirectToAction("Index");
		}
	}
}