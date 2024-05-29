using Api.Errors;
using Aplicacion.Servicios;
using Aplicacion.ServiciosGlobales;
using Dominio.Facturacion;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Facturacion
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrosController : ControllerBase
    {
        private readonly ILogger<RegistrosController> _logger;
        public RegistrosController(ILogger<RegistrosController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Registros>>> GetAllRegistros()
        {
            try
            {
                IServicioAplicacion<Registros> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Registros>>();
                // Se ejecuta una consulta SQL asincrónica para obtener todos los registros.
                var registros = await repositorio.EjecutarConsultaSqlAsync<Registros>("select * from registros");

                if (registros.Any())
                {
                    // Si se encontraron registros, se devuelve una respuesta exitosa con la lista de registros.
                    return Ok(registros.ToList());
                }
                else
                {
                    throw new ApiException(404, "No hay registros registrados.");
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
                _logger.LogError(ex, "Error al obtener los registros.");
                throw new ApiException(500, "Ha ocurrido un error interno al obtener los registros. Detalle: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Registros>> GetRegistro(int id)
        {
            try
            {
                IServicioAplicacion<Registros> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Registros>>();
                var registro = await repositorio.EjecutarConsultaSqlAsync<Registros>($"select * from registros where id = {id}");

                if (registro.Any())
                {
                    return Ok(registro.First());
                }
                else
                {
                    throw new ApiException(404, "El registro con el ID '" + id.ToString() + "' no está registrado.");
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el registro " + id);
                throw new ApiException(500, "Ha ocurrido un error interno al obtener el registro. Detalle: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<List<Registros>>> CreateRegistro(Registros Registro)
        {
            try
            {
                // Validación si esxiste el registro
                var validacion = await SelectRegistroId(Registro.Id);

                if (validacion == null)
                {
                    IServicioAplicacion<Registros> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Registros>>();

                    // Genera el objeto para la petición
                    object param = new
                    {
                        p_fecha = Registro.Fecha,
                        p_responsableId = Registro.ResponsableId,
                        p_clienteId = Registro.ClienteId,
                        p_tipoRegistro = Registro.TipoRegistro,
                        p_total = Registro.Total,
                        p_comentario = Registro.Comentario
                    };

                    // Inserta el registro en la base de datos
                    var result = await repositorio.ProcedimientoSqlAsync<Registros>("SpRegistrosInsertar", param);

                    // Respuesta exitosa que devuelve todos los registros de la base de datos
                    return Ok(await SelectAllRegistros());
                }
                else
                {
                    // En caso de que el registro ya exista
                    throw new ApiException(409, "El registro con el código '" + Registro.Id + "' ya está registrado.");
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el registro.");
                throw new ApiException(500, "Ha ocurrido un error interno al crear el registro. Detalle: " + ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<List<Registros>>> UpdateRegistro(Registros Registro)
        {
            try
            {
                var result = await SelectRegistroId(Registro.Id);
                if (result != null)
                {
                    IServicioAplicacion<Registros> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Registros>>();

                    // Genera el objeto para la petición
                    object param = new
                    {
                        p_id = Registro.Id,
                        p_fecha = Registro.Fecha,
                        p_responsableId = Registro.ResponsableId,
                        p_clienteId = Registro.ClienteId,
                        p_tipoRegistro = Registro.TipoRegistro,
                        p_total = Registro.Total,
                        p_comentario = Registro.Comentario
                    };

                    await repositorio.ProcedimientoSqlAsync<Registros>("SpRegistrosActualizar", param);
                    return Ok(await SelectAllRegistros());
                }
                else
                {
                    throw new ApiException(404, $"El registro con el código '{Registro.Id}' no está registrado.");
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el registro " + Registro.Id);
                throw new ApiException(500, "Ha ocurrido un error interno al actualizar el registro. Detalle: " + ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult<List<Registros>>> DeleteRegistro(Registros Registro)
        {
            try
            {
                var result = await SelectRegistroId(Registro.Id);
                if (result != null)
                {
                    IServicioAplicacion<Registros> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Registros>>();
                    await repositorio.EjecutarConsultaSqlAsync<Registros>($"delete from registros where id = {Registro.Id}");
                    return Ok(await SelectAllRegistros());
                }
                else
                {
                    throw new ApiException(404, $"El registro con el código '{Registro.Id}' no está registrado.");
                }

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el registro {Registro.Id}");
                throw new ApiException(500, "Ha ocurrido un error interno al eliminar el registro. Detalle: " + ex.Message);
            }
        }

        private static async Task<IEnumerable<Registros>> SelectAllRegistros()
        {
            IServicioAplicacion<Registros> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Registros>>();
            return await repositorio.EjecutarConsultaSqlAsync<Registros>("select * from registros", null);
        }

        private static async Task<Registros> SelectRegistroId(int id)
        {
            IServicioAplicacion<Registros> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Registros>>();
            var response = await repositorio.EjecutarConsultaSqlAsync<Registros>($"select * from registros where id = {id}");
            return response.FirstOrDefault();
        }
    }
}
