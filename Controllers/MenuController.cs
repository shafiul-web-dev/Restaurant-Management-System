using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Data;
using Restaurant_Management_System.DTO;
using Restaurant_Management_System.Models;

namespace RestaurantAPI.Controllers
{
	[Route("api/menu")]
	[ApiController]
	public class MenuController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public MenuController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetMenu(
			[FromQuery] string category,
			[FromQuery] string sortBy = "name",
			[FromQuery] string sortDirection = "asc",
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10)
		{
			var query = _context.MenuItems.AsQueryable();

			
			if (!string.IsNullOrEmpty(category))
			{
				query = query.Where(m => m.Category == category);
			}

			
			query = sortBy switch
			{
				"price" => sortDirection == "asc" ? query.OrderBy(m => m.Price) : query.OrderByDescending(m => m.Price),
				"name" => sortDirection == "asc" ? query.OrderBy(m => m.Name) : query.OrderByDescending(m => m.Name),
				_ => query
			};

			
			var totalRecords = await query.CountAsync();
			var menuItems = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize)
				.Select(m => new MenuItemDto
				{
					Id = m.Id,
					Name = m.Name,
					Category = m.Category,
					Price = m.Price,
					IsAvailable = m.IsAvailable
				})
				.ToListAsync();

			return Ok(new { TotalRecords = totalRecords, PageNumber = pageNumber, PageSize = pageSize, Data = menuItems });
		}

		
		[HttpPost]
		public async Task<ActionResult<MenuItem>> AddMenuItem(MenuItem menuItem)
		{
			var itemExists = await _context.MenuItems.AnyAsync(m => m.Name == menuItem.Name);
			if (itemExists)
			{
				return BadRequest(new { message = "Menu item already exists!" });
			}

			if (menuItem.Price <= 0)
			{
				return BadRequest(new { message = "Price must be greater than zero!" });
			}

			_context.MenuItems.Add(menuItem);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetMenu), new { id = menuItem.Id }, menuItem);
		}

		
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateMenuItem(int id, MenuItem updatedItem)
		{
			var validCategories = new List<string> { "Starter", "Main Course", "Dessert" };

			if (!validCategories.Contains(updatedItem.Category))
			{
				return BadRequest(new { message = "Invalid category! Must be Starter, Main Course, or Dessert." });
			}

			var menuItem = await _context.MenuItems.FindAsync(id);
			if (menuItem == null)
				return NotFound(new { message = "Menu item not found!" });

			menuItem.Name = updatedItem.Name;
			menuItem.Category = updatedItem.Category;
			menuItem.Price = updatedItem.Price;
			menuItem.IsAvailable = updatedItem.IsAvailable;

			await _context.SaveChangesAsync();
			return NoContent();
		}

		
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteMenuItem(int id)
		{
			var menuItem = await _context.MenuItems.FindAsync(id);
			if (menuItem == null)
				return NotFound(new { message = "Menu item not found!" });

			var isOrdered = await _context.OrderItems.AnyAsync(o => o.MenuItemId == id);
			if (isOrdered)
			{
				return BadRequest(new { message = "Menu item cannot be deleted—it is part of an active order!" });
			}

			_context.MenuItems.Remove(menuItem);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}
}