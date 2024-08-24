using webChat;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:7028");
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DAL>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<DAL>>();
    string connectionString = "Host=postgres_db;Port=5432;Database=chat_db;Username=postgres;Password=1";

    return new DAL(connectionString, logger);
});

builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:7031")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseSwagger()
    .UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
});
app.UseRouting();
app.UseCors("AllowSpecificOrigin");
app.UseWebSockets();
app.MapControllers();

await app.RunAsync();
