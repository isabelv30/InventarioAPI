namespace Aplicacion.Servicios
{
    public interface IServicioAplicacion<U>
    {
        /// <summary>
        /// Ejecuta una consulta SQL de forma asincrónica y devuelve los resultados como una colección de objetos del tipo genérico U.
        /// </summary>
        /// <typeparam name="U">El tipo de objetos que se esperan como resultado de la consulta.</typeparam>
        /// <param name="sql">La consulta SQL que se va a ejecutar.</param>
        /// <param name="parametros">Los parámetros opcionales para la consulta SQL.</param>
        /// <returns>Una tarea que representa la ejecución de la consulta y una colección de objetos del tipo U como resultado.</returns>
        Task<IEnumerable<U>> EjecutarConsultaSqlAsync<U>(string sql, object? parametros = null);

        /// <summary>
        /// Ejecuta una consulta SQL de forma asincrónica y devuelve un valor único del tipo genérico U como resultado.
        /// </summary>
        /// <typeparam name="U">El tipo de valor que se espera como resultado de la consulta.</typeparam>
        /// <param name="sql">La consulta SQL que se va a ejecutar.</param>
        /// <param name="parametros">Los parámetros opcionales para la consulta SQL.</param>
        /// <returns>Una tarea que representa la ejecución de la consulta y un valor único del tipo U como resultado.</returns>
        Task<U> EjecutarScalarSqlAsync<U>(string sql, object? parametros = null);

        /// <summary>
        /// Ejecuta una consulta SQL de forma asincrónica y devuelve el número de filas afectadas como resultado.
        /// </summary>
        /// <param name="sql">La consulta SQL que se va a ejecutar.</param>
        /// <param name="parametros">Los parámetros opcionales para la consulta SQL.</param>
        /// <returns>Una tarea que representa la ejecución de la consulta y el número de filas afectadas como resultado.</returns>
        Task<int> EjecutarSqlAsync(string sql, object? parametros = null);

        /// <summary>
        /// Ejecuta un procedimiento almacenado de SQL de forma asincrónica y devuelve los resultados como una colección de objetos del tipo genérico U.
        /// </summary>
        /// <typeparam name="U">El tipo de objetos que se esperan como resultado del procedimiento almacenado.</typeparam>
        /// <param name="sql">El nombre o la sentencia del procedimiento almacenado a ejecutar.</param>
        /// <param name="parametros">Los parámetros opcionales para el procedimiento almacenado.</param>
        /// <returns>Una tarea que representa la ejecución del procedimiento almacenado y una colección de objetos del tipo U como resultado.</returns>
        Task<IEnumerable<U>> ProcedimientoSqlAsync<U>(string sql, object parametros = null);
    }
}
