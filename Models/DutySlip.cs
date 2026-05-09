using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace SharavaniTours.Models
{
    public class DutySlip : BaseEntity
    {
        public int Id { get; set; }

        // 🔹 Trip Relation
        public int TripId { get; set; }

        [ValidateNever]
        public Trip? Trip { get; set; }

        // 🔹 Auto Generated Slip Number
        [Required]
        public string DutySlipNo { get; set; }

        // =====================================================
        // 🚀 TRIP EXECUTION
        // =====================================================

        public int StartKM { get; set; }

        public int EndKM { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        // =====================================================
        // 💰 CHARGES
        // =====================================================

        public decimal TollCharges { get; set; }

        public decimal ParkingCharges { get; set; }

        // =====================================================
        // 📍 REPORTING DETAILS
        // =====================================================

        public DateTime ReportingTime { get; set; }

        public string ReportingAddress { get; set; }

        // =====================================================
        // 📄 DUTY DETAILS
        // =====================================================

        public string DutyType { get; set; }

        public string PaymentMode { get; set; }

        public string NextDayInstruction { get; set; }
    }
}