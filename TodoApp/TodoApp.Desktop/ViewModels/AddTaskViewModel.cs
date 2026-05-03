using System.Collections.ObjectModel;
using System.Windows.Input;
using TodoApp.Data;
using TodoApp.Desktop.Services;
using TodoApp.Models;

namespace TodoApp.Desktop.ViewModels;

public class AddTaskViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly TodoRepository _todoRepository;
    private readonly Profile _profile;
    private string _text = string.Empty;
    private TodoStatus _status = TodoStatus.NotStarted;

    public AddTaskViewModel(MainViewModel mainViewModel, TodoRepository todoRepository, Profile profile)
    {
        _mainViewModel = mainViewModel;
        _todoRepository = todoRepository;
        _profile = profile;
        StatusOptions = new ObservableCollection<TodoStatus>(Enum.GetValues<TodoStatus>());
        SaveCommand = new RelayCommand(_ => Save());
        CancelCommand = new RelayCommand(_ => _mainViewModel.ShowTodoList());
    }

    public string Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }

    public TodoStatus Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public ObservableCollection<TodoStatus> StatusOptions { get; }

    public ICommand SaveCommand { get; }

    public ICommand CancelCommand { get; }

    private void Save()
    {
        ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(Text))
        {
            ErrorMessage = "\u0412\u0432\u0435\u0434\u0438\u0442\u0435 \u0442\u0435\u043A\u0441\u0442 \u0437\u0430\u0434\u0430\u0447\u0438.";
            return;
        }

        var item = new TodoItem(Text);
        item.SetStatus(Status);
        _todoRepository.Add(item, _profile.Id);
        _mainViewModel.ShowTodoList();
    }
}
