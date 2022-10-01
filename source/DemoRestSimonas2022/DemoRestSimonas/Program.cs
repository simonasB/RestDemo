using DemoRestSimonas.Data;
using DemoRestSimonas.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Microsoft.EntityFrameworkCore.SqlServer
// Microsoft.EntityFrameworkCore.Tools
// dotnet tool install --global dotnet-ef
// dotnet ef migrations add
// dotnet ef database update

builder.Services.AddControllers();

builder.Services.AddDbContext<ForumDbContext>();
builder.Services.AddTransient<ITopicsRepository, TopicsRepository>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();