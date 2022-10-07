namespace dominikz.Api.Models;

public class StorageFile
{
    public Guid Id { get; set; }
    public FileCagetory Category { get; set; }
    public FileExtension Extension { get; set; }
}

public enum FileCagetory
{
    IMAGE
}

public enum FileExtension
{
    PNG,
    JPG
}