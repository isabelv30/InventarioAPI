using Api.Dominio.General;
using Api.Errors;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Api.Controllers.General
{
    [ApiController]
    [Route("api/[controller]")]
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
                    var pais = await connection.QueryAsync<Pais>("select * from tblpaises");
                    var departamentos = await connection.QueryAsync<Departamento>("select * from tbldepartamentos");
                    
                    if(departamentos.Any())
                    {
                        foreach (var d in departamentos)
                        {
                            if(pais.Any())
                                d.Pais = pais.Where(p => p.Id == d.PaisId).FirstOrDefault();
                        }
                        return Ok(departamentos.ToList());
                    }
                    else
                    {
                        throw new ApiException(404, "No hay departamentos registrados.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener departamentos.");
                throw new ApiException(500, "Ha ocurrido un error interno al obtener los departamentos. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene un departamento por su ID.
        /// </summary>
        /// <param name="departamentoId">ID del departamento.</param>
        /// <returns>El departamento correspondiente al ID <paramref name="departamentoId"/> en la base de datos.</returns>
        [HttpGet("{departamentoId:int}")]
        public async Task<ActionResult<Departamento>> GetDepartamento(int departamentoId)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var departamento = await connection.QueryAsync<Departamento>("select * from tbldepartamentos where id = @Id",
                        new { Id = departamentoId });

                    if (departamento.Any())
                    {
                        foreach (var d in departamento)
                        {
                            var pais = await connection.QueryAsync<Pais>("select * from tblpaises where id = @id", new {id = d.PaisId});
                            if (pais.Any())
                                d.Pais = pais.FirstOrDefault();
                        }
                        return Ok(departamento.FirstOrDefault());
                    }
                    else
                    {
                        throw new ApiException(404, "El departamento con el id '" + departamentoId + "' no está registrado.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el departamento " + departamentoId);
                throw new ApiException(500, "Ha ocurrido un error interno al obtener el departamento. Detalle: " + ex.Message);
            }
        }
    }
}
