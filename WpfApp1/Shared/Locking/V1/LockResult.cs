namespace WpfApp1.Shared.Locking.V1;

/// <summary>
/// Result of a lock operation with status and details
/// </summary>
public class LockResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public LockState CurrentState { get; set; }
    public ILockable Entity { get; set; }

    public static LockResult Succeeded(ILockable entity, string message = "Lock operation succeeded")
    {
        return new LockResult
        {
            Success = true,
            Message = message,
            CurrentState = entity.LockState,
            Entity = entity
        };
    }

    public static LockResult Failed(ILockable entity, string message)
    {
        return new LockResult
        {
            Success = false,
            Message = message,
            CurrentState = entity.LockState,
            Entity = entity
        };
    }
}