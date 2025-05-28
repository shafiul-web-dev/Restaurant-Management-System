namespace Restaurant_Management_System.Models
{
	public class MenuItem
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Category { get; set; } // Starter, Main Course, Dessert
		public decimal Price { get; set; }
		public bool IsAvailable { get; set; } // Availability status
	}
}
