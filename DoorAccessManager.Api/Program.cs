using DoorAccessManager.Api;
using DoorAccessManager.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersCustom()
                .AddValidators();

builder.Services.AddServices(typeof(CoreIdentifier))
                .ConfigureDatabase(builder.Configuration.GetConnectionString("SQLite"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseErrorHandler()
    .UseRouting();

app.UseHttpsRedirection();

app.MapControllers();

Initializer.InitializeDatabase(app);

app.Run();