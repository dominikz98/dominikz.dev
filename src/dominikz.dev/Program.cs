using dominikz.dev.Endpoints;
using dominikz.dev.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<BrowserService>()
    .AddScoped<BlogEndpoints>()
    .AddScoped<CookbookEndpoints>()
    .AddScoped<MediaEndpoints>()
    .AddScoped<MovieEndpoints>()
    .AddScoped<GameEndpoints>()
    .AddScoped<BookEndpoints>()
    .AddHttpClient<ApiClient>((client) => client.BaseAddress = new Uri("http://localhost:5187/"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
