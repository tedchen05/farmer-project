using Farmer_Project.Entities;
using Farmer_Project.Models.Article;

namespace Farmer_Project.Services.Article
{
    // 文章管理商業邏輯
    public interface IArticleService
    {
        Task<List<FarmersArticles>> GetBlogAsync(string? cropsType);
        Task<FarmersArticles?> GetArticleAsync(int farmersId, int articlesId);
        Task<FarmersArticlesViewModel?> BuildEditModelAsync(int id);
        Task<ArticleSaveResult> AddAsync(FarmersArticlesViewModel model, bool isPublish);
        Task<ArticleSaveResult> EditAsync(FarmersArticlesViewModel model, bool isPublish);
        Task<bool> DeleteAsync(int deleteArticlesId);
    }
}
