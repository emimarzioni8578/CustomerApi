using CustomersApi.DataAccess;
using CustomersApi.DataAccess.Repositories;
using CustomersApi.Business;
using Microsoft.EntityFrameworkCore;
using CustomersApi.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// EF Core: registra el DbContext con la cadena de conexión de appsettings.json.
builder.Services.AddDbContext<CustomersDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de dependencias: Controller -> Service -> Repository -> DbContext.
builder.Services.AddScoped<ICustomersRepository, CustomersRepository>();
builder.Services.AddScoped<ICustomersService, CustomersService>();

builder.Services.AddSingleton<SingletonMarker>();
builder.Services.AddScoped<ScopedMarker>();
builder.Services.AddTransient<TransientMarker>();
builder.Services.AddScoped<ConsumerA>();
builder.Services.AddScoped<ConsumerB>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "CustomersApi v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
