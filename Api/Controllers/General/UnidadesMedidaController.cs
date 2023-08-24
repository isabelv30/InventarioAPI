using Api.Models.General;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Api.Controllers.General
{
    [ApiController]
    [Route("[controller]")]
    public class UnidadesMedidaController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UnidadesMedidaController> _logger;

        public UnidadesMedidaController(ILogger<UnidadesMedidaController> logger,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los UnidadesMedida.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<UnidadesMedida>>> GetAllUnidadesMedida()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var unidadesMedida = await connection.QueryAsync<UnidadesMedida>("select * from TblUnidadesMedida");
                    return Ok(unidadesMedida.ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener unidades de medida.");
                return StatusCode(500, "Error al obtener unidades de medida.");
            }
        }

        /// <summary>
        /// Obtiene un UnidadesMedida por su ID.
        /// </summary>
        /// <param name="unidadMedidaId">ID del UnidadesMedida.</param>
        [HttpGet("{unidadMedidaId:int}")]
        public async Task<ActionResult<UnidadesMedida>> GetUnidadesMedida(int unidadMedidaId)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var unidadMedida = await connection.QueryAsync<UnidadesMedida>("select * from TblUnidadesMedida where id = @Id",
                        new { Id = unidadMedidaId });
                    return Ok(unidadMedida.First());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la unidad de medida " + unidadMedidaId);
                return StatusCode(500, "Error al obtener unidades de medida.");
            }
        }
    }
}
