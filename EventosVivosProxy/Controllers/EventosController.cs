using EventosVivos.Application.DTOs;
using EventosVivos.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventosVivosProxy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventosController : ControllerBase
{
    private readonly IEventoService _eventoService;

    public EventosController(IEventoService eventoService)
    {
        _eventoService = eventoService;
    }

    // RF-01: Crear Evento
    [HttpPost]
    public async Task<IActionResult> CrearEvento([FromBody] EventoResponseDto request)
    {
        try
        {
            var creado = await _eventoService.CrearAsync(request);
            return CreatedAtAction(nameof(ObtenerEvento), new { id = creado.Id }, creado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    // RF-02: Obtener evento por ID
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerEvento(int id)
    {
        var evento = await _eventoService.GetByIdAsync(id);
        return evento is null ? NotFound() : Ok(evento);
    }

    // RF-02: Listar todos los eventos
    [HttpGet]
    public async Task<IActionResult> ListarEventos()
    {
        var eventos = await _eventoService.GetAllAsync();
        return Ok(eventos);
    }

    // Actualizar evento
    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarEvento(int id, [FromBody] EventoResponseDto request)
    {
        try
        {
            var actualizado = await _eventoService.ActualizarAsync(id, request);
            return Ok(actualizado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    // Eliminar evento
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarEvento(int id)
    {
        await _eventoService.EliminarAsync(id);
        return NoContent();
    }
}
