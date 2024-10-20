using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tr1.v2.Context;
using tr1.v2.DTOs;
using tr1.v2.Models;

namespace tr1.v2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JuegosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JuegosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetJuegos()
        {
            try
            {
                // Primero obtenemos los datos relacionados con JuegoGenero y Consola usando JuegoConsola
                var juegoGenerosConsolas = await _context.JuegoGenero
                    .Include(jg => jg.Juego)            // Relación con la entidad Juego
                    .Include(jg => jg.Genero)           // Relación con la entidad Genero
                    .ToListAsync();

                // Ahora obtenemos las consolas relacionadas con cada juego usando JuegoConsola
                var juegoConsolas = await _context.JuegoConsola
                    .Include(jc => jc.Consola)         // Relación con la entidad Consola
                    .ToListAsync();

                if (juegoGenerosConsolas == null || !juegoGenerosConsolas.Any())
                {
                    return NotFound("No se encontraron juegos.");
                }

                // Agrupamos los resultados por JuegoId
                var groupedResult = juegoGenerosConsolas
                    .GroupBy(jg => jg.JuegoId)    // Agrupamos por JuegoId
                    .Select(g => new
                    {
                        JuegoId = g.Key,
                        Juego = new
                        {
                            Id = g.First().Juego.Id,
                            Nombre = g.First().Juego.Nombre,
                            Descripcion = g.First().Juego.Descripcion,
                            Fecha = g.First().Juego.Fecha,
                            Desarrolladora = g.First().Juego.Desarrolladora
                        },
                        Generos = g.Select(jg => new
                        {
                            GeneroId = jg.GeneroId,
                            GeneroNombre = jg.Genero.Nombre
                        }).ToList(),
                        Consolas = juegoConsolas
                            .Where(jc => jc.JuegoId == g.Key)  // Relacionamos JuegoConsola por JuegoId
                            .Select(jc => new
                            {
                                ConsolaId = jc.ConsolaId,
                                ConsolaNombre = jc.Consola.Nombre
                            }).Distinct().ToList()  // Para evitar duplicados
                    }).ToList();

                return Ok(groupedResult);
            }
            catch (Exception ex)
            {
                // Registro del error y respuesta con error interno
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Juegos/1
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetJuegoConDetalles(int id)
        {
            try
            {

                //var sdas = await _context.Juegos
                    //.Include(jg => jg.JuegoConsola);
                // Obtenemos el juego con sus géneros (JuegoGenero)
                var juegoGeneros = await _context.JuegoGenero
                    .Include(jg => jg.Juego)           // Relación con la entidad Juego
                    .Include(jg => jg.Genero)          // Relación con la entidad Genero
                    .Where(jg => jg.JuegoId == id)     // Filtramos por el Id del juego
                    .ToListAsync();

                // Obtenemos las consolas relacionadas con el juego usando JuegoConsola
                var juegoConsolas = await _context.JuegoConsola
                    .Include(jc => jc.Consola)         // Relación con la entidad Consola
                    .Where(jc => jc.JuegoId == id)     // Filtramos por el Id del juego
                    .ToListAsync();

                if (juegoGeneros == null || !juegoGeneros.Any())
                {
                    return NotFound($"No se encontró el juego con Id {id}.");
                }

                // Construimos la respuesta con los detalles del juego, sus géneros y consolas
                var groupedResult = new
                {
                    JuegoId = id,
                    Juego = new
                    {
                        Id = juegoGeneros.First().Juego.Id,
                        Nombre = juegoGeneros.First().Juego.Nombre,
                        Descripcion = juegoGeneros.First().Juego.Descripcion,
                        Fecha = juegoGeneros.First().Juego.Fecha,
                        Desarrolladora = juegoGeneros.First().Juego.Desarrolladora
                    },
                    Generos = juegoGeneros.Select(jg => new
                    {
                        GeneroId = jg.GeneroId,
                        GeneroNombre = jg.Genero.Nombre
                    }).ToList(),
                    Consolas = juegoConsolas
                        .Select(jc => new
                        {
                            ConsolaId = jc.ConsolaId,
                            ConsolaNombre = jc.Consola.Nombre
                        }).Distinct().ToList()
                };

                return Ok(groupedResult);
            }
            catch (Exception ex)
            {
                // Registro del error y respuesta con error interno
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }


        // PUT: api/Juegos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJuego(int id, Juego juego)
        {
            if (id != juego.Id)
            {
                return BadRequest();
            }

            _context.Entry(juego).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JuegoExists(id))
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

        [HttpPost]
        public async Task<ActionResult<JuegoDTO>> PostJuego(JuegoDTO juegoDTO)
        {
            var juego = new Juego
            {
                Nombre = juegoDTO.Nombre,
                Descripcion = juegoDTO.Descripcion,
                Fecha = juegoDTO.Fecha,
                Desarrolladora = juegoDTO.Desarrolladora,
            };

            _context.Juegos.Add(juego);

            await _context.SaveChangesAsync();

            var juegoId = juego.Id; 

            foreach (var generoId in juegoDTO.Generos)
            {
                var juegoGenero = new JuegoGenero
                {
                    JuegoId = juego.Id,
                    GeneroId = generoId,
                };
                _context.JuegoGenero.Add(juegoGenero);
            }
            foreach (var consolaId in juegoDTO.Consolas)
            {
                var juegoConsola = new JuegoConsola
                {
                    ConsolaId = consolaId,
                    JuegoId = juego.Id,
                };
                _context.JuegoConsola.Add(juegoConsola);
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJuegoConDetalles", new { id = juego.Id }, juego);
        }


        // DELETE: api/Juegos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJuego(int id)
        {
            var juego = await _context.Juegos.FindAsync(id);
            if (juego == null)
            {
                return NotFound();
            }

            _context.Juegos.Remove(juego);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JuegoExists(int id)
        {
            return _context.Juegos.Any(e => e.Id == id);
        }
    }
}




