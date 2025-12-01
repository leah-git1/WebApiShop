using Microsoft.EntityFrameworkCore;
using Repository;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IUsersServices, UsersServices>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddDbContext<ShopContext>(option => option.UseSqlServer("Data Source=srv2\\pupils;Initial Catalog=MyDB_329114565;Integrated Security=True;Trust Server Certificate=True"));
// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
