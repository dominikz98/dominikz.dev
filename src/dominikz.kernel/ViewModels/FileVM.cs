using dominikz.kernel.Contracts;

namespace dominikz.kernel.ViewModels;

public class FileVM
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public FileCagetoryEnum Category { get; set; }
    public FileExtensionEnum Extension { get; set; }
}
