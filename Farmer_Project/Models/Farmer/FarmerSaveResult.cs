namespace Farmer_Project.Models.Farmer
{
    // 小農資料儲存結果
    public class FarmerSaveResult
    {
        public bool ImageInvalid { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
