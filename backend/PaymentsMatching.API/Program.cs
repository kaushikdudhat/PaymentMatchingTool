using Microsoft.EntityFrameworkCore;
using PaymentsMatching.API.Data;
using PaymentsMatching.API.Middleware;
using PaymentsMatching.API.Repositories;
using PaymentsMatching.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Payments Matching API",
        Version = "v1",
        Description = "API for matching internal system payments against payment provider records."
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMatchResultRepository, MatchResultRepository>();
builder.Services.AddScoped<IMatchingService, MatchingService>();
builder.Services.AddScoped<ICsvParserService, CsvParserService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payments Matching API v1");
        c.RoutePrefix = "swagger";
    });

    app.MapGet("/", () => Results.Ok(new
    {
        Message = "Payments Matching API is running.",
        Swagger = "/swagger"
    }));
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

app.Run();
