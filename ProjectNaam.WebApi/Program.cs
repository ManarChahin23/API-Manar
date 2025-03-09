using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// ✅ Voeg controllers toe
builder.Services.AddControllers();

// ✅ Voeg Swagger toe
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Haal de database connection string op (fallback naar environment variabele)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       Environment.GetEnvironmentVariable("DefaultConnection");
Console.WriteLine($"Using Connection String: {connectionString}");

// ✅ Voeg Identity en Dapper Stores toe
builder.Services.AddAuthorization();
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddDapperStores(options => options.ConnectionString = connectionString);

var app = builder.Build();

// ✅ Middleware volgorde is belangrijk!
app.UseHttpsRedirection();
app.UseAuthorization(); // Moet vóór de routes komen

// ✅ Voeg Identity API routes toe onder `/auth`
app.MapGroup("/auth").MapIdentityApi<IdentityUser>();

// ✅ Voeg controllers en API endpoints toe
app.MapControllers();

// ✅ Swagger alleen in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Start de API
app.Run();
