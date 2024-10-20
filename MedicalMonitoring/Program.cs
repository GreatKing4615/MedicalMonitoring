using DAL;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using BL.Services;
using DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MonitoringContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DataBaseConnectionString") ?? throw new ArgumentNullException("DataBaseConnectionString"),
                      assembly => assembly.MigrationsAssembly(typeof(MonitoringContext).Assembly.FullName));
});

// Register repositories
builder.Services.AddScoped<IResearchRepository, ResearchRepository>();
builder.Services.AddScoped<IResearchHistoryRepository, ResearchHistoryRepository>();
builder.Services.AddScoped<IServiceHistoryRepository, ServiceHistoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();

// Register services
builder.Services.AddScoped<IResearchService, ResearchService>();
builder.Services.AddScoped<IResearchHistoryService, ResearchHistoryService>();
builder.Services.AddScoped<IServiceHistoryService, ServiceHistoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Medical monitoring V1");
    });
}

app.UseHttpsRedirection();

// app.UseAuthorization(); // Remove this line if you are not using authorization

app.MapControllers();

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MonitoringContext>();
    dbContext.Database.GetAppliedMigrations();
}

app.Run();
