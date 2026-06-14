using EventosVivos.Application.DTOs;
using EventosVivos.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventosVivosProxy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservasController : ControllerBase
{
    private readonly IReservaService _reservaService;

    public ReservasController(IReservaService reservaService)
    {
        _reservaService = reservaService;
    }

    // Listar todas las reservas
    [HttpGet]
    public async Task<IActionResult> ListarReservas()
    {
        var reservas = await _reservaService.GetAllAsync();
        return Ok(reservas);
    }

    // Obtener reserva por ID
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerReserva(int id)
    {
        var reserva = await _reservaService.GetByIdAsync(id);
        return reserva is null ? NotFound() : Ok(reserva);
    }

    // Listar reservas de un evento
    [HttpGet("evento/{eventoId}")]
    public async Task<IActionResult> ListarPorEvento(int eventoId)
    {
        var reservas = await _reservaService.GetByEventoIdAsync(eventoId);
        return Ok(reservas);
    }

    // RF-03: Crear Reserva
    [HttpPost]
    public async Task<IActionResult> CrearReserva([FromBody] CrearReservaRequest request)
    {
        try
        {
            var reserva = await _reservaService.CrearAsync(request);
            return CreatedAtAction(nameof(ObtenerReserva), new { id = reserva.Id }, reserva);
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

    // RF-04: Confirmar Pago
    [HttpPost("{id}/confirmar")]
    public async Task<IActionResult> ConfirmarPago(int id)
    {
        try
        {
            var reservaConfirmada = await _reservaService.ConfirmarPagoAsync(id);
            return Ok(reservaConfirmada);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    // RF-05: Cancelar Reserva
    [HttpPost("{id}/cancelar")]
    public async Task<IActionResult> CancelarReserva(int id)
    {
        try
        {
            await _reservaService.CancelarAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}
