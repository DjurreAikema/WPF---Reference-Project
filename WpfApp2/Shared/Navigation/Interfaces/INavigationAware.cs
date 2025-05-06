namespace WpfApp2.Shared.Navigation.Interfaces;

public interface INavigationAware
{
    void OnNavigatedTo();
    void OnNavigatedFrom(bool isInStack);
}