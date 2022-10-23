using dominikz.kernel.Contracts;

namespace dominikz.api.Models;

public class StorageFile
{
    public Guid Id { get; set; }
    public FileCagetoryEnum Category { get; set; }
    public FileExtensionEnum Extension { get; set; }

    public ICollection<Media> Medias { get; set; } = new List<Media>();
}
