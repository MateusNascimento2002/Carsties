using AuctionService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

try
{
    DbInitializer.Initialize(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.Run();