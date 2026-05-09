using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharavaniTours.Data;
using SharavaniTours.Models;
using SharavaniTours.Models.ViewModels;

namespace SharavaniTours.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            DashboardVM vm = new DashboardVM();

            // =========================
            // TRIPS
            // =========================

            vm.TotalTrips = await _context.Trips.CountAsync();

            vm.PendingTrips = await _context.Trips
                .CountAsync(x => x.Status == TripStatus.Pending);

            vm.CompletedTrips = await _context.Trips
                .CountAsync(x => x.Status == TripStatus.Completed);

            vm.CancelledTrips = await _context.Trips
                .CountAsync(x => x.Status == TripStatus.Cancelled);

            // =========================
            // BILL COUNT SUMMARY
            // =========================

            vm.TotalBills = await _context.BillPayments.CountAsync();

            vm.CompletedBills = await _context.BillPayments
                .CountAsync(x => x.PaymentStatus == BillPaymentStatus.Paid);

            vm.PendingBills = await _context.BillPayments
                .CountAsync(x =>
                    x.PaymentStatus == BillPaymentStatus.Pending ||
                    x.PaymentStatus == BillPaymentStatus.Partial);

            // =========================
            // BILL AMOUNT SUMMARY
            // =========================

            vm.TotalBillAmount = await _context.BillPayments
                .SumAsync(x => (decimal?)x.FinalAmount) ?? 0;

            vm.TotalReceivedAmount = await _context.BillPayments
                .Where(x => x.PaymentStatus == BillPaymentStatus.Paid)
                .SumAsync(x => (decimal?)x.FinalAmount) ?? 0;

            vm.PendingAmount =
                vm.TotalBillAmount - vm.TotalReceivedAmount;

            // =========================
            // OTHER
            // =========================

            vm.TotalClients = await _context.Clients.CountAsync();

            vm.TotalVehicles = await _context.Vehicles.CountAsync();

            vm.TotalDrivers = await _context.Users.CountAsync();

            return View(vm);
        }

    }
}