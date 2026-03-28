var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// simple endpoint
app.MapGet("/test", async () =>
{
    await Task.Delay(10); // simulate work
    return "OK from backend";
});

app.Run();