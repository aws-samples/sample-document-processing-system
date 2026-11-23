using DocumentProcessor.Web.Components;
using DocumentProcessor.Web.Configuration;
using DocumentProcessor.Web.Data;
using DocumentProcessor.Web.Extensions;
using DocumentProcessor.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddApplicationOptions(builder.Configuration);

var connection = await DatabaseConnectionResolver.ResolveAsync(
    builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured."),
    builder.Configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions());

builder.Services.AddDocumentProcessing(connection);

var app = builder.Build();

if (connection.Warning is not null)
{
    app.Logger.LogWarning("{Warning}", connection.Warning);
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    await using var db = await dbContextFactory.CreateDbContextAsync();
    // EnsureCreated creates tables only when the database has no tables at all. In the
    // MAM417 workshop, the AWS Transform database job deploys its own schema (dps_dbo) to
    // the same Aurora database, so EnsureCreated would skip this app's table and every
    // query would fail with 'relation "Documents" does not exist'. Create the table
    // whenever it's missing instead.
    var creator = db.GetService<IRelationalDatabaseCreator>();
    if (!await creator.ExistsAsync())
    {
        await creator.CreateAsync();
    }

    var documentsTable = db.Model.FindEntityType(typeof(Document))!.GetTableName()!;
    var documentsTableExists = await db.Database
        .SqlQuery<bool>($"SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = current_schema() AND table_name = {documentsTable}) AS \"Value\"")
        .SingleAsync();
    if (!documentsTableExists)
    {
        await creator.CreateTablesAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

// MapStaticAssets resolves framework assets such as _framework/blazor.web.js in every
// environment; UseStaticFiles only finds them when running as Development.
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
