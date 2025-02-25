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

    public static IObservable<T> NotifyOnError<T>(
        this IObservable<T> source,
        Subject<NotificationMessage> notifications,
        Func<Exception, string> errorMessageFactory,
        T? fallbackValue = default)
    {
        return source.Catch((Exception e) =>
        {
            var errorMessage = errorMessageFactory(e);
            notifications.OnNext(new NotificationMessage(errorMessage, false));
            return Observable.Return(fallbackValue)!;
        });
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