using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Auth;

[ApiController]
[Route("api/[controller]")]
public class AgentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AgentsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Agent>>> GetAgentsWithUserInfo()
    {
        var agentsWithUserInfo = await _context.Agents
            .Include(a => a.User)
            .Select(a => new Agent
            {
                AgentId = a.AgentId,
                AgentName = a.AgentName,
                UserId = a.UserId,
               
            })
            .ToListAsync();

        return Ok(agentsWithUserInfo);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Agent>> GetAgentById(int id)
    {
        var agentWithUserInfo = await _context.Agents
            .Where(a => a.AgentId == id)
            .Include(a => a.User)
            .Select(a => new Agent
            {
                AgentId = a.AgentId,
                AgentName = a.AgentName,
                UserId = a.UserId,
              
            })
            .FirstOrDefaultAsync();

        if (agentWithUserInfo == null)
        {
            return NotFound();
        }

        return Ok(agentWithUserInfo);
    }
    [HttpGet("AgentRentHouseCount/{agentId}")]
    public IActionResult GetAgentRentHouseCount(int agentId)
    {
        var result = _context.Properties
            .Where(p => p.AgentId == agentId)
            .Join(_context.Agents, p => p.AgentId, a => a.AgentId, (p, a) => new { Property = p, Agent = a })
            .Join(_context.TypeProperties, pa => pa.Property.TypeId, t => t.TypeId, (pa, t) => new { PropertyAgent = pa, TypeProperty = t })
            .Where(pt => pt.TypeProperty.TypeName == "Rent")
            .GroupBy(pt => new { pt.PropertyAgent.Agent.AgentId, pt.TypeProperty.TypeName })
            .Select(group => new
            {
                AgentId = group.Key.AgentId,
                TypeName = group.Key.TypeName,
                AgentTypeCount = group.Count()
            })
            .ToList();

        return Ok(result);
    }


[HttpPost]
    public async Task<IActionResult> PostAgent(int userId, [FromBody] Agent agent)
    {
        // Check if the user exists
        ApplicationUser user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return NotFound("User not found");
        }

        // Check if the user has the "Agent" role
        if (!await _userManager.IsInRoleAsync(user, UserRoles.Agent))
        {
            return Forbid("User does not have the necessary role to create an agent");
        }

        // Check if the associated user has a valid UserName
        if (string.IsNullOrEmpty(user.UserName))
        {
            return BadRequest("User must have a valid UserName");
        }

        // Associate the Agent with the User
        agent.UserId = userId;

      

        // Add the agent to the context and save changes
        _context.Agents.Add(agent);
        await _context.SaveChangesAsync();

        // Return the created Agent entity with user information
        return CreatedAtAction(nameof(GetAgentById), new { id = agent.AgentId }, agent);
    }




    [HttpPut("{id}")]
    public async Task<IActionResult> PutAgent(int id, Agent agent)
    {
        if (id != agent.AgentId)
        {
            return BadRequest();
        }

        _context.Entry(agent).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AgentExists(id))
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
    public async Task<IActionResult> DeleteAgent(int id)
    {
        var agent = await _context.Agents.FindAsync(id);
        if (agent == null)
        {
            return NotFound();
        }

        _context.Agents.Remove(agent);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AgentExists(int id)
    {
        return _context.Agents.Any(e => e.AgentId == id);
    }
}
