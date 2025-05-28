namespace Restaurant_Management_System.DTO
{
	using System.ComponentModel.DataAnnotations;

	public class CreateOrderItemDto
	{
		[Required(ErrorMessage = "MenuItemId is required.")]
		public int MenuItemId { get; set; }

		[Required(ErrorMessage = "Quantity must be at least 1.")]
		[Range(1, 10, ErrorMessage = "You can order between 1 and 10 items.")]
		public int Quantity { get; set; }
	}
}
