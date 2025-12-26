using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", false, true);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority =
            $"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/v2.0";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false 
        };
    });

builder.Services.AddOcelot();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ERP", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("ERP");

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();
app.Run();
