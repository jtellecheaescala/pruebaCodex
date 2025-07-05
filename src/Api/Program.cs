using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var key = "SuperSecretKey@345";
var issuer = "PruebaCodex";
var audience = "PruebaCodexUsers";
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = signingKey
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

var users = new List<(string Username, string Password)>();

app.MapPost("/api/token", (TokenRequest request) =>
{
    if (request.ClientId == "myClient" && request.ClientSecret == "mySecret")
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, request.ClientId)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Results.Ok(new { token = tokenHandler.WriteToken(token) });
    }
    return Results.Unauthorized();
});

app.MapPost("/api/login", [Microsoft.AspNetCore.Authorization.Authorize]() =>
{
    return Results.Ok(new { message = "Operación exitosa" });
});

app.MapPost("/api/usuarios", [Microsoft.AspNetCore.Authorization.Authorize](UsuarioRequest request) =>
{
    users.Add((request.Username, request.Password));
    return Results.Ok();
});

app.MapGet("/api/usuarios", [Microsoft.AspNetCore.Authorization.Authorize]() =>
{
    return Results.Ok(users.Select(u => new { username = u.Username }));
});

app.Run();

record TokenRequest(string ClientId, string ClientSecret);
record UsuarioRequest(string Username, string Password);
