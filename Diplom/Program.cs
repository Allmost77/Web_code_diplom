using Diplom.Components;
using Diplom.Data.DbContext;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddDbContext<AppDbContext>(options => {options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));});
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