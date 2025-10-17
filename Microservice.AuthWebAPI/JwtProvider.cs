using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Microservice.AuthWebAPI;

public sealed class JwtProvider
{
    public string CreateToken()
    {
        var claims = new List<Claim>()
        {
            new Claim("fullname","Taner Saydam"),
            new Claim("scope","product.getall"),
            new Claim("scope","product.create"),
            new Claim("scope","product.update"),
            new Claim("scope","product.delete"),
            new Claim("UserType","registered"),
        };

        string key = "f992aad9-aac1-468d-8809-60092577a53basdsada21be3aa-0051-40df-a77b-154611fb5cec3e040cb-e6c1-4474-aebf-e75844cdcb7e70eb3-9bfc-4ecf-be35-f628b4d0f6d878f0196-dasd192f-4e59asdawd-8f88-a706f4e1c724a";
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
        JwtSecurityToken jwtSecurityToken = new(
            issuer: "Issuer",
            audience: "Audience",
            claims: claims,
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials);

        JwtSecurityTokenHandler handler = new();
        var token = handler.WriteToken(jwtSecurityToken);

        return token;
    }
}
