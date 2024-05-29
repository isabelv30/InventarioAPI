using Api.Errors;
using Aplicacion.Servicios;
using Aplicacion.ServiciosGlobales;
using Dominio.Facturacion;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Facturacion
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagosController : ControllerBase
    {
        private readonly ILogger<PagosController> _logger;
        public PagosController(ILogger<PagosController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Pagos>>> GetAllPagos()
        {
            try
            {
                IServicioAplicacion<Pagos> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Pagos>>();
                // Se ejecuta una consulta SQL asincrónica para obtener todos los pagos.
                var pagos = await repositorio.EjecutarConsultaSqlAsync<Pagos>("select * from pagos");

                if (pagos.Any())
                {
                    // Si se encontraron pagos, se devuelve una respuesta exitosa con la lista de pagos.
                    return Ok(pagos.ToList());
                }
                else
                {
                    throw new ApiException(404, "No hay pagos registrados.");
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
                _logger.LogError(ex, "Error al obtener los pagos.");
                throw new ApiException(500, "Ha ocurrido un error interno al obtener los pagos. Detalle: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pagos>> GetPago(int id)
        {
            try
            {
                IServicioAplicacion<Pagos> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Pagos>>();
                var pago = await repositorio.EjecutarConsultaSqlAsync<Pagos>($"select * from pagos where id = {id}");

                if (pago.Any())
                {
                    return Ok(pago.First());
                }
                else
                {
                    throw new ApiException(404, "El pago con el ID '" + id.ToString() + "' no está registrado.");
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el pago " + id);
                throw new ApiException(500, "Ha ocurrido un error interno al obtener el pago. Detalle: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<List<Pagos>>> CreatePago(Pagos pago)
        {
            try
            {
                // Validación si esxiste el pago
                var validacion = await SelectPagoId(pago.Id);

                if (validacion == null)
                {
                    IServicioAplicacion<Pagos> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Pagos>>();

                    // Genera el objeto para la petición
                    object param = new
                    {
                        p_fecha = pago.Fecha,
                        p_monto = pago.Monto,
                        p_registroId = pago.RegistroId,
                        p_medioPago = pago.MedioPagoId,
                    };

                    // Inserta el pago en la base de datos
                    var result = await repositorio.ProcedimientoSqlAsync<Pagos>("SpPagosInsertar", param);

                    // Respuesta exitosa que devuelve todos los pagos de la base de datos
                    return Ok(await SelectAllPagos());
                }
                else
                {
                    // En caso de que el pago ya exista
                    throw new ApiException(409, "El pago con el código '" + pago.Id + "' ya está registrado.");
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el pago.");
                throw new ApiException(500, "Ha ocurrido un error interno al crear el pago. Detalle: " + ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<List<Pagos>>> UpdatePago(Pagos pago)
        {
            try
            {
                var result = await SelectPagoId(pago.Id);
                if (result != null)
                {
                    IServicioAplicacion<Pagos> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Pagos>>();

                    // Genera el objeto para la petición
                    object param = new
                    {
                        p_id = pago.Id,
                        p_fecha = pago.Fecha,
                        p_monto = pago.Monto,
                        p_registroId = pago.RegistroId,
                        p_medioPago = pago.MedioPagoId,
                    };

                    await repositorio.ProcedimientoSqlAsync<Pagos>("SpPagosActualizar", param);
                    return Ok(await SelectAllPagos());
                }
                else
                {
                    throw new ApiException(404, $"El pago con el código '{pago.Id}' no está registrado.");
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el pago " + pago.Id);
                throw new ApiException(500, "Ha ocurrido un error interno al actualizar el pago. Detalle: " + ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult<List<Pagos>>> DeletePago(Pagos pago)
        {
            try
            {
                var result = await SelectPagoId(pago.Id);
                if (result != null)
                {
                    IServicioAplicacion<Pagos> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Pagos>>();
                    await repositorio.EjecutarConsultaSqlAsync<Pagos>($"delete from pagos where id = {pago.Id}");
                    return Ok(await SelectAllPagos());
                }
                else
                {
                    throw new ApiException(404, $"El pago con el código '{pago.Id}' no está registrado.");
                }

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el pago {pago.Id}");
                throw new ApiException(500, "Ha ocurrido un error interno al eliminar el pago. Detalle: " + ex.Message);
            }
        }

        private static async Task<IEnumerable<Pagos>> SelectAllPagos()
        {
            IServicioAplicacion<Pagos> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Pagos>>();
            return await repositorio.EjecutarConsultaSqlAsync<Pagos>("select * from pago", null);
        }

        private static async Task<Pagos> SelectPagoId(int id)
        {
            IServicioAplicacion<Pagos> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Pagos>>();
            var response = await repositorio.EjecutarConsultaSqlAsync<Pagos>($"select * from pago where id = {id}");
            return response.FirstOrDefault();
        }
    }
}
