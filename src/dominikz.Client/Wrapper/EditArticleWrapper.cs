using dominikz.Domain.Attributes;
using dominikz.Domain.Structs;
using dominikz.Domain.ViewModels.Blog;

namespace dominikz.Client.Wrapper;

public class EditArticleWrapper : EditArticleVm
{
    [ListNotEmpty(Max = 1)]
    public List<FileStruct> Images { get; set; } = new();
}