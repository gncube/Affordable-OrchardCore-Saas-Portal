using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, configBuilder) =>
{
    configBuilder.ReadFrom.Configuration(hostingContext.Configuration)
    .Enrich.FromLogContext();
});

builder.Services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
builder.Services.Configure<FormOptions>(options => options.ValueCountLimit = 4096);

var orchardBuilder = builder.Services
    .AddOrchardCms();

if (!builder.Environment.IsDevelopment())
{
    orchardBuilder.AddDatabaseShellsConfiguration();
}
else
{
    orchardBuilder.AddSetupFeatures("OrchardCore.AutoSetup");
}

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

// Add Antiforgery
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.Configure<CookieTempDataProviderOptions>(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseHsts();
app.UseOrchardCore(c => c.UseSerilogTenantNameLogging());
app.UsePoweredByOrchardCore(false);

app.Run();
