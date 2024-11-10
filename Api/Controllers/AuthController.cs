using Api.Errors;
using Api.Global;
using Dominio.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly TokenService _tokenService;

        public AuthController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] Login login)
        {
            var user = await AuthService.ValidateUserCredentialsAsync(login.Username, login.Password);

            if (user == null)
            {
                throw new ApiException(401, "Credenciales incorrectas.");
            }

            string token = _tokenService.GenerateToken(user.Username, user.Username);
            return Ok(new { Token = token });
        }

    }
}
