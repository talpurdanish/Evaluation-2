using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Evaluation.Helpers
{
    public class AppExceptionHandlerMiddleware : AbstractExceptionHandlerMiddleware
    {


        public AppExceptionHandlerMiddleware(RequestDelegate next) : base(next)
        {

        }

        public override (HttpStatusCode code, string message) GetResponse(Exception exception)
        {
            var code = exception switch
            {
                KeyNotFoundException
                                    or FileNotFoundException => HttpStatusCode.NotFound,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                InvalidOperationException
                                or AppException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError,
            };
            // if exception has no localized message, use default message
            // default message can be something like "Something went wrong"
            var message = exception.Message ?? "Sorry we encountered an error. We are trying to solve it. The website will be up and running shortly, please";
            return (code, JsonConvert.SerializeObject(new JsonResult(message)));
        }
    }
}
