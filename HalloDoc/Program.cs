using Business_Logic.Interface;
using Business_Logic.LogicRepositories;
using Data_Layer.CustomModels;
using Data_Layer.DataContext;
using HalloDoc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HelloDocMvc.Repository.Repositories;
using Rotativa.AspNetCore;
using Data_Layer.DataModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationDbContext")));


builder.Services.AddScoped<ILogin,ILogicRepositories>();
builder.Services.AddScoped<IPatientReq,patientReqRepo>();
builder.Services.AddScoped<IConciergeReq,ConciergeReqRepo>();
builder.Services.AddScoped<IFamilyFriendReq,familyFriendRepo>();
builder.Services.AddScoped<IBusinessReq,BusinessReqRepo>();
builder.Services.AddScoped<IpatientDashboard, patientDashRepo>();
builder.Services.AddScoped<IsubmitInfoMe, submitInfoMe>();
builder.Services.AddScoped<IpatientProfile, patientProfileRepo>();
builder.Services.AddScoped<IsomeoneElse, reqSomeOneElse>();
builder.Services.AddScoped<IAdminDashboard,AdminDashboardRepo>();
builder.Services.AddScoped<IviewCase,viewCaseRepo>();
builder.Services.AddScoped<IviewNotes,viewNotesRepo>();
builder.Services.AddScoped<IcancelCase,cancelCaseRepo>();
builder.Services.AddScoped<IjwtService, JwtService>();
builder.Services.AddScoped<IProviderMenu, providerMenuRepo>();
builder.Services.AddScoped<IProviderDashboard, ProviderDashboardRepo>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IGenericRepository<WeeklyTimeSheet>, GenericRepository<WeeklyTimeSheet>>();
builder.Services.AddScoped<IGeneralService,GeneralService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();//For Session

builder.Services.AddSignalR();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/Admin") || context.Request.Path.StartsWithSegments("/Home") || context.Request.Path.StartsWithSegments("/Provider"))
    {
        context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        context.Response.Headers.Add("Pragma", "no-cache");
        context.Response.Headers.Add("Expires", "0");
    }

    await next.Invoke();
});

app.UseSession();//For Session

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseRotativa();


app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chat");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=patientSite}/{id?}");

app.Run();
