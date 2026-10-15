using Farmer_Project.Helpers;
using Farmer_Project.Repositories.Article;
using Farmer_Project.Repositories.Farmer;
using Farmer_Project.Repositories.Login;
using Farmer_Project.Repositories.Member;
using Farmer_Project.Services.Article;
using Farmer_Project.Services.Contact;
using Farmer_Project.Services.Farmer;
using Farmer_Project.Services.Login;
using Farmer_Project.Services.Member;
using Farmer_Project.Services.Message;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//建立資料表映射串接
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 註冊登入資料存取
builder.Services.AddScoped<ILoginRepository, LoginRepository>();

// 註冊登入服務
builder.Services.AddScoped<ILoginService, LoginService>();

// 註冊會員管理
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IMemberService, MemberService>();

// 註冊訊息服務
builder.Services.AddScoped<IMessageService, MessageService>();

// 註冊聯絡我們服務
builder.Services.AddScoped<IContactService, ContactService>();

// 註冊小農管理
builder.Services.AddScoped<IFarmerRepository, FarmerRepository>();
builder.Services.AddScoped<IFarmerService, FarmerService>();

// 註冊文章管理
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IArticleService, ArticleService>();

// 註冊寄信與圖片服務
builder.Services.AddScoped<EmailHelper>();
builder.Services.AddScoped<ImageHelper>();

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
    name: "articleArticles",
    pattern: "Article/Articles/{FarmersId}/{ArticlesId}",
    defaults: new { controller = "Article", action = "Articles" });

app.MapControllerRoute(
    name: "farmerDetail",
    pattern: "Farmer/FarmerDetail/{FarmersId}/{ArticlesId}",
    defaults: new { controller = "Farmer", action = "FarmerDetail" });

app.MapControllerRoute(
    name: "farmerIndex",
    pattern: "Farmer/FarmersIndex/{CropsType}",
    defaults: new { controller = "Farmer", action = "FarmersIndex" });

app.MapControllerRoute(
    name: "articleBlog",
    pattern: "Article/Blog/{CropsType?}",
    defaults: new { controller = "Article", action = "Blog" });

//預設路由
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
