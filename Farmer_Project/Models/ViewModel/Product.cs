namespace Farmer_Project.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } // 商品名稱
        public string Description { get; set; } // 商品描述
        public decimal Price { get; set; } // 商品價格
    }
}
