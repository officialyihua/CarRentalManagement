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
    public class ModelsController : ControllerBase
    {
        //Refactored
        //private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        //public ModelsController(ApplicationDbContext context)
        public ModelsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Models
        [HttpGet]
        //public async Task<ActionResult<IEnumerable<Model>>> GetModels()
        public async Task<IActionResult> GetModels()
        {
            //return await _context.Models.ToListAsync();
            var makes = await _unitOfWork.Models.GetAll();
            return Ok(makes);
        }

        // GET: api/Models/5
        [HttpGet("{id}")]
        //public async Task<ActionResult<Model>> GetModel(int id)
        public async Task<IActionResult> GetModel(int id)
        {
            //var make = await _context.Models.FindAsync(id);
            var make = await _unitOfWork.Models.Get(q => q.Id == id);

            if (make == null)
            {
                return NotFound();
            }

            //return make;
            return Ok(make);
        }

        // PUT: api/Models/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModel(int id, Model make)
        {
            if (id != make.Id)
            {
                return BadRequest();
            }

            //_context.Entry(make).State = EntityState.Modified;
            _unitOfWork.Models.Update(make);

            try
            {
                //await _context.SaveChangesAsync();
                await _unitOfWork.Save(HttpContext);
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!ModelExists(id))
                if (!await ModelExists(id))
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

        // POST: api/Models
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Model>> PostModel(Model make)
        {
            //_context.Models.Add(make);
            //await _context.SaveChangesAsync();
            await _unitOfWork.Models.Insert(make);
            await _unitOfWork.Save(HttpContext);

            return CreatedAtAction("GetModel", new { id = make.Id }, make);
        }

        // DELETE: api/Models/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModel(int id)
        {
            //var make = await _context.Models.FindAsync(id);
            var make = await _unitOfWork.Models.Get(q => q.Id == id);
            if (make == null)
            {
                return NotFound();
            }

            //_context.Models.Remove(make);
            //await _context.SaveChangesAsync();
            await _unitOfWork.Models.Delete(id);
            await _unitOfWork.Save(HttpContext);

            return NoContent();
        }

        //private bool ModelExists(int id)
        private async Task<bool> ModelExists(int id)
        {
            //return _context.Models.Any(e => e.Id == id);
            var make = await _unitOfWork.Models.Get(q => q.Id == id);
            return make != null;
        }
    }
}
