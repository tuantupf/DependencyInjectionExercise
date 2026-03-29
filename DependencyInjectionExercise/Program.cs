using DependencyInjectionExercise.Data;
using DependencyInjectionExercise.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<BookStoreContext>(options =>
    options.UseInMemoryDatabase("BookStoreDb"));

builder.Services.AddSingleton<DiscountService>();
builder.Services.AddTransient<OrderTrackingService>();
builder.Services.AddSingleton<NotificationHub>();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BookStoreContext>();
    context.Database.EnsureCreated();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();
