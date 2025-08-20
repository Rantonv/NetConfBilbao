var builder = WebApplication.CreateBuilder(args);

// Servicios necesarios para Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Otros servicios
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// Configuración CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7206", "http://localhost:5274")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Middleware de Swagger (normalmente solo en desarrollo)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors(); // <-- Aquí

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();