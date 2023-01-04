using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using dominikz.api.Attributes;
using dominikz.api.Utils;
using dominikz.shared.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.api.Models.ViewModels;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class FileUploadWrapper<T>
{
    [Required]
    [ValidFile]
    [ListNotEmpty]
    public required List<IFormFile> Files { get; init; } = new ();

    [Required]
    [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
    public required T ViewModel { get; init; }
}