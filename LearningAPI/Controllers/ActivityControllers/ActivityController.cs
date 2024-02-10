using AutoMapper;
using Uplay.Models.ActivityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Uplay.Controllers.ActivityControllers
{
	[ApiController]
	[Route("[controller]")]
	public class ActivityController : ControllerBase
	{
		private readonly MyDbContext _context;
		private readonly IMapper _mapper;
		private readonly ILogger<ActivityController> _logger;

		public ActivityController(MyDbContext context, IMapper mapper,
			ILogger<ActivityController> logger)
		{
			_context = context;
			_mapper = mapper;
			_logger = logger;
		}

		[HttpGet]
		[ProducesResponseType(typeof(IEnumerable<ActivityDTO>), StatusCodes.Status200OK)]
		public IActionResult GetAll(string? search)
		{
			try
			{
				IQueryable<Activity> result = _context.Activitys.Include(t => t.User);
				if (search != null)
				{
					result = result.Where(x => x.activity_Name.Contains(search)
						|| x.activity_Desc.Contains(search));
				}

				var list = result.OrderByDescending(x => x.CreatedAt).ToList();
				IEnumerable<ActivityDTO> data = list.Select(t => _mapper.Map<ActivityDTO>(t));
				return Ok(data);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when get all activities");
				return StatusCode(500);
			}
		}

		[HttpGet("{id}")]
		[ProducesResponseType(typeof(ActivityDTO), StatusCodes.Status200OK)]
		public IActionResult GetActivity(int id)
		{
			try
			{
				Activity? activity = _context.Activitys.Include(t => t.User)
				.FirstOrDefault(t => t.Id == id);
				if (activity == null)
				{
					return NotFound();
				}
				ActivityDTO data = _mapper.Map<ActivityDTO>(activity);
				return Ok(data);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when get activity by id");
				return StatusCode(500);
			}
		}

		[HttpPost, Authorize]
		[ProducesResponseType(typeof(ActivityDTO), StatusCodes.Status200OK)]
		public IActionResult AddActivity(AddActivityRequest activity)
		{
			try
			{
				int userId = GetUserId();
				var now = DateTime.Now;
				var myActivity = new Activity()
				{
					activity_Name = activity.activity_Name.Trim(),
					tag_Name = activity.tag_Name.Trim(),
					activity_Desc = activity.activity_Desc.Trim(),
					ImageFile = activity.ImageFile,
					CreatedAt = now,
					UpdatedAt = now,
					UserId = userId
				};

				_context.Activitys.Add(myActivity);
				_context.SaveChanges();

				Activity? newActivity = _context.Activitys.Include(t => t.User)
					.FirstOrDefault(t => t.Id == myActivity.Id);
				ActivityDTO activityDTO = _mapper.Map<ActivityDTO>(newActivity);
				return Ok(activityDTO);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when add activity");
				return StatusCode(500);
			}
		}

		[HttpPut("{id}"), Authorize]
		public IActionResult UpdateActivity(int id, UpdateActivityRequest activity)
		{
			try
			{
				var myActivity = _context.Activitys.Find(id);
				if (myActivity == null)
				{
					return NotFound();
				}

				int userId = GetUserId();
				if (myActivity.UserId != userId)
				{
					return Forbid();
				}

				if (activity.activity_Name != null)
				{
					myActivity.activity_Name = activity.activity_Name.Trim();
				}
				if (activity.tag_Name != null)
				{
					myActivity.tag_Name = activity.tag_Name.Trim();
				}
				if (activity.activity_Desc != null)
				{
					myActivity.activity_Desc = activity.activity_Desc.Trim();
				}
				if (activity.ImageFile != null)
				{
					myActivity.ImageFile = activity.ImageFile;
				}
				myActivity.UpdatedAt = DateTime.Now;

				_context.SaveChanges();
				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when update activity");
				return StatusCode(500);
			}
		}

		[HttpDelete("{id}"), Authorize]
		public IActionResult DeleteActivity(int id)
		{
			try
			{
				var myActivity = _context.Activitys.Find(id);
				if (myActivity == null)
				{
					return NotFound();
				}

				int userId = GetUserId();
				if (myActivity.UserId != userId)
				{
					return Forbid();
				}

				_context.Activitys.Remove(myActivity);
				_context.SaveChanges();
				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when delete activity");
				return StatusCode(500);
			}
		}

		private int GetUserId()
		{
			return Convert.ToInt32(User.Claims
				.Where(c => c.Type == ClaimTypes.NameIdentifier)
				.Select(c => c.Value).SingleOrDefault());
		}
	}
}
