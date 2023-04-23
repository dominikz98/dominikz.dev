using System.Diagnostics.CodeAnalysis;
using dominikz.Domain.Attributes;
using dominikz.Domain.Structs;

namespace dominikz.Client.Utils;

public class EditWithImageWrapper<T> where T : class, new()
{
    [NotNull] public T ViewModel { get; set; } = new();
    [ListNotEmpty(Max = 1)] public List<FileStruct> Images { get; set; } = new();
}