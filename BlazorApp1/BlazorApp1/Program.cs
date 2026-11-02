using BlazorApp1.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Stateless (no fields, no DbContext), so Singleton is safe.
// If it used a DbContext, it would have to be Scoped.
builder.Services.AddSingleton<BlazorApp1.Services.IRecordValidator, BlazorApp1.Services.RecordValidator>();

// Blazor Server: a scoped DbContext would live for the whole user session (circuit),
// so we use a factory and create a short-lived context per operation.
builder.Services.AddDbContextFactory<BlazorApp1.Data.AppDbContext>(options =>
    Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseSqlServer(
        options, builder.Configuration.GetConnectionString("StudyERP")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
