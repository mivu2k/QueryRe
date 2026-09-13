using QueryRe.Data.Services;
using QueryRe.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// These are the four main services used by QueryRe.
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddScoped(provider =>
{
    IConfiguration config = provider.GetRequiredService<IConfiguration>();
    string connectionString = config.GetConnectionString("QueryDatabase")
        ?? throw new Exception("The SQL Server connection string is missing.");

    return new SqlService(connectionString);
});
builder.Services.AddScoped<QueryService>();
builder.Services.AddSingleton<HistoryService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
