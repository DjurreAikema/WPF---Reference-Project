using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp1.Shared.Abstract;

namespace WpfApp1.Shared.ExtensionMethods;

public static class OperationTrackingExtensions
{
    /// <summary>
    /// Tracks an asynchronous operation by updating a state subject with progress indicators
    /// </summary>
    public static IObservable<TResult> TrackAsyncOperation<TSource, TResult, TState>(
        this IObservable<TSource> source,
        BehaviorSubject<TState> stateSubject,
        Func<TSource, Task<TResult>> asyncOperation)
        where TState : BaseState<TState>
    {
        return source
            .Synchronize()
            .Do(_ => stateSubject.OnNext(stateSubject.Value.WithInProgress(true)))
            .SelectMany(obj => Observable.FromAsync(() => asyncOperation(obj))
                .Materialize() // Convert OnError to OnNext(Notification)
                .Do(notification => {
                    if (notification.Kind == NotificationKind.OnError) {
                        stateSubject.OnNext(stateSubject.Value.WithInProgress(false));
                    }
                })
                .Finally(() => stateSubject.OnNext(stateSubject.Value.WithInProgress(false)))
                .Dematerialize()); // Convert back to regular notifications
    }
}