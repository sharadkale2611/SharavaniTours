namespace SharavaniTours.Models
{
	public class RateCard : BaseEntity
	{
		public int Id { get; set; }
		public string Name { get; set; } // Local / Outstation

		public int BaseKM { get; set; }
		public int BaseHours { get; set; }
		public decimal BasePrice { get; set; }

		public decimal ExtraKMRate { get; set; }
		public decimal ExtraHourRate { get; set; }
		public decimal OutstationPerDayKM { get; set; } = 300;
		public decimal OutstationRatePerDay { get; set; }
	}
}
