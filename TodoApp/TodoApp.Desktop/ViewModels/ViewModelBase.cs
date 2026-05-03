namespace TodoApp.Desktop.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    private string _errorMessage = string.Empty;

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }
}
