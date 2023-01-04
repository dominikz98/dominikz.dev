using System.ComponentModel.DataAnnotations;
using dominikz.shared.Attributes;
using dominikz.shared.Contracts;

namespace dominikz.shared.ViewModels.Blog;

public class AddArticleVm
{
    [Required] [MinLength(5)] public string Title { get; set; } = string.Empty;

    [Required] [MinLength(5)] public string HtmlText { get; set; } = string.Empty;

    [DataType(DataType.DateTime)] public DateTime? PublishDate { get; set; }

    [RequiredEnum<ArticleCategoryEnum>(Blacklist = new[] { ArticleCategoryEnum.Unknown })]
    public ArticleCategoryEnum Category { get; set; }

    [Required] [ListNotEmpty] public List<string> Tags { get; set; } = new();
}