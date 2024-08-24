var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.WebHost.UseUrls("http://0.0.0.0:7031");

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=GetHistory}/{action=Index}");

app.Run();