using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Uplay.Models;
using Microsoft.EntityFrameworkCore;

namespace Uplay.Controllers
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

        [HttpGet]
        public IActionResult GetAll(string? search)
        {
            IQueryable<ReferralTracking> result = _context.ReferralTrackings
                 .Include(t => t.User)
                 .Include(t => t.ReferringUser); // Ensure you include the ReferringUser

            if (!string.IsNullOrEmpty(search))
            {
                result = result.Where(x => x.User.Username.Contains(search)
                    || x.User.Email.Contains(search)
                    || x.User.ReferralCode.Contains(search)
                    || x.User.ReferredCode.Contains(search)
                    || (x.ReferringUser != null && (x.ReferringUser.Username.Contains(search)
                        || x.ReferringUser.Email.Contains(search))));
            }

            var list = result
                .OrderByDescending(x => x.User.CreatedAt)
                .ToList();


            var data = list.Select(t => new
            {
                ReferralId = t.Id,
                Status = t.Status,
                UserId = t.UserId,
                ReferringUserId = t.ReferringUserId,
                ReferredUsername = t.User.Username, // Username of the referred user
                ReferringUsername = t.ReferringUser != null ? t.ReferringUser.Username : null, // Username of the referring user
                DateFulfilled = t.User.CreatedAt // CreatedAt from the referred user
            });


            return Ok(data);
        }


        // POST: ReferralTracking/RecordReferral
        [HttpPost("RecordReferral")]
        public IActionResult RecordReferral([FromBody] string referredCode)
        {
            var referringUser = _context.Users.FirstOrDefault(u => u.ReferralCode == referredCode);
            if (referringUser == null)
            {
                return NotFound("Referral code not found.");
            }

            var referralTracking = new ReferralTracking
            {
                UserId = referringUser.Id,
                Status = "Pending"
            };

            _context.ReferralTrackings.Add(referralTracking);
            _context.SaveChanges();

            return Ok("Referral recorded successfully.");
        }

        // PUT: ReferralTracking/UpdateReferralStatus/{id}
        [HttpPut("UpdateReferralStatus/{id}")]
        public IActionResult UpdateReferralStatus(int id, [FromBody] string status)
        {
            var referralTracking = _context.ReferralTrackings.Find(id);
            if (referralTracking == null)
            {
                return NotFound("Referral record not found.");
            }

            referralTracking.Status = status;
            _context.SaveChanges();

            return Ok("Referral status updated successfully.");
        }

        [HttpGet("MyReferrals")]
        [Authorize] // Ensure that only authenticated users can access this method
        public IActionResult GetMyReferrals()
        {
            // Assuming the user's ID is stored in the claims as a string (you might need to adjust this based on your authentication setup)
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("User ID not found.");
            }

            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            var referrals = _context.ReferralTrackings
                .Where(r => r.ReferringUserId == userId) // Filter referrals where the current user is the referring user
                .Include(r => r.User) // Include the referred user
                .Select(r => new
                {
                    ReferralId = r.Id,
                    Status = r.Status,
                    ReferredUsername = r.User.Username, // Username of the referred user
                    DateReferred = r.User.CreatedAt // CreatedAt from the referred user
                })
                .ToList();

            return Ok(referrals);
        }


    }
}
