namespace Restaurant_Management_System.DTO
{
	using System.ComponentModel.DataAnnotations;

	public class CreateOrderDto
	{
		[Required(ErrorMessage = "Table number is required.")]
		public int TableNumber { get; set; }

		[Required(ErrorMessage = "At least one item is required.")]
		public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
	}
}
