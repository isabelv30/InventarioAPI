using Api.Errors;
using Api.Models.General;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Api.Controllers.General
{
    [ApiController]
    [Route("api/[controller]")]
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
        /// Obtiene todas las unidades de medida.
        /// </summary>
        /// <returns>Una lista de las unidades de medida en la base de datos.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentran unidades de medida registradas.</exception>
        [HttpGet]
        public async Task<ActionResult<List<UnidadesMedida>>> GetAllUnidadesMedida()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var unidadesMedida = await connection.QueryAsync<UnidadesMedida>("select * from tblunidadesmedida");
                    if (unidadesMedida.Any())
                    {
                        return Ok(unidadesMedida.ToList());
                    }
                    else
                    {
                        throw new ApiException(404, "No hay unidades de medida registradas.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener unidades de medida.");
                throw new ApiException(500, "Ha ocurrido un error interno al obtener las unidades de medida. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una unidad de medida por su ID.
        /// </summary>
        /// <param name="unidadMedidaId">ID de la unidad de medida.</param>
        /// <returns>Se lanza si no se encuentra la unidad de medida correspondiente al ID <paramref name="unidadMedidaId"/> de la unidad de medida.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentra la unidad de medida correspondiente al ID <paramref name="unidadMedidaId"/> de la unidad de medida.</exception>
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
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la unidad de medida " + unidadMedidaId);
                return StatusCode(500, "Error al obtener unidades de medida.");
            }
        }
    }
}
