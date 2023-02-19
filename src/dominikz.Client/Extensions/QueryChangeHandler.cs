using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace dominikz.Client.Extensions;

public static class QueryChangeHandler
{
    private static bool _init;
    private static List<TriggerRegistration> _registrations = new();

    public static bool TrackQuery(this NavigationManager navigationManager, Func<Task> action)
    {
        InitIfRequired(navigationManager);
        
        var trigger = navigationManager.ToAbsoluteUri(navigationManager.Uri).GetLeftPart(UriPartial.Path);
        var exists = _registrations.Any(x => x.Trigger.Equals(trigger, StringComparison.OrdinalIgnoreCase));
        if (exists)
            return false;

        _registrations.Add(new TriggerRegistration(trigger, action));
        return true;
    }

    private static void InitIfRequired(NavigationManager navigationManager)
    {
        if (_init)
            return;

        navigationManager.LocationChanged += OnLocationChanged;
        _init = true;
    }

    private static async void OnLocationChanged(object? sender, LocationChangedEventArgs args)
    {
        var trigger = _registrations.Where(x => args.Location.Contains(x.Trigger, StringComparison.OrdinalIgnoreCase)).ToList();
        _registrations = trigger;
        foreach (var registration in _registrations)
        {
            Console.WriteLine($"Executing for trigger \"{registration.Trigger}\"");
            await registration.Action.Invoke();
        }
    }

    record TriggerRegistration(string Trigger, Func<Task> Action);
}