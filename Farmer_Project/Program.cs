using Farmer_Project.Helpers;
using Farmer_Project.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//建立資料表映射串接
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 註冊會員服務
builder.Services.AddScoped<IMemberService, MemberService>();

// 配置服務
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache(); // 用於存儲 Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 設定 Session 的過期時間
    options.Cookie.HttpOnly = true;                // 增強安全性
    options.Cookie.IsEssential = true;             // 確保 Cookie 符合 GDPR
});

// 添加 HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// 添加管理者授權策略
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAssertion(context =>
        {
            var userSort = context.User.FindFirst("Sort")?.Value;
            return userSort == "1"; // 1 表示管理者
        }));
});


// 構建應用程式
var app = builder.Build();

// 自動執行遷移與更新資料庫
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession(); // 啟用 Session
app.UseRouting();

app.UseAuthorization();



//客製化路由
app.MapControllerRoute(
    name: "farmersArticles",
    pattern: "Farmers/Articles/{FarmersId}/{ArticlesId}",
    defaults: new { controller = "Farmers", action = "Articles" });

app.MapControllerRoute(
    name: "farmersDetail",
    pattern: "Farmers/FarmerDetail/{FarmersId}/{ArticlesId}",
    defaults: new { controller = "Farmers", action = "FarmerDetail" });

app.MapControllerRoute(
    name: "farmersIndex",
    pattern: "Farmers/FarmersIndex/{CropsType}",
    defaults: new { controller = "Farmers", action = "FarmersIndex" });

app.MapControllerRoute(
    name: "farmersBlog",
    pattern: "Farmers/Blog/{CropsType?}",
    defaults: new { controller = "Farmers", action = "Blog" });

//預設路由
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
