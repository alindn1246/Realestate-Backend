using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Auth;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PropertiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Property>>> GetProperties()
    {
        var properties = await _context.Properties
            .Include(p => p.Type)
            .Include(p => p.Agent)
            .ToListAsync();

        return Ok(properties);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Property>> GetProperty(int id)
    {
        var property = await _context.Properties
            .Include(p => p.Type)
            .Include(p => p.Agent)
            .FirstOrDefaultAsync(p => p.PropertyId == id);

        if (property == null)
        {
            return NotFound();
        }

        // Return a new object with the desired properties
        var result = new
        {
            property.PropertyId,
            property.Name,
            property.TypeId,
            property.AgentId,
            property.OwnerName,
            property.Address,
            property.Longitude,
            property.Latitude,
            property.Price,
            property.NbOfBeds,
            property.Size,
            AgentName = property.Agent?.AgentName,
            TypeName = property.Type?.TypeName
        };

        return Ok(result);
    }

    [HttpGet("TopProperty")]
public async Task<ActionResult<int>> GetTopPropertyId()
{
    var topPropertyId = await _context.Properties
        .OrderByDescending(p => p.PropertyId)
        .Select(p => p.PropertyId)
        .FirstOrDefaultAsync();

    if (topPropertyId == 0)
    {
        // No properties found
        return NotFound();
    }

    return Ok(topPropertyId);
}


    [HttpGet("Filter")]
    public async Task<ActionResult<IEnumerable<Property>>> Filter(
     [FromQuery] string address,
     [FromQuery] int? minPrice,
     [FromQuery] int? maxPrice,
     [FromQuery] int? size,
     [FromQuery] int? numberOfBeds)
    {
        var query = _context.Properties
            .Include(p => p.Type)
            .Include(p => p.Agent)
            .AsQueryable();

        // Ensure that address is not null or empty
        if (string.IsNullOrEmpty(address))
        {
            return BadRequest("Address is required.");
        }

        // Apply filters
        query = query.Where(p => p.Address.Contains(address));

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        if (size.HasValue)
        {
            query = query.Where(p => p.Size == size.Value);
        }

        if (numberOfBeds.HasValue)
        {
            if (numberOfBeds.Value < 5)
            {
                // Retrieve properties with an exact match
                query = query.Where(p => p.NbOfBeds == numberOfBeds.Value);
            }
            else
            {
                // Retrieve properties with bedrooms greater than or equal to 5
                query = query.Where(p => p.NbOfBeds >= 5);
            }
        }

        var properties = await query.ToListAsync();

        return Ok(properties);
    }







    [HttpPost]
    public async Task<ActionResult<Property>> PostProperty(Property property)
    {
        _context.Properties.Add(property);
        await _context.SaveChangesAsync();
        return CreatedAtAction("GetProperty", new { id = property.PropertyId }, property);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProperty(int id, Property property)
    {
        if (id != property.PropertyId)
        {
            return BadRequest();
        }

        _context.Entry(property).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PropertyExists(id))
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProperty(int id)
    {
        var property = await _context.Properties.FindAsync(id);
        if (property == null)
        {
            return NotFound();
        }

        _context.Properties.Remove(property);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PropertyExists(int id)
    {
        return _context.Properties.Any(e => e.PropertyId == id);
    }
}
