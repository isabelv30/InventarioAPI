using Api.Dominio.General;
using Api.Errors;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Api.Controllers.General
{
    [ApiController]
    [Route("api/[controller]")]
    public class TiposMovimientoController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TiposMovimientoController> _logger;

        public TiposMovimientoController(ILogger<TiposMovimientoController> logger,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los Tipos de Movimiento.
        /// </summary>
        /// <returns>Los tipos de movimiento de la base de datos.</returns>
        [HttpGet]
        public async Task<ActionResult<List<TipoMovimiento>>> GetAlltiposMovimiento()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var tiposMovimiento = await connection.QueryAsync<TipoMovimiento>("select * from TblTiposMovimiento");

                    if (tiposMovimiento.Any())
                    {
                        return Ok(tiposMovimiento.ToList());
                    }
                    else
                    {
                        throw new ApiException(404, "No hay tipos de movimiento registrados.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los tipos de movimiento.");
                throw new ApiException(500, "Ha ocurrido un error interno al obtener los tipos de movimiento. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene un tipoMovimiento por su ID.
        /// </summary>
        /// <param name="codigo">ID del tipoMovimiento.</param>
        [HttpGet("{codigo}")]
        public async Task<ActionResult<TipoMovimiento>> GetTipoMovimiento(string codigo)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var tipoMovimiento = await connection.QueryAsync<TipoMovimiento>("select * from TblTiposMovimiento where codigo = @codigo",
                        new { codigo = codigo });

                    if (tipoMovimiento.Any())
                    {
                        return Ok(tipoMovimiento.First());
                    }
                    else
                    {
                        throw new ApiException(404, "El tipo de movimiento con el código '" + codigo + "' no está registrado.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el tipo de movimiento " + codigo);
                throw new ApiException(500, "Ha ocurrido un error interno al obtener el tipo de movimiento. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Crea un nuevo tipoMovimiento.
        /// </summary>
        /// <param name="tipoMovimiento">Información del tipoMovimiento a crear.</param>
        /// <returns>Todos los tipos de movimiento.</returns>
        [HttpPost]
        public async Task<ActionResult<List<TipoMovimiento>>> CreateTipoMovimiento(TipoMovimiento tipoMovimiento)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var result = await SelectTiposMovimientoId(connection, tipoMovimiento.Codigo);
                    if (!result.Any())
                    {
                        await connection.ExecuteAsync("insert into TblTiposMovimiento ([Codigo], [Nombre], [Descripcion]) values (@Codigo, @Nombre, @Descripcion)", tipoMovimiento);
                        return Ok(await SelectAllTiposMovimiento(connection));
                    }
                    else
                    {
                        throw new ApiException(409, "El tipo de movimiento con el código '" + tipoMovimiento.Codigo + "' ya está registrado.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el tipo de movimiento.");
                throw new ApiException(500, "Ha ocurrido un error interno al crear el tipo de movimiento. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Actualiza un tipo de movimiento.
        /// </summary>
        /// <param name="tipoMovimiento">Información del tipoMovimiento a actualizar.</param>
        /// <returns>Respuesta del servidor a cerca de la petición.</returns>
        [HttpPut]
        public async Task<ActionResult<List<TipoMovimiento>>> UpdateTipoMovimiento(TipoMovimiento tipoMovimiento)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var result = await SelectTiposMovimientoId(connection, tipoMovimiento.Codigo);
                    if (result.Count() > 0)
                    {
                        await connection.ExecuteAsync("update TblTiposMovimiento set nombre = @nombre, descripcion = @descripcion where codigo = @codigo", tipoMovimiento);
                        return Ok(await SelectAllTiposMovimiento(connection));
                    }
                    else
                    {
                        throw new ApiException(404, "El tipo de movimiento con el código '" + tipoMovimiento.Codigo + "' no está registrado.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el tipo de movimiento " + tipoMovimiento.Id);
                throw new ApiException(500, "Ha ocurrido un error interno al actualizar el tipo de movimiento. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Elimina un tipo de movimiento.
        /// </summary>
        /// <param name="tipoMovimiento">Información del tipo de movimiento a eliminar.</param>
        /// <returns>Respuesta del servidor a cerca de la petición y el resultado del método SelectAllTiposMovimiento.</returns>
        [HttpDelete]
        public async Task<ActionResult<List<TipoMovimiento>>> DeleteTipoMovimiento(TipoMovimiento tipoMovimiento)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var result = await SelectTiposMovimientoId(connection, tipoMovimiento.Codigo);
                    if (result.Count() > 0)
                    {
                        await connection.ExecuteAsync("delete from TblTiposMovimiento where codigo = @codigo", tipoMovimiento);
                        return Ok(await SelectAllTiposMovimiento(connection));
                    }
                    else
                    {
                        throw new ApiException(404, "El tipo de movimiento con el código '" + tipoMovimiento.Codigo + "' no está registrado.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el tipo de movimiento " + tipoMovimiento.Id);
                throw new ApiException(500, "Ha ocurrido un error interno al eliminar el tipo de movimiento. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Selecciona todos los tipos de movimiento.
        /// </summary>
        /// <param name="connection">Cadena de conexión a la base de datos.</param>
        /// <returns>Todos los tipos de movimiento de la tabla tbltiposMovimiento.</returns>
        private static async Task<IEnumerable<TipoMovimiento>> SelectAllTiposMovimiento(SqlConnection connection)
        {
            return await connection.QueryAsync<TipoMovimiento>("select * from TblTiposMovimiento");
        }

        /// <summary>
        /// Consulta un tipo de movimiento por su identificador.
        /// </summary>
        /// <param name="connection">Cadena de conexión a la base de datos.</param>
        /// <param name="codigo">Identificador del tipo de movimiento.</param>
        /// <returns>Tipo de movimiento o 0 en caso de no encontrarlo.</returns>
        private static async Task<IEnumerable<TipoMovimiento>> SelectTiposMovimientoId(SqlConnection connection, string codigo)
        {
            return await connection.QueryAsync<TipoMovimiento>("select * from tbltiposMovimiento where codigo = @codigo",
                new {codigo = codigo});
        }
    }
}
