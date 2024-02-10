using Microsoft.AspNetCore.Mvc;
using Uplay.Models.SurveyModels;

namespace Uplay.Controllers.FeedbackModels
{
	[ApiController]
	[Route("[controller]")]
	public class Cust_SurveyController : Controller
	{
		private readonly MyDbContext _context;
		public Cust_SurveyController(MyDbContext context)
		{
			_context = context;
		}

		//private static readonly List<Cust_Survey> list = new();
		[HttpGet]
		public IActionResult GetAll(string? search)
		{
			IQueryable<Cust_Survey> result = _context.Customer_Surveys;
			if (search != null)
			{
				result = result.Where(x => x.Comments.Contains(search));
			}
			var list = result.OrderByDescending(x => x.Satisfaction).ToList(); //sort by rating
			return Ok(list);
		}

		[HttpGet("{satisfaction}")] //not working atm
		public IActionResult GetTutorial(int Satisfaction)
		{
			Cust_Survey? survey = _context.Customer_Surveys.Find(Satisfaction);
			return Ok(survey);
		}

		[HttpPost]
		public IActionResult AddSurvey(Cust_Survey survey)
		{
			{
				var now = DateTime.Now;
				var mySurvey = new Cust_Survey()
				{
					Satisfaction = survey.Satisfaction,
					Comments = survey.Comments.Trim(),
					CreatedAt = now,
					UpdatedAt = now
				};

				_context.Customer_Surveys.Add(mySurvey);
				_context.SaveChanges();
				return Ok(mySurvey);
			}

		}

		[HttpDelete("{id}")]
		public IActionResult DeleteSurvey(int id)
		{
			var mySurvey = _context.Customer_Surveys.Find(id);
			if (mySurvey == null)
			{
				return NotFound();
			}
			_context.Customer_Surveys.Remove(mySurvey);
			_context.SaveChanges();
			return Ok();
		}

	}
}
