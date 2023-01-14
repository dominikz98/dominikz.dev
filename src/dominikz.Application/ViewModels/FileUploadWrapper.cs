using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using dominikz.Application.Attributes;
using dominikz.Application.Utils;
using dominikz.Domain.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.Application.ViewModels;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class FileUploadWrapper<T>
{
    [Required]
    [ValidFile]
    [ListNotEmpty]
    public required List<IFormFile> Files { get; set; } = new ();

    [Required]
    [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
    public required T ViewModel { get; init; }
}