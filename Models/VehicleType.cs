namespace SharavaniTours.Models
{
	public class VehicleType
	{
		public int Id { get; set; }
		public string Name { get; set; } // Dzire, Ertiga
		public ICollection<Vehicle> Vehicles { get; set; }
		public ICollection<RateCard> RateCards { get; set; }
	}
}
