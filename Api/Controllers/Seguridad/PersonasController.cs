using Api.Dominio.Seguridad;
using Api.Errors;
using Aplicacion.Servicios;
using Aplicacion.ServiciosGlobales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Seguridad
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class PersonasController : Controller
    {
        private readonly ILogger<PersonasController> _logger;

        public PersonasController(ILogger<PersonasController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las personas.
        /// </summary>
        /// <returns>Una lista de las personas en la base de datos.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentran personas registradas.</exception>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<Personas>>> GetAllPersonas()
        {
            try
            {
                IServicioAplicacion<Personas> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Personas>>();
                // Se ejecuta una consulta SQL asincrónica para obtener todos los usuarios.
                var personas = await repositorio.EjecutarConsultaSqlAsync<Personas>("select * from personas");

                if (personas.Any())
                {
                    // Si se encontraron usuarios, se devuelve una respuesta exitosa con la lista de usuarios.
                    return Ok(personas.ToList());
                }
                else
                {
                    throw new ApiException(404, "No hay personas registrados.");
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
                IServicioAplicacion<Personas> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Personas>>();
                var query = "SELECT * FROM personas WHERE identificacion = @identificacion";
                var persona = await repositorio.EjecutarConsultaSqlAsync<Usuarios>(query, new { identificacion = identificacion });

                if (persona.Any())
                {
                    return Ok(persona.First());
                }
                else
                {
                    throw new ApiException(404, "La persona con el número de identificación '" + identificacion.ToString() + "' no está registrada.");
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
                // Validación si esxiste el usuario
                var validacion = await SelectPersonaId(persona.Id.ToString());

                if (validacion == null)
                {
                    IServicioAplicacion<Personas> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Personas>>();

                    // Genera el objeto para la petición
                    object param = new
                    {
                        p_tipoIdentificacion = persona.TipoIdentificacion,
                        p_identificacion = persona.Identificacion,
                        p_nombre = persona.Nombre,
                        p_apellido = persona.Apellido,
                        p_activo = persona.Activo,
                        p_tipoPersona = persona.TipoPersona,
                        p_direccion = persona.Direccion,
                        p_correo = persona.Correo,
                    };

                    // Inserta el usuario en la base de datos
                    var result = await repositorio.ProcedimientoSqlAsync<Usuarios>("SpPersonasInsertar", param);

                    // Respuesta exitosa que devuelve todos los usuarios de la base de datos
                    return Ok(await SelectAllPersonas());
                }
                else
                {
                    // En caso de que el usuario ya exista
                    throw new ApiException(409, "La persona con el número de identificación '" + persona.Identificacion + "' ya está registrada.");
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
                var result = await SelectPersonaId(persona.Identificacion);
                if (result != null)
                {
                    IServicioAplicacion<Personas> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Personas>>();

                    // Genera el objeto para la petición
                    object param = new
                    {
                        p_id = persona.Id,
                        p_tipoIdentificacion = persona.TipoIdentificacion,
                        p_identificacion = persona.Identificacion,
                        p_nombre = persona.Nombre,
                        p_apellido = persona.Apellido,
                        p_activo = persona.Activo,
                        p_tipoPersona = persona.TipoPersona,
                        p_direccion = persona.Direccion,
                        p_correo = persona.Correo,
                    };

                    await repositorio.ProcedimientoSqlAsync<Personas>("SpPersonasActualizar", param);
                    return Ok(await SelectAllPersonas());
                }
                else
                {
                    throw new ApiException(404, $"La persona con el número de identificación '{persona.Identificacion}' no está registrada.");
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
                var result = await SelectPersonaId(persona.Identificacion);
                if (result != null)
                {
                    IServicioAplicacion<Usuarios> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Usuarios>>();
                    await repositorio.EjecutarConsultaSqlAsync<Usuarios>($"delete from personas where id = {persona.Id}");
                    return Ok(await SelectAllPersonas());
                }
                else
                {
                    throw new ApiException(404, $"La persona con el número de identificación '{persona.Identificacion}' no está registrada.");
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

        private static async Task<IEnumerable<Personas>> SelectAllPersonas()
        {
            IServicioAplicacion<Personas> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Personas>>();
            return await repositorio.EjecutarConsultaSqlAsync<Personas>("select * from personas", null);
        }

        private static async Task<Personas> SelectPersonaId(string id)
        {
            IServicioAplicacion<Personas> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Personas>>();
            var response = await repositorio.EjecutarConsultaSqlAsync<Personas>($"select * from personas where identificacion = {id}");
            return response.FirstOrDefault();
        }
    }
}
