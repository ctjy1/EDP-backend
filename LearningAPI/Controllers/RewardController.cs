using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Uplay;
using Uplay.Models.RewardModels;

namespace Uplay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RewardController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RewardController> _logger;

        public RewardController(MyDbContext context, IMapper mapper, 
            ILogger<RewardController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RewardDTO>), StatusCodes.Status200OK)]
        public IActionResult GetAll(string? search)
        {
            try
            {
                IQueryable<Reward> result = _context.Rewards.Include(t => t.User);
                if (search != null)
                {
                    result = result.Where(x => x.RewardName.Contains(search)
                        || x.Description.Contains(search));
                }
                var list = result.OrderByDescending(x => x.ExpiryDate).ToList();
                IEnumerable<RewardDTO> data = list.Select(t => _mapper.Map<RewardDTO>(t));
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when get all rewards");
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RewardDTO), StatusCodes.Status200OK)]
        public IActionResult GetReward(int id)
        {
            try
            {
                Reward? reward = _context.Rewards.Include(t => t.User)
                .FirstOrDefault(t => t.Id == id);
                if (reward == null)
                {
                    return NotFound();
                }
                RewardDTO data = _mapper.Map<RewardDTO>(reward);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when get reward by id");
                return StatusCode(500);
            }
        }

		[HttpGet("{id}/similar_rewards")]
		[ProducesResponseType(typeof(IEnumerable<RewardDTO>), StatusCodes.Status200OK)]
		public IActionResult GetSimilarRewards(int id, string? search)
		{
			try
			{
				var reward = _context.Rewards.Find(id);
				if (reward == null)
				{
					return NotFound();
				}

				var list = _context.Rewards
					.Where(r => r.RewardName == reward.RewardName && r.DeletedAt == null &&
								(search == null || EF.Functions.Like(r.RewardName, $"%{search}%") || EF.Functions.Like(r.Description, $"%{search}%")))
					.Include(t => t.User)
					.ToList();

				var data = _mapper.Map<IEnumerable<RewardDTO>>(list);
				return Ok(data);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when getting similar rewards");
				return StatusCode(500);
			}
		}

		[HttpPost, Authorize]
        [ProducesResponseType(typeof(RewardDTO), StatusCodes.Status200OK)]
        public IActionResult AddReward(AddRewardRequest reward)
        {
            try
            {
                int userId = GetUserId();
                var now = DateTime.Now;
                var myReward = new Reward()
                {
                    RewardName = reward.RewardName.Trim(),
                    Description = reward.Description.Trim(),
                    Discount = reward.Discount,
                    PointsRequired = reward.PointsRequired,
                    ExpiryDate = reward.ExpiryDate,
                    DeletedAt = null,
                    UserId = userId
                };

                _context.Rewards.Add(myReward);
                _context.SaveChanges();

                Reward? newReward = _context.Rewards.Include(t => t.User)
                    .FirstOrDefault(t => t.Id == myReward.Id);
                RewardDTO rewardDTO = _mapper.Map<RewardDTO>(newReward);
                return Ok(rewardDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when adding reward");
                return StatusCode(500);
            }
        }

        [HttpPut("{id}"), Authorize]
        public IActionResult UpdateReward(int id, UpdateRewardRequest reward)
        {
            try
            {
                var myReward = _context.Rewards.Find(id);
                if (myReward == null)
                {
                    return NotFound();
                }

                int userId = GetUserId();
                if (myReward.UserId != userId)
                {
                    return Forbid();
                }

                if (reward.RewardName != null)
                {
                    myReward.RewardName = reward.RewardName.Trim();
                }
                if (reward.Description != null)
                {
                    myReward.Description = reward.Description.Trim();
                }
                if (reward.Discount != null)
                {
                    myReward.Discount = reward.Discount;
                }
                if (reward.PointsRequired != null)
                {
                    myReward.PointsRequired = reward.PointsRequired;
                }

                if (reward.ExpiryDate != null)
                {
                    myReward.ExpiryDate = reward.ExpiryDate;
                }

                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating reward");
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}"), Authorize]
        public IActionResult DeleteReward(int id)
        {
            try
            {
                var myReward = _context.Rewards.Find(id);
                if (myReward == null)
                {
                    return NotFound();
                }

                int userId = GetUserId();
                if (myReward.UserId != userId)
                {
                    return Forbid();
                }

                _context.Rewards.Remove(myReward);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting reward");
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
