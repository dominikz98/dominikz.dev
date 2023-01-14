using dominikz.Application;
using dominikz.Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
    );
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();
builder.Services.AddAuthPolicies();
builder.Services.AddNoobitClient();
builder.Services.AddJustWatchClient();
builder.Services.AddContext(builder.Configuration, builder.Environment.IsDevelopment());
builder.Services.AddStorage();
builder.Services.AddUrlHelper();
builder.Services.AddUtils();
builder.Services.AddHostedServices();
builder.Services.AddMediatR(typeof(Program));
builder.AddOptions();
builder.AddRateLimit();
builder.AddJwtAuth();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseResponseCaching();
app.UseCors();
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
