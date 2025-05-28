namespace Restaurant_Management_System.DTO
{
	public class MenuItemDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Category { get; set; }
		public decimal Price { get; set; }
		public bool IsAvailable { get; set; }
	}
}
