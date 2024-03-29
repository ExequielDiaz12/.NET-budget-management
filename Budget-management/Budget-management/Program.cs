using Budget_management.Interfaces;
using Budget_management.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IRepositorioTipoCuenta, RepositorioTipoCuenta>();//ponemos transient porque este servicio no va a compartir datos
builder.Services.AddTransient<IServicioUsuario, ServicioUsuario>();
builder.Services.AddTransient<IRepositorioCuenta,RepositorioCuenta>();
builder.Services.AddTransient<IRepositorioCategoria,RepositorioCategoria>();
builder.Services.AddTransient<IRepositorioTransaccion,RepositorioTransaccion>();    
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IServicioReporte, ServicioReporte>();
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transaccion}/{action=Index}/{id?}");

app.Run();
