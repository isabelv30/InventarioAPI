using Api.Models.General;
using Api.Models.Inventario;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Api.Controllers.General
{
    [ApiController]
    [Route("[controller]")]
    public class DepartamentosController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DepartamentosController> _logger;

        public DepartamentosController(ILogger<DepartamentosController> logger,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los departamentos.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Departamento>>> GetAllDepartamentos()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var pais = await connection.QueryAsync<Pais>("select * from TblPaises");
                    var departamentos = await connection.QueryAsync<Departamento>("select * from tbldepartamentos");
                    foreach (var d in departamentos)
                    {
                        d.Pais = pais.Where(p => p.Id == d.PaisId).FirstOrDefault();
                    }
                    return Ok(departamentos.ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener departamentos.");
                return StatusCode(500, "Error al obtener departamentos.");
            }
        }

        /// <summary>
        /// Obtiene un departamento por su ID.
        /// </summary>
        /// <param name="departamentoId">ID del departamento.</param>
        [HttpGet("{departamentoId:int}")]
        public async Task<ActionResult<Departamento>> Getdepartamento(int departamentoId)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var pais = await connection.QueryAsync<Pais>("select * from TblPaises");
                    var departamento = await connection.QueryAsync<Departamento>("select * from tbldepartamentos where id = @Id",
                        new { Id = departamentoId });

                    foreach (var d in departamento)
                    {
                        d.Pais = pais.Where(p => p.Id == d.PaisId).FirstOrDefault();
                    }

                    return Ok(departamento.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el departamento " + departamentoId);
                return StatusCode(500, "Error al obtener el departamento.");
            }
        }
    }
}
