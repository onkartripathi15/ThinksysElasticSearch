using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ThinksysElasticSearchApi.Data;
using ThinksysElasticSearchApi.Model;

namespace ThinksysElasticSearchApi.Middleware
{
    public class ApiKeyMiddleware
    {

        private readonly RequestDelegate _next;
      
        private const string APIKEY = "XApiKey";
        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;           
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(APIKEY, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api Key was not provided ");
                return;
            }
            IMatchApiKey _matchApiKey = new MatchApiKey(context.RequestServices.GetRequiredService<IConfiguration>());
            UserModel userModel= _matchApiKey.MatchApiKeyByDB(extractedApiKey);
            //var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
            //var apiKey = appSettings.GetValue<string>(APIKEY);
            if (!(userModel.XApiKey??"").Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client");
                return;
            }
            context.Request.Headers.Add("UserID",userModel.Id.ToString());
            await _next(context);
        }

    }
}
