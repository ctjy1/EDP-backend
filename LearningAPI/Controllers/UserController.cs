using UPlay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace UPlay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public UserController(MyDbContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            // Trim string values
            request.FirstName = request.FirstName.Trim();
            request.LastName = request.LastName.Trim();
            request.Username = request.Username.Trim();
            request.Address1 = request.Address1.Trim();
            request.Address2 = request.Address2.Trim();
            request.Email = request.Email.Trim().ToLower();
            request.Password = request.Password.Trim();

            // Check email
            var foundUser = _context.Users.Where(
            x => x.Email == request.Email).FirstOrDefault();
            if (foundUser != null)
            {
                string message = "Email already exists.";
                return BadRequest(new { message });
            }

            // Create user object
            var now = DateTime.Now;
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Username = request.Username,
                Email = request.Email,
                ContactNumber = request.ContactNumber,
                Address1 = request.Address1,
                Address2 = request.Address2,
                ReferredCode = request.ReferredCode,
                Password = passwordHash,
                CreatedAt = now,
                UpdatedAt = now
            };

            // Generate referral code for this specific user
            user.GenerateReferralCode(); // Call GenerateReferralCode on the user instance

            // Add user
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok();

        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            // Trim string values
            request.Email = request.Email.Trim().ToLower();
            request.Password = request.Password.Trim();
            // Check email and password
            string message = "Email or password is not correct.";
            var foundUser = _context.Users.Where(
            x => x.Email == request.Email).FirstOrDefault();
            if (foundUser == null)
            {
                return BadRequest(new { message });
            }
            bool verified = BCrypt.Net.BCrypt.Verify(
            request.Password, foundUser.Password);
            if (!verified)
            {
                return BadRequest(new { message });
            }
            // Return user info
            var user = new
            {
                foundUser.Id,
                foundUser.Email,
                foundUser.Username,
                foundUser.FirstName,
                foundUser.LastName,
                foundUser.ContactNumber,
                foundUser.Address1,
                foundUser.Address2,
                foundUser.ReferralCode,
                foundUser.ReferredCode

            };
            string accessToken = CreateToken(foundUser);
            return Ok(new { user, accessToken });
        }

        private string CreateToken(User user)
        {
            string secret = _configuration.GetValue<string>(
            "Authentication:Secret");
            int tokenExpiresDays = _configuration.GetValue<int>(
            "Authentication:TokenExpiresDays");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.GivenName, user.FirstName), // New claim for first name
            new Claim(ClaimTypes.Surname, user.LastName),    // New claim for last name
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("Contact", user.ContactNumber),
            new Claim("Address1", user.Address1 ?? ""),
            new Claim("Address2", user.Address2 ?? ""),
            new Claim("ReferralCode", user.ReferralCode),


            }),
                Expires = DateTime.UtcNow.AddDays(tokenExpiresDays),
                SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(securityToken);
            return token;
        }

        [HttpGet("auth"), Authorize]
        public IActionResult Auth()
        {
            var id = Convert.ToInt32(User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
            var username = User.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
            var firstName = User.Claims.Where(c => c.Type == ClaimTypes.GivenName).Select(c => c.Value).SingleOrDefault();
            var lastName = User.Claims.Where(c => c.Type == ClaimTypes.Surname).Select(c => c.Value).SingleOrDefault();
            var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
            var contactNumber = User.Claims.Where(c => c.Type == "Contact").Select(c => c.Value).SingleOrDefault();
            var address1 = User.Claims.Where(c => c.Type == "Address1").Select(c => c.Value).SingleOrDefault();
            var address2 = User.Claims.Where(c => c.Type == "Address2").Select(c => c.Value).SingleOrDefault();
            var referralCode = User.Claims.Where(c => c.Type == "ReferralCode").Select(c => c.Value).SingleOrDefault();


            if (id != 0 && username != null && firstName != null && lastName != null && email != null)
            {
                var user = new
                {
                    id,
                    username,
                    firstName,
                    lastName,
                    email,
                    contactNumber,
                    address1,
                    address2,
                    referralCode

                };
                return Ok(new { user });
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<object>> GetAll(string? search)
        {
            IQueryable<User> result = _context.Users;

            if (!string.IsNullOrEmpty(search))
            {
                result = result.Where(x => x.FirstName.Contains(search)
                    || x.LastName.Contains(search) || x.Username.Contains(search));
            }

            var list = result.OrderByDescending(x => x.CreatedAt).ToList();
            var data = list.Select(t => new
            {
                t.Id,
                t.FirstName,
                t.LastName,
                t.Username,
                t.Email,
                t.ContactNumber,
                t.Address1,
                t.Address2,
                t.ReferredCode,
                t.ReferralCode,
                t.CreatedAt,
                t.UpdatedAt,
                UserId = t.Id
            });

            return Ok(data);
        }

        [HttpGet("{id}")]
        public ActionResult<object> GetUsers(int id)
        {
            User? user = _context.Users.FirstOrDefault(t => t.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var data = new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Username,
                user.Email,
                user.ContactNumber,
                user.Address1,
                user.Address2,
                user.ReferredCode,
                user.ReferralCode,
                user.CreatedAt,
                user.UpdatedAt,
                UserId = user.Id
            };

            return Ok(data);
        }

        //       [HttpPut("{id}"), Authorize]
        //public IActionResult UpdateUserDetails(int id, [FromBody] UserDTO userDTO)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var myUser = _context.Users.Find(id);
        //    if (myUser == null)
        //    {
        //        return NotFound();
        //    }

        //    myUser.FirstName = userDTO.FirstName?.Trim() ?? myUser.FirstName;
        //    myUser.LastName = userDTO.LastName?.Trim() ?? myUser.LastName;
        //    myUser.Username = userDTO.Username?.Trim() ?? myUser.Username;
        //    myUser.ContactNumber = userDTO.ContactNumber?.Trim() ?? myUser.ContactNumber;
        //    myUser.Address1 = userDTO.Address1?.Trim() ?? myUser.Address1;
        //    myUser.Address2 = userDTO.Address2?.Trim() ?? myUser.Address2;
        //    myUser.UpdatedAt = DateTime.Now;

        //    _context.SaveChanges();

        //    return Ok();
        //}

        [HttpDelete("{id}"), Authorize]
        public IActionResult DeleteUser(int id)
        {
            var myUser = _context.Users.Find(id);
            if (myUser == null)
            {
                return NotFound();
            }

            _context.Users.Remove(myUser);
            _context.SaveChanges();
            return Ok();
        }


    }
}


/*        [HttpPut("{id}")]
        public IActionResult UpdateUserDetails(int id, User user)
        {
            var myUser = _context.Users.Find(id);
            if (myUser == null)
            {
                return NotFound();
            }

            myUser.FirstName = user.FirstName.Trim();
            myUser.LastName = user.LastName.Trim();
            myUser.Username = user.Username.Trim();
            myUser.ContactNumber = user.ContactNumber;
            myUser.Address1= user.Address1;
            myUser.Address2 = user.Address2;
            myUser.Password = user.Password.Trim();
            myUser.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return Ok();
        }*/