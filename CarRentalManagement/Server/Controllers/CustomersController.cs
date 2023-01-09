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
    public class CustomersController : ControllerBase
    {
        //Refactored
        //private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        //public CustomersController(ApplicationDbContext context)
        public CustomersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Customers
        [HttpGet]
        //public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        public async Task<IActionResult> GetCustomers()
        {
            //return await _context.Customers.ToListAsync();
            var makes = await _unitOfWork.Customers.GetAll();
            return Ok(makes);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        //public async Task<ActionResult<Customer>> GetCustomer(int id)
        public async Task<IActionResult> GetCustomer(int id)
        {
            //var make = await _context.Customers.FindAsync(id);
            var make = await _unitOfWork.Customers.Get(q => q.Id == id);

            if (make == null)
            {
                return NotFound();
            }

            //return make;
            return Ok(make);
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer make)
        {
            if (id != make.Id)
            {
                return BadRequest();
            }

            //_context.Entry(make).State = EntityState.Modified;
            _unitOfWork.Customers.Update(make);

            try
            {
                //await _context.SaveChangesAsync();
                await _unitOfWork.Save(HttpContext);
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!CustomerExists(id))
                if (!await CustomerExists(id))
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

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer make)
        {
            //_context.Customers.Add(make);
            //await _context.SaveChangesAsync();
            await _unitOfWork.Customers.Insert(make);
            await _unitOfWork.Save(HttpContext);

            return CreatedAtAction("GetCustomer", new { id = make.Id }, make);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            //var make = await _context.Customers.FindAsync(id);
            var make = await _unitOfWork.Customers.Get(q => q.Id == id);
            if (make == null)
            {
                return NotFound();
            }

            //_context.Customers.Remove(make);
            //await _context.SaveChangesAsync();
            await _unitOfWork.Customers.Delete(id);
            await _unitOfWork.Save(HttpContext);

            return NoContent();
        }

        //private bool CustomerExists(int id)
        private async Task<bool> CustomerExists(int id)
        {
            //return _context.Customers.Any(e => e.Id == id);
            var make = await _unitOfWork.Customers.Get(q => q.Id == id);
            return make != null;
        }
    }
}
