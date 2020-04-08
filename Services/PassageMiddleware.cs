using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SPADemoCRUD.Controllers;
using SPADemoCRUD.Models;
using System.Threading.Tasks;

namespace SPADemoCRUD.Services
{
    public class PassageMiddleware
    {
        private readonly RequestDelegate _next;

        public PassageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IOptions<AppConfig> options, SessionUser sessionUser)
        {
            AuthorizationController.UpdateSessionCookies(httpContext, options.Value);
            await _next.Invoke(httpContext);
        }
    }
}
