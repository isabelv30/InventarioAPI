using Api.Controllers;
using Api.Models.Inventario;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Api.Controllers.Inventario
{
    [ApiController]
    [Route("[controller]")]
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
        /// Obtiene todos los articulos.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Articulos>>> GetAllArticulos()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var articulos = await connection.QueryAsync<Articulos>("select * from tblarticulos");
                    return Ok(articulos.ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los artículos.");
                return StatusCode(500, "Error al obtener los artículos.");
            }
        }

        /// <summary>
        /// Obtiene un articulo por su ID.
        /// </summary>
        /// <param name="articuloId">ID del articulo.</param>
        [HttpGet("{articuloId:int}")]
        public async Task<ActionResult<Articulos>> GetArticulo(int articuloId)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var articulo = await connection.QueryAsync<Articulos>("select * from tblarticulos where id = @Id",
                        new { Id = articuloId });
                    return Ok(articulo.First());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el artículo " + articuloId);
                return StatusCode(500, "Error al obtener el artículo.");
            }
        }

        // <summary>
        /// Crea un nuevo articulo.
        /// </summary>
        /// <param name="articulo">Información del articulo a crear.</param>
        [HttpPost]
        public async Task<ActionResult<List<Articulos>>> Createarticulo(Articulos articulo)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.ExecuteAsync("INSERT INTO TblArticulos (Codigo, Stock, Nombre, Descripcion, PrecioCompra, PrecioVenta, CategoriaId, UnidadMedidaId, EstadoId) VALUES (@Codigo, @Stock, @Nombre, @Descripcion, @PrecioCompra, @PrecioVenta, @CategoriaId, @UnidadMedidaId, @EstadoId)", articulo);
                    return Ok(await SelectAllarticulos(connection));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el artículo.");
                return StatusCode(500, "Error al crear el artículo.");
            }
        }

        // <summary>
        /// Actualiza un articulo.
        /// </summary>
        /// <param name="articulo">Información del articulo a actualizar.</param>
        [HttpPut]
        public async Task<ActionResult<List<Articulos>>> Updatearticulo(Articulos articulo)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.ExecuteAsync("UPDATE TblArticulos SET Codigo = @Codigo, Stock = @Stock, Nombre = @Nombre, Descripcion = @Descripcion, PrecioCompra = @PrecioCompra, PrecioVenta = @PrecioVenta, CategoriaId = @CategoriaId, UnidadMedidaId = @UnidadMedidaId, EstadoId = @EstadoId WHERE Id = @Id", articulo);
                    return Ok(await SelectAllarticulos(connection));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el artículo " + articulo.Id);
                return StatusCode(500, "Error al actualizar el artículo.");
            }
        }

        // <summary>
        /// Elimina un articulo.
        /// </summary>
        /// <param name="articulo">Información del articulo a eliminar.</param>
        [HttpDelete]
        public async Task<ActionResult<List<Articulos>>> Deletearticulo(Articulos articulo)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.ExecuteAsync("delete from tblarticulos where id = @id", articulo);
                    return Ok(await SelectAllarticulos(connection));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el artículo " + articulo.Id);
                return StatusCode(500, "Error al eliminar el artículo.");
            }
        }

        /// <summary>
        /// Selecciona todos los articulos.
        /// </summary>
        /// <param name="connection">Cadena de conexión a la base de datos.</param>
        /// <returns>Todos los articulos de la tabla tblarticulos.</returns>
        private static async Task<IEnumerable<Articulos>> SelectAllarticulos(SqlConnection connection)
        {
            return await connection.QueryAsync<Articulos>("select * from tblarticulos");
        }
    }
}
