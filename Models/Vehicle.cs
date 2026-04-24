using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharavaniTours.Models
{
	public class Vehicle : BaseEntity
	{
		public int Id { get; set; }
		public string VehicleNo { get; set; }
		public int VehicleTypeId { get; set; }

		[ValidateNever]
		public VehicleType VehicleType { get; set; }
		public string DriverId { get; set; }

		[ForeignKey("DriverId")]   // CORRECT WAY
		public ApplicationUser? Driver { get; set; }
	}
}
