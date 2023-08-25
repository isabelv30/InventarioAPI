using Api.Controllers.Inventario;
using Api.Models.General;
using Api.Models.Inventario;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Api.Controllers.General
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaisesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaisesController> _logger;

        public PaisesController(ILogger<PaisesController> logger,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los pais.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Pais>>> GetAllPais()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var pais = await connection.QueryAsync<Pais>("select * from TblPaises");
                    return Ok(pais.ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pais.");
                return StatusCode(500, "Error al obtener pais.");
            }
        }

        /// <summary>
        /// Obtiene un pais por su ID.
        /// </summary>
        /// <param name="paisId">ID del pais.</param>
        [HttpGet("{paisId:int}")]
        public async Task<ActionResult<Pais>> GetPais(int paisId)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var pais = await connection.QueryAsync<Pais>("select * from TblPaises where id = @Id",
                        new { Id = paisId });
                    return Ok(pais.First());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el pais " + paisId);
                return StatusCode(500, "Error al obtener el pais.");
            }
        }

        // <summary>
        /// Crea un nuevo pais.
        /// </summary>
        /// <param name="pais">Información del pais a crear.</param>
        //[HttpPost]
        //public async Task<ActionResult<List<Pais>>> CreatePais(Pais pais)
        //{
        //    try
        //    {
        //        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //        {
        //            await connection.ExecuteAsync("insert into TblPaises ([Nombre], [CodigoIso2], [CodigoIso3]) values (@Nombre, @CodigoIso2, @CodigoIso3)", pais);
        //            return Ok(await SelectAllPais(connection));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al crear el pais.");
        //        return StatusCode(500, "Error al crear el pais.");
        //    }
        //}

        // <summary>
        /// Actualiza un pais.
        /// </summary>
        /// <param name="pais">Información del pais a actualizar.</param>
        //[HttpPut]
        //public async Task<ActionResult<List<Pais>>> UpdatePais(Pais pais)
        //{
        //    try
        //    {
        //        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //        {
        //            await connection.ExecuteAsync("update TblPaises set Nombre = @Nombre, CodigoIso2 = @CodigoIso2, CodigoIso3 = @CodigoIso3 where Id = @Id", pais);
        //            return Ok(await SelectAllPais(connection));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al actualizar el pais " + pais.Id);
        //        return StatusCode(500, "Error al actualizar el pais.");
        //    }
        //}

        // <summary>
        /// Elimina un pais.
        /// </summary>
        /// <param name="pais">Información del pais a eliminar.</param>
        //[HttpDelete]
        //public async Task<ActionResult<List<Pais>>> DeletePais(Pais pais)
        //{
        //    try
        //    {
        //        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //        {
        //            await connection.ExecuteAsync("delete from TblPaises where id = @id", pais);
        //            return Ok(await SelectAllPais(connection));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al eliminar el pais " + pais.Id);
        //        return StatusCode(500, "Error al eliminar el pais.");
        //    }
        //}

        /// <summary>
        /// Selecciona todos los pais.
        /// </summary>
        /// <param name="connection">Cadena de conexión a la base de datos.</param>
        /// <returns>Todos los pais de la tabla TblPaises.</returns>
        private static async Task<IEnumerable<Pais>> SelectAllPais(SqlConnection connection)
        {
            return await connection.QueryAsync<Pais>("select * from TblPaises");
        }
    }
}
