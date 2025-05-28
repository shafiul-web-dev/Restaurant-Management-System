namespace Restaurant_Management_System.DTO
{
	public class OrderDto
	{
		public int Id { get; set; }
		public int TableNumber { get; set; }
		public DateTime OrderTime { get; set; }
		public decimal TotalAmount { get; set; }
		public string OrderStatus { get; set; }
		public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
	}
}
