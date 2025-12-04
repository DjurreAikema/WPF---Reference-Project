namespace WpfApp2.Shared.ExtensionMethods;

public static class ObjectCastingExtensions
{
    public static T? SafeCast<T>(this object? obj, T? fallbackValue = null) where T : class
    {
        switch (obj)
        {
            case null:
                Console.WriteLine($"[Warning] Attempted to cast null to {typeof(T).Name}");
                return fallbackValue;

            case T casted:
                return casted;

            default:
                Console.WriteLine($"[Error] Failed to cast {obj.GetType().Name} to {typeof(T).Name}");
                return fallbackValue;
        }
    }
}