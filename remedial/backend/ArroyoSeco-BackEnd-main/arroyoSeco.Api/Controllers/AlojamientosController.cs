using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using arroyoSeco.Infrastructure.Data;
using arroyoSeco.Application.Features.Alojamiento.Commands.Crear;

namespace arroyoSeco.Api.Controllers;

[ApiController]
[Route("api/alojamientos")]
public class AlojamientosController : ControllerBase
{
    private readonly AppDbContext _ctx;
    private readonly CrearAlojamientoCommandHandler _crear;

    public AlojamientosController(AppDbContext ctx, CrearAlojamientoCommandHandler crear)
    {
        _ctx = ctx;
        _crear = crear;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? estatus)
    {
        // Traemos los alojamientos sin filtrar por 'Estado' para evitar el error de compilación
        var q = _ctx.Alojamientos.AsQueryable();
        
        var list = await q
            .Select(a => new {
                a.Id, 
                a.Nombre, 
                a.Ubicacion, 
                a.PrecioPorNoche, 
                a.FotoPrincipal
                // Se eliminó la línea de 'Estado/Estatus' porque no existe en la entidad
            }).ToListAsync();

        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearAlojamientoCommand cmd, CancellationToken ct)
    {
        var id = await _crear.Handle(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var alojamiento = await _ctx.Alojamientos
            .Include(a => a.Fotos)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (alojamiento == null) return NotFound();
        return Ok(alojamiento);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, [FromBody] CrearAlojamientoCommand dto, CancellationToken ct)
    {
        var a = await _ctx.Alojamientos
            .Include(x => x.Fotos)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (a == null) return NotFound();
        
        a.Nombre = dto.Nombre;
        a.Ubicacion = dto.Ubicacion;
        a.MaxHuespedes = dto.MaxHuespedes;
        a.Habitaciones = dto.Habitaciones;
        a.Banos = dto.Banos;
        a.PrecioPorNoche = dto.PrecioPorNoche;
        a.FotoPrincipal = dto.FotoPrincipal;
        
        if (dto.FotosUrls != null)
        {
            a.Fotos.Clear();
            var nuevasFotos = dto.FotosUrls.Select((u, i) => new arroyoSeco.Domain.Entities.Alojamientos.FotoAlojamiento 
            { 
                Url = u, 
                Orden = i + 1 
            });
            foreach(var foto in nuevasFotos) a.Fotos.Add(foto);
        }
        
        await _ctx.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
    {
        var a = await _ctx.Alojamientos.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (a == null) return NotFound();

        _ctx.Alojamientos.Remove(a);
        await _ctx.SaveChangesAsync(ct);
        return NoContent();
    }
}
