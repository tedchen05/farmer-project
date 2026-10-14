using Farmer_Project.Models.Entity;
using Farmer_Project.Models.ViewModel;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<FarmersInfo> FarmersInfo { set; get; }
    public DbSet<FarmersArticles> FarmersArticles { set; get; }
    public DbSet<FarmersArticlesDetails> FarmersArticlesDetails { get; set; }
    public DbSet<MemberInformation> MemberInformations { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 設置 FarmersInfo 的索引
        modelBuilder.Entity<FarmersInfo>()
            .HasIndex(f => f.FarmName)
            .IsUnique(); // FarmName 唯一

        modelBuilder.Entity<FarmersInfo>()
            .HasIndex(f => f.Email)
            .IsUnique(); // Email 唯一

        // 設置 FarmersArticlesDetails 複合主鍵
        modelBuilder.Entity<FarmersArticlesDetails>()
                .HasKey(f => new { f.ArticlesId, f.DetailId });

        // 設置級聯刪除
        modelBuilder.Entity<FarmersArticles>()
            .HasOne(fa => fa.FarmersInfo) // 與 FarmersInfo 的關聯
            .WithMany(fi => fi.FarmersArticles) // FarmersInfo 可能有多篇文章
            .HasForeignKey(fa => fa.FarmersId) // 外鍵
            .OnDelete(DeleteBehavior.Cascade); // 級聯刪除

        // 如果 FarmersArticlesDetails 也與 FarmersInfo 或 FarmersArticles 關聯，設置級聯刪除
        modelBuilder.Entity<FarmersArticlesDetails>()
            .HasOne(fad => fad.FarmersArticles) // 與 FarmersArticles 的關聯
            .WithMany(fa => fa.FarmersArticlesDetails) // FarmersArticles 可能有多個詳細項目
            .HasForeignKey(fad => fad.ArticlesId) // 外鍵
            .OnDelete(DeleteBehavior.Cascade); // 級聯刪除

    }

    
}