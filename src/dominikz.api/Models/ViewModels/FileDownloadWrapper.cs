namespace dominikz.api.Models.ViewModels;

public record FileDownloadWrapper(Stream Data, string Name, string ContentType);