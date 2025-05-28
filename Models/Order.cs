namespace Restaurant_Management_System.Models
{
	public class Order
	{
		public int Id { get; set; }
		public int TableNumber { get; set; }
		public DateTime OrderTime { get; set; }
		public decimal TotalAmount { get; set; }
		public string OrderStatus { get; set; } // Pending, Served, Completed
		public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
	}
}
