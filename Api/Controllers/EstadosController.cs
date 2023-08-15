using Api.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Api.Controllers
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
        public async Task<ActionResult<List<Estado>>> GetAllEstados()
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var estados = await connection.QueryAsync<Estado>("select * from tblestados");
                return Ok(estados.ToList());
            }
        }

        /// <summary>
        /// Obtiene un estado por su ID.
        /// </summary>
        /// <param name="estadoId">ID del estado.</param>
        [HttpGet("{estadoId:int}")]
        public async Task<ActionResult<Estado>> GetEstado(int estadoId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var estado = await connection.QueryAsync<Estado>("select * from tblestados where id = @Id",
                    new { Id = estadoId });
                return Ok(estado.First());
            }
        }

        // <summary>
        /// Crea un nuevo estado.
        /// </summary>
        /// <param name="estado">Información del estado a crear.</param>
        [HttpPost]
        public async Task<ActionResult<List<Estado>>> CreateEstado(Estado estado)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.ExecuteAsync("insert into tblestados (nombre, descripcion) values (@nombre, @descripcion)", estado);
                return Ok(await SelectAllEstados(connection));
            }
        }

        // <summary>
        /// Actualiza un estado.
        /// </summary>
        /// <param name="estado">Información del estado a actualizar.</param>
        [HttpPut]
        public async Task<ActionResult<List<Estado>>> UpdateEstado(Estado estado)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.ExecuteAsync("update tblestados set nombre = @nombre, descripcion = @descripcion where id = @id", estado);
                return Ok(await SelectAllEstados(connection));
            }
        }

        // <summary>
        /// Elimina un estado.
        /// </summary>
        /// <param name="estado">Información del estado a eliminar.</param>
        [HttpDelete]
        public async Task<ActionResult<List<Estado>>> DeleteEstado(Estado estado)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.ExecuteAsync("delete from tblestados where id = @id", estado);
                return Ok(await SelectAllEstados(connection));
            }
        }

        /// <summary>
        /// Selecciona todos los estados.
        /// </summary>
        /// <param name="connection">Cadena de conexión a la base de datos.</param>
        /// <returns>Todos los estados de la tabla tblestados.</returns>
        private static async Task<IEnumerable<Estado>> SelectAllEstados(SqlConnection connection)
        {
            return await connection.QueryAsync<Estado>("select * from tblestados");
        }
    }
}
