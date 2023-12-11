using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Auth;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeuturePropertyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FeuturePropertyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FeutureProperty
        [HttpGet]
        public IActionResult GetFeutureProperties()
        {
            var feutureProperties = _context.FeutureProperties
                .Include(fp => fp.Property)
                .Include(fp => fp.Feature)
                .ToList();

            return Ok(feutureProperties);
        }

        // POST: api/FeutureProperty
        [HttpPost]
        public IActionResult PostFeutureProperty(int PropertyId, int FeatureId)
        {
            var property = _context.Properties.Find(PropertyId); // Change from Article to Properties
            var feature = _context.Features.Find(FeatureId); // Change from Tag to Features

            if (property == null || feature == null) // Fix variable names
            {
                return NotFound("Property or Feature not found"); // Fix error message
            }

            var feutureProperty = new FeutureProperty
            {
                Property = property,
                Feature = feature
            };

            _context.FeutureProperties.Add(feutureProperty);
            _context.SaveChanges();

            return Ok(feutureProperty);
        }

        // POST: api/FeutureProperty
        [HttpPost("MultipleFeatures")]
        public IActionResult PostFeuturesProperty(int PropertyId, [FromBody] List<int> FeatureIds)
        {
            var property = _context.Properties.Find(PropertyId);

            if (property == null)
            {
                return NotFound("Property not found");
            }

            var features = _context.Features.Where(f => FeatureIds.Contains(f.FeatureId)).ToList();

            if (features.Count != FeatureIds.Count)
            {
                return NotFound("One or more features not found");
            }

            var feutureProperties = features.Select(feature => new FeutureProperty
            {
                Property = property,
                Feature = feature
            }).ToList();

            _context.FeutureProperties.AddRange(feutureProperties);
            _context.SaveChanges();

            return Ok(feutureProperties);
        }

    }
}
