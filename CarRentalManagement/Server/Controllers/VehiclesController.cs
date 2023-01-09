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
    public class VehiclesController : ControllerBase
    {
        //Refactored
        //private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        //public VehiclesController(ApplicationDbContext context)
        public VehiclesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Vehicles
        [HttpGet]
        //public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicles()
        public async Task<IActionResult> GetVehicles()
        {
            //return await _context.Vehicles.ToListAsync();
            var makes = await _unitOfWork.Vehicles.GetAll();
            return Ok(makes);
        }

        // GET: api/Vehicles/5
        [HttpGet("{id}")]
        //public async Task<ActionResult<Vehicle>> GetVehicle(int id)
        public async Task<IActionResult> GetVehicle(int id)
        {
            //var make = await _context.Vehicles.FindAsync(id);
            var make = await _unitOfWork.Vehicles.Get(q => q.Id == id);

            if (make == null)
            {
                return NotFound();
            }

            //return make;
            return Ok(make);
        }

        // PUT: api/Vehicles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVehicle(int id, Vehicle make)
        {
            if (id != make.Id)
            {
                return BadRequest();
            }

            //_context.Entry(make).State = EntityState.Modified;
            _unitOfWork.Vehicles.Update(make);

            try
            {
                //await _context.SaveChangesAsync();
                await _unitOfWork.Save(HttpContext);
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!VehicleExists(id))
                if (!await VehicleExists(id))
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

        // POST: api/Vehicles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vehicle>> PostVehicle(Vehicle make)
        {
            //_context.Vehicles.Add(make);
            //await _context.SaveChangesAsync();
            await _unitOfWork.Vehicles.Insert(make);
            await _unitOfWork.Save(HttpContext);

            return CreatedAtAction("GetVehicle", new { id = make.Id }, make);
        }

        // DELETE: api/Vehicles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            //var make = await _context.Vehicles.FindAsync(id);
            var make = await _unitOfWork.Vehicles.Get(q => q.Id == id);
            if (make == null)
            {
                return NotFound();
            }

            //_context.Vehicles.Remove(make);
            //await _context.SaveChangesAsync();
            await _unitOfWork.Vehicles.Delete(id);
            await _unitOfWork.Save(HttpContext);

            return NoContent();
        }

        //private bool VehicleExists(int id)
        private async Task<bool> VehicleExists(int id)
        {
            //return _context.Vehicles.Any(e => e.Id == id);
            var make = await _unitOfWork.Vehicles.Get(q => q.Id == id);
            return make != null;
        }
    }
}
