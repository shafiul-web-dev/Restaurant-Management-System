namespace Restaurant_Management_System.Models
{
	public class TableReservation
	{
		public int Id { get; set; }
		public int TableNumber { get; set; }
		public string CustomerName { get; set; }
		public DateTime ReservationDateTime { get; set; }
		public int NumberOfGuests { get; set; }
	}
}
