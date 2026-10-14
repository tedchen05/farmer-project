
public class FarmersArticlesViewModel
{
    public int FarmersId { get; set; }
    public int? ArticlesId { get; set; }
    public ArticleViewModel Articles { get; set; } = new ArticleViewModel();
    public List<FarmersArticlesDetailsViewModel> ArticleDetails { get; set; } = new List<FarmersArticlesDetailsViewModel>();
}

public class ArticleViewModel
{
    public string ArticleType { get; set; }
    public string ArticleTitle { get; set; }
    public IFormFile? ArticleImage { get; set; }
    public string? ArticleImagePath { get; set; }
    public string ArticleSummary { get; set; }
}


public class FarmersArticlesDetailsViewModel
{
    public int DetailId { get; set; }
    public string SubTitle { get; set; }
    public IFormFile? SubImage { get; set; }
    public string? SubImagePath { get; set; }
    public string SubContent { get; set; }
}