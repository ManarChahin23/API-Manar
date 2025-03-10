using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ Haal de database connection string op (fallback naar environment variabele)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       Environment.GetEnvironmentVariable("DefaultConnection");
Console.WriteLine($"Using Connection String: {connectionString}");

// ✅ CORS instellen zodat Unity toegang krijgt
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ✅ Voeg controllers toe
builder.Services.AddControllers();

// ✅ Voeg Swagger toe
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Voeg Identity en Dapper Stores toe
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddDapperStores(options => options.ConnectionString = connectionString);

// ✅ Voeg JWT authenticatie toe
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

// ✅ Middleware volgorde is BELANGRIJK!
app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins"); // Activeer CORS

app.UseAuthentication(); // Moet vóór Authorization komen
app.UseAuthorization(); // Moet vóór API-routes komen

// ✅ Voeg Identity API routes toe onder `/auth`
app.MapGroup("/auth").MapIdentityApi<IdentityUser>();

// ✅ Voeg controllers en API endpoints toe
app.MapControllers();

// ✅ Voeg Swagger UI toe
app.UseSwagger();
app.UseSwaggerUI();

// ✅ Zorg voor betere foutmeldingen
app.UseExceptionHandler("/error");

// ✅ Start de API
app.Run();
