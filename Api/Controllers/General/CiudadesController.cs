using Api.Dominio.General;
using Api.Errors;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Api.Controllers.General
{
    [ApiController]
    [Route("api/[controller]")]
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

                    if (ciudades.Any())
                    {
                        foreach (var c in ciudades)
                        {
                            if(departamentos.Any())
                                c.Departamento = departamentos.Where(x => x.Id == c.DepartamentoId).FirstOrDefault();

                            if(paises.Any())
                                c.Pais = paises.Where(p => p.Id == c.PaisId).FirstOrDefault();
                        }
                        return Ok(ciudades.ToList());
                    }
                    else
                    {
                        throw new ApiException(404, "No hay ciudades registradas.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las ciudades.");
                throw new ApiException(500, "Ha ocurrido un error interno al obtener las ciudades. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una ciudad por su ID.
        /// </summary>
        /// <param name="ciudadId">ID de la ciudad.</param>
        /// <returns>La ciudad que corresponde al ID <paramref name="ciudadId"/> en la base de datos.</returns>
        [HttpGet("{ciudadId:int}")]
        public async Task<ActionResult<Ciudad>> GetCiudad(int ciudadId)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var ciudad = await connection.QueryAsync<Ciudad>("select * from tblciudades where id = @Id",
                        new { Id = ciudadId });

                    if(ciudad.Any())
                    {
                        foreach (var c in ciudad)
                        {
                            // Trae el país de la ciudad
                            var pais = await connection.QueryAsync<Pais>("select * from TblPaises where id = @Id", new { Id = c.PaisId });
                            if (pais.Any())
                                c.Pais = pais.FirstOrDefault();

                            // Trae el departamento de la ciudad
                            var departamento = await connection.QueryAsync<Departamento>("select * from tbldepartamentos", new { Id = c.DepartamentoId });
                            if(departamento.Any())
                                c.Departamento = departamento.FirstOrDefault();
                        }

                        return Ok(ciudad.FirstOrDefault());
                    }
                    else
                    {
                        throw new ApiException(404, "La ciudad con el id '" + ciudadId + "' no está registrada.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la ciudad " + ciudadId);
                throw new ApiException(500, "Ha ocurrido un error interno al obtener la ciudad. Detalle: " + ex.Message);
            }
        }
    }
}
