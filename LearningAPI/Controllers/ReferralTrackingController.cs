using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using UPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace UPlay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReferralTrackingController : ControllerBase
    {
        private readonly MyDbContext _context;
        public ReferralTrackingController(MyDbContext context)
        {
            _context = context;
        }

        /*private int GetUserId()
        {
            return Convert.ToInt32(User.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => c.Value).SingleOrDefault());
        }*/

        [HttpGet]
        public IActionResult GetAll(string? search)
        {
            // Assuming ReferralCode and ReferredCode are the matching fields
            IQueryable<ReferralTracking> result = _context.ReferralTrackings
                .Include(t => t.User);

            if (search != null)
            {
                result = result.Where(x => x.User.Username.Contains(search)
                    || x.User.Email.Contains(search)
                    || x.User.ReferralCode.Contains(search)
                    || x.User.ReferredCode.Contains(search));
            }

            var list = result
                .Where(x => x.User.ReferralCode != null && x.User.ReferredCode != null && x.User.ReferralCode == x.User.ReferredCode)
                .OrderByDescending(x => x.User.CreatedAt)
                .ToList();

            var data = list.Select(t => new
            {
                t.Id,
                t.Status,
                t.User?.Username,
                t.User?.Email,
                t.User?.ReferredCode,
                t.User?.ReferralCode,
                t.User?.CreatedAt,
                UserId = t.User?.Id, // Assuming Id is the user identifier property
                User = new
                {
                    t.User?.Username
                }
            });

            return Ok(data);
        }



        /*[HttpGet("{id}")]
        public IActionResult GetGalleries(int id)
        {
            ReferralTracking? referral = _context.ReferralTrackings.Include(t => t.User).FirstOrDefault(t => t.Id == id);
            if (referral == null)
            {
                return NotFound();
            }
            var data = new
            {
                referral.Id,
                referral.ReferredCode,
                referral.User.Username,
                referral.User.Email,
                referral.User.Id,
                referral.Status,
                referral.CreatedAt,
                referral.UpdatedAt,
                referral.UserId,
                User = new
                {
                    referral.User?.Username
                }
            };
            return Ok(data);
        }*/

    }
}