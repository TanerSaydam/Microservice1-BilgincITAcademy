using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Microservice.AuthLayer;

public static class ServiceRegistrar
{
    public static void AddAuthLayer(this IServiceCollection services)
    {
        services.AddAuthentication()
        .AddJwtBearer(opt =>
        {
            string key = "f992aad9-aac1-468d-8809-60092577a53basdsada21be3aa-0051-40df-a77b-154611fb5cec3e040cb-e6c1-4474-aebf-e75844cdcb7e70eb3-9bfc-4ecf-be35-f628b4d0f6d878f0196-dasd192f-4e59asdawd-8f88-a706f4e1c724a";

            opt.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = "Issuer",
                ValidAudience = "Audience",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        });
        services.AddAuthorization();
    }
}
