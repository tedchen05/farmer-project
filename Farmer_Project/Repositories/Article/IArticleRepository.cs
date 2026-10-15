using Farmer_Project.Entities;

namespace Farmer_Project.Repositories.Article
{
    // 文章與段落資料存取
    public interface IArticleRepository
    {
        List<FarmersArticles> GetFarmerIntros(string cropsType);
        FarmersArticles? GetPublishedArticle(int farmersId, int articlesId);
        List<FarmersArticles> GetBlog(string? cropsType);
        FarmersArticles? GetByIdWithDetails(int id);
        FarmersArticles? GetByIdAndFarmerWithDetails(int articlesId, int farmersId);
        FarmersArticles? GetById(int id);
        void AddArticle(FarmersArticles article);
        void AddDetail(FarmersArticlesDetails detail);
        void RemoveArticle(FarmersArticles article);
        void Save();
    }
}
