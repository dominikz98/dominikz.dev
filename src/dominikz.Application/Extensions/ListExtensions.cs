namespace dominikz.Application.Extensions;

public static class ListExtensions
{
    public static IFormFile? GetBySingleOrId(this List<IFormFile> files, Guid id)
        => files.Where(x => x.Length > 0)
            .Where(x => x.FileName != string.Empty)
            .Where(x => Path.GetFileNameWithoutExtension(x.FileName).Equals(id.ToString(), StringComparison.OrdinalIgnoreCase) || files.Count == 1)
            .FirstOrDefault();
}