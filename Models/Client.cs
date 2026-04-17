using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharavaniTours.Models
{
	public class Client : BaseEntity
	{
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public string GST { get; set; }

		public string Address { get; set; }

		[ValidateNever]
		public List<ClientUser> Users { get; set; }
	}


}