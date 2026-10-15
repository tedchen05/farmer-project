using Farmer_Project.Entities;
using Farmer_Project.Models.Article;

namespace Farmer_Project.Services.Article
{
    // 文章管理商業邏輯
    public interface IArticleService
    {
        List<FarmersArticles> GetBlog(string? cropsType);
        FarmersArticles? GetArticle(int farmersId, int articlesId);
        FarmersArticlesViewModel? BuildEditModel(int id);
        Task<ArticleSaveResult> AddAsync(FarmersArticlesViewModel model, bool isPublish);
        Task<ArticleSaveResult> EditAsync(FarmersArticlesViewModel model, bool isPublish);
        bool Delete(int deleteArticlesId);
    }
}
