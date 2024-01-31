using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using UPlay.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace UPlay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GalleryController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        public GalleryController(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        private int GetUserId()
        {
            return Convert.ToInt32(User.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => c.Value).SingleOrDefault());
        }

        [HttpGet]
        public IActionResult GetAll(string? search)
        {
            IQueryable<Gallery> result = _context.Galleries.Include(t => t.User);
            if (search != null)
            {
                result = result.Where(x => x.Title.Contains(search)
                || x.Caption.Contains(search) || x.Location.Contains(search));
            }
            var list = result.OrderByDescending(x => x.CreatedAt).ToList();
            var data = list.Select(t => new
            {
                t.Id,
                t.Title,
                t.Caption,
                t.Location,
                t.ImageFile,
                t.CreatedAt,
                t.UpdatedAt,
                t.UserId,
                User = new
                {
                    t.User?.Username
                }
            });
            return Ok(data);
        }

        [HttpGet("{id}")]
        public IActionResult GetGalleries(int id)
        {
            Gallery? gallery = _context.Galleries.Include(t => t.User).FirstOrDefault(t => t.Id == id);
            if (gallery == null)
            {
                return NotFound();
            }
            var data = new
            {
                gallery.Id,
                gallery.Title,
                gallery.Caption,
                gallery.Location,
                gallery.ImageFile,
                gallery.CreatedAt,
                gallery.UpdatedAt,
                gallery.UserId,
                User = new
                {
                    gallery.User?.Username
                }
            };
            return Ok(data);
        }


        [HttpPost, Authorize]
        public IActionResult AddGallery(Gallery gallery)
        {
            int userId = GetUserId();
            var now = DateTime.Now;
            var myGallery = new Gallery()
            {
                Title = gallery.Title.Trim(),
                Caption = gallery.Caption.Trim(),
                Location = gallery.Location.Trim(),
                ImageFile = gallery.ImageFile,
                CreatedAt = now,
                UpdatedAt = now,
                UserId = userId
            };
            _context.Galleries.Add(myGallery);
            _context.SaveChanges();
            return Ok(myGallery);
        }

        [HttpPut("{id}"), Authorize]
        public IActionResult UpdateGallery(int id, Gallery gallery)
        {
            var myGallery = _context.Galleries.Find(id);
            if (myGallery == null)
            {
                return NotFound();
            }

            int userId = GetUserId();
            if (myGallery.UserId != userId)
            {
                return Forbid();
            }

            myGallery.Title = gallery.Title.Trim();
            myGallery.Caption = gallery.Caption.Trim();
            myGallery.Location = gallery.Location.Trim();
            myGallery.ImageFile = gallery.ImageFile;
            myGallery.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}"), Authorize]
        public IActionResult DeleteGallery(int id)
        {
            var myGallery = _context.Galleries.Find(id);
            if (myGallery == null)
            {
                return NotFound();
            }

            int userId = GetUserId();
            if (myGallery.UserId != userId)
            {
                return Forbid();
            }

            _context.Galleries.Remove(myGallery);
            _context.SaveChanges();
            return Ok();
        }
    }
}
