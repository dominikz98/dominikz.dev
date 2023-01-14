namespace dominikz.Application.ViewModels;

public record FileDownloadWrapper(Stream Data, string Name, string ContentType);