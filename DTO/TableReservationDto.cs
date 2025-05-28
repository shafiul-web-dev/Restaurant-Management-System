namespace Restaurant_Management_System.DTO
{
	public class TableReservationDto
	{
		public int Id { get; set; }
		public int TableNumber { get; set; }
		public string CustomerName { get; set; }
		public DateTime ReservationDateTime { get; set; }
		public int NumberOfGuests { get; set; }
	}
}
