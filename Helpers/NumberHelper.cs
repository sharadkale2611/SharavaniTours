namespace SharavaniTours.Helpers
{
	public static class NumberHelper
	{
		public static string NumberToWords(int number)
		{
			if (number == 0) return "Zero";

			string[] units = { "", "One", "Two", "Three", "Four", "Five", "Six",
						   "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve",
						   "Thirteen", "Fourteen", "Fifteen", "Sixteen",
						   "Seventeen", "Eighteen", "Nineteen" };

			string[] tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty",
						  "Sixty", "Seventy", "Eighty", "Ninety" };

			if (number < 20)
				return units[number];

			if (number < 100)
				return tens[number / 10] + " " + units[number % 10];

			if (number < 1000)
				return units[number / 100] + " Hundred " + NumberToWords(number % 100);

			if (number < 100000)
				return NumberToWords(number / 1000) + " Thousand " + NumberToWords(number % 1000);

			if (number < 10000000)
				return NumberToWords(number / 100000) + " Lakh " + NumberToWords(number % 100000);

			return NumberToWords(number / 10000000) + " Crore " + NumberToWords(number % 10000000);
		}
	}
}
