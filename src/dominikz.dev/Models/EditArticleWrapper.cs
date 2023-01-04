using dominikz.dev.Components.Files;
using dominikz.shared.Attributes;
using dominikz.shared.ViewModels.Blog;

namespace dominikz.dev.Models;

public class EditArticleWrapper : EditArticleVm
{
    [ListNotEmpty(Max = 1)]
    public List<FileStruct> Images { get; set; } = new();
}