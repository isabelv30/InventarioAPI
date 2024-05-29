using Api.Dominio.Seguridad;
using Api.Errors;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Api.Controllers.Seguridad
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonasController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PersonasController> _logger;

        public PersonasController(ILogger<PersonasController> logger,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las personas.
        /// </summary>
        /// <returns>Una lista de las personas en la base de datos.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentran personas registradas.</exception>
        [HttpGet]
        public async Task<ActionResult<List<Personas>>> GetAllPersonas()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    //var personas = await connection.QueryAsync<Personas>("Sp_Personas_Select", null, commandType: CommandType.StoredProcedure);

                    var personas = await connection.QueryAsync<Personas>("select * from personas");

                    if (personas.Any())
                    {
                        return Ok(personas.ToList());
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
                _logger.LogError(ex, "Error al obtener personas.");
                throw new ApiException(500, "Ha ocurrido un error interno al obtener las personas. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una persona por su identificador.
        /// </summary>
        /// <param name="identificacion">Obtiene una persona por su identificación.</param>
        /// <returns>la persona que se consulta por su identificación.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentra la persona correspondiente la identificación <paramref name="identificacion"/> de la persona.</exception>
        [HttpGet("{identificacion}")]
        public async Task<ActionResult<Personas>> GetPersona(string identificacion)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var persona = await connection.QueryAsync<Personas>("Sp_Personas_Select_Id",
                        new { identificacion }, 
                        commandType: CommandType.StoredProcedure);

                    if (persona.Any())
                    {
                        return Ok(persona.First());
                    }
                    else
                    {
                        throw new ApiException(404, "La persona con la identificación '" + identificacion + "' no está registrada.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la persona " + identificacion);
                throw new ApiException(500, "Ha ocurrido un error interno al obtener la persona. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Crea una nueva persona.
        /// </summary>
        /// <param name="persona">Información de la persona a crear.</param>
        /// <returns>Las personas en la base de datos. Consulte <see cref="SelectAllPersonas"/> para obtener más detalles.</returns>
        /// <exception cref="ApiException">Se lanza si ya existe la persona correspondiente a la identificación de la <paramref name="persona"/>.</exception>
        [HttpPost]
        public async Task<ActionResult<List<Personas>>> CreatePersona(Personas persona)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var result = await SelectPersonaId(connection, persona.Identificacion);
                    if (!result.Any())
                    {
                        await connection.ExecuteAsync("Sp_Personas_Insert", persona, commandType: CommandType.StoredProcedure);
                        return Ok(await SelectAllPersonas(connection));
                    }
                    else
                    {
                        throw new ApiException(409, "La persona con el número de identificación '" + persona.Identificacion + "' ya está registrada.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la persona.");
                throw new ApiException(500, "Ha ocurrido un error interno al crear la persona. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Actualiza una persona.
        /// </summary>
        /// <param name="persona">Información de la personas a actualizar.</param>
        /// <returns>Las personas en la base de datos. Consulte <see cref="SelectAllPersonas"/> para obtener más detalles.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentra la persona correspondiente la identificación de la persona <paramref name="persona"/>.</exception>
        [HttpPut]
        public async Task<ActionResult<List<Personas>>> UpdatePersona(Personas persona)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var result = await SelectPersonaId(connection, persona.Identificacion);
                    if (result.Any())
                    {
                        await connection.ExecuteAsync("Sp_Personas_Update", persona, commandType: CommandType.StoredProcedure);
                        return Ok(await SelectAllPersonas(connection));
                    }
                    else
                    {
                        throw new ApiException(404, "La persona con la identificación '" + persona.Identificacion + "' no está registrada.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la persona " + persona.Identificacion);
                throw new ApiException(500, "Ha ocurrido un error interno al actualizar la persona. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Elimina una persona.
        /// </summary>
        /// <param name="persona">La persona a eliminar.</param>
        /// <returns>Las personas de la base de ddatos. Para más detalles ver </returns>
        /// <exception cref="ApiException"></exception>
        [HttpDelete]
        public async Task<ActionResult<List<Personas>>> DeletePersona(Personas persona)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var result = await SelectPersonaId(connection, persona.Identificacion);
                    if (result.Any())
                    {
                        await connection.ExecuteAsync("Sp_Personas_Delete", 
                            new { identificacion = persona.Identificacion }, 
                            commandType: CommandType.StoredProcedure);

                        return Ok(await SelectAllPersonas(connection));
                    }
                    else
                    {
                        throw new ApiException(404, "La persona con la identificación '" + persona.Identificacion + "' no está registrada.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la persona " + persona.Identificacion);
                throw new ApiException(500, "Ha ocurrido un error interno al eliminar la persona. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Selecciona todas las personas.
        /// </summary>
        /// <param name="connection">Cadena de conexión a la base de datos.</param>
        /// <returns>Todas las personas de la tabla tblpersonas.</returns>
        private static async Task<IEnumerable<Personas>> SelectAllPersonas(SqlConnection connection)
        {
            return await connection.QueryAsync<Personas>("select * from tblpersonas");
        }

        /// <summary>
        /// Consulta una persona por su identificador.
        /// </summary>
        /// <param name="connection">Cadena de conexión a la base de datos.</param>
        /// <param name="id">Identificador de la personas.</param>
        /// <returns>La persona que corresponde a la <paramref name="identificacion"/> en la base de datos.</returns>
        private static async Task<IEnumerable<Personas>> SelectPersonaId(SqlConnection connection, string identificacion)
        {
            return await connection.QueryAsync<Personas>("select * from tblpersonas where identificacion = @identificacion", new { identificacion });
        }
    }
}
