using AccesoDatos.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Servicios
{
    public class ServicioAplicacion<T> : IServicioAplicacion<T> where T : class
    {
        private readonly IRepositorio<T> _repositorio;

        public ServicioAplicacion(IRepositorio<T> repositorio)
        {
            _repositorio = repositorio;
        }

        /// <summary>
        /// Ejecuta una consulta SQL de forma asincrónica y devuelve el resultado como una colección de tipo T.
        /// Gracias a la inyección de dependencias es ejecutado desde la capa de presentación y trae la información de la capa de acceso a datos.
        /// </summary>
        /// <typeparam name="T">El tipo de datos esperado en el resultado.</typeparam>
        /// <param name="sql">La consulta SQL que se ejecutará.</param>
        /// <param name="parametros">Los parámetros opcionales que se pueden pasar a la consulta.</param>
        /// <returns>Una colección de tipo T que contiene el resultado de la consulta. </returns>
        public async Task<IEnumerable<T>> EjecutarConsultaSqlAsync<T>(string sql, object parametros = null)
        {
            // Este método invoca el correspondiente método de la capa de acceso a datos.
            return await _repositorio.ExecuteQueryAsync<T>(sql, parametros);
        }

        /// <summary>
        /// Ejecuta una consulta SQL de forma asincrónica y devuelve un único valor escalar de tipo T.
        /// </summary>
        /// <typeparam name="T">El tipo de datos esperado para el valor escalar.</typeparam>
        /// <param name="sql">La consulta SQL que se ejecutará.</param>
        /// <param name="parametros">Los parámetros opcionales que se pueden pasar a la consulta.</param>
        /// <returns>Un valor escalar de tipo T que contiene el resultado de la consulta.</returns>
        public async Task<T> EjecutarScalarSqlAsync<T>(string sql, object parametros = null)
        {
            // Este método invoca el correspondiente método de la capa de acceso a datos.
            return await _repositorio.ExecuteScalarAsync<T>(sql, parametros);
        }

        /// <summary>
        /// Ejecuta un comando SQL de forma asincrónica (por ejemplo, INSERT, UPDATE, DELETE) y devuelve el número de filas afectadas.
        /// </summary>
        /// <param name="sql">El comando SQL que se ejecutará.</param>
        /// <param name="parametros">Los parámetros opcionales que se pueden pasar al comando.</param>
        /// <returns>El número de filas afectadas por el comando SQL.</returns>
        public async Task<int> EjecutarSqlAsync(string sql, object parametros = null)
        {
            // Este método invoca el correspondiente método de la capa de acceso a datos.
            return await _repositorio.ExecuteAsync(sql, parametros);
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado SQL de forma asincrónica y devuelve el resultado como una colección de tipo T.
        /// </summary>
        /// <typeparam name="T">El tipo de datos esperado en el resultado.</typeparam>
        /// <param name="sql">El nombre o código del procedimiento almacenado SQL que se ejecutará.</param>
        /// <param name="parametros">Los parámetros opcionales que se pueden pasar al procedimiento almacenado.</param>
        /// <returns>Una colección de tipo T que contiene el resultado del procedimiento almacenado.</returns>
        public async Task<IEnumerable<T>> ProcAlmacenadoSqlAsync<T>(string sql, object parametros = null)
        {
            // Este método invoca el correspondiente método de la capa de acceso a datos.
            return await _repositorio.ExecuteStoredProcedureAsync<T>(sql, parametros);
        }

    }
}
