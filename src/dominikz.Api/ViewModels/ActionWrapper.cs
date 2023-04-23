using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace dominikz.Api.ViewModels;

public class ActionWrapper<T>
{
    public T? ViewModel { get; }
    public List<string> Errors { get; } = new();
    public bool IsValid => Errors.Count == 0;
    
    public ActionWrapper(T viewModel)
        => ViewModel = viewModel;

    public ActionWrapper(string error)
        => Errors.Add(error);

    public ModelStateDictionary ToErrorList()
    {
        var dic = new ModelStateDictionary();
        for (var i = 0; i < Errors.Count; i++)
            dic.AddModelError(i.ToString(), Errors[i]);

        return dic;
    }
}