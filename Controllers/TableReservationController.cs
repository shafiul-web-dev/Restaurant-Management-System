using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Data;
using Restaurant_Management_System.DTO;
using Restaurant_Management_System.Models;

namespace RestaurantAPI.Controllers
{
	[Route("api/reservations")]
	[ApiController]
	public class TableReservationController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public TableReservationController(ApplicationDbContext context)
		{
			_context = context;
		}

		
		[HttpGet]
		public async Task<ActionResult<IEnumerable<TableReservationDto>>> GetReservations(
			[FromQuery] int? tableNumber,
			[FromQuery] string sortBy = "ReservationDateTime",
			[FromQuery] string sortDirection = "asc",
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10)
		{
			var query = _context.TableReservations.AsQueryable();

			
			if (tableNumber.HasValue)
			{
				query = query.Where(r => r.TableNumber == tableNumber);
			}

			
			query = sortBy switch
			{
				"CustomerName" => sortDirection == "asc" ? query.OrderBy(r => r.CustomerName) : query.OrderByDescending(r => r.CustomerName),
				"ReservationDateTime" => sortDirection == "asc" ? query.OrderBy(r => r.ReservationDateTime) : query.OrderByDescending(r => r.ReservationDateTime),
				_ => query
			};

			
			var totalRecords = await query.CountAsync();
			var reservations = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize)
				.Select(r => new TableReservationDto
				{
					Id = r.Id,
					TableNumber = r.TableNumber,
					CustomerName = r.CustomerName,
					ReservationDateTime = r.ReservationDateTime,
					NumberOfGuests = r.NumberOfGuests
				})
				.ToListAsync();

			return Ok(new { TotalRecords = totalRecords, PageNumber = pageNumber, PageSize = pageSize, Data = reservations });
		}

		
		[HttpPost]
		public async Task<ActionResult<TableReservation>> AddReservation(CreateReservationDto reservationDto)
		{
			var existingReservation = await _context.TableReservations.AnyAsync(r =>
				r.TableNumber == reservationDto.TableNumber &&
				r.ReservationDateTime.Date == reservationDto.ReservationDateTime.Date);

			if (existingReservation)
			{
				return BadRequest(new { message = "Table is already booked for this date." });
			}

			var reservation = new TableReservation
			{
				TableNumber = reservationDto.TableNumber,
				CustomerName = reservationDto.CustomerName,
				ReservationDateTime = reservationDto.ReservationDateTime,
				NumberOfGuests = reservationDto.NumberOfGuests
			};

			_context.TableReservations.Add(reservation);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, reservation);
		}

		
		[HttpGet("{id}")]
		public async Task<ActionResult<TableReservationDto>> GetReservationById(int id)
		{
			var reservation = await _context.TableReservations
				.Where(r => r.Id == id)
				.Select(r => new TableReservationDto
				{
					Id = r.Id,
					TableNumber = r.TableNumber,
					CustomerName = r.CustomerName,
					ReservationDateTime = r.ReservationDateTime,
					NumberOfGuests = r.NumberOfGuests
				})
				.FirstOrDefaultAsync();

			return reservation == null ? NotFound(new { message = "Reservation not found." }) : Ok(reservation);
		}

		
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateReservation(int id, CreateReservationDto reservationDto)
		{
			var reservation = await _context.TableReservations.FindAsync(id);
			if (reservation == null)
			{
				return NotFound(new { message = "Reservation not found." });
			}

			
			if (reservation.ReservationDateTime < DateTime.Now)
			{
				return BadRequest(new { message = "Past reservations cannot be modified." });
			}

			
			var existingReservation = await _context.TableReservations.AnyAsync(r =>
				r.Id != id &&
				r.TableNumber == reservationDto.TableNumber &&
				r.ReservationDateTime.Date == reservationDto.ReservationDateTime.Date);

			if (existingReservation)
			{
				return BadRequest(new { message = "Table is already booked for this date." });
			}

			reservation.TableNumber = reservationDto.TableNumber;
			reservation.CustomerName = reservationDto.CustomerName;
			reservation.ReservationDateTime = reservationDto.ReservationDateTime;
			reservation.NumberOfGuests = reservationDto.NumberOfGuests;

			await _context.SaveChangesAsync();
			return Ok(new { message = "Reservation updated successfully." });
		}

		
		[HttpDelete("{id}")]
		public async Task<IActionResult> CancelReservation(int id)
		{
			var reservation = await _context.TableReservations.FindAsync(id);
			if (reservation == null)
			{
				return NotFound(new { message = "Reservation not found." });
			}

			_context.TableReservations.Remove(reservation);
			await _context.SaveChangesAsync();
			return Ok(new { message = "Reservation canceled." });
		}
	}
}