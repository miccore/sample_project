using Miccore.Clean.Sample.Application;
using Miccore.Clean.Sample.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddFastEndpoints().SwaggerDocument();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

#region builder configuration
#endregion


// Add Middlewares
var app = builder.Build();

app.MapOpenApi();
app.UseFastEndpoints(c => {
    c.Endpoints.RoutePrefix = "api";
}).UseSwaggerGen();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.Run();

public partial class Program { }