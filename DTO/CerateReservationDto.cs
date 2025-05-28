namespace Restaurant_Management_System.DTO
{
	using System.ComponentModel.DataAnnotations;

	public class CreateReservationDto
	{
		[Required(ErrorMessage = "Table number is required.")]
		public int TableNumber { get; set; }

		[Required(ErrorMessage = "Customer name is required.")]
		public string CustomerName { get; set; }

		[Required(ErrorMessage = "Reservation date and time are required.")]
		public DateTime ReservationDateTime { get; set; }

		[Required(ErrorMessage = "Number of guests is required.")]
		[Range(1, 10, ErrorMessage = "Number of guests must be between 1 and 10.")]
		public int NumberOfGuests { get; set; }
	}
}
