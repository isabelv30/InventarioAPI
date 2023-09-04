using Api.Dominio.Inventario;
using Api.Errors;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Api.Controllers.Inventario
{
    [ApiController]
    [Route("api/[controller]")]
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
        /// Obtiene todas las categorias.
        /// </summary>
        /// <returns>Una lista de las categorías en la base de datos.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentran categorías registradas.</exception>
        [HttpGet]
        public async Task<ActionResult<List<Categorias>>> GetAllCategorias()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var categorias = await connection.QueryAsync<Categorias>("select * from tblcategorias");
                    
                    if(categorias.Any())
                    {
                        return Ok(categorias.ToList());
                    }
                    else
                    {
                        throw new ApiException(404, "No hay categorías registradas.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorias.");
                throw new ApiException(500, "Ha ocurrido un error interno al obtener las categorias. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene un categoría por su ID.
        /// </summary>
        /// <param name="categoriaId">Id de la categoría.</param>
        /// <returns>La categoría que corresponde al ID <paramref name="categoriaId"/> en la base de datos.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentra la categoría correspondiente al ID <paramref name="categoriaId"/>.</exception>
        [HttpGet("{categoriaId:int}")]
        public async Task<ActionResult<Categorias>> GetCategoria(int categoriaId)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var categoria = await connection.QueryAsync<Categorias>("select * from tblcategorias where id = @Id",
                        new { Id = categoriaId });
                    
                    if (categoria.Any())
                    {
                        return Ok(categoria.First());
                    }
                    else
                    {
                        throw new ApiException(404, "La categoría con el id '" + categoriaId + "' no está registrada.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el categoría " + categoriaId);
                throw new ApiException(500, "Ha ocurrido un error interno al obtener la categoría. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Crea una nueva categoría.
        /// </summary>
        /// <param name="categoria">Información de la categoría a crear.</param>
        /// <returns>Las categorías en la base de datos. Consulte <see cref="SelectAllCategorias"/> para obtener más detalles.</returns>
        /// <exception cref="ApiException">Se lanza si ya existe la categoría correspondiente al ID de la categoría <paramref name="categoria"/>.</exception>
        [HttpPost]
        public async Task<ActionResult<List<Categorias>>> CreateCategoria(Categorias categoria)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var result = await SelectCategoriaId(connection, categoria.Id);
                    if (!result.Any())
                    {
                        await connection.ExecuteAsync("insert into tblcategorias (nombre, descripcion) values (@nombre, @descripcion)", categoria);
                        return Ok(await SelectAllCategorias(connection));
                    }
                    else
                    {
                        throw new ApiException(409, "La categoría con el Id '" + categoria.Id + "' ya está registrada.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la categoría.");
                throw new ApiException(500, "Ha ocurrido un error interno al crear la categoría. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Actualiza un categoría.
        /// </summary>
        /// <param name="categoria">Información de la categoría a actualizar.</param>
        /// <returns>Las categorías en la base de datos. Consulte <see cref="SelectAllCategorias"/> para obtener más detalles.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentra la categoría correspondiente al ID de la categoría <paramref name="categoria"/>.</exception>
        [HttpPut]
        public async Task<ActionResult<List<Categorias>>> UpdateCategoria(Categorias categoria)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var result = await SelectCategoriaId(connection, categoria.Id);
                    if (result.Any())
                    {
                        await connection.ExecuteAsync("update tblcategorias set nombre = @nombre, descripcion = @descripcion where id = @id", categoria);
                        return Ok(await SelectAllCategorias(connection));
                    }
                    else
                    {
                        throw new ApiException(404, "La categoría con el ID '" + categoria.Id + "' no está registrada.");
                    } 
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la categoría " + categoria.Id);
                throw new ApiException(500, "Ha ocurrido un error interno al actualizar la categoría. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Elimina una categoría.
        /// </summary>
        /// <param name="categoria">Información de la categoría a eliminar.</param>
        /// <returns>Las categorías en la base de datos. Consulte <see cref="SelectAllCategorias"/> para obtener más detalles.</returns>
        /// <exception cref="ApiException">Se lanza si no se encuentra la categoría correspondiente al ID de la categoría <paramref name="categoria"/>.</exception>
        [HttpDelete]
        public async Task<ActionResult<List<Categorias>>> DeleteCategoria(Categorias categoria)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var result = await SelectCategoriaId(connection, categoria.Id);
                    if (result.Any())
                    {
                        await connection.ExecuteAsync("delete from tblcategorias where id = @id", categoria);
                        return Ok(await SelectAllCategorias(connection));
                    }
                    else
                    {
                        throw new ApiException(404, "La categoría con el ID '" + categoria.Id + "' no está registrada.");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la categoría " + categoria.Id);
                throw new ApiException(500, "Ha ocurrido un error interno al eliminar la categoría. Detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Selecciona todas las categorías.
        /// </summary>
        /// <param name="connection">Cadena de conexión a la base de datos.</param>
        /// <returns>Todas las categorías de la tabla tblcategorias.</returns>
        private static async Task<IEnumerable<Categorias>> SelectAllCategorias(SqlConnection connection)
        {
            return await connection.QueryAsync<Categorias>("select * from tblcategorias");
        }

        /// <summary>
        /// Consulta una categoría por su identificador.
        /// </summary>
        /// <param name="connection">Cadena de conexión a la base de datos.</param>
        /// <param name="id">Identificador de la categoría.</param>
        /// <returns>La categoría que corresponde al ID <paramref name="id"/> en la base de datos.</returns>
        private static async Task<IEnumerable<Categorias>> SelectCategoriaId(SqlConnection connection, int id)
        {
            return await connection.QueryAsync<Categorias>("select * from tblcategorias where id = @Id", new { id });
        }
    }
}
