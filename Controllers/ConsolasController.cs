using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tr1.v2.Context;
using tr1.v2.Models;

namespace tr1.v2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsolasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConsolasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Consolas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Consola>>> GetConsolas()
        {
            return await _context.Consolas.ToListAsync();
        }

        // GET: api/Consolas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Consola>> GetConsola(int id)
        {
            var consola = await _context.Consolas.FindAsync(id);

            if (consola == null)
            {
                return NotFound();
            }

            return consola;
        }

        // PUT: api/Consolas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConsola(int id, Consola consola)
        {
            if (id != consola.Id)
            {
                return BadRequest();
            }

            _context.Entry(consola).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConsolaExists(id))
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

        // POST: api/Consolas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Consola>> PostConsola(Consola consola)
        {
            _context.Consolas.Add(consola);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetConsola", new { id = consola.Id }, consola);
        }

        // DELETE: api/Consolas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConsola(int id)
        {
            var consola = await _context.Consolas.FindAsync(id);
            if (consola == null)
            {
                return NotFound();
            }

            _context.Consolas.Remove(consola);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ConsolaExists(int id)
        {
            return _context.Consolas.Any(e => e.Id == id);
        }
    }
}
