var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();
builder.Services.AddRazorPages(); // Registrar Razor Pages
builder.Services.AddControllers(); // Registrar controladores

// Habilitar CORS para permitir peticiones desde cualquier origen (solo para desarrollo)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors(); // Usar CORS

app.UseAuthorization();

app.MapRazorPages(); // Usar Razor Pages
app.MapControllers(); // Mapear controladores

app.Run();
