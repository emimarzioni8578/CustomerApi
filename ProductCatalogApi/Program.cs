using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProductCatalogApi.DataAccess;
using ProductCatalogApi.DataAccess.Repositories;
using ProductCatalogApi.Business;
using ProductCatalogApi.Validators;
using ProductCatalogApi.Middlewares; // add the namespace that contains ExceptionHandlingMiddleware

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<CatalogoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogoConnection")));

builder.Services.AddValidatorsFromAssembly(typeof(ProductValidator).Assembly);

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

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
