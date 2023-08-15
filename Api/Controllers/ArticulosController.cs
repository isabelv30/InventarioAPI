using Api.Controllers;
using Api.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Api.Controllersarticulo
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

        [HttpGet]
        public async Task<ActionResult<List<Articulo>>> GetAllArticulos()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var articulos = await connection.QueryAsync<Articulo>("select * from tblestados");
            return Ok(articulos);
        }
    }
}
