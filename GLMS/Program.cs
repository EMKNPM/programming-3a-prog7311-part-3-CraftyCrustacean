using GLMS.ApiClient;
using GLMS.Commands;
using GLMS.Data;
using GLMS.Services;
using Microsoft.EntityFrameworkCore;
using GLMS.ApiClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Tell the app what database to use when the controller asks
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddSingleton<RequestInvoker>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<ICurrencyExchangeService, CurrencyExchangeService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
});
builder.Services.AddScoped<IContractEligibilityService, ContractEligiblilityService>();
builder.Services.AddHttpClient<IGlmsApiClient, GlmsApiClient>(client =>
{
    var apiBase = builder.Configuration["GlmsApi:BaseUrl"]
                  ?? "https://localhost:7090/";
    client.BaseAddress = new Uri(apiBase);
    client.Timeout = TimeSpan.FromSeconds(15);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
