using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalManagement.Server.Data;
using CarRentalManagement.Server.Domain;
using CarRentalManagement.Server.IRepository;

namespace CarRentalManagement.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        //Refactored
        //private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        //public BookingsController(ApplicationDbContext context)
        public BookingsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Bookings
        [HttpGet]
        //public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        public async Task<IActionResult> GetBookings()
        {
            //return await _context.Bookings.ToListAsync();
            var makes = await _unitOfWork.Bookings.GetAll();
            return Ok(makes);
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        //public async Task<ActionResult<Booking>> GetBooking(int id)
        public async Task<IActionResult> GetBooking(int id)
        {
            //var make = await _context.Bookings.FindAsync(id);
            var make = await _unitOfWork.Bookings.Get(q => q.Id == id);

            if (make == null)
            {
                return NotFound();
            }

            //return make;
            return Ok(make);
        }

        // PUT: api/Bookings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, Booking make)
        {
            if (id != make.Id)
            {
                return BadRequest();
            }

            //_context.Entry(make).State = EntityState.Modified;
            _unitOfWork.Bookings.Update(make);

            try
            {
                //await _context.SaveChangesAsync();
                await _unitOfWork.Save(HttpContext);
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!BookingExists(id))
                if (!await BookingExists(id))
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

        // POST: api/Bookings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(Booking make)
        {
            //_context.Bookings.Add(make);
            //await _context.SaveChangesAsync();
            await _unitOfWork.Bookings.Insert(make);
            await _unitOfWork.Save(HttpContext);

            return CreatedAtAction("GetBooking", new { id = make.Id }, make);
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            //var make = await _context.Bookings.FindAsync(id);
            var make = await _unitOfWork.Bookings.Get(q => q.Id == id);
            if (make == null)
            {
                return NotFound();
            }

            //_context.Bookings.Remove(make);
            //await _context.SaveChangesAsync();
            await _unitOfWork.Bookings.Delete(id);
            await _unitOfWork.Save(HttpContext);

            return NoContent();
        }

        //private bool BookingExists(int id)
        private async Task<bool> BookingExists(int id)
        {
            //return _context.Bookings.Any(e => e.Id == id);
            var make = await _unitOfWork.Bookings.Get(q => q.Id == id);
            return make != null;
        }
    }
}
