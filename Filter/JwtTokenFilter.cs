using EnglishLearningAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EnglishLearningAPI.Filters
{
    public class JwtTokenFilter
    {
        private readonly RequestDelegate _next;
        private readonly List<(string Path, string Method)> _bypassTokens = new List<(string, string)>
        {
            ("api/users/register", "POST"),
            ("api/users/login", "POST"),
            ("api/users/reset", "POST"),
            ("api/users/new_password", "POST"),
            ("api/course/user", "GET"),
            ("api/topics/user", "GET")
        };

        public JwtTokenFilter(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (IsBypassToken(request))
            {
                await _next(context);
                return;
            }

            if (!request.Headers.TryGetValue("Authorization", out StringValues authHeader) || !authHeader.ToString().StartsWith("Bearer "))
            {
                response.StatusCode = StatusCodes.Status401Unauthorized;
                await response.WriteAsync("{\"message\": \"Unauthorized\"}");
                return;
            }

            var token = authHeader.ToString().Substring(7);
            var jwtTokenUtil = context.RequestServices.GetRequiredService<IJwtTokenUtil>();
            var email = jwtTokenUtil.GetEmailFromToken(token);

            if (email != null && context.User.Identity?.IsAuthenticated == false)
            {
                var userDetailsService = context.RequestServices.GetRequiredService<IUserDetailsService>();
                var userDetails = await userDetailsService.LoadUserByUsernameAsync(email);

                if (jwtTokenUtil.ValidateToken(token, userDetails))
                {
                    var claimsPrincipal = jwtTokenUtil.GetClaimsFromToken(token);
                    var identity = new ClaimsIdentity(claimsPrincipal.Claims, "jwt");
                    context.User = new ClaimsPrincipal(identity);
                }
                else
                {
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    await response.WriteAsync("{\"message\": \"Invalid token\"}");
                    return;
                }
            }

            await _next(context);
        }

        private bool IsBypassToken(HttpRequest request)
        {
            var path = request.Path.Value ?? string.Empty;
            return _bypassTokens.Any(bypassToken => path.Contains(bypassToken.Path) && request.Method.Equals(bypassToken.Method, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
