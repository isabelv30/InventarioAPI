using Api.Controllers.Inventario;
using Api.Dominio.General;
using Api.Errors;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Api.Controllers.General
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaisesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaisesController> _logger;

        public PaisesController(ILogger<PaisesController> logger,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los pais.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Pais>>> GetAllPaises()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var pais = await connection.QueryAsync<Pais>("select * from tblpaises");
                    if (pais.Any())
                    {
                        return Ok(pais.ToList());
                    }
                    else
                    {
                        throw new ApiException(404, "No hay personas registradas.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los países.");
                throw new ApiException(500, "Ha ocurrido un error interno al obtener los países. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene un pais por su ID.
        /// </summary>
        /// <param name="paisId">ID del pais.</param>
        [HttpGet("{paisId:int}")]
        public async Task<ActionResult<Pais>> GetPais(int paisId)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var pais = await connection.QueryAsync<Pais>("select * from tblpaises where id = @Id",
                        new { Id = paisId });
                    if (pais.Any())
                    {
                        return Ok(pais.First());
                    }
                    else
                    {
                        throw new ApiException(404, "El país con el ID '" + paisId + "' no está registrado.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el pais " + paisId);
                throw new ApiException(500, "Ha ocurrido un error interno al obtener el país. Detalle: " + ex.Message);
            }
        }
    }
}
