using Api.Dominio.Inventario;
using Api.Errors;
using Aplicacion.Servicios;
using Aplicacion.ServiciosGlobales;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Inventario
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticulosController : ControllerBase
    {
        private readonly ILogger<ArticulosController> _logger;
        

        public ArticulosController(ILogger<ArticulosController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los artículos.
        /// </summary>
        /// <returns>Una lista de los artículos en la base de datos.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentran artículos registrados.</exception>
        [HttpGet]
        public async Task<ActionResult<List<Articulos>>> GetAllArticulos()
        {
            try
            {
                IServicioAplicacion<Articulos> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Articulos>>();
                // Se ejecuta una consulta SQL asincrónica para obtener todos los artículos.
                var articulos = await repositorio.EjecutarConsultaSqlAsync<Articulos>("select * from articulos");

                if (articulos.Any())
                {
                    // Si se encontraron artículos, se devuelve una respuesta exitosa con la lista de artículos.
                    return Ok(articulos.ToList());
                }
                else
                {
                    throw new ApiException(404, "No hay artículos registrados.");
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
                _logger.LogError(ex, "Error al obtener los artículos.");
                throw new ApiException(500, "Ha ocurrido un error interno al obtener los artículos. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene un artículo por su ID.
        /// </summary>
        /// <param name="codigo">Obtiene un artículo por su ID.</param>
        /// <returns>El artículo específico que se consulta por el codigo.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentra el artículo correspondiente al id <paramref name="id"/> del artículo.</exception>
        [HttpGet("{id}")]
        public async Task<ActionResult<Articulos>> GetArticulo(int id)
        {
            try
            {
                IServicioAplicacion<Articulos> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Articulos>>();
                var articulo = await repositorio.EjecutarConsultaSqlAsync<Articulos>($"select * from articulos where id = {id}");
                
                if (articulo.Any())
                {
                    return Ok(articulo.First());
                }
                else
                {
                    throw new ApiException(404, "El artículo con el ID '" + id.ToString() + "' no está registrado.");
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el artículo " + id);
                throw new ApiException(500, "Ha ocurrido un error interno al obtener el artículo. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Crea un nuevo artículo.
        /// </summary>
        /// <param name="articulo">Información del artículo a crear.</param>
        /// <returns>Los artículos que devuelve el método SelectAllArticulos.</returns>
        /// <exception cref="ApiException">Se lanza si ya existe el artículo correspondiente al código del artículo <paramref name="articulo"/>.</exception>
        [HttpPost]
        public async Task<ActionResult<List<Articulos>>> CreateArticulo(Articulos articulo)
        {
            try
            {
                // Validación si esxiste el artículo
                var validacion = await SelectArticuloId(articulo.Id);

                if (validacion == null)
                {
                    IServicioAplicacion<Articulos> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Articulos>>();

                    // Genera el objeto para la petición
                    object param = new
                    {
                        p_nombre = articulo.Nombre,
                        p_descripcion = articulo.Descripcion,
                        p_precio = articulo.Precio,
                        p_stock = articulo.Stock,
                        p_CategoriaId = articulo.CategoriaId
                    };

                    // Inserta el artículo en la base de datos
                    var result = await repositorio.ProcedimientoSqlAsync<Articulos>("SpArticulosInsertar", param);

                    // Respuesta exitosa que devuelve todos los artículos de la base de datos
                    return Ok(await SelectAllArticulos());
                }
                else
                {
                    // En caso de que el artículo ya exista
                    throw new ApiException(409, "El artículo con el código '" + articulo.Id + "' ya está registrado.");
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el artículo.");
                throw new ApiException(500, "Ha ocurrido un error interno al crear el artículo. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Actualiza un articulo.
        /// </summary>
        /// <param name="articulo">Información del articulo a actualizar.</param>
        /// <returns>Los artículos que devuelve el método SelectAllArticulos.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentra el artículo correspondiente al código del artículo <paramref name="articulo"/>.</exception>
        [HttpPut]
        public async Task<ActionResult<List<Articulos>>> UpdateArticulo(Articulos articulo)
        {
            try
            {
                var result = await SelectArticuloId(articulo.Id);
                if (result != null)
                {
                    IServicioAplicacion<Articulos> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Articulos>>();

                    // Genera el objeto para la petición
                    object param = new
                    {
                        p_id = articulo.Id,
                        p_nombre = articulo.Nombre,
                        p_descripcion = articulo.Descripcion,
                        p_precio = articulo.Precio,
                        p_stock = articulo.Stock,
                        p_CategoriaId = articulo.CategoriaId
                    };

                    await repositorio.ProcedimientoSqlAsync<Articulos>("SpArticulosActualizar", param);
                    return Ok(await SelectAllArticulos());
                }
                else
                {
                    throw new ApiException(404, $"El artículo con el código '{articulo.Id}' no está registrado.");
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el artículo " + articulo.Id);
                throw new ApiException(500, "Ha ocurrido un error interno al actualizar el artículo. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Elimina un articulo.
        /// </summary>
        /// <param name="articulo">Información del articulo a eliminar.</param>
        /// <returns>Los artículos que devuelve el método SelectAllArticulos.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentra el artículo correspondiente al código del artículo <paramref name="articulo"/>.</exception>
        [HttpDelete]
        public async Task<ActionResult<List<Articulos>>> Deletearticulo(Articulos articulo)
        {
            try
            {
                var result = await SelectArticuloId(articulo.Id);
                if (result != null)
                {
                    IServicioAplicacion<Articulos> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Articulos>>();
                    await repositorio.EjecutarConsultaSqlAsync<Articulos>($"delete from articulos where id = {articulo.Id}");
                    return Ok(await SelectAllArticulos());
                }
                else
                {
                    throw new ApiException(404, $"El artículo con el código '{articulo.Id}' no está registrado.");
                }
                
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el artículo {articulo.Id}");
                throw new ApiException(500, "Ha ocurrido un error interno al eliminar el artículo. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Selecciona todos los articulos.
        /// </summary>
        /// <returns>Todos los articulos de la tabla tblarticulos.</returns>
        private static async Task<IEnumerable<Articulos>> SelectAllArticulos()
        {
            IServicioAplicacion<Articulos> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Articulos>>();
            return await repositorio.EjecutarConsultaSqlAsync<Articulos>("select * from articulos", null);
        }

        /// <summary>
        /// Selecciona un artículo por identificador.
        /// </summary>
        /// <param name="codigo">Identificador del artículo.</param>
        /// <returns>El artículo que se está consultando.</returns>
        private static async Task<Articulos> SelectArticuloId(int id)
        {
            IServicioAplicacion<Articulos> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Articulos>>();
            var response = await repositorio.EjecutarConsultaSqlAsync<Articulos>($"select * from articulos where id = {id}");
            return response.FirstOrDefault();
        }
    }
}
