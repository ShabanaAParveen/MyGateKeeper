using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Text.Json;

ThreadPool.SetMinThreads(1000, 1000);

var builder = WebApplication.CreateBuilder(args);
//Base address 
builder.Services.Configure<ServiceUrls>(
    builder.Configuration.GetSection("Services"));

builder.Services.AddHttpClient("authz", (sp, client) =>
{
    var config = sp.GetRequiredService<IOptions<ServiceUrls>>().Value;
    client.BaseAddress = new Uri(config.AuthZ.BaseUrl);
});
// 1. Configs & Kestrel
builder.WebHost.ConfigureKestrel(options => options.Configure(builder.Configuration.GetSection("Kestrel")));
builder.Services.Configure<ServiceUrls>(builder.Configuration.GetSection("Services"));

// 2. HttpClient for Orchestration
builder.Services.AddHttpClient("Orchestrator");

// 3. YARP & Services
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddRateLimiter(options => {
    options.AddFixedWindowLimiter("fixed", opt => {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromSeconds(1);
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = false,
            ValidateLifetime = false
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// ==========================================
// ✅ LOGIN & DASHBOARD ORCHESTRATION MIDDLEWARE
// ==========================================
// ==========================================
// ✅ FINAL LOGIN ORCHESTRATION MIDDLEWARE
// ==========================================
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/auth/login") && HttpMethods.IsPost(context.Request.Method))
    {
        var clientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();
        var config = context.RequestServices.GetRequiredService<IConfiguration>();
        var client = clientFactory.CreateClient();

        // 1. LOGIN -> AUTHENTICATION SERVER (Port 5281 from auth-cluster)
        var authServer = config.GetValue<string>("ReverseProxy:Clusters:auth-cluster:Destinations:d1:Address")?.TrimEnd('/');

        context.Request.EnableBuffering();
        var loginReq = new HttpRequestMessage(HttpMethod.Post, $"{authServer}/auth/login");
        loginReq.Content = new StreamContent(context.Request.Body);
        loginReq.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        loginReq.Content.Headers.ContentLength = context.Request.ContentLength;

        var loginRes = await client.SendAsync(loginReq);
        if (!loginRes.IsSuccessStatusCode)
        {
            context.Response.StatusCode = (int)loginRes.StatusCode;
            return;
        }

        // 2. EXTRACT TOKEN
        var authJson = await loginRes.Content.ReadFromJsonAsync<JsonElement>();
        var token = authJson.TryGetProperty("token", out var t) ? t.GetString() : null;
        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = 401;
            return;
        }

        // 3. DASHBOARD -> AUTHORIZATION SERVER (Port 5206 from Services:AuthZ)

        var authzServer = config.GetValue<string>("Services:AuthZ:BaseUrl")?.TrimEnd('/');

        // Note: Your image shows [HttpPost("dashboard-context")], 
        // ensure the string here matches your actual route path.
        var dashUri = new Uri($"{authzServer}/authz/dashboard-context");

        // ✅ CHANGE TO HttpMethod.Post
        var dashReq = new HttpRequestMessage(HttpMethod.Post, dashUri);

        // Log the token length (don't log the whole token for security) to ensure it's not empty
        Console.WriteLine($">>> [TRACE] Sending Token to Authz. Length: {token?.Length ?? 0}");

        // ✅ Try setting the header manually to be 100% sure of the format
        // dashReq.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token}");
        dashReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var dashRes = await client.SendAsync(dashReq);


        // 4. RETURN FINAL RESPONSE
        var dashContent = await dashRes.Content.ReadAsByteArrayAsync();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 200;
        await context.Response.Body.WriteAsync(dashContent);
        return;
    }
    await next();
});


app.MapControllers();
app.MapReverseProxy();

app.Run();

// =====================
// UPDATED DTOs
// =====================
public class ServiceUrls
{
    public AuthZService AuthZ { get; set; } = new();
    public ServiceDetail AuthzServer { get; set; } = new(); // Authorization (Dashboard Context)
}

public class AuthZService
{
    public string BaseUrl { get; set; } = string.Empty;
}

public class ServiceDetail { public string BaseUrl { get; set; } = string.Empty; }
