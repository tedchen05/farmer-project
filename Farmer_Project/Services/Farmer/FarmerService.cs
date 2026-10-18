using Farmer_Project.Entities;
using Farmer_Project.Helpers;
using Farmer_Project.Models.Farmer;
using Farmer_Project.Repositories.Article;
using Farmer_Project.Repositories.Farmer;
using Farmer_Project.Repositories.Member;
using Microsoft.Extensions.Logging;

namespace Farmer_Project.Services.Farmer
{
    public class FarmerService : IFarmerService
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly ImageHelper _imageHelper;
        private readonly ILogger<FarmerService> _logger;

        public FarmerService(IFarmerRepository farmerRepository,
                             IArticleRepository articleRepository,
                             IMemberRepository memberRepository,
                             ImageHelper imageHelper,
                             ILogger<FarmerService> logger)
        {
            _farmerRepository = farmerRepository;
            _articleRepository = articleRepository;
            _memberRepository = memberRepository;
            _imageHelper = imageHelper;
            _logger = logger;
        }

        // 後台主頁資料(小農 + 會員)
        public async Task<FarmersAndMembersViewModel> GetDashboardAsync()
        {
            return new FarmersAndMembersViewModel
            {
                FarmersInfos = await _farmerRepository.GetAllWithArticlesAsync(),
                MemberInformations = await _memberRepository.GetAllAsync()
            };
        }

        // 小農搜尋，空字串回傳全部
        public async Task<FarmersAndMembersViewModel> SearchAsync(string search)
        {
            var viewModel = new FarmersAndMembersViewModel();

            if (string.IsNullOrEmpty(search))
            {
                viewModel.FarmersInfos = await _farmerRepository.GetAllWithArticlesAsync();
                return viewModel;
            }

            var transSearch = TransDictionary(search);
            viewModel.FarmersInfos = await _farmerRepository.SearchAsync(search, transSearch);
            return viewModel;
        }

        // 某作物類型的小農介紹文章
        public async Task<List<FarmersArticles>> GetFarmerIntrosAsync(string cropsType)
        {
            return await _articleRepository.GetFarmerIntrosAsync(cropsType);
        }

        // 小農故事(單篇已發佈文章)
        public async Task<FarmersArticles?> GetFarmerStoryAsync(int farmersId, int articlesId)
        {
            return await _articleRepository.GetPublishedArticleAsync(farmersId, articlesId);
        }

        // 依Id取得小農
        public async Task<FarmersInfo?> GetByIdAsync(int id)
        {
            return await _farmerRepository.GetByIdAsync(id);
        }

        // 新增小農
        public async Task<FarmerSaveResult> AddAsync(FarmersInfo farmer, IFormFile? image)
        {
            var result = new FarmerSaveResult();

            try
            {
                if (image != null)
                {
                    if (!_imageHelper.IsImageFormat(image))
                    {
                        result.ImageInvalid = true;
                        result.Message = "請上傳有效的圖片格式（jpg, jpeg, png, gif）";
                        return result;
                    }

                    var md5Hash = _imageHelper.ComputeMd5Hash(image);
                    farmer.Image = await _imageHelper.SaveImage(image, md5Hash);
                }
                else
                {
                    farmer.Image = "/images/f-1.jpg";
                }

                _farmerRepository.Add(farmer);
                await _farmerRepository.SaveAsync();

                result.Success = true;
                result.Message = "資料儲存成功！";
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增小農資料失敗, 農場名: {FarmName}", farmer.FarmName);
                result.Success = false;
                result.Message = "農場名或 Email已有資料，請更換！";
                return result;
            }
        }

        // 修改小農
        public async Task<FarmerSaveResult> EditAsync(int id, FarmersInfo model, IFormFile? image)
        {
            var result = new FarmerSaveResult();

            if (id != 0)
            {
                var farmer = await _farmerRepository.GetByIdAsync(id);
                if (farmer == null)
                {
                    result.Success = false;
                    result.Message = "找不到對應的農場資料";
                    return result;
                }

                if (image != null)
                {
                    if (!_imageHelper.IsImageFormat(image))
                    {
                        result.ImageInvalid = true;
                        result.Message = "請上傳有效的圖片格式（jpg, jpeg, png, gif）";
                        return result;
                    }

                    var md5Hash = _imageHelper.ComputeMd5Hash(image);
                    farmer.Image = await _imageHelper.SaveImage(image, md5Hash);
                }

                farmer.Name = model.Name;
                farmer.FarmName = model.FarmName;
                farmer.Address = model.Address;
                farmer.Phone = model.Phone;
                farmer.Email = model.Email;
                farmer.CropsType = model.CropsType;
                farmer.PlantType = model.PlantType;

                await _farmerRepository.SaveAsync();
            }

            result.Success = true;
            return result;
        }

        // 刪除小農，找不到回傳false
        public async Task<bool> DeleteAsync(int deleteFarmersId)
        {
            var farmer = await _farmerRepository.GetByIdAsync(deleteFarmersId);
            if (farmer == null)
            {
                return false;
            }

            _farmerRepository.Remove(farmer);
            await _farmerRepository.SaveAsync();
            return true;
        }

        // 中文作物名稱轉英文
        private string TransDictionary(string input)
        {
            if (input.All(c => char.IsLetter(c) && char.IsAscii(c)))
            {
                return input;
            }

            var translateDict = new Dictionary<string, string>
            {
                { "咖啡", "Coffee" },
                { "芒果", "Mango" }
            };

            if (translateDict.TryGetValue(input, out var translated))
            {
                return translated;
            }

            return input;
        }
    }
}
