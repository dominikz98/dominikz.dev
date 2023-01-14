using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components;

public partial class Rating
{
    [Parameter] public int Value { get; set; }
    [Parameter] public bool ShowValue { get; set; } = true;
    
    private int FullStars => CountDivisibles(Value, 20);
    private bool HalfStar => CountDivisibles(Value, 20) * 2 != CountDivisibles(Value, 10);
    private int EmptyStars => 5 - (FullStars + (HalfStar ? 1 : 0));

    private static int CountDivisibles(int value, int divide)
    {
        var counter = 0;

        for (var i = 1; i <= value; i++)
            if (i % divide == 0)
                counter++;

        return counter;
    }
}