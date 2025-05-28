using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;

namespace Restaurant_Management_System.Data
{
	public class ApplicationDbContext : DbContext
	{

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
		public DbSet<MenuItem> MenuItems { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }
		public DbSet<TableReservation> TableReservations { get; set; }

	}
}
