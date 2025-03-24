using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Haal de database connection string op 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       Environment.GetEnvironmentVariable("DefaultConnection");
Console.WriteLine($"Using Connection String: {connectionString}");


// CORS instellen zodat Unity toegang krijgt
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Voeg controllers toe
builder.Services.AddControllers();

// Voeg Swagger toe
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Voeg Identity en Dapper Stores toe
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddDapperStores(options => options.ConnectionString = connectionString);


//  Voeg JWT authenticatie toe
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
{
    throw new Exception("❌ JWT Key is te kort of ontbreekt. Voeg een veilige key toe in appsettings.json!");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
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
