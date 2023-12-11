using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Auth;

namespace JWTAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageAgentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ImageAgentController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AgentImage>>> GetImages()
        {
            return await _context.AgentImages
                .Select(x => new AgentImage
                {
                    ImageId = x.ImageId,
                    ImageName = x.ImageName,
                    ImageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/Images/{x.ImageName}",
                    AgentId = x.AgentId // Include PropertyId in the response
                })
                .ToListAsync();
        }

       

        [HttpGet("{id}")]
        public async Task<ActionResult<AgentImage>> GetImage(int id, ActionResult<AgentImage> AgentImageS)
        {
            var imageModel = await _context.AgentImages.FindAsync(id);

            if (imageModel == null)
            {
                return NotFound();
            }

            var agentImage = new AgentImage
            {
                ImageId = imageModel.ImageId,
                ImageName = imageModel.ImageName,
                ImageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/Images/{imageModel.ImageName}",
                AgentId = imageModel.AgentId // Include PropertyId in the response
            };

            return agentImage;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutImage(int id, [FromForm] AgentImage imageModel)
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
        public async Task<ActionResult<IEnumerable<PropertyImage>>> PostImage([FromForm] List<IFormFile> imageFiles, [FromForm] int agentId)
        {
            try
            {
                List<AgentImage> uploadedImages = new List<AgentImage>();

                foreach (var imageFile in imageFiles)
                {
                    var imageModel = new AgentImage
                    {
                        AgentId = agentId,
                        ImageName = await SaveImage(imageFile)
                    };

                    _context.AgentImages.Add(imageModel);
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
        public async Task<ActionResult<AgentImage>> DeleteImage(int id)
        {
            var imageModel = await _context.AgentImages.FindAsync(id);
            if (imageModel == null)
            {
                return NotFound();
            }
            DeleteImage(imageModel.ImageName);
            _context.AgentImages.Remove(imageModel);
            await _context.SaveChangesAsync();

            return imageModel;
        }

        private bool ImageExists(int id)
        {
            return _context.AgentImages.Any(e => e.ImageId == id);
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
