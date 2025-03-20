using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp1.Shared.Abstract;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Shared.ExtensionMethods;

public static class ReactiveExtensions
{
    /// <summary>
    /// Executes an async operation with state tracking, error handling, and notifications
    /// </summary>
    public static IObservable<TResult?> ExecuteAsyncOperation<TSource, TResult, TState>(
        this IObservable<TSource> source,
        BehaviorSubject<TState> stateSubject,
        Func<TSource, Task<TResult>> asyncOperation,
        Subject<NotificationMessage> notifications,
        string successMessage,
        Func<Exception, string> errorMessageFactory,
        TResult? fallbackValue = default)
        where TState : BaseState<TState>
    {
        return source
            .Synchronize() // Ensure operations don't overlap
            .SelectMany(sourceItem =>
                Observable.FromAsync(async () =>
                {
                    // Set the state to in progress
                    stateSubject.OnNext(stateSubject.Value.WithInProgress(true));

                    try
                    {
                        // Execute the operation
                        var result = await asyncOperation(sourceItem);

                        // Show success notification
                        notifications.OnNext(new NotificationMessage(successMessage, true));

                        return result;
                    }
                    catch (Exception ex)
                    {
                        // Show error notification
                        var errorMessage = errorMessageFactory(ex);
                        notifications.OnNext(new NotificationMessage(errorMessage, false));

                        // Return fallback value instead of throwing an error
                        return fallbackValue;
                    }
                    finally
                    {
                        // Reset the in-progress state
                        stateSubject.OnNext(stateSubject.Value.WithInProgress(false));
                    }
                })
            );
    }
}