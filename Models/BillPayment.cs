using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharavaniTours.Models
{
    public enum BillPaymentStatus
    {
        Pending,
        Partial,
        Paid,
        Cancelled
    }

    public class BillPayment : BaseEntity
    {
        public int Id { get; set; }

        // =====================================================
        // 📄 BILL DETAILS
        // =====================================================

        [Required(ErrorMessage = "Bill date is required")]
        public DateTime BillDate { get; set; }

        [Required(ErrorMessage = "End location is required")]
        [StringLength(200)]
        public string Location { get; set; }

        [Required(ErrorMessage = "Vehicle number is required")]
        [StringLength(50)]
        public string VehicleNo { get; set; }

        [Required(ErrorMessage = "Vehicle type is required")]
        [StringLength(100)]
        public string VehicleType { get; set; }

        // =====================================================
        // 🔢 REFERENCES
        // =====================================================

        [Required(ErrorMessage = "Duty slip number is required")]
        [StringLength(100)]
        public string DutySlipNo { get; set; }

        [Required(ErrorMessage = "Bill number is required")]
        [StringLength(100)]
        public string BillNo { get; set; }

        // =====================================================
        // 💰 AMOUNT DETAILS
        // =====================================================

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TollParkingAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TDS { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalAmount { get; set; }

        // =====================================================
        // 💳 PAYMENT
        // =====================================================

        public BillPaymentStatus PaymentStatus { get; set; }
            = BillPaymentStatus.Pending;

        public DateTime? PaymentDate { get; set; }

        // =====================================================
        // 📝 OPTIONAL
        // =====================================================

        [StringLength(500)]
        public string? Remarks { get; set; }

        // =====================================================
        // 🔗 TRIP RELATION
        // =====================================================

        public int? TripId { get; set; }

        [ForeignKey("TripId")]
        [ValidateNever]
        public Trip? Trip { get; set; }
    }
}