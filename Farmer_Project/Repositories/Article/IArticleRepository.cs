using Farmer_Project.Entities;

namespace Farmer_Project.Repositories.Article
{
    // 文章與段落資料存取
    public interface IArticleRepository
    {
        Task<List<FarmersArticles>> GetFarmerIntrosAsync(string cropsType);
        Task<FarmersArticles?> GetPublishedArticleAsync(int farmersId, int articlesId);
        Task<List<FarmersArticles>> GetBlogAsync(string? cropsType);
        Task<FarmersArticles?> GetByIdWithDetailsAsync(int id);
        Task<FarmersArticles?> GetByIdAndFarmerWithDetailsAsync(int articlesId, int farmersId);
        Task<FarmersArticles?> GetByIdAsync(int id);
        void AddArticle(FarmersArticles article);
        void AddDetail(FarmersArticlesDetails detail);
        void RemoveArticle(FarmersArticles article);
        Task SaveAsync();
    }
}
