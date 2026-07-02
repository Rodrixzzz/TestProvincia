using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TestProvincia.Application.Interfaces;
using TestProvincia.Application.Services;
using TestProvincia.Application.Validations;
using TestProvincia.Domain.Interfaces;
using TestProvincia.Infrastructure.Data;
using TestProvincia.Infrastructure.Repositories;
using TestProvinciaApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TestProvincia", Version = "v1" });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

});

builder.Services.AddDbContext<AppDbContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
          b => b.EnableRetryOnFailure(
              maxRetryCount: 3,
              maxRetryDelay: TimeSpan.FromSeconds(5),
              errorNumbersToAdd: null)
              .MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
      )
);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<DuplicatedExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}
else
{
    app.UseHttpsRedirection();
}
app.UseAuthorization();
app.MapControllers();
app.UseExceptionHandler();
app.Run();
