using PuntoVentaInventario.Data;
using Microsoft.EntityFrameworkCore;  // ? AGREGAR ESTO

var builder = WebApplication.CreateBuilder(args);

// AGREGAR ESTO:
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var allowedOrigins = builder.Configuration.GetValue<string>("AllowedOrigins")!;

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(builder => {
        builder.WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader(); 
    }); 
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
