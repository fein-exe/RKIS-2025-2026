using System.Collections.ObjectModel;
using System.Windows.Input;
using TodoApp.Data;
using TodoApp.Desktop.Services;
using TodoApp.Models;

namespace TodoApp.Desktop.ViewModels;

public class TodoListViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly TodoRepository _todoRepository;
    private readonly Profile _profile;
    private string _searchText = string.Empty;
    private StatusFilterOption _selectedFilter;
    private TodoItem? _selectedTodo;
    private TodoStatus _selectedStatus = TodoStatus.NotStarted;
    private bool _isUpdatingSelectedStatus;

    public TodoListViewModel(MainViewModel mainViewModel, TodoRepository todoRepository, Profile profile)
    {
        _mainViewModel = mainViewModel;
        _todoRepository = todoRepository;
        _profile = profile;
        FilterOptions = new ObservableCollection<StatusFilterOption>(
        [
            new StatusFilterOption("\u0412\u0441\u0435 \u0441\u0442\u0430\u0442\u0443\u0441\u044B", null),
            new StatusFilterOption("NotStarted", TodoStatus.NotStarted),
            new StatusFilterOption("InProgress", TodoStatus.InProgress),
            new StatusFilterOption("Completed", TodoStatus.Completed),
            new StatusFilterOption("Postponed", TodoStatus.Postponed),
            new StatusFilterOption("Failed", TodoStatus.Failed)
        ]);
        _selectedFilter = FilterOptions[0];
        StatusOptions = new ObservableCollection<TodoStatus>(Enum.GetValues<TodoStatus>());
        Todos = new ObservableCollection<TodoItem>();
        ShowAddTaskCommand = new RelayCommand(_ => _mainViewModel.ShowAddTask());
        EditTaskCommand = new RelayCommand(_ => EditSelectedTask(), _ => SelectedTodo is not null);
        DeleteTaskCommand = new RelayCommand(_ => DeleteSelectedTask(), _ => SelectedTodo is not null);
        LogoutCommand = new RelayCommand(_ => _mainViewModel.SignOut());
        RefreshCommand = new RelayCommand(_ => LoadTodos());
        LoadTodos();
    }

    public string ProfileName => _profile.GetInfo();

    public ObservableCollection<TodoItem> Todos { get; }

    public ObservableCollection<StatusFilterOption> FilterOptions { get; }

    public ObservableCollection<TodoStatus> StatusOptions { get; }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                LoadTodos();
            }
        }
    }

    public StatusFilterOption SelectedFilter
    {
        get => _selectedFilter;
        set
        {
            if (SetProperty(ref _selectedFilter, value))
            {
                LoadTodos();
            }
        }
    }

    public TodoItem? SelectedTodo
    {
        get => _selectedTodo;
        set
        {
            if (SetProperty(ref _selectedTodo, value) && value is not null)
            {
                _isUpdatingSelectedStatus = true;
                SelectedStatus = value.Status;
                _isUpdatingSelectedStatus = false;
            }
        }
    }

    public TodoStatus SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            if (SetProperty(ref _selectedStatus, value) && !_isUpdatingSelectedStatus)
            {
                SaveSelectedStatus();
            }
        }
    }

    public ICommand ShowAddTaskCommand { get; }

    public ICommand EditTaskCommand { get; }

    public ICommand DeleteTaskCommand { get; }

    public ICommand LogoutCommand { get; }

    public ICommand RefreshCommand { get; }

    private void LoadTodos()
    {
        ErrorMessage = string.Empty;
        var items = _todoRepository.GetAll(_profile.Id).AsEnumerable();
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            items = items.Where(todo => todo.Text.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        if (SelectedFilter.Status is not null)
        {
            items = items.Where(todo => todo.Status == SelectedFilter.Status);
        }

        Todos.Clear();
        foreach (var item in items)
        {
            Todos.Add(item);
        }
    }

    private void EditSelectedTask()
    {
        if (SelectedTodo is not null)
        {
            _mainViewModel.ShowEditTask(SelectedTodo);
        }
    }

    private void DeleteSelectedTask()
    {
        if (SelectedTodo is null)
        {
            return;
        }

        _todoRepository.Delete(SelectedTodo.Id, _profile.Id);
        SelectedTodo = null;
        LoadTodos();
    }

    private void SaveSelectedStatus()
    {
        if (SelectedTodo is null)
        {
            return;
        }

        try
        {
            _todoRepository.SetStatus(SelectedTodo.Id, SelectedStatus, _profile.Id);
            LoadTodos();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"\u041D\u0435 \u0443\u0434\u0430\u043B\u043E\u0441\u044C \u0438\u0437\u043C\u0435\u043D\u0438\u0442\u044C \u0441\u0442\u0430\u0442\u0443\u0441: {ex.Message}";
        }
    }
}
