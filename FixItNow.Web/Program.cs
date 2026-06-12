using FixItNow.Application;
using FixItNow.Infrastructure;
using FixItNow.Web;
using FixItNow.Web.Components;
using FixItNow.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRequiredServices()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

// Auto-migrate on startup — acceptable for portfolio MVP, replace with CI pipeline for production
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FixItNowDataContext>();
    db.Database.Migrate();
}

app.Run();