namespace WpfApp2.Shared.Classes;

public class NotificationMessage(string text, bool isSuccess)
{
    public string Text { get; } = text;
    public bool IsSuccess { get; } = isSuccess;
}