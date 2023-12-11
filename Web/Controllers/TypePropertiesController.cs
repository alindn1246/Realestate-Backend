using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Auth;

[Authorize(Roles = UserRoles.Admin)]
[ApiController]
[Route("api/[controller]")]
public class TypePropertiesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TypePropertiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TypeProperty>>> GetTypeProperties()
    {
        var typeProperties = await _context.TypeProperties.ToListAsync();

        return Ok(typeProperties);
    }

    
    [HttpGet("{id}")]
    public async Task<ActionResult<TypeProperty>> GetTypeProperty(int id)
    {
        var typeProperty = await _context.TypeProperties.FindAsync(id);

        if (typeProperty == null)
        {
            return NotFound();
        }

        return Ok(typeProperty);
    }

    [HttpPost]
    public async Task<ActionResult<TypeProperty>> PostTypeProperty(TypeProperty typeProperty)
    {
        _context.TypeProperties.Add(typeProperty);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetTypeProperty", new { id = typeProperty.TypeId }, typeProperty);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTypeProperty(int id, TypeProperty typeProperty)
    {
        if (id != typeProperty.TypeId)
        {
            return BadRequest();
        }

        _context.Entry(typeProperty).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TypePropertyExists(id))
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
    public async Task<IActionResult> DeleteTypeProperty(int id)
    {
        var typeProperty = await _context.TypeProperties.FindAsync(id);
        if (typeProperty == null)
        {
            return NotFound();
        }

        _context.TypeProperties.Remove(typeProperty);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TypePropertyExists(int id)
    {
        return _context.TypeProperties.Any(e => e.TypeId == id);
    }
}
