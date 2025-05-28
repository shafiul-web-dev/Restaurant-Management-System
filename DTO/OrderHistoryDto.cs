namespace Restaurant_Management_System.DTO
{
	public class OrderHistoryDto
	{
		public int Id { get; set; }
		public int TableNumber { get; set; }
		public DateTime OrderTime { get; set; }
		public decimal TotalAmount { get; set; }
		public string OrderStatus { get; set; }
	}
}
