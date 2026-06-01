using Diplom.Components;
using Diplom.Data.DbContext;
using Diplom.Data.Services;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.Configure<CircuitOptions>(options =>
{
    options.DetailedErrors = builder.Environment.IsDevelopment();
});

builder.Services.AddMudServices();
builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<LightingObjectService>();
builder.Services.AddScoped<TaskService>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true); 
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapGet("/", async context =>
{
    context.Response.Redirect("/home", permanent: false);
    await Task.CompletedTask;
});

app.Run();
