using DoorAccessManager.Api;
using DoorAccessManager.Core;
using DoorAccessManager.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersCustom()
                .AddValidators();

builder.Services.AddRepositories(typeof(DataIdentifier))
                .AddServices(typeof(CoreIdentifier))
                .AddMappings()
                .ConfigureDatabase(builder.Configuration.GetConnectionString("SQLite"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerCustom();
builder.Services.AddJwt();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwaggerCustom();

app.UseErrorHandler()
    .UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Initializer.InitializeDatabase(app);

app.Run();