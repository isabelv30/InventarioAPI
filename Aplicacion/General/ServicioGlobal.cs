using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.General
{
    public class ServicioGlobal
    {
        // Instancia única del servicio
        private static readonly Lazy<ServicioGlobal> instance = new(() => new ServicioGlobal());
        // Propiedad pública para acceder a la instancia única
        public static ServicioGlobal Instance => instance.Value;
        public IServiceProvider ServiceProvider { get; set; }

        public string? MyProperty { get; set; }
    }
}
