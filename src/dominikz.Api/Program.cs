using dominikz.Api.Extensions;
using dominikz.Infrastructure;

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();
builder.Services.AddAuthPolicies();
builder.Services.AddFinancialClients();
builder.Services.AddMedlanClient();
builder.Services.AddNoobitClient();
builder.Services.AddSupermarktCheckClient();
builder.Services.AddContext(builder.Configuration, builder.Environment.IsDevelopment());
builder.Services.AddStorage(builder.Configuration);
builder.Services.AddUrlHelper();
builder.Services.AddUtils();
builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.AddOptions();
builder.AddRateLimit();
builder.AddJwtAuth();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCaching();
app.UseCors();
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.Run();