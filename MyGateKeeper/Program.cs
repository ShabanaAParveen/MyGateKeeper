using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

ThreadPool.SetMinThreads(1000, 1000);
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.Configure(builder.Configuration.GetSection("Kestrel"));
});
//builder.Services.AddRateLimiter(options =>
//{
//    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
//        RateLimitPartition.GetFixedWindowLimiter(
//            partitionKey: "global",
//            factory: _ => new FixedWindowRateLimiterOptions
//            {
//                PermitLimit = 180,
//                Window = TimeSpan.FromSeconds(1),
//                QueueLimit = 150,
//                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
//            }));
//});

builder.Services.AddControllers();
// YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .ConfigureHttpClient((context, handler) =>
    {
        handler.MaxConnectionsPerServer = 500;
    });

// Rate Limiting (VERY IMPORTANT for load test stability)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(1);
        opt.PermitLimit = 170;// allow burst
        opt.QueueLimit = 100;
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    });
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


//app.UseAuthentication();
//app.UseAuthorization();
app.MapControllers();
app.MapGet("/test", async () =>
{
    await Task.Delay(10);
    return "OK";
}).RequireRateLimiting("fixed");
app.UseRateLimiter();
app.MapReverseProxy().RequireRateLimiting("fixed");
app.Run();
