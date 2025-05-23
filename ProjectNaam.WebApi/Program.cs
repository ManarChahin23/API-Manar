﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ProjectNaam.WebApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Haal de database connection string op 
var connectionString = builder.Configuration.GetConnectionString("SqlConnectionString") ??
                       Environment.GetEnvironmentVariable("SqlConnectionString");
Console.WriteLine($"Using Connection String: {connectionString}");

builder.Services.AddScoped<IEnvironment2DRepository>(provider =>
    new Environment2DRepository(connectionString));

builder.Services.AddScoped<IObject2DRepository>(provider =>
    new Object2DRepository(connectionString));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});


builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Set up Identity and Dapper stores for IdentityUser
builder.Services.AddAuthorization();
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddDapperStores(options =>
    {
        options.ConnectionString = connectionString;
    });


builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080);
});


builder.Services.AddAuthorization(); // Voeg autorisatie toe

var app = builder.Build();


app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins"); 

app.UseAuthentication();
app.UseAuthorization(); 

// Voeg Identity API routes toe onder `/auth`
app.MapGroup("/auth").MapIdentityApi<IdentityUser>();

app.MapGet("/", () => "API is up");


app.MapControllers();


app.UseSwagger();
app.UseSwaggerUI();


app.UseExceptionHandler("/error");


app.Run();
