using GLMS.Data;
using GLMS.Services;
using GLMS.Commands;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
var connectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IContractEligibilityService, ContractEligiblilityService>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<ICurrencyExchangeService, CurrencyExchangeService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
});
builder.Services.AddSingleton<RequestInvoker>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GLMS.Data.ApplicationDbContext>();
    if (db.Database.IsRelational())
    {
        int retries = 10;
        while (retries > 0)
        {
            try
            {
                db.Database.Migrate();
                break;
            }
            catch (Exception ex) when (retries > 1)
            {
                Console.WriteLine($"not ready: {ex.Message}.. retrying in 5s ({retries - 1} left)...");
                Thread.Sleep(5000);
                retries--;
            }
        }
    }
}

// Configure the HTTP request pipeline.

app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }