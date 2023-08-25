using Api.Controllers;
using Api.Errors;
using Api.Models.General;
using Api.Models.Inventario;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Api.Controllers.Inventario
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticulosController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ArticulosController> _logger;

        public ArticulosController(ILogger<ArticulosController> logger,
            IConfiguration configuration)
        {
            _configuration = configuration;
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
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var articulos = await connection.QueryAsync<Articulos>("select * from tblarticulos");

                    if (articulos.Any())
                    {
                        return Ok(articulos.ToList());
                    }
                    else
                    {
                        throw new ApiException(404, "No hay artículos registrados.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los artículos.");
                throw new ApiException(500, "Ha ocurrido un error interno al obtener los artículos. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene un artículo por su ID.
        /// </summary>
        /// <param name="codigo">Codigo del artículo.</param>
        /// <returns>El artículo específico que se consulta por el codigo.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentra el artículo correspondiente al código <paramref name="codigo"/> del artículo.</exception>
        [HttpGet("{codigo}")]
        public async Task<ActionResult<Articulos>> GetArticulo(string codigo)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var articulo = await connection.QueryAsync<Articulos>("select * from tblarticulos where codigo = @Codigo",
                        new { Codigo = codigo });

                    if (articulo.Any())
                    {
                        return Ok(articulo.First());
                    }
                    else
                    {
                        throw new ApiException(404, "El artículo con el código '" + codigo + "' no está registrado.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el artículo " + codigo);
                throw new ApiException(500, "Ha ocurrido un error interno al obtener el artículo. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Crea un nuevo articulo.
        /// </summary>
        /// <param name="articulo">Información del articulo a crear.</param>
        /// <returns>Los artículos que devuelve el método SelectAllArticulos.</returns>
        /// <exception cref="ApiException">Se lanza si ya existe el artículo correspondiente al código del artículo <paramref name="articulo"/>.</exception>
        [HttpPost]
        public async Task<ActionResult<List<Articulos>>> CreateArticulo(Articulos articulo)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var result = await SelectArticuloId(connection, articulo.Codigo);
                    if (!result.Any())
                    {
                        await connection.ExecuteAsync("insert into TblArticulos (Codigo, Stock, Nombre, Descripcion, PrecioCompra, PrecioVenta, CategoriaId, UnidadMedidaId, EstadoId) VALUES (@Codigo, @Stock, @Nombre, @Descripcion, @PrecioCompra, @PrecioVenta, @CategoriaId, @UnidadMedidaId, @EstadoId)", articulo);
                        return Ok(await SelectAllArticulos(connection));
                    }
                    else
                    {
                        throw new ApiException(409, "El artículo con el código '" + articulo.Codigo + "' ya está registrado.");
                    }
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
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var result = await SelectArticuloId(connection, articulo.Codigo);
                    if (result.Any())
                    {
                        await connection.ExecuteAsync("UPDATE TblArticulos SET Codigo = @Codigo, Stock = @Stock, Nombre = @Nombre, Descripcion = @Descripcion, PrecioCompra = @PrecioCompra, PrecioVenta = @PrecioVenta, CategoriaId = @CategoriaId, UnidadMedidaId = @UnidadMedidaId, EstadoId = @EstadoId WHERE Id = @Id", articulo);
                        return Ok(await SelectAllArticulos(connection));
                    }
                    else
                    {
                        throw new ApiException(404, "El artículo con el código '" + articulo.Codigo + "' no está registrado.");
                    }
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
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var result = await SelectArticuloId(connection, articulo.Codigo);
                    if (result.Any())
                    {
                        await connection.ExecuteAsync("delete from tblarticulos where id = @id", articulo);
                        return Ok(await SelectAllArticulos(connection));
                    }
                    else
                    {
                        throw new ApiException(404, "El artículo con el código '" + articulo.Codigo + "' no está registrado.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el artículo " + articulo.Id);
                throw new ApiException(500, "Ha ocurrido un error interno al eliminar el artículo. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Selecciona todos los articulos.
        /// </summary>
        /// <param name="connection">Cadena de conexión a la base de datos.</param>
        /// <returns>Todos los articulos de la tabla tblarticulos.</returns>
        private static async Task<IEnumerable<Articulos>> SelectAllArticulos(SqlConnection connection)
        {
            return await connection.QueryAsync<Articulos>("select * from tblarticulos");
        }

        /// <summary>
        /// Selecciona un artículo por identificador.
        /// </summary>
        /// <param name="connection">Cadena de conexión a la base de datos.</param>
        /// <param name="codigo">Identificador del artículo.</param>
        /// <returns>El artículo que se está consultando.</returns>
        private static async Task<IEnumerable<Articulos>> SelectArticuloId (SqlConnection connection, string codigo)
        {
            return await connection.QueryAsync<Articulos>("select * from tblarticulos where codigo = @Codigo", 
                new { Codigo = codigo });
        }
    }
}
