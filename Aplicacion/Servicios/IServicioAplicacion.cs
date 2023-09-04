using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Servicios
{
    public interface IServicioAplicacion<U>
    {
        Task<IEnumerable<U>> EjecutarConsultaSqlAsync<U>(string sql, object parametros = null);

        Task<U> EjecutarScalarSqlAsync<U>(string sql, object parametros = null);

        Task<int> EjecutarSqlAsync(string sql, object parametros = null);

        Task<IEnumerable<U>> ProcAlmacenadoSqlAsync<U>(string sql, object parametros = null);
    }
}
