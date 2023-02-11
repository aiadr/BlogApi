using System.Text.Json.Serialization;
using Blog.Api.Configuration;
using Blog.Infrastructure;
using Blog.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.ConfigureSwagger();

builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.ConfigureInfrastructure(builder.Configuration.GetConnectionString("BlogContext"));
builder.Services.ConfigureServices();
builder.Services.ConfigureApiMapping();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.Services.UseInfrastructure();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();