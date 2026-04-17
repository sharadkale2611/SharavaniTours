using System.Security.Claims;

namespace SharavaniTours.Helpers
{
	public static class AccountRedirectHelper
	{
		public static class RedirectHelper
		{
			public static string GetRedirectUrl(ClaimsPrincipal user)
			{
				if (user.IsInRole("Admin"))
					return "/Admin/Dashboard";

				if (user.IsInRole("Driver"))
					return "/Driver/MyTrips";

				return "/";
			}
		}
	}
}
