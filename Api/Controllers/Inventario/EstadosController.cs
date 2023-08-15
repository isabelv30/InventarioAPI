using Api.Models.Inventario;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Api.Controllers.Inventario
{
    [ApiController]
    [Route("[controller]")]
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
        [HttpGet(Name = "GetEstados")]
        public async Task<ActionResult<List<Estados>>> GetAllEstados()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var estados = await connection.QueryAsync<Estados>("select * from tblestados");
                    return Ok(estados.ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estados.");
                return StatusCode(500, "Error al obtener estados.");
            }
        }

        /// <summary>
        /// Obtiene un estado por su ID.
        /// </summary>
        /// <param name="estadoId">ID del estado.</param>
        [HttpGet("{estadoId:int}")]
        public async Task<ActionResult<Estados>> GetEstado(int estadoId)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var estado = await connection.QueryAsync<Estados>("select * from tblestados where id = @Id",
                        new { Id = estadoId });
                    return Ok(estado.First());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el estado " + estadoId);
                return StatusCode(500, "Error al obtener el estado.");
            }
        }

        // <summary>
        /// Crea un nuevo estado.
        /// </summary>
        /// <param name="estado">Información del estado a crear.</param>
        [HttpPost]
        public async Task<ActionResult<List<Estados>>> CreateEstado(Estados estado)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.ExecuteAsync("insert into tblestados (nombre, descripcion) values (@nombre, @descripcion)", estado);
                    return Ok(await SelectAllEstados(connection));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el estado.");
                return StatusCode(500, "Error al crear el estado.");
            }
        }

        // <summary>
        /// Actualiza un estado.
        /// </summary>
        /// <param name="estado">Información del estado a actualizar.</param>
        [HttpPut]
        public async Task<ActionResult<List<Estados>>> UpdateEstado(Estados estado)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.ExecuteAsync("update tblestados set nombre = @nombre, descripcion = @descripcion where id = @id", estado);
                    return Ok(await SelectAllEstados(connection));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el estado " + estado.Id);
                return StatusCode(500, "Error al actualizar el estado.");
            }
        }

        // <summary>
        /// Elimina un estado.
        /// </summary>
        /// <param name="estado">Información del estado a eliminar.</param>
        [HttpDelete]
        public async Task<ActionResult<List<Estados>>> DeleteEstado(Estados estado)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.ExecuteAsync("delete from tblestados where id = @id", estado);
                    return Ok(await SelectAllEstados(connection));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el estado " + estado.Id);
                return StatusCode(500, "Error al eliminar el estado.");
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
    }
}
