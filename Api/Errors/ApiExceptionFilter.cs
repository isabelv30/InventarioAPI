using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Api.Errors
{
    public class ApiExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; set; }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is ApiException apiException)
            {
                context.Result = new ObjectResult(new
                {
                    error = new
                    {
                        code = apiException.StatusCode,
                        message = apiException.Message
                    }
                })
                {
                    StatusCode = apiException.StatusCode,
                    ContentTypes = { "application/json" }
                };

                context.ExceptionHandled = true;
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // No se necesita código aquí
        }
    }
}
