using Farmer_Project.Entities;
using Farmer_Project.Helpers;
using Farmer_Project.Models.Farmer;
using Farmer_Project.Repositories.Article;
using Farmer_Project.Repositories.Farmer;
using Farmer_Project.Repositories.Member;

namespace Farmer_Project.Services.Farmer
{
    public class FarmerService : IFarmerService
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly ImageHelper _imageHelper;

        public FarmerService(IFarmerRepository farmerRepository,
                             IArticleRepository articleRepository,
                             IMemberRepository memberRepository,
                             ImageHelper imageHelper)
        {
            _farmerRepository = farmerRepository;
            _articleRepository = articleRepository;
            _memberRepository = memberRepository;
            _imageHelper = imageHelper;
        }

        // 後台主頁資料(小農 + 會員)
        public FarmersAndMembersViewModel GetDashboard()
        {
            return new FarmersAndMembersViewModel
            {
                FarmersInfos = _farmerRepository.GetAllWithArticles(),
                MemberInformations = _memberRepository.GetAll()
            };
        }

        // 小農搜尋，空字串回傳全部
        public FarmersAndMembersViewModel Search(string search)
        {
            var viewModel = new FarmersAndMembersViewModel();

            if (string.IsNullOrEmpty(search))
            {
                viewModel.FarmersInfos = _farmerRepository.GetAllWithArticles();
                return viewModel;
            }

            var transSearch = TransDictionary(search);
            viewModel.FarmersInfos = _farmerRepository.Search(search, transSearch);
            return viewModel;
        }

        // 某作物類型的小農介紹文章
        public List<FarmersArticles> GetFarmerIntros(string cropsType)
        {
            return _articleRepository.GetFarmerIntros(cropsType);
        }

        // 小農故事(單篇已發佈文章)
        public FarmersArticles? GetFarmerStory(int farmersId, int articlesId)
        {
            return _articleRepository.GetPublishedArticle(farmersId, articlesId);
        }

        // 依Id取得小農
        public FarmersInfo? GetById(int id)
        {
            return _farmerRepository.GetById(id);
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
                _farmerRepository.Save();

                result.Success = true;
                result.Message = "資料儲存成功！";
                return result;
            }
            catch
            {
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
                var farmer = _farmerRepository.GetById(id);
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

                _farmerRepository.Save();
            }

            result.Success = true;
            return result;
        }

        // 刪除小農，找不到回傳false
        public bool Delete(int deleteFarmersId)
        {
            var farmer = _farmerRepository.GetById(deleteFarmersId);
            if (farmer == null)
            {
                return false;
            }

            _farmerRepository.Remove(farmer);
            _farmerRepository.Save();
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
