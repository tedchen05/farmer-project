using Farmer_Project.Entities;
using Microsoft.EntityFrameworkCore;

namespace Farmer_Project.Repositories.Article
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly AppDbContext _dbContext;

        public ArticleRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 取得某作物類型、已發佈的小農介紹文章
        public List<FarmersArticles> GetFarmerIntros(string cropsType)
        {
            return _dbContext.FarmersArticles
                .Include(f => f.FarmersInfo)
                .Include(fd => fd.FarmersArticlesDetails)
                .Where(a => a.FarmersInfo != null &&
                            a.FarmersInfo.CropsType != null &&
                            a.FarmersInfo.CropsType.ToLower() == cropsType.ToLower() &&
                            a.ArticleType == "小農介紹" &&
                            a.IsPublished)
                .ToList();
        }

        // 取得已發佈的單篇文章(含小農與段落)
        public FarmersArticles? GetPublishedArticle(int farmersId, int articlesId)
        {
            return _dbContext.FarmersArticles
                .Include(f => f.FarmersInfo)
                .Include(fad => fad.FarmersArticlesDetails)
                .FirstOrDefault(a => a.FarmersId == farmersId &&
                                     a.ArticlesId == articlesId &&
                                     a.IsPublished);
        }

        // 取得部落格(一般文章)，可依作物類型篩選
        public List<FarmersArticles> GetBlog(string? cropsType)
        {
            var query = _dbContext.FarmersArticles
                .Include(a => a.FarmersInfo)
                .Where(a => a.ArticleType == "一般文章" && a.IsPublished);

            if (!string.IsNullOrEmpty(cropsType) && cropsType != "All")
            {
                query = query.Where(a => a.FarmersInfo.CropsType == cropsType);
            }

            return query.OrderByDescending(a => a.CreatedDate).ToList();
        }

        // 依文章Id取得(含段落)
        public FarmersArticles? GetByIdWithDetails(int id)
        {
            return _dbContext.FarmersArticles
                .Include(a => a.FarmersArticlesDetails)
                .FirstOrDefault(a => a.ArticlesId == id);
        }

        // 依文章Id與小農Id取得(含段落)
        public FarmersArticles? GetByIdAndFarmerWithDetails(int articlesId, int farmersId)
        {
            return _dbContext.FarmersArticles
                .Include(a => a.FarmersArticlesDetails)
                .FirstOrDefault(a => a.ArticlesId == articlesId && a.FarmersId == farmersId);
        }

        // 依文章Id取得
        public FarmersArticles? GetById(int id)
        {
            return _dbContext.FarmersArticles.FirstOrDefault(a => a.ArticlesId == id);
        }

        // 新增文章
        public void AddArticle(FarmersArticles article)
        {
            _dbContext.FarmersArticles.Add(article);
        }

        // 新增段落
        public void AddDetail(FarmersArticlesDetails detail)
        {
            _dbContext.FarmersArticlesDetails.Add(detail);
        }

        // 刪除文章
        public void RemoveArticle(FarmersArticles article)
        {
            _dbContext.FarmersArticles.Remove(article);
        }

        // 儲存變更
        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
