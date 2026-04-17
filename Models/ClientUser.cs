using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharavaniTours.Models
{
	public class ClientUser : BaseEntity
	{
		public int Id { get; set; }

		public int ClientId { get; set; }

		[ForeignKey("ClientId")]   // ADD THIS
		[ValidateNever]
		public Client Client { get; set; }

		public string FullName { get; set; }
		public string MobileNo { get; set; }
	}
}
