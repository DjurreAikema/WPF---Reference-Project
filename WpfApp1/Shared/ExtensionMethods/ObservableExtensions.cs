using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp1.Shared.Classes;

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

    private static IObservable<T> NotifyOnSuccess<T>(
        this IObservable<T> source,
        Subject<NotificationMessage> notifications,
        string successMessage)
    {
        return source.Do(_ => notifications.OnNext(new NotificationMessage(successMessage, true)));
    }

    private static IObservable<T> NotifyOnError<T>(
        this IObservable<T> source,
        Subject<NotificationMessage> notifications,
        Func<Exception, string> errorMessageFactory,
        T? fallbackValue = default)
    {
        // The key fix is using Materialize/Dematerialize pattern to prevent
        // the error from terminating the sequence
        return source
            .Materialize()
            .Select(notification => {
                if (notification.Kind == NotificationKind.OnError)
                {
                    var errorMessage = errorMessageFactory(notification.Exception);
                    notifications.OnNext(new NotificationMessage(errorMessage, false));

                    // Return OnNext with fallback value instead of error
                    return Notification.CreateOnNext(fallbackValue);
                }
                return notification;
            })
            .Dematerialize();
    }

    public static IObservable<T> NotifyOnSuccessAndError<T>(
        this IObservable<T> source,
        Subject<NotificationMessage> notifications,
        string successMessage,
        Func<Exception, string> errorMessageFactory,
        T? fallbackValue = default)
    {
        return source
            .NotifyOnSuccess(notifications, successMessage)
            .NotifyOnError(notifications, errorMessageFactory, fallbackValue);
    }
}