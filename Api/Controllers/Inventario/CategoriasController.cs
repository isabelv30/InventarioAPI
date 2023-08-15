using Api.Models.Inventario;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Api.Controllers.Inventario
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriasController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(ILogger<CategoriasController> logger,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los categorias.
        /// </summary>
        [HttpGet(Name = "Getcategorias")]
        public async Task<ActionResult<List<Categorias>>> GetAllCategorias()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var categorias = await connection.QueryAsync<Categorias>("select * from tblcategorias");
                    return Ok(categorias.ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorias.");
                return StatusCode(500, "Error al obtener categorias.");
            }
        }

        /// <summary>
        /// Obtiene un categoria por su ID.
        /// </summary>
        /// <param name="categoriaId">ID del categoria.</param>
        [HttpGet("{categoriaId:int}")]
        public async Task<ActionResult<Categorias>> GetCategoria(int categoriaId)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var categoria = await connection.QueryAsync<Categorias>("select * from tblcategorias where id = @Id",
                        new { Id = categoriaId });
                    return Ok(categoria.First());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el categoria " + categoriaId);
                return StatusCode(500, "Error al obtener el categoria.");
            }
        }

        // <summary>
        /// Crea un nuevo categoria.
        /// </summary>
        /// <param name="categoria">Información del categoria a crear.</param>
        [HttpPost]
        public async Task<ActionResult<List<Categorias>>> CreateCategoria(Categorias categoria)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.ExecuteAsync("insert into tblcategorias (nombre, descripcion) values (@nombre, @descripcion)", categoria);
                    return Ok(await SelectAllCategorias(connection));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el categoria.");
                return StatusCode(500, "Error al crear el categoria.");
            }
        }

        // <summary>
        /// Actualiza un categoria.
        /// </summary>
        /// <param name="categoria">Información del categoria a actualizar.</param>
        [HttpPut]
        public async Task<ActionResult<List<Categorias>>> UpdateCategoria(Categorias categoria)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.ExecuteAsync("update tblcategorias set nombre = @nombre, descripcion = @descripcion where id = @id", categoria);
                    return Ok(await SelectAllCategorias(connection));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el categoria " + categoria.Id);
                return StatusCode(500, "Error al actualizar el categoria.");
            }
        }

        // <summary>
        /// Elimina un categoria.
        /// </summary>
        /// <param name="categoria">Información del categoria a eliminar.</param>
        [HttpDelete]
        public async Task<ActionResult<List<Categorias>>> DeleteCategoria(Categorias categoria)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.ExecuteAsync("delete from tblcategorias where id = @id", categoria);
                    return Ok(await SelectAllCategorias(connection));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el categoria " + categoria.Id);
                return StatusCode(500, "Error al eliminar el categoria.");
            }
        }

        /// <summary>
        /// Selecciona todos los categorias.
        /// </summary>
        /// <param name="connection">Cadena de conexión a la base de datos.</param>
        /// <returns>Todos los categorias de la tabla tblcategorias.</returns>
        private static async Task<IEnumerable<Categorias>> SelectAllCategorias(SqlConnection connection)
        {
            return await connection.QueryAsync<Categorias>("select * from tblcategorias");
        }
    }
}
