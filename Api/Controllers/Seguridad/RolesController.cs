using Api.Controllers.Facturacion;
using Api.Dominio.Seguridad;
using Api.Errors;
using Aplicacion.Servicios;
using Aplicacion.ServiciosGlobales;
using Dominio.Facturacion;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Seguridad
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly ILogger<RolesController> _logger;
        public RolesController(ILogger<RolesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Roles>>> GetAllRoles()
        {
            try
            {
                IServicioAplicacion<Roles> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Roles>>();
                // Se ejecuta una consulta SQL asincrónica para obtener todos los rols.
                var roles = await repositorio.EjecutarConsultaSqlAsync<Roles>("select * from roles");

                if (roles.Any())
                {
                    // Si se encontraron rols, se devuelve una respuesta exitosa con la lista de rols.
                    return Ok(roles.ToList());
                }
                else
                {
                    throw new ApiException(404, "No hay roles registrados.");
                }

            }
            catch (ApiException)
            {
                // Si se trata de una ApiException, se relanza para mantener la consistencia de la respuesta.
                throw;
            }
            catch (Exception ex)
            {
                // Si ocurre un error interno no esperado, se registra en el registro de errores y se devuelve una ApiException con un código de estado 500.
                _logger.LogError(ex, "Error al obtener los roles.");
                throw new ApiException(500, "Ha ocurrido un error interno al obtener los roles. Detalle: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Roles>> GetRol(int id)
        {
            try
            {
                IServicioAplicacion<Roles> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Roles>>();
                var rol = await repositorio.EjecutarConsultaSqlAsync<Roles>($"select * from roles where id = {id}");

                if (rol.Any())
                {
                    return Ok(rol.First());
                }
                else
                {
                    throw new ApiException(404, "El rol con el ID '" + id.ToString() + "' no está registrado.");
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol " + id);
                throw new ApiException(500, "Ha ocurrido un error interno al obtener el rol. Detalle: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<List<Roles>>> CreateRol(Roles rol)
        {
            try
            {
                // Validación si esxiste el rol
                var validacion = await SelectRolId(rol.Id);

                if (validacion == null)
                {
                    IServicioAplicacion<Roles> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Roles>>();

                    // Genera el objeto para la petición
                    object param = new
                    {
                        p_nombre = rol.Nombre,
                    };

                    // Inserta el rol en la base de datos
                    var result = await repositorio.ProcedimientoSqlAsync<Roles>("SpRolesInsertar", param);

                    // Respuesta exitosa que devuelve todos los rols de la base de datos
                    return Ok(await SelectAllRoles());
                }
                else
                {
                    // En caso de que el rol ya exista
                    throw new ApiException(409, "El rol con el código '" + rol.Id + "' ya está registrado.");
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el rol.");
                throw new ApiException(500, "Ha ocurrido un error interno al crear el rol. Detalle: " + ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<List<Roles>>> UpdateRol(Roles rol)
        {
            try
            {
                var result = await SelectRolId(rol.Id);
                if (result != null)
                {
                    IServicioAplicacion<Roles> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Roles>>();

                    // Genera el objeto para la petición
                    object param = new
                    {
                        p_id = rol.Id,
                        p_nombre = rol.Nombre,
                    };

                    await repositorio.ProcedimientoSqlAsync<Roles>("SpRolesActualizar", param);
                    return Ok(await SelectAllRoles());
                }
                else
                {
                    throw new ApiException(404, $"El rol con el código '{rol.Id}' no está registrado.");
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol " + rol.Id);
                throw new ApiException(500, "Ha ocurrido un error interno al actualizar el rol. Detalle: " + ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult<List<Roles>>> DeleteRol(Roles rol)
        {
            try
            {
                var result = await SelectRolId(rol.Id);
                if (result != null)
                {
                    IServicioAplicacion<Roles> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Roles>>();
                    await repositorio.EjecutarConsultaSqlAsync<Roles>($"delete from roles where id = {rol.Id}");
                    return Ok(await SelectAllRoles());
                }
                else
                {
                    throw new ApiException(404, $"El rol con el código '{rol.Id}' no está registrado.");
                }

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el rol {rol.Id}");
                throw new ApiException(500, "Ha ocurrido un error interno al eliminar el rol. Detalle: " + ex.Message);
            }
        }

        private static async Task<IEnumerable<Roles>> SelectAllRoles()
        {
            IServicioAplicacion<Roles> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Roles>>();
            return await repositorio.EjecutarConsultaSqlAsync<Roles>("select * from roles", null);
        }

        private static async Task<Roles> SelectRolId(int id)
        {
            IServicioAplicacion<Roles> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Roles>>();
            var response = await repositorio.EjecutarConsultaSqlAsync<Roles>($"select * from roles where id = {id}");
            return response.FirstOrDefault();
        }
    }
}
