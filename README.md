# EventosVivosProxy 

API REST construida con **ASP.NET Core (.NET 10)** para la gestión de eventos en vivo y sus reservas.  
Implementa **Clean Architecture** en cuatro capas y persiste los datos en una base de datos **en memoria** (EF Core InMemory) que se inicializa automáticamente con datos de prueba al arrancar.

---

## Tabla de contenidos

1. [Requisitos previos](#1-requisitos-previos)
2. [Clonar y restaurar el proyecto](#2-clonar-y-restaurar-el-proyecto)
3. [Ejecución local](#3-ejecución-local)
4. [Explorar la API con Swagger](#4-explorar-la-api-con-swagger)
5. [Endpoints disponibles](#5-endpoints-disponibles)
6. [Justificación de la arquitectura](#6-justificación-de-la-arquitectura)
7. [Seed inicial de datos](#7-seed-inicial-de-datos)
8. [Reglas de negocio](#8-reglas-de-negocio)
9. [Estructura del proyecto](#9-estructura-del-proyecto)

---

## 1. Requisitos previos

| Herramienta | Versión mínima | Descarga |
|---|---|---|
| .NET SDK | **10.0** | https://dotnet.microsoft.com/download |
| Git | cualquier estable | https://git-scm.com |
| IDE opcional | Visual Studio 2022/2026 · VS Code · Rider | — |

Verificar la instalación del SDK:

```bash
dotnet --version
# Debe mostrar 10.x.x
```

---

## 2. Clonar y restaurar el proyecto

```bash
# Clonar el repositorio
git clone https://github.com/QuintroCristian/EventosVivosProxy.git
cd EventosVivosProxy

# Restaurar dependencias NuGet
dotnet restore
```

---

## 3. Ejecución local

### Opción A — CLI de .NET (recomendada)

```bash
# Desde la raíz del repositorio
dotnet run --project EventosVivosProxy/EventosVivosProxy.csproj
```

La aplicación queda disponible en:

| Protocolo | URL |
|---|---|
| HTTP | http://localhost:5200 |
| HTTPS | https://localhost:7200 |
| Swagger UI | http://localhost:5200/swagger |

> **Nota:** La primera vez que se usa HTTPS en desarrollo puede ser necesario confiar en el certificado de desarrollo:
> ```bash
> dotnet dev-certs https --trust
> ```

### Opción B — Visual Studio

1. Abrir `EventosVivosProxy.sln`.
2. Seleccionar el perfil de inicio **`http`** o **`https`** en la barra de herramientas.
3. Presionar **F5** (con depuración) o **Ctrl+F5** (sin depuración).

### Opción C — Perfil IIS Express

Seleccionar el perfil **IIS Express** en Visual Studio.  
El navegador abrirá automáticamente `http://localhost:5600/swagger/index.html`.

---

## 4. Explorar la API con Swagger

Una vez en ejecución, navegar a:

```
http://localhost:5200/swagger
```

La interfaz muestra todos los endpoints agrupados por etiqueta (**Eventos** / **Reservas**), con ejemplos de request/response, tipos de datos y códigos de respuesta HTTP documentados con XML comments.

---

## 5. Endpoints disponibles

### Eventos (`/api/Eventos`)

| Método | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/Eventos/CrearEvento` | Crea un nuevo evento (RF-01) |
| `GET` | `/api/Eventos/ObtenerEvento/{id}` | Obtiene un evento por ID (RF-02) |
| `GET` | `/api/Eventos/ListarEventos` | Lista todos los eventos (RF-02) |
| `GET` | `/api/Eventos/Venues` | Lista los venues disponibles |
| `PUT` | `/api/Eventos/ActualizarEvento/{id}` | Actualiza un evento existente |
| `DELETE` | `/api/Eventos/EliminarEvento/{id}` | Elimina un evento por ID |

### Reservas (`/api/Reservas`)

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/Reservas/ListarReservas` | Lista todas las reservas |
| `GET` | `/api/Reservas/ObtenerReserva/{id}` | Obtiene una reserva por ID |
| `GET` | `/api/Reservas/evento/ListarPorEvento/{eventoId}` | Lista reservas de un evento |
| `POST` | `/api/Reservas/CrearReserva` | Crea una nueva reserva (RF-03) |
| `POST` | `/api/Reservas/ConfirmarPago/{id}` | Confirma el pago de una reserva (RF-04) |
| `POST` | `/api/Reservas/CancelarReserva/{id}` | Cancela una reserva (RF-05) |

### Ejemplos de payload

**Crear Evento**
```json
{
  "titulo": "Cumbre de Innovación Tecnológica 2026",
  "descripcion": "Conferencia anual sobre tendencias en IA, cloud computing y desarrollo de software moderno.",
  "fechaInicio": "2026-09-15T18:00:00Z",
  "fechaFin":    "2026-09-15T22:00:00Z",
  "tipo": 0,
  "venueId": 1,
  "capacidadMaxima": 150,
  "precio": 120.00
}
```

**Crear Reserva**
```json
{
  "eventoId": 1,
  "nombreComprador": "Carlos Quintero",
  "emailComprador": "carlos@example.com",
  "cantidad": 2
}
```

---

## 6. Justificación de la arquitectura

El proyecto adopta **Clean Architecture** (también conocida como Onion Architecture) organizada en cuatro capas con dependencias que apuntan siempre hacia el núcleo:

```
┌──────────────────────────────────────────────────────┐
│           EventosVivosProxy  (Presentación)          │
│    Controllers · Program.cs · Swagger · CORS         │
├──────────────────────────────────────────────────────┤
│        EventosVivos.Application  (Aplicación)        │
│    Services · DTOs · Interfaces de servicio          │
├──────────────────────────────────────────────────────┤
│      EventosVivos.Infrastructure  (Infraestructura)  │
│    DbContext · Repositorios EF Core InMemory         │
├──────────────────────────────────────────────────────┤
│           EventosVivos.Domain  (Dominio)             │
│    Entities · Enums · Interfaces de repositorio      │
└──────────────────────────────────────────────────────┘
```

### Por qué esta arquitectura

| Decisión | Justificación |
|---|---|
| **Dominio sin dependencias externas** | `EventosVivos.Domain` no referencia ningún framework. Las reglas de negocio (RN-01 a RN-07) viven en las entidades y son testeables en aislamiento. |
| **Inversión de dependencias (DIP)** | Las capas superiores dependen de interfaces (`IEventoRepository`, `IReservaRepository`, `IVenueRepository`), no de implementaciones concretas. Esto permite sustituir EF Core InMemory por SQL Server, PostgreSQL, etc., sin tocar la lógica de negocio. |
| **Capa de Aplicación con Casos de Uso** | `EventoService` y `ReservaService` orquestan la lógica de aplicación (validaciones cruzadas, generación de código de reserva `EV-{6dígitos}`), manteniendo los controladores delgados. |
| **Infraestructura aislada** | El `DbContext` y sus mappings con Fluent API solo existen en la capa de infraestructura. El dominio desconoce EF Core. |
| **Base de datos InMemory** | Elegida para simplificar la ejecución local sin requerir un motor externo. La capa de infraestructura puede reemplazarse por una implementación con base de datos persistente sin afectar el resto. |
| **CORS abierto en desarrollo** | La política `AllowAll` facilita integraciones con frontends (ej. Angular en `localhost:4200`) durante el desarrollo. En producción debe restringirse a orígenes específicos. |

---

## 7. Seed inicial de datos

La base de datos **en memoria** se vacía con cada reinicio de la aplicación. El seed se ejecuta automáticamente en dos fases al arrancar:

### Fase 1 — Venues (OnModelCreating)

Definidos como datos estáticos en `EventosVivosDbContext.OnModelCreating` mediante `HasData`:

| ID | Nombre | Capacidad | Ciudad |
|---|---|---|---|
| 1 | Auditorio Central | 200 | Bogotá |
| 2 | Sala Norte | 50 | Bogotá |
| 3 | Arena Sur | 500 | Medellín |

Estos venues son la referencia para crear eventos. La capacidad del evento **no puede exceder** la del venue (RN-01).

### Fase 2 — Eventos de prueba (Program.cs)

Insertados en `Program.cs` dentro de un scope de servicios, si la tabla `Eventos` está vacía:

| # | Título | Venue | Capacidad | Precio | Tipo |
|---|---|---|---|---|---|
| 1 | Cumbre de Innovación Tecnológica 2026 | Auditorio Central | 200 | $120.00 | Conferencia |
| 2 | Taller Práctico de Clean Architecture | Sala Norte | 50 | $75.00 | Taller |
| 3 | Noche de Jazz en Vivo 2026 | Arena Sur | 500 | $95.00 | Concierto |

> Todas las fechas se calculan dinámicamente como `DateTime.UtcNow.Date + 30 días`, por lo que siempre estarán en el futuro y serán válidas al momento de ejecutar la aplicación.

### Cómo usar los datos semilla

1. **Listar eventos disponibles** — `GET /api/Eventos/ListarEventos`
2. **Consultar venues** — `GET /api/Eventos/Venues`
3. **Crear una reserva** para el evento ID `1`:
   ```json
   POST /api/Reservas/CrearReserva
   {
	 "eventoId": 1,
	 "nombreComprador": "Ana García",
	 "emailComprador": "ana@example.com",
	 "cantidad": 2
   }
   ```
4. **Confirmar el pago** — `POST /api/Reservas/ConfirmarPago/1`
5. **Cancelar** si es necesario — `POST /api/Reservas/CancelarReserva/1`

---

## 8. Reglas de negocio

| Código | Descripción |
|---|---|
| **RN-01** | La capacidad del evento no puede exceder la del venue asignado. |
| **RN-03** | Los eventos en fin de semana no pueden iniciar después de las 22:00. |
| **RN-04** | No se permiten reservas faltando menos de 1 hora para el inicio del evento. |
| **RN-05** | A menos de 24 horas del evento, máximo 5 entradas por transacción. Para eventos con precio > $100, máximo 10 entradas. |
| **RN-06** | El estado del evento se calcula: si estaba `Activo` y `DateTime.UtcNow > FechaFin`, pasa automáticamente a `Completado`. |
| **RN-07** | Las cancelaciones de reservas ya confirmadas registran penalización (`Penalizada = true`). |

### Enumeraciones de referencia

**TipoEvento**

| Valor | Nombre |
|---|---|
| `0` | Conferencia |
| `1` | Taller |
| `2` | Concierto |

**EstadoEvento**

| Valor | Nombre |
|---|---|
| `0` | Activo |
| `1` | Cancelado |
| `2` | Completado |

**EstadoReserva**

| Valor | Nombre |
|---|---|
| `0` | PendientePago |
| `1` | Confirmada |
| `2` | Cancelada |

---

## 9. Estructura del proyecto

```
EventosVivosProxy/
├── EventosVivosProxy/                   # Capa de Presentación (API)
│   ├── Controllers/
│   │   ├── EventosController.cs         # Endpoints RF-01, RF-02
│   │   └── ReservasController.cs        # Endpoints RF-03, RF-04, RF-05
│   ├── Properties/launchSettings.json   # Perfiles de ejecución
│   ├── Program.cs                       # Bootstrap, DI, seed de eventos
│   └── appsettings.json
│
├── EventosVivos.Application/            # Capa de Aplicación
│   ├── DTOs/
│   │   ├── EventoResponseDto.cs
│   │   ├── ReservaResponseDto.cs
│   │   ├── CrearReservaRequest.cs
│   │   └── VenueDto.cs
│   ├── Interfaces/Services/
│   │   ├── IEventoService.cs
│   │   └── IReservaService.cs
│   └── UseCases/Services/
│       ├── EventoService.cs
│       └── ReservaService.cs
│
├── EventosVivos.Domain/                 # Capa de Dominio (núcleo)
│   ├── Entities/
│   │   ├── Evento.cs                    # Reglas RN-01, RN-03, RN-06
│   │   ├── Reserva.cs                   # Reglas RN-04, RN-05, RN-07
│   │   └── Venue.cs
│   ├── Enums/
│   │   ├── EstadoEvento.cs
│   │   ├── EventoTipo.cs
│   │   └── ReservaEstado.cs
│   └── Interfaces/Repositories/
│       ├── IEventoRepository.cs
│       ├── IReservaRepository.cs
│       └── IVenueRepository.cs
│
└── EventosVivos.Infrastructure/         # Capa de Infraestructura
	└── Persistence/
		├── EventosVivosDbContext.cs      # Fluent API + seed de Venues
		└── Repositories/
			├── EventoRepository.cs
			├── ReservaRepository.cs
			└── VenueRepository.cs
```

---

## Notas adicionales

- **Persistencia efímera:** al ser InMemory, los datos se pierden con cada reinicio. Esto es intencional para facilitar pruebas limpias y repetibles.  
- **Certificado HTTPS en desarrollo:** si el navegador rechaza el certificado, ejecutar `dotnet dev-certs https --trust` y reiniciar el navegador.  
- **XML Documentation:** el proyecto genera el archivo XML de comentarios durante la compilación, habilitando las descripciones completas en Swagger automáticamente.
