using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy => policy
            .WithOrigins("http://localhost:3000")
            .WithHeaders("Content-Type")
            .WithMethods("GET", "POST")
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .SetPreflightMaxAge(TimeSpan.FromMinutes(3)));
});

var app = builder.Build();

app.UseCors("CorsPolicy");
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
