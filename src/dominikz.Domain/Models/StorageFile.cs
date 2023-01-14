using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Files;

namespace dominikz.Domain.Models;

public class StorageFile
{
    public Guid Id { get; set; }
    public FileCategoryEnum Category { get; set; }
    public FileExtensionEnum Extension { get; set; }

    public Media? Media { get; set; }
    public Person? Person { get; set; }
    public Article? Article { get; set; }
}
