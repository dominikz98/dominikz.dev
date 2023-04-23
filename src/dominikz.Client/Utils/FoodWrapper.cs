using System.Diagnostics.CodeAnalysis;
using dominikz.Domain.ViewModels.Cookbook;

namespace dominikz.Client.Utils;

public struct FoodWrapper
{
    public Guid Id { get; }
    public string Name { get; set; }

    public FoodWrapper(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public FoodWrapper(FoodListVm vm)
    {
        Id = vm.Id;
        Name = vm.Name;
    }
    
    public FoodWrapper(FoodVm vm)
    {
        Id = vm.Id ?? Guid.Empty;
        Name = vm.Name;
    }
    
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is FoodWrapper data 
           && data.Id == Id 
           && data.Name == Name;

    public override int GetHashCode()
        => Id.GetHashCode() ^ Name.GetHashCode();
    
    public static bool operator ==(FoodWrapper x, FoodWrapper y)
        => x.Id == y.Id 
           && x.Name == y.Name;
    
    public static bool operator !=(FoodWrapper x, FoodWrapper y)
        => x.Id != y.Id 
           || x.Name != y.Name;

}