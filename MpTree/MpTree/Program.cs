using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MpTree.DBControl;
using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы в контейнер.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "MpTree API",
        Description = "Веб-API для управления музыкальными файлами проекта MpTree."
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Настраиваем подключение к SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton(new SqliteController(connectionString));
builder.Services.AddScoped<SongDao>();

var app = builder.Build();

// Инициализируем базу данных
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var songDao = services.GetRequiredService<SongDao>();
        songDao.InitializeTable();
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Таблица 'Songs' успешно инициализирована (или уже существовала).");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Произошла ошибка при инициализации базы данных.");
    }
}

// Настраиваем конвейер обработки HTTP-запросов
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MpTree API V1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Настраиваем URL-адреса
var urls = builder.Configuration.GetSection("Urls");
if (urls.Exists())
{
    app.Urls.Clear();
    app.Urls.Add(urls["Http"]);
    app.Urls.Add(urls["Https"]);
}

app.Run();
