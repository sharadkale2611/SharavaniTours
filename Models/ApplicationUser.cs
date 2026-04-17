using Microsoft.AspNetCore.Identity;

namespace SharavaniTours.Models
{
	public class ApplicationUser: IdentityUser
	{
		public string FullName { get; set; }
	}
}
