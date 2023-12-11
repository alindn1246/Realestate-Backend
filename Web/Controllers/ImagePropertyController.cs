using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Auth;

namespace JWTAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagePropertyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ImagePropertyController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyImage>>> GetImages()
        {
            return await _context.PropertyImages
                .Select(x => new PropertyImage
                {
                    ImageId = x.ImageId,
                    ImageName = x.ImageName,
                    ImageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/Images/{x.ImageName}",
                    PropertyId = x.PropertyId // Include PropertyId in the response
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyImage>> GetImage(int id)
        {
            var imageModel = await _context.PropertyImages.FindAsync(id);

            if (imageModel == null)
            {
                return NotFound();
            }

            var propertyImage = new PropertyImage
            {
                ImageId = imageModel.ImageId,
                ImageName = imageModel.ImageName,
                ImageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/Images/{imageModel.ImageName}",
                PropertyId = imageModel.PropertyId // Include PropertyId in the response
            };

            return propertyImage;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutImage(int id, [FromForm] PropertyImage imageModel)
        {
            if (id != imageModel.ImageId)
            {
                return BadRequest();
            }

            if (imageModel.ImageFile != null)
            {
                DeleteImage(imageModel.ImageName);
                imageModel.ImageName = await SaveImage(imageModel.ImageFile);
            }

            _context.Entry(imageModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<PropertyImage>>> PostImage([FromForm] List<IFormFile> imageFiles, [FromForm] int propertyId)
        {
            try
            {
                List<PropertyImage> uploadedImages = new List<PropertyImage>();

                foreach (var imageFile in imageFiles)
                {
                    var imageModel = new PropertyImage
                    {
                        PropertyId = propertyId,
                        ImageName = await SaveImage(imageFile)
                    };

                    _context.PropertyImages.Add(imageModel);
                    uploadedImages.Add(imageModel);
                }

                await _context.SaveChangesAsync();

                return Ok(uploadedImages);
            }
            catch (Exception ex)
            {
                // Log the exception details
                return StatusCode(500, "Internal Server Error");
            }
        }



        [HttpDelete("{id}")]
        public async Task<ActionResult<PropertyImage>> DeleteImage(int id)
        {
            var imageModel = await _context.PropertyImages.FindAsync(id);
            if (imageModel == null)
            {
                return NotFound();
            }
            DeleteImage(imageModel.ImageName);
            _context.PropertyImages.Remove(imageModel);
            await _context.SaveChangesAsync();

            return imageModel;
        }

        private bool ImageExists(int id)
        {
            return _context.PropertyImages.Any(e => e.ImageId == id);
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile)
        {
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return imageName;
        }

        [NonAction]
        public void DeleteImage(string imageName)
        {
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }
    }
}
