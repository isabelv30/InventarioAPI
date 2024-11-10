using Api.Dominio.Seguridad;
using Aplicacion.Servicios;
using Aplicacion.ServiciosGlobales;

namespace Api.Global
{
    public class AuthService
    {
        public AuthService() { }

        public static async Task<Usuarios> ValidateUserCredentialsAsync(string username, string password)
        {
            IServicioAplicacion<Usuarios> repositorio = ServicioGlobal.Instance.ServiceProvider.GetRequiredService<IServicioAplicacion<Usuarios>>();
            // Se ejecuta una consulta SQL asincrónica para obtener todos los usuarios.
            var usuarios = await repositorio.EjecutarConsultaSqlAsync<Usuarios>($"SELECT * FROM usuarios WHERE username = @Username", new { Username = username });
            var user = usuarios.FirstOrDefault();
            if (user == null)
            {
                return null; // Usuario no existe
            }

            // Validar la contraseña
            if (VerifyPassword(password, user.Password))
            {
                return user;
            }

            return null; // Contraseña incorrecta
        }

        private static bool VerifyPassword(string passwordInputed, string passwordDatabase )
        {
            return passwordDatabase == passwordInputed ? true : false;
        }

    }
}
