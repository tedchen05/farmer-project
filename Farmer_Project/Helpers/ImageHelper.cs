using System.Security.Cryptography;
using System.Text;

namespace Farmer_Project.Helpers
{
    // 圖片處理共用服務
    public class ImageHelper
    {
        // 判斷圖片格式是否有效
        public bool IsImageFormat(IFormFile image)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(image.FileName)?.ToLower();
            return allowedExtensions.Contains(fileExtension);
        }

        // 計算圖片內容的MD5值
        public string ComputeMd5Hash(IFormFile image)
        {
            using (var md5 = MD5.Create())
            using (var stream = image.OpenReadStream())
            {
                byte[] hashBytes = md5.ComputeHash(stream);
                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        // 將圖片存進wwwroot/images，以MD5為檔名，已存在則直接回傳路徑
        public async Task<string> SaveImage(IFormFile image, string md5Hash)
        {
            string fileExtension = Path.GetExtension(image.FileName);
            string fileName = md5Hash + fileExtension;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            if (System.IO.File.Exists(filePath))
            {
                return "/images/" + fileName;
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return "/images/" + fileName;
        }
    }
}
