﻿using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Uplay.Models.BudgetModels;

namespace Uplay.Controllers.BudgetControllers
{
	[ApiController]
	[Route("[controller]")]
	public class OrderController : ControllerBase
	{
		private readonly MyDbContext _context;
		public OrderController(MyDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public IActionResult GetAll()
		{
			IQueryable<Order> result = _context.Orders
				.Include(o => o.User)
				.Include(o => o.OrderDetails);  // Include OrderDetails navigation property

			var list = result.ToList();

			var data = list.Select(order => new
			{
				order.Id,
				order.OrderDate,
				order.TotalAmount,
				order.UserId,
				User = new
				{
					// FIX ME
					// order.User?.Name
				},
				OrderDetails = order.OrderDetails.Select(orderDetail => new
				{
					orderDetail.Id,
					orderDetail.Service,
					orderDetail.Participants,
					orderDetail.Quantity,
					orderDetail.Date,
					orderDetail.Time,
					orderDetail.Price
					// Include other properties from OrderDetails as needed
				}).ToList()
			});

			return Ok(data);
		}



	}
}