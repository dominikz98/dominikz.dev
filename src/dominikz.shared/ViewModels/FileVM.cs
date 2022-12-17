using dominikz.shared.Contracts;

namespace dominikz.shared.ViewModels;

public class FileVM
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public FileCategoryEnum Category { get; set; }
    public FileExtensionEnum Extension { get; set; }
}
