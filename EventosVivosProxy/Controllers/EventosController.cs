using EventosVivos.Application.DTOs;
using EventosVivos.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EventosVivosProxy.Controllers;

/// <summary>Gestión de eventos en vivo.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class EventosController : ControllerBase
{
    private readonly IEventoService _eventoService;

    public EventosController(IEventoService eventoService)
    {
        _eventoService = eventoService;
    }

    /// <summary>Crea un nuevo evento (RF-01).</summary>
    /// <remarks>
    /// **Reglas de negocio:**
    /// - Título: 5–100 caracteres.
    /// - Descripción: 10–500 caracteres.
    /// - Precio mayor que 0.
    /// - FechaInicio en el futuro.
    /// - FechaFin posterior a FechaInicio.
    /// - CapacidadMaxima no puede exceder la del Venue (RN-01).
    /// - Fin de semana: inicio no puede superar las 22:00 (RN-03).
    ///
    /// **Tipos de evento:** `0` = Conferencia · `1` = Taller · `2` = Concierto
    ///
    /// **Ejemplo de request:**
    /// ```json
    /// {
    ///   "titulo": "Cumbre de Innovación Tecnológica 2025",
    ///   "descripcion": "Conferencia anual sobre tendencias en IA, cloud computing y desarrollo de software moderno.",
    ///   "fechaInicio": "2025-09-15T18:00:00Z",
    ///   "fechaFin":    "2025-09-15T22:00:00Z",
    ///   "tipo": 0,
    ///   "estado": 0,
    ///   "venueNombre": "Plaza Mayor",
    ///   "venueCiudad": "Medellín",
    ///   "capacidadMaxima": 300,
    ///   "precio": 120.00
    /// }
    /// ```
    /// </remarks>
    /// <param name="request">Datos del evento. El campo <c>id</c> se ignora en la creación.</param>
    [HttpPost("CrearEvento")]
    [SwaggerOperation(Summary = "Crea un nuevo evento", OperationId = "CrearEvento", Tags = new[] { "Eventos" })]
    [SwaggerResponse(201, "Evento creado exitosamente.", typeof(EventoResponseDto))]
    [SwaggerResponse(400, "Datos inválidos o regla de negocio violada.", typeof(object))]
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

    /// <summary>Obtiene un evento por su identificador (RF-02).</summary>
    /// <param name="id">Identificador único del evento. Ejemplo: <c>1</c></param>
    [HttpGet("[action]/{id}")]
    [SwaggerOperation(Summary = "Obtiene un evento por ID", OperationId = "ObtenerEvento", Tags = new[] { "Eventos" })]
    [SwaggerResponse(200, "Evento encontrado.", typeof(EventoResponseDto))]
    [SwaggerResponse(404, "Evento no encontrado.")]
    public async Task<IActionResult> ObtenerEvento(int id)
    {
        var evento = await _eventoService.GetByIdAsync(id);
        return evento is null ? NotFound() : Ok(evento);
    }

    /// <summary>Lista todos los eventos disponibles (RF-02).</summary>
    [HttpGet("ListarEventos")]
    [SwaggerOperation(Summary = "Lista todos los eventos", OperationId = "ListarEventos", Tags = new[] { "Eventos" })]
    [SwaggerResponse(200, "Lista de eventos.", typeof(IEnumerable<EventoResponseDto>))]
    public async Task<IActionResult> ListarEventos()
    {
        var eventos = await _eventoService.GetAllAsync();
        return Ok(eventos);
    }

    /// <summary>Actualiza los datos de un evento existente.</summary>
    /// <remarks>
    /// Reemplaza todos los campos del evento indicado.
    ///
    /// **Ejemplo de request:**
    /// ```json
    /// {
    ///   "id": 1,
    ///   "titulo": "Cumbre de Innovación Tecnológica 2025 — Edición Especial",
    ///   "descripcion": "Edición especial con nuevos ponentes internacionales sobre IA generativa.",
    ///   "fechaInicio": "2025-09-15T18:00:00Z",
    ///   "fechaFin":    "2025-09-16T00:00:00Z",
    ///   "tipo": 0,
    ///   "estado": 0,
    ///   "venueNombre": "Plaza Mayor",
    ///   "venueCiudad": "Medellín",
    ///   "capacidadMaxima": 300,
    ///   "precio": 150.00
    /// }
    /// ```
    /// </remarks>
    /// <param name="id">Identificador del evento a actualizar.</param>
    /// <param name="request">Nuevos datos del evento.</param>
    [HttpPut("[action]/{id}")]
    [SwaggerOperation(Summary = "Actualiza un evento", OperationId = "ActualizarEvento", Tags = new[] { "Eventos" })]
    [SwaggerResponse(200, "Evento actualizado.", typeof(EventoResponseDto))]
    [SwaggerResponse(400, "Datos inválidos o regla de negocio violada.", typeof(object))]
    [SwaggerResponse(404, "Evento no encontrado.")]
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

    /// <summary>Elimina un evento por su identificador.</summary>
    /// <param name="id">Identificador del evento a eliminar.</param>
    [HttpDelete("[action]/{id}")]
    [SwaggerOperation(Summary = "Elimina un evento", OperationId = "EliminarEvento", Tags = new[] { "Eventos" })]
    [SwaggerResponse(204, "Evento eliminado exitosamente.")]
    [SwaggerResponse(404, "Evento no encontrado.")]
    public async Task<IActionResult> EliminarEvento(int id)
    {
        await _eventoService.EliminarAsync(id);
        return NoContent();
    }
}
