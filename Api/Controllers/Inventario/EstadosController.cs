using Api.Errors;
using Api.Models.Inventario;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Api.Controllers.Inventario
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstadosController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EstadosController> _logger;

        public EstadosController(ILogger<EstadosController> logger,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los estados.
        /// </summary>
        /// <returns>Una lista de los estados en la base de datos.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentran estados registrados.</exception>
        [HttpGet]
        public async Task<ActionResult<List<Estados>>> GetAllEstados()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var estados = await connection.QueryAsync<Estados>("select * from tblestados");
                    if (estados.Any())
                    {
                        return Ok(estados.ToList());
                    }
                    else
                    {
                        throw new ApiException(404, "No hay estados registrados.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estados.");
                throw new ApiException(500, "Ha ocurrido un error interno al obtener los estados. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene un estado por su ID.
        /// </summary>
        /// <param name="estadoId">ID del estado.</param>
        /// <returns>El estado específico que se consulta por el ID.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentra el estado correspondiente al ID <paramref name="estadoId"/> del estado.</exception>
        [HttpGet("{estadoId}")]
        public async Task<ActionResult<Estados>> GetEstado(string estadoId)
        {
            if (!int.TryParse(estadoId, out int estadoIdNumeric))
            {
                throw new ApiException(400, "El ID del estado debe ser un número entero válido.");
            }

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var estado = await connection.QueryAsync<Estados>("select * from tblestados where id = @Id",
                        new { Id = estadoId });

                    if (estado.Any())
                    {
                        return Ok(estado.First());
                    }
                    else
                    {
                        throw new ApiException(404, "El estado con el ID '" + estadoId + "' no está registrado.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el estado " + estadoId);
                throw new ApiException(500, "Ha ocurrido un error interno al obtener el estado. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Crea un nuevo estado.
        /// </summary>
        /// <param name="estado">Información del estado a crear.</param>
        /// <returns>Los estados de la base de datos. Consulte <see cref="SelectAllEstados"/> para obtener más detalles.</returns>
        /// <exception cref="ApiException"></exception>
        [HttpPost]
        public async Task<ActionResult<List<Estados>>> CreateEstado(Estados estado)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var result = await SelectEstadoId(connection, estado.Id);
                    if (!result.Any())
                    {
                        await connection.ExecuteAsync("insert into tblestados (nombre, descripcion) values (@nombre, @descripcion)", estado);
                        return Ok(await SelectAllEstados(connection));
                    }
                    else
                    {
                        throw new ApiException(409, "El estado con el ID '" + estado.Id + "' ya está registrado.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el estado.");
                throw new ApiException(500, "Ha ocurrido un error interno al crear el estado. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Actualiza un estado.
        /// </summary>
        /// <param name="estado">Información del estado a actualizar.</param>
        /// <returns>Los estados de la base de datos. Consulte <see cref="SelectAllEstados"/> para obtener más detalles.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentra el estado correspondiente al ID del estado <paramref name="estado"/>.</exception>
        [HttpPut]
        public async Task<ActionResult<List<Estados>>> UpdateEstado(Estados estado)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var result = await SelectEstadoId(connection, estado.Id);
                    if (result.Any())
                    {
                        await connection.ExecuteAsync("update tblestados set nombre = @nombre, descripcion = @descripcion where id = @id", estado);
                        return Ok(await SelectAllEstados(connection));
                    }
                    else
                    {
                        throw new ApiException(404, "El estado con el ID '" + estado.Id + "' no está registrado.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el estado " + estado.Id);
                throw new ApiException(500, "Ha ocurrido un error interno al actualizar el estado. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Elimina un estado.
        /// </summary>
        /// <param name="estado">Información del estado a eliminar.</param>
        /// <returns>Los estados de la base de datos. Consulte <see cref="SelectAllEstados"/> para obtener más detalles.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentra el estado correspondiente al ID del estado <paramref name="estado"/>.</exception>
        [HttpDelete]
        public async Task<ActionResult<List<Estados>>> DeleteEstado(Estados estado)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var result = await SelectEstadoId(connection, estado.Id);
                    if (result.Any())
                    {
                        await connection.ExecuteAsync("delete from tblestados where id = @id", estado);
                        return Ok(await SelectAllEstados(connection));
                    }
                    else
                    {
                        throw new ApiException(404, "El estado con el ID '" + estado.Id + "' no está registrado.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el estado " + estado.Id);
                throw new ApiException(500, "Ha ocurrido un error interno al eliminar el estado. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Selecciona todos los estados.
        /// </summary>
        /// <param name="connection">Cadena de conexión a la base de datos.</param>
        /// <returns>Todos los estados de la tabla tblestados.</returns>
        private static async Task<IEnumerable<Estados>> SelectAllEstados(SqlConnection connection)
        {
            return await connection.QueryAsync<Estados>("select * from tblestados");
        }

        /// <summary>
        /// Consulta un estado por su identificador.
        /// </summary>
        /// <param name="connection">Cadena de conexión a la base de datos.</param>
        /// <param name="estadoId">Identificador del estado.</param>
        /// <returns>El estado que se está consultando.</returns>
        private static async Task<IEnumerable<Estados>> SelectEstadoId(SqlConnection connection, int estadoId)
        {
            return await connection.QueryAsync<Estados>("select * from tblestados where id = @id", new {id = estadoId});
        }
    }
}
