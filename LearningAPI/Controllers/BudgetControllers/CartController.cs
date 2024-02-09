using Microsoft.AspNetCore.Mvc;
using Uplay.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Text.Encodings.Web;
using System.Text;
using Uplay.Models.BudgetModels;

namespace Uplay.Controllers.BudgetControllers
{
	[ApiController]
	[Route("[controller]")]
	public class CartController : ControllerBase
	{
		private readonly MyDbContext _context;
		public CartController(MyDbContext context)
		{
			_context = context;
		}

		private int GetUserId()
		{
			return Convert.ToInt32(User.Claims
			.Where(c => c.Type == ClaimTypes.NameIdentifier)
			.Select(c => c.Value).SingleOrDefault());
		}

		/*
        private string GetUserEmail()
        {
            var userEmailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            if (userEmailClaim != null)
            {
                return userEmailClaim.Value;
            }

            throw new InvalidOperationException("User Email claim not found.");
        }
        */


		[HttpGet]
		public IActionResult GetAll(string? search)
		{
			IQueryable<Cart> result = _context.Carts.Include(t => t.User);
			if (search != null)
			{
				result = result.Where(x => x.Service.Contains(search)
				|| x.Participants.Contains(search));
			}
			var list = result.OrderByDescending(x => x.CreatedAt).ToList();
			var data = list.Select(t => new
			{
				t.Id,
				t.Service,
				t.Participants,
				t.Quantity,
				t.Date,
				t.Time,
				t.Price,
				t.CreatedAt,
				t.UpdatedAt,
				t.UserId,
				User = new
				{
					// FIX ME
					// t.User?.Name
				}
			});
			return Ok(data);
		}

		[HttpGet("{id}")]
		public IActionResult GetCart(int id)
		{
			Cart? cart = _context.Carts.Include(t => t.User).FirstOrDefault(t => t.Id == id);
			if (cart == null)
			{
				return NotFound();
			}
			var data = new
			{
				cart.Id,
				cart.Service,
				cart.Participants,
				cart.Quantity,
				cart.Date,
				cart.Time,
				cart.Price,
				cart.CreatedAt,
				cart.UpdatedAt,
				cart.UserId,
				User = new
				{
					// FIX ME
					// cart.User?.Name
				}
			};
			return Ok(data);
		}

		[HttpPost, Authorize]
		public IActionResult AddCart(Cart cart)
		{
			int userId = GetUserId();
			var now = DateTime.Now;
			var myCart = new Cart()
			{
				Service = cart.Service.Trim(),
				Participants = cart.Participants.Trim(),
				Quantity = cart.Quantity,
				Date = cart.Date,
				Time = cart.Time,
				Price = cart.Price,
				CreatedAt = now,
				UpdatedAt = now,
				UserId = userId
			};
			_context.Carts.Add(myCart);
			_context.SaveChanges();
			return Ok(myCart);
		}


		[HttpPut("{id}"), Authorize]
		public IActionResult UpdateCart(int id, Cart cart)
		{
			var myCart = _context.Carts.Find(id);
			if (myCart == null)
			{
				return NotFound();
			}

			int userId = GetUserId();
			if (myCart.UserId != userId)
			{
				return Forbid();
			}

			myCart.Service = cart.Service.Trim();
			myCart.Participants = cart.Participants.Trim();
			myCart.Quantity = cart.Quantity;
			myCart.Date = cart.Date;
			myCart.Time = cart.Time;
			myCart.Price = cart.Price;
			myCart.UpdatedAt = DateTime.Now;
			_context.SaveChanges();
			return Ok();
		}


		[HttpDelete("{id}"), Authorize]
		public IActionResult DeleteCart(int id)
		{
			var myCart = _context.Carts.Find(id);
			if (myCart == null)
			{
				return NotFound();
			}

			int userId = GetUserId();
			if (myCart.UserId != userId)
			{
				return Forbid();
			}

			_context.Carts.Remove(myCart);
			_context.SaveChanges();
			return Ok();
		}


		[HttpDelete("all"), Authorize]
		public IActionResult DeleteAllCarts()
		{
			int userId = GetUserId();
			var userCarts = _context.Carts.Where(cart => cart.UserId == userId).ToList();

			// Check if the user has any carts to delete
			if (userCarts.Count > 0)
			{
				_context.Carts.RemoveRange(userCarts);
				_context.SaveChanges();
			}

			return Ok();
		}


		[HttpPost("checkout"), Authorize]
		public IActionResult Checkout()
		{
			int userId = GetUserId();
			var userCarts = _context.Carts.Where(cart => cart.UserId == userId).ToList();

			if (userCarts.Count == 0)
			{
				return BadRequest(new { message = "No items in the cart to checkout." });
			}

			string userEmail = "gabrielforschwork@gmail.com";


			string frontendBaseUrl = "http://localhost:3000";
			string userOrdersLink = $"{frontendBaseUrl}/userorders";

			// Constructing the email body with service details
			StringBuilder emailBody = new StringBuilder();
			emailBody.AppendLine("Thank you for booking with us! Here are the details of your order:");

			foreach (var cart in userCarts)
			{
				emailBody.AppendLine("<hr>");
				emailBody.AppendLine($"Service: {cart.Service}");
				// Include other details as needed
				emailBody.AppendLine("<hr>");
			}

			emailBody.AppendLine($"Click <a href=\"{userOrdersLink}\">here</a> to view your orders.");

			var client = new SmtpClient("smtp.gmail.com", 587)
			{
				Credentials = new NetworkCredential("gabrielforschwork", "kagv cpsp ftsz oaxl"),
				EnableSsl = true
			};

			MailMessage mail = new MailMessage("uplay@gmail.com", userEmail, "Your booking has been confirmed", emailBody.ToString());
			mail.IsBodyHtml = true;
			client.Send(mail);

			var order = new Order
			{
				OrderDate = DateTime.Now,
				TotalAmount = userCarts.Sum(cart => cart.Price),
				UserId = userId,
				OrderDetails = new List<OrderDetails>()  // Initialize OrderDetails collection
			};

			foreach (var cart in userCarts)
			{
				order.OrderDetails.Add(new OrderDetails
				{
					Service = cart.Service,
					Participants = cart.Participants,
					Quantity = cart.Quantity,
					Date = cart.Date,
					Time = cart.Time,
					Price = cart.Price,
				});
			}

			_context.Orders.Add(order);
			_context.Carts.RemoveRange(userCarts);
			_context.SaveChanges();

			return Ok(new { orderId = order.Id });
		}


	}
}
