using System.Reactive.Linq;

namespace WpfApp1.Shared.ExtensionMethods;

public static class ObservableExtensions
{
    public static IObservable<T> ObserveOnCurrentSynchronizationContext<T>(this IObservable<T> source)
    {
        var context = SynchronizationContext.Current;
        if (context is null)
            throw new InvalidOperationException("No SynchronizationContext is currently associated with the thread.");

        return source.ObserveOn(context);
    }
}