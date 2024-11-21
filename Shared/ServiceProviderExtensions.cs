using Microsoft.Extensions.DependencyInjection;

namespace WpfApp1.Shared;

public static class ServiceProviderExtensions
{
    public static T Create<T>(this IServiceProvider serviceProvider) where T : class
    {
        return ActivatorUtilities.CreateInstance<T>(serviceProvider);
    }
}