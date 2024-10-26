using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tr1.v2.Context;
using tr1.v2.Models;

namespace tr1.v2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValoracionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ValoracionesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Valoraciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Valoracion>>> GetValoraciones()
        {
            return await _context.Valoraciones.ToListAsync();
        }

        // GET: api/Valoraciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Valoracion>> GetValoracion(int id)
        {
            var valoracion = await _context.Valoraciones.FindAsync(id);

            if (valoracion == null)
            {
                return NotFound();
            }

            return valoracion;
        }

        // PUT: api/Valoraciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutValoracion(int id, Valoracion valoracion)
        {
            if (id != valoracion.Id)
            {
                return BadRequest();
            }

            _context.Entry(valoracion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ValoracionExists(id))
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

        // POST: api/Valoraciones/like
        [HttpPost("like")]
        [Authorize]
        public async Task<ActionResult<Valoracion>> PostLike(Valoracion valoracion)
        {
            // Verificar si el usuario ya valoró el juego con un dislike
            var dislikeExistente = await _context.Valoraciones
                .FirstOrDefaultAsync(v => v.UsuarioId == valoracion.UsuarioId
                                       && v.JuegoId == valoracion.JuegoId
                                       && v.Tipo == "dislike");

            if (dislikeExistente != null)
            {
                // Si existe un dislike, eliminarlo
                _context.Valoraciones.Remove(dislikeExistente);

                // Decrementar CantDislikes en el juego
                var juego = await _context.Juegos.FindAsync(valoracion.JuegoId);
                juego.CantDislikes -= 1;

                await _context.SaveChangesAsync();
            }

            // Verificar si el usuario ya valoró el juego con un like
            var valoracionExistente = await _context.Valoraciones
                .FirstOrDefaultAsync(v => v.UsuarioId == valoracion.UsuarioId
                                       && v.JuegoId == valoracion.JuegoId
                                       && v.Tipo == "like");

            if (valoracionExistente != null)
            {
                // Si existe un like, eliminarlo
                _context.Valoraciones.Remove(valoracionExistente);

                // Decrementar CantLikes en el juego
                var juego = await _context.Juegos.FindAsync(valoracion.JuegoId);
                juego.CantLikes -= 1;

                await _context.SaveChangesAsync();

                return Ok(new { Message = "Like eliminado." });
            }

            // Asignar el tipo como "like" si no existe
            valoracion.Tipo = "like";

            // Agregar la nueva valoración
            _context.Valoraciones.Add(valoracion);

            // Incrementar CantLikes en el juego
            var juegoCreado = await _context.Juegos.FindAsync(valoracion.JuegoId);
            juegoCreado.CantLikes += 1;

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetValoracion", new { id = valoracion.Id }, valoracion);
        }


        // POST: api/Valoraciones/dislike
        [HttpPost("dislike")]
        [Authorize]
        public async Task<ActionResult<Valoracion>> PostDislike(Valoracion valoracion)
        {
            // Verificar si el usuario ya valoró el juego con un like
            var likeExistente = await _context.Valoraciones
                .FirstOrDefaultAsync(v => v.UsuarioId == valoracion.UsuarioId
                                       && v.JuegoId == valoracion.JuegoId
                                       && v.Tipo == "like");

            if (likeExistente != null)
            {
                // Si existe un like, eliminarlo
                _context.Valoraciones.Remove(likeExistente);

                // Decrementar CantLikes en el juego
                var juego = await _context.Juegos.FindAsync(valoracion.JuegoId);
                juego.CantLikes -= 1;

                await _context.SaveChangesAsync();
            }

            // Verificar si el usuario ya valoró el juego con un dislike
            var valoracionExistente = await _context.Valoraciones
                .FirstOrDefaultAsync(v => v.UsuarioId == valoracion.UsuarioId
                                       && v.JuegoId == valoracion.JuegoId
                                       && v.Tipo == "dislike");

            if (valoracionExistente != null)
            {
                // Si existe un dislike, eliminarlo
                _context.Valoraciones.Remove(valoracionExistente);

                // Decrementar CantDislikes en el juego
                var juego = await _context.Juegos.FindAsync(valoracion.JuegoId);
                juego.CantDislikes -= 1;

                await _context.SaveChangesAsync();

                return Ok(new { Message = "Dislike eliminado." });
            }

            // Asignar el tipo como "dislike" si no existe
            valoracion.Tipo = "dislike";

            // Agregar la nueva valoración
            _context.Valoraciones.Add(valoracion);

            // Incrementar CantDislikes en el juego
            var juegoCreado = await _context.Juegos.FindAsync(valoracion.JuegoId);
            juegoCreado.CantDislikes += 1;

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetValoracion", new { id = valoracion.Id }, valoracion);
        }





        // DELETE: api/Valoraciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteValoracion(int id)
        {
            var valoracion = await _context.Valoraciones.FindAsync(id);
            if (valoracion == null)
            {
                return NotFound();
            }

            _context.Valoraciones.Remove(valoracion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ValoracionExists(int id)
        {
            return _context.Valoraciones.Any(e => e.Id == id);
        }
    }
}
