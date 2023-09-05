using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.ServiciosGlobales
{
    public interface IServicioGlobal
    {
        IServiceProvider ServiceProvider { get; set; }
    }
}
