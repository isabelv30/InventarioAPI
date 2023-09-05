using AccesoDatos.Conexion;
using AccesoDatos.Repositorio;
using Aplicacion.ServiciosGlobales;
using Aplicacion.Servicios;

namespace Api.Global
{
    public class InyeccionDependencias
    {
        /// <summary>
        /// Configura y crea un proveedor de servicios para la inyección de dependencias.
        /// </summary>
        /// <param name="cadena">La cadena de conexión que se utilizará para la configuración.</param>
        /// <returns>Un proveedor de servicios configurado para la inyección de dependencias.</returns>
        public static IServiceProvider ConfigureServices(string cadena)
        {
            var services = new ServiceCollection();
            // Configurar los servicios necesarios
            services.AddSingleton<ICadenaConexion>(new CadenaConexion(cadena))
                    .AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>))
                    .AddSingleton(typeof(IServicioAplicacion<>), typeof(ServicioAplicacion<>));
            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Genera el proveedor de servicios y lo asigna a la variable global.
        /// </summary>
        public static void GenerateService(string cadena)
        {
            try
            {
                IServiceProvider serviceProvider = ConfigureServices(cadena);
                ServicioGlobal.Instance.ServiceProvider = serviceProvider;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
