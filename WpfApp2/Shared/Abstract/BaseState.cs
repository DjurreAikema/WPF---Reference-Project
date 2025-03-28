namespace WpfApp2.Shared.Abstract;

public abstract record BaseState<T> where T : BaseState<T>
{
    public bool Loading { get; init; }
    public bool InProgress { get; init; }

    // Returns concrete type T instead of BaseState
    public abstract T WithInProgress(bool inProgress);
}