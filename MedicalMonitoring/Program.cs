using DAL;
using Microsoft.EntityFrameworkCore;

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

app.Run();
