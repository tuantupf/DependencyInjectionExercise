using DependencyInjectionExercise.Application;
using DependencyInjectionExercise.Infrastructure.Data;
using DependencyInjectionExercise.Infrastructure.Discounts;
using DependencyInjectionExercise.Infrastructure.Notifications;
using DependencyInjectionExercise.Infrastructure.Repositories;
using DependencyInjectionExercise.Infrastructure.Tracking;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<BookStoreContext>(options =>
    options.UseInMemoryDatabase("BookStoreDb"));

builder.Services.AddSingleton<DiscountService>();
builder.Services.AddScoped<OrderTrackingService>();
builder.Services.AddSingleton<NotificationHub>();

builder.Services.AddKeyedScoped<INotificationSender, EmailNotificationSender>("email");
builder.Services.AddKeyedScoped<INotificationSender, SmsNotificationSender>("sms");
builder.Services.AddKeyedScoped<INotificationSender, PushNotificationSender>("push");

builder.Services.AddScoped<NotificationService>();

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IOrderService, OrderService>();

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
