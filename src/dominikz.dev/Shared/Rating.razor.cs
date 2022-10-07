using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared;

public partial class Rating
{
    [Parameter]
    public int Value { get; set; }

    public int FullStars { get => CountDivisibles(Value, 20); }
    public bool HalfStar { get => (CountDivisibles(Value, 20) * 2) != CountDivisibles(Value, 10); }
    public int EmptyStars { get => 5 - (FullStars + (HalfStar ? 1 : 0)); }

    private static int CountDivisibles(int value, int divide)
    {
        var counter = 0;

        for (int i = 1; i <= value; i++)
            if (i % divide == 0)
                counter++;

        return counter;
    }
}
