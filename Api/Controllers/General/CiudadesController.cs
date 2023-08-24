using Api.Models.General;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Api.Controllers.General
{
    [ApiController]
    [Route("[controller]")]
    public class CiudadesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CiudadesController> _logger;

        public CiudadesController(ILogger<CiudadesController> logger,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las ciudades.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Ciudad>>> GetAllCiudades()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var paises = await connection.QueryAsync<Pais>("select * from TblPaises");
                    var departamentos = await connection.QueryAsync<Departamento>("select * from tbldepartamentos");
                    var ciudades = await connection.QueryAsync<Ciudad>("select * from tblciudades");
                    foreach (var c in ciudades)
                    {
                        c.Departamento = departamentos.Where(x => x.Id == c.DepartamentoId).FirstOrDefault();
                        c.Pais = paises.Where(p => p.Id == c.PaisId).FirstOrDefault();
                    }
                    return Ok(ciudades.ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las ciudades.");
                return StatusCode(500, "Error al obtener las ciudades.");
            }
        }

        /// <summary>
        /// Obtiene una ciudad por su ID.
        /// </summary>
        /// <param name="ciudadId">ID de la ciudad.</param>
        [HttpGet("{ciudadId:int}")]
        public async Task<ActionResult<Ciudad>> GetCiudad(int ciudadId)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var ciudad = await connection.QueryAsync<Ciudad>("select * from tblciudades where id = @Id",
                        new { Id = ciudadId });

                    foreach (var c in ciudad)
                    {
                        // Tare el pais de la ciudad
                        var paises = await connection.QueryAsync<Pais>("select * from TblPaises where id = @Id",
                            new { Id = c.PaisId });
                        c.Pais = paises.FirstOrDefault();

                        // Trae el departamento de la ciudad
                        var departamentos = await connection.QueryAsync<Departamento>("select * from tbldepartamentos", 
                            new { Id = c.DepartamentoId });
                        c.Departamento = departamentos.FirstOrDefault();
                    }

                    return Ok(ciudad.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la ciudad " + ciudadId);
                return StatusCode(500, "Error al obtener la ciudad.");
            }
        }
    }
}
