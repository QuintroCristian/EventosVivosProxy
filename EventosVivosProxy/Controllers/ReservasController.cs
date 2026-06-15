using EventosVivos.Application.DTOs;
using EventosVivos.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EventosVivosProxy.Controllers;

/// <summary>Gestión de reservas de eventos.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class ReservasController : ControllerBase
{
    private readonly IReservaService _reservaService;

    public ReservasController(IReservaService reservaService)
    {
        _reservaService = reservaService;
    }

    /// <summary>Lista todas las reservas.</summary>
    [HttpGet("ListarReservas")]
    [SwaggerOperation(Summary = "Lista todas las reservas", OperationId = "ListarReservas", Tags = new[] { "Reservas" })]
    [SwaggerResponse(200, "Lista de reservas.", typeof(IEnumerable<ReservaResponseDto>))]
    public async Task<IActionResult> ListarReservas()
    {
        var reservas = await _reservaService.GetAllAsync();
        return Ok(reservas);
    }

    /// <summary>Obtiene una reserva por su identificador.</summary>
    /// <param name="id">Identificador único de la reserva. Ejemplo: <c>1</c></param>
    [HttpGet("ObtenerReserva/{id}")]
    [SwaggerOperation(Summary = "Obtiene una reserva por ID", OperationId = "ObtenerReserva", Tags = new[] { "Reservas" })]
    [SwaggerResponse(200, "Reserva encontrada.", typeof(ReservaResponseDto))]
    [SwaggerResponse(404, "Reserva no encontrada.")]
    public async Task<IActionResult> ObtenerReserva(int id)
    {
        var reserva = await _reservaService.GetByIdAsync(id);
        return reserva is null ? NotFound() : Ok(reserva);
    }

    /// <summary>Lista las reservas de un evento específico.</summary>
    /// <param name="eventoId">Identificador del evento. Ejemplo: <c>1</c></param>
    [HttpGet("evento/ListarPorEvento/{eventoId}")]
    [SwaggerOperation(Summary = "Lista reservas de un evento", OperationId = "ListarPorEvento", Tags = new[] { "Reservas" })]
    [SwaggerResponse(200, "Lista de reservas del evento.", typeof(IEnumerable<ReservaResponseDto>))]
    public async Task<IActionResult> ListarPorEvento(int eventoId)
    {
        var reservas = await _reservaService.GetByEventoIdAsync(eventoId);
        return Ok(reservas);
    }

    /// <summary>Crea una nueva reserva para un evento (RF-03).</summary>
    /// <remarks>
    /// **Reglas de negocio:**
    /// - El evento debe estar en estado `Activo`.
    /// - La cantidad debe ser mayor que 0.
    /// - Las entradas no pueden superar la capacidad disponible del evento.
    ///
    /// **Estados de reserva:** `0` = PendientePago · `1` = Confirmada · `2` = Cancelada
    ///
    /// **Ejemplo de request:**
    /// ```json
    /// {
    ///   "eventoId": 1,
    ///   "nombreComprador": "Carlos Quintero",
    ///   "emailComprador": "carlos@example.com",
    ///   "cantidad": 2
    /// }
    /// ```
    /// </remarks>
    [HttpPost("CrearReserva")]
    [SwaggerOperation(Summary = "Crea una nueva reserva", OperationId = "CrearReserva", Tags = new[] { "Reservas" })]
    [SwaggerResponse(201, "Reserva creada exitosamente.", typeof(ReservaResponseDto))]
    [SwaggerResponse(400, "Datos inválidos o regla de negocio violada.", typeof(object))]
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

    /// <summary>Confirma el pago de una reserva (RF-04).</summary>
    /// <remarks>
    /// Cambia el estado de <c>PendientePago</c> → <c>Confirmada</c>.
    /// Solo es posible si la reserva está en estado <c>PendientePago</c>.
    /// </remarks>
    /// <param name="id">Identificador de la reserva a confirmar. Ejemplo: <c>1</c></param>
    [HttpPost("ConfirmarPago/{id}")]
    [SwaggerOperation(Summary = "Confirma el pago de una reserva (RF-04)", OperationId = "ConfirmarPago", Tags = new[] { "Reservas" })]
    [SwaggerResponse(200, "Reserva confirmada.", typeof(ReservaResponseDto))]
    [SwaggerResponse(400, "La reserva no puede confirmarse en su estado actual.", typeof(object))]
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

    /// <summary>Cancela una reserva existente (RF-05).</summary>
    /// <remarks>
    /// Cambia el estado a <c>Cancelada</c>.
    /// Si la reserva ya fue <c>Confirmada</c>, se aplica penalización (RN-05).
    /// </remarks>
    /// <param name="id">Identificador de la reserva a cancelar. Ejemplo: <c>1</c></param>
    [HttpPost("CancelarReserva/{id}")]
    [SwaggerOperation(Summary = "Cancela una reserva (RF-05)", OperationId = "CancelarReserva", Tags = new[] { "Reservas" })]
    [SwaggerResponse(204, "Reserva cancelada exitosamente.")]
    [SwaggerResponse(400, "La reserva no puede cancelarse en su estado actual.", typeof(object))]
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
