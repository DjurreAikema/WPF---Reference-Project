namespace WpfApp1.Shared.Interfaces;

public interface IBaseState
{
    public bool Loading { get; init; }
    public bool InProgress { get; init; }
}