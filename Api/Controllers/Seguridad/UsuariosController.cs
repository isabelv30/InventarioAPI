using Api.Dominio.Seguridad;
using Api.Errors;
using Aplicacion.Servicios;
using Aplicacion.ServiciosGlobales;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Seguridad
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ILogger<UsuariosController> _logger;
        public UsuariosController(ILogger<UsuariosController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Usuarios>>> GetAllUsuarios()
        {
            try
            {
                IServicioAplicacion<Usuarios> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Usuarios>>();
                // Se ejecuta una consulta SQL asincrónica para obtener todos los usuarios.
                var usuarios = await repositorio.EjecutarConsultaSqlAsync<Usuarios>("select * from usuarios");

                if (usuarios.Any())
                {
                    // Si se encontraron usuarios, se devuelve una respuesta exitosa con la lista de usuarios.
                    return Ok(usuarios.ToList());
                }
                else
                {
                    throw new ApiException(404, "No hay usuarios registrados.");
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
                _logger.LogError(ex, "Error al obtener los usuarios.");
                throw new ApiException(500, "Ha ocurrido un error interno al obtener los usuarios. Detalle: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuarios>> GetUsuario(int id)
        {
            try
            {
                IServicioAplicacion<Usuarios> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Usuarios>>();
                var usuario = await repositorio.EjecutarConsultaSqlAsync<Usuarios>($"select * from usuarios where id = {id}");

                if (usuario.Any())
                {
                    return Ok(usuario.First());
                }
                else
                {
                    throw new ApiException(404, "El usuario con el ID '" + id.ToString() + "' no está registrado.");
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario " + id);
                throw new ApiException(500, "Ha ocurrido un error interno al obtener el usuario. Detalle: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<List<Usuarios>>> CreateUsuario(Usuarios Usuario)
        {
            try
            {
                // Validación si esxiste el usuario
                var validacion = await SelectUsuarioId(Usuario.Id);

                if (validacion == null)
                {
                    IServicioAplicacion<Usuarios> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Usuarios>>();

                    // Genera el objeto para la petición
                    object param = new
                    {
                        p_username = Usuario.Username,
                        p_password = Usuario.Password,
                        p_rolId = Usuario.RolId,
                        p_personaId = Usuario.PersonaId,
                    };

                    // Inserta el usuario en la base de datos
                    var result = await repositorio.ProcedimientoSqlAsync<Usuarios>("SpUsuariosInsertar", param);

                    // Respuesta exitosa que devuelve todos los usuarios de la base de datos
                    return Ok(await SelectAllUsuarios());
                }
                else
                {
                    // En caso de que el usuario ya exista
                    throw new ApiException(409, "El usuario con el código '" + Usuario.Id + "' ya está registrado.");
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario.");
                throw new ApiException(500, "Ha ocurrido un error interno al crear el usuario. Detalle: " + ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<List<Usuarios>>> UpdateUsuario(Usuarios Usuario)
        {
            try
            {
                var result = await SelectUsuarioId(Usuario.Id);
                if (result != null)
                {
                    IServicioAplicacion<Usuarios> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Usuarios>>();

                    // Genera el objeto para la petición
                    object param = new
                    {
                        p_id = Usuario.Id,
                        p_username = Usuario.Username,
                        p_password = Usuario.Password,
                        p_rolId = Usuario.RolId,
                        p_personaId = Usuario.PersonaId,
                    };

                    await repositorio.ProcedimientoSqlAsync<Usuarios>("SpUsuariosActualizar", param);
                    return Ok(await SelectAllUsuarios());
                }
                else
                {
                    throw new ApiException(404, $"El usuario con el código '{Usuario.Id}' no está registrado.");
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario " + Usuario.Id);
                throw new ApiException(500, "Ha ocurrido un error interno al actualizar el usuario. Detalle: " + ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult<List<Usuarios>>> DeleteUsuario(Usuarios Usuario)
        {
            try
            {
                var result = await SelectUsuarioId(Usuario.Id);
                if (result != null)
                {
                    IServicioAplicacion<Usuarios> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Usuarios>>();
                    await repositorio.EjecutarConsultaSqlAsync<Usuarios>($"delete from usuarios where id = {Usuario.Id}");
                    return Ok(await SelectAllUsuarios());
                }
                else
                {
                    throw new ApiException(404, $"El usuario con el código '{Usuario.Id}' no está registrado.");
                }

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el usuario {Usuario.Id}");
                throw new ApiException(500, "Ha ocurrido un error interno al eliminar el usuario. Detalle: " + ex.Message);
            }
        }

        private static async Task<IEnumerable<Usuarios>> SelectAllUsuarios()
        {
            IServicioAplicacion<Usuarios> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Usuarios>>();
            return await repositorio.EjecutarConsultaSqlAsync<Usuarios>("select * from usuarios", null);
        }

        private static async Task<Usuarios> SelectUsuarioId(int id)
        {
            IServicioAplicacion<Usuarios> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Usuarios>>();
            var response = await repositorio.EjecutarConsultaSqlAsync<Usuarios>($"select * from usuarios where id = {id}");
            return response.FirstOrDefault();
        }
    }
}
