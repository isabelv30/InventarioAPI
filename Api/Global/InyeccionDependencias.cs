using AccesoDatos.Conexion;
using AccesoDatos.Repositorio;
using Aplicacion.General;
using Aplicacion.Servicios;

namespace Api.Global
{
    public class InyeccionDependencias
    {
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
