using Microsoft.AspNetCore.Mvc;
using Uplay.Models.SurveyModels;

namespace Uplay.Controllers.FeedbackModels
{
	[ApiController]
	[Route("[controller]")]
	public class Cust_FeedbackController : ControllerBase
	{
		private readonly MyDbContext _context;
		public Cust_FeedbackController(MyDbContext context)
		{
			_context = context;
		}

		//private static readonly List<Cust_Feedback> list = new();
		[HttpGet]
		public IActionResult GetAll(string? search)
		{
			IQueryable<Cust_Feedback> result = _context.Customer_Feedback;
			if (search != null)
			{
				result = result.Where(x => x.Enquiry_Subject.Contains(search)
				|| x.Customer_Enquiry.Contains(search));
			}
			var list = result.OrderByDescending(x => x.CreatedAt).ToList();
			return Ok(list);
		}

		[HttpGet("{id}")]
		public IActionResult GetFeedback(int id)
		{
			Cust_Feedback? feedback = _context.Customer_Feedback.Find(id);
			if (feedback == null)
			{
				return NotFound();
			}
			return Ok(feedback);
		}

		[HttpPost]
		public IActionResult AddFeedback(Cust_Feedback feedback)
		{
			var now = DateTime.Now;
			var myFeedback = new Cust_Feedback()
			{
				Enquiry_Subject = feedback.Enquiry_Subject.Trim(),
				Customer_Enquiry = feedback.Customer_Enquiry.Trim(),
				CreatedAt = now,
				UpdatedAt = now,
			};

			_context.Customer_Feedback.Add(myFeedback);
			_context.SaveChanges();
			return Ok(myFeedback);
		}

		[HttpDelete("{id}")]
		public IActionResult DeleteFeedback(int id)
		{
			var myFeedback = _context.Customer_Feedback.Find(id);
			if (myFeedback == null)
			{
				return NotFound();
			}
			_context.Customer_Feedback.Remove(myFeedback);
			_context.SaveChanges();
			return Ok();
		}



	}
}
