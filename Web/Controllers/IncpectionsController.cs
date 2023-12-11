using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Web.Auth;

namespace JWTAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncpectionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IncpectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Incpections>>> GetBookings()
        {
            var bookings = await _context.Incpect
                .Where(incpection => incpection.AvailableDateTime != null)
                .ToListAsync();

            return bookings;
        }

        [HttpGet("GetAvailableDates")]
        public ActionResult<IEnumerable<string>> GetAvailableDates()
        {
            // Retrieve available dates from your data source (e.g., database)
            var availableDates = _context.Bookings
                .Where(b => b.AvailableDateTime > DateTime.Now)
                .Select(b => b.AvailableDateTime.Date.ToString("yyyy-MM-ddTHH:mm:ss"))
                .Distinct()
                .ToList();

            return Ok(availableDates);
        }
        [HttpGet("GetJoinedData")]
        public ActionResult<IEnumerable<object>> GetJoinedData(int agentId)
        {
            var joinedData = _context.Incpect
                .Join(
                    _context.Properties,
                    i => i.PropertyId,
                    p => p.PropertyId,
                    (i, p) => new
                    {
                        i.BookingId,
                        i.AvailableDateTime,
                        i.ReservedDateTime,
                        i.PropertyId,
                        p.AgentId
                    }
                )
                .Where(j => j.AgentId == agentId)
                .Take(1000) // You can adjust this as needed
                .ToList();

            return Ok(joinedData);
        }


        // GET: api/Booking/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Incpections>> GetBooking(int id)
        {
            var booking = await _context.Incpect.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }

        // POST: api/Booking
        [Authorize(Roles = UserRoles.Agent)]
        [HttpPost]
        public async Task<ActionResult<Incpections>> PostBooking(Incpections booking)
        {
            _context.Incpect.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, booking);
        }

        // PUT: api/Booking/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, Incpections booking)
        {
            if (id != booking.BookingId)
            {
                return BadRequest();
            }

            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Booking/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Incpect.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            _context.Incpect.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
