using Farmer_Project.Entities;
using Farmer_Project.Helpers;
using Farmer_Project.Models.Article;
using Farmer_Project.Repositories.Article;

namespace Farmer_Project.Services.Article
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ImageHelper _imageHelper;

        public ArticleService(IArticleRepository articleRepository, ImageHelper imageHelper)
        {
            _articleRepository = articleRepository;
            _imageHelper = imageHelper;
        }

        // 部落格文章清單
        public List<FarmersArticles> GetBlog(string? cropsType)
        {
            return _articleRepository.GetBlog(cropsType);
        }

        // 單篇部落格文章
        public FarmersArticles? GetArticle(int farmersId, int articlesId)
        {
            return _articleRepository.GetPublishedArticle(farmersId, articlesId);
        }

        // 組編輯用的表單資料，查無回傳null
        public FarmersArticlesViewModel? BuildEditModel(int id)
        {
            var article = _articleRepository.GetByIdWithDetails(id);
            if (article == null)
            {
                return null;
            }

            return new FarmersArticlesViewModel
            {
                FarmersId = article.FarmersId ?? 0,
                ArticlesId = article.ArticlesId,
                Articles = new ArticleViewModel
                {
                    ArticleType = article.ArticleType,
                    ArticleTitle = article.ArticleTitle,
                    ArticleImagePath = article.ArticleImagePath,
                    ArticleSummary = article.ArticleSummary
                },
                ArticleDetails = article.FarmersArticlesDetails.Select(detail => new FarmersArticlesDetailsViewModel
                {
                    DetailId = detail.DetailId,
                    SubTitle = detail.SubTitle,
                    SubImagePath = detail.SubImagePath,
                    SubContent = detail.SubContent
                }).ToList()
            };
        }

        // 新增文章與段落
        public async Task<ArticleSaveResult> AddAsync(FarmersArticlesViewModel model, bool isPublish)
        {
            var result = new ArticleSaveResult();

            // 文章圖片
            var image = model.Articles.ArticleImage;
            string imagePath;
            if (image != null)
            {
                if (!_imageHelper.IsImageFormat(image))
                {
                    return ImageError(result, "image", "請上傳有效的圖片格式（jpg, jpeg, png, gif）");
                }
                var md5Hash = _imageHelper.ComputeMd5Hash(image);
                imagePath = await _imageHelper.SaveImage(image, md5Hash);
            }
            else
            {
                imagePath = "/images/default.jpg";
            }

            var articleEntity = new FarmersArticles
            {
                FarmersId = model.FarmersId,
                ArticleType = model.Articles.ArticleType ?? "未選擇",
                ArticleTitle = model.Articles.ArticleTitle ?? "未上傳",
                ArticleImagePath = imagePath,
                ArticleSummary = model.Articles.ArticleSummary ?? "未上傳",
                IsPublished = isPublish
            };

            _articleRepository.AddArticle(articleEntity);
            _articleRepository.Save();
            var articleID = articleEntity.ArticlesId;

            // 段落
            foreach (var detail in model.ArticleDetails)
            {
                string subImagePath;
                if (detail.SubImage != null)
                {
                    if (!_imageHelper.IsImageFormat(detail.SubImage))
                    {
                        return ImageError(result, "image", "請上傳有效的圖片格式（jpg, jpeg, png, gif）");
                    }
                    var md5Hash = _imageHelper.ComputeMd5Hash(detail.SubImage);
                    subImagePath = await _imageHelper.SaveImage(detail.SubImage, md5Hash);
                }
                else
                {
                    subImagePath = "/images/default.jpg";
                }

                _articleRepository.AddDetail(new FarmersArticlesDetails
                {
                    ArticlesId = articleID,
                    DetailId = detail.DetailId,
                    SubTitle = detail.SubTitle ?? "未上傳",
                    SubImagePath = subImagePath,
                    SubContent = detail.SubContent ?? "未上傳"
                });
            }
            _articleRepository.Save();

            result.Success = true;
            return result;
        }

        // 修改文章與段落
        public async Task<ArticleSaveResult> EditAsync(FarmersArticlesViewModel model, bool isPublish)
        {
            var result = new ArticleSaveResult();

            var article = _articleRepository.GetByIdAndFarmerWithDetails(model.ArticlesId ?? 0, model.FarmersId);
            if (article == null)
            {
                result.NotFound = true;
                return result;
            }

            // 文章圖片
            var image = model.Articles.ArticleImage;
            string imagePath;
            if (image != null)
            {
                if (!_imageHelper.IsImageFormat(image))
                {
                    return ImageError(result, "image", "請上傳有效的圖片格式（jpg, jpeg, png, gif）");
                }
                var md5Hash = _imageHelper.ComputeMd5Hash(image);
                imagePath = await _imageHelper.SaveImage(image, md5Hash);
            }
            else
            {
                imagePath = article.ArticleImagePath;
            }

            article.ArticleType = model.Articles.ArticleType ?? "未選擇";
            article.ArticleTitle = model.Articles.ArticleTitle ?? "未填寫";
            article.ArticleSummary = model.Articles.ArticleSummary ?? "未填寫";
            article.ArticleImagePath = imagePath ?? "未上傳";
            article.IsPublished = isPublish;

            // 段落
            foreach (var detailData in model.ArticleDetails)
            {
                var detail = article.FarmersArticlesDetails.FirstOrDefault(d => d.DetailId == detailData.DetailId);
                string subImagePath;

                if (detail != null)
                {
                    if (detailData.SubImage != null)
                    {
                        if (!_imageHelper.IsImageFormat(detailData.SubImage))
                        {
                            return ImageError(result, "subImage", "請上傳有效的段落圖片格式（jpg, jpeg, png, gif）");
                        }
                        var md5Hash = _imageHelper.ComputeMd5Hash(detailData.SubImage);
                        subImagePath = await _imageHelper.SaveImage(detailData.SubImage, md5Hash);
                    }
                    else
                    {
                        subImagePath = detail.SubImagePath;
                    }

                    detail.SubTitle = detailData.SubTitle ?? "未填寫";
                    detail.SubImagePath = subImagePath;
                    detail.SubContent = detailData.SubContent ?? "未填寫";
                }
                else
                {
                    var newDetail = new FarmersArticlesDetails
                    {
                        ArticlesId = model.ArticlesId ?? 0,
                        DetailId = detailData.DetailId,
                        SubTitle = detailData.SubTitle ?? "未填寫",
                        SubContent = detailData.SubContent ?? "未填寫"
                    };

                    if (detailData.SubImage != null)
                    {
                        if (!_imageHelper.IsImageFormat(detailData.SubImage))
                        {
                            return ImageError(result, "subImage", "請上傳有效的段落圖片格式（jpg, jpeg, png, gif）");
                        }
                        var md5Hash = _imageHelper.ComputeMd5Hash(detailData.SubImage);
                        newDetail.SubImagePath = await _imageHelper.SaveImage(detailData.SubImage, md5Hash);
                    }
                    else
                    {
                        newDetail.SubImagePath = "/images/default.jpg";
                    }

                    article.FarmersArticlesDetails.Add(newDetail);
                }
            }

            // 移除沒有提交的段落
            var submittedDetailIds = model.ArticleDetails.Select(d => d.DetailId).ToList();
            var detailsToRemove = article.FarmersArticlesDetails.Where(d => !submittedDetailIds.Contains(d.DetailId)).ToList();
            foreach (var item in detailsToRemove)
            {
                article.FarmersArticlesDetails.Remove(item);
            }

            _articleRepository.Save();

            result.Success = true;
            return result;
        }

        // 刪除文章，找不到回傳false
        public bool Delete(int deleteArticlesId)
        {
            var article = _articleRepository.GetById(deleteArticlesId);
            if (article == null)
            {
                return false;
            }

            _articleRepository.RemoveArticle(article);
            _articleRepository.Save();
            return true;
        }

        // 設定圖片格式錯誤結果
        private ArticleSaveResult ImageError(ArticleSaveResult result, string field, string message)
        {
            result.Success = false;
            result.ErrorField = field;
            result.ErrorMessage = message;
            return result;
        }
    }
}
