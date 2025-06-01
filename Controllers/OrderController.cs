using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Data;
using Restaurant_Management_System.DTO;
using Restaurant_Management_System.Models;

namespace RestaurantAPI.Controllers
{
	[Route("api/orders")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public OrderController(ApplicationDbContext context)
		{
			_context = context;
		}

		
		[HttpGet]
		public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(
			[FromQuery] string status,
			[FromQuery] string sortBy = "OrderTime",
			[FromQuery] string sortDirection = "asc",
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10)
		{
			var query = _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem).AsQueryable();

			
			if (!string.IsNullOrEmpty(status))
			{
				query = query.Where(o => o.OrderStatus == status);
			}

			
			query = sortBy switch
			{
				"TableNumber" => sortDirection == "asc" ? query.OrderBy(o => o.TableNumber) : query.OrderByDescending(o => o.TableNumber),
				"OrderTime" => sortDirection == "asc" ? query.OrderBy(o => o.OrderTime) : query.OrderByDescending(o => o.OrderTime),
				_ => query
			};

			
			var totalRecords = await query.CountAsync();
			var orders = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize)
				.Select(o => new OrderDto
				{
					Id = o.Id,
					TableNumber = o.TableNumber,
					OrderTime = o.OrderTime,
					TotalAmount = o.TotalAmount,
					OrderStatus = o.OrderStatus,
					OrderItems = o.OrderItems.Select(oi => new OrderItemDto
					{
						Id = oi.Id,
						MenuItemName = oi.MenuItem.Name,
						Quantity = oi.Quantity
					}).ToList()
				})
				.ToListAsync();

			return Ok(new { TotalRecords = totalRecords, PageNumber = pageNumber, PageSize = pageSize, Data = orders });
		}

		[HttpGet("history")]
		public async Task<ActionResult<IEnumerable<OrderHistoryDto>>> GetOrderHistory(
	    [FromQuery] string orderStatus,
    	[FromQuery] DateTime? startDate,
    	[FromQuery] DateTime? endDate,
    	[FromQuery] string sortBy = "OrderTime",
    	[FromQuery] string sortDirection = "asc",
    	[FromQuery] int pageNumber = 1,
    	[FromQuery] int pageSize = 10)
		{
			var query = _context.Orders.AsQueryable();

			
			if (!string.IsNullOrEmpty(orderStatus))
			{
				query = query.Where(o => o.OrderStatus == orderStatus);
			}

			
			if (startDate.HasValue && endDate.HasValue)
			{
				query = query.Where(o => o.OrderTime >= startDate.Value && o.OrderTime <= endDate.Value);
			}

			
			query = sortBy switch
			{
				"TotalAmount" => sortDirection == "asc" ? query.OrderBy(o => o.TotalAmount) : query.OrderByDescending(o => o.TotalAmount),
				"OrderTime" => sortDirection == "asc" ? query.OrderBy(o => o.OrderTime) : query.OrderByDescending(o => o.OrderTime),
				_ => query
			};

			
			var totalRecords = await query.CountAsync();
			var orders = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize)
				.Select(o => new OrderHistoryDto
				{
					Id = o.Id,
					TableNumber = o.TableNumber,
					OrderTime = o.OrderTime,
					TotalAmount = o.TotalAmount,
					OrderStatus = o.OrderStatus
				})
				.ToListAsync();

			return Ok(new { TotalRecords = totalRecords, PageNumber = pageNumber, PageSize = pageSize, Data = orders });
		}

		
		[HttpPost]
		public async Task<ActionResult<Order>> AddOrder(CreateOrderDto orderDto)
		{
			if (!orderDto.OrderItems.Any())
			{
				return BadRequest(new { message = "Order must contain at least one item." });
			}

			var order = new Order
			{
				TableNumber = orderDto.TableNumber,
				OrderTime = DateTime.Now,
				OrderStatus = "Pending",
				OrderItems = orderDto.OrderItems.Select(oi => new OrderItem
				{
					MenuItemId = oi.MenuItemId,
					Quantity = oi.Quantity
				}).ToList()
			};

			
			decimal totalAmount = 0;
			foreach (var item in order.OrderItems)
			{
				var menuItem = await _context.MenuItems.FindAsync(item.MenuItemId);
				if (menuItem == null)
				{
					return BadRequest(new { message = $"Menu item with ID {item.MenuItemId} not found." });
				}

				totalAmount += menuItem.Price * item.Quantity;
			}

			order.TotalAmount = totalAmount;
			_context.Orders.Add(order);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
		}

		
		[HttpGet("{id}")]
		public async Task<ActionResult<OrderDto>> GetOrderById(int id)
		{
			var order = await _context.Orders
				.Include(o => o.OrderItems)
				.ThenInclude(oi => oi.MenuItem)
				.Where(o => o.Id == id)
				.Select(o => new OrderDto
				{
					Id = o.Id,
					TableNumber = o.TableNumber,
					OrderTime = o.OrderTime,
					TotalAmount = o.TotalAmount,
					OrderStatus = o.OrderStatus,
					OrderItems = o.OrderItems.Select(oi => new OrderItemDto
					{
						Id = oi.Id,
						MenuItemName = oi.MenuItem.Name,
						Quantity = oi.Quantity
					}).ToList()
				})
				.FirstOrDefaultAsync();

			return order == null ? NotFound(new { message = "Order not found." }) : Ok(order);
		}

		
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
		{
			var validStatuses = new List<string> { "Pending", "Served", "Completed" };
			if (!validStatuses.Contains(status))
			{
				return BadRequest(new { message = "Invalid order status." });
			}

			var order = await _context.Orders.FindAsync(id);
			if (order == null)
			{
				return NotFound(new { message = "Order not found." });
			}

			order.OrderStatus = status;
			await _context.SaveChangesAsync();
			return Ok(new { message = "Order status updated successfully." });
		}
	}
}