using dominikz.Domain.Enums.Cookbook;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components;

public partial class NutriScore
{
    [Parameter] public NutriScoreValue Value { get; set; }
    [Parameter] public bool ShowScale { get; set; } = true;
}