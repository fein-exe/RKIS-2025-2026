using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Desktop.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly AppDbContext _dbContext;
    private readonly ProfileRepository _profileRepository;
    private readonly TodoRepository _todoRepository;
    private ViewModelBase _currentViewModel;
    private Profile? _currentProfile;

    public MainViewModel()
    {
        _dbContext = new AppDbContext();
        _dbContext.Database.Migrate();
        _profileRepository = new ProfileRepository(_dbContext);
        _todoRepository = new TodoRepository(_dbContext);
        _currentViewModel = new LoginViewModel(this, _profileRepository);
    }

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

    public Profile? CurrentProfile
    {
        get => _currentProfile;
        private set => SetProperty(ref _currentProfile, value);
    }

    public void ShowLogin()
    {
        CurrentViewModel = new LoginViewModel(this, _profileRepository);
    }

    public void ShowRegister()
    {
        CurrentViewModel = new RegisterViewModel(this, _profileRepository);
    }

    public void SignIn(Profile profile)
    {
        CurrentProfile = profile;
        ShowTodoList();
    }

    public void SignOut()
    {
        CurrentProfile = null;
        ShowLogin();
    }

    public void ShowTodoList()
    {
        if (CurrentProfile is null)
        {
            ShowLogin();
            return;
        }

        CurrentViewModel = new TodoListViewModel(this, _todoRepository, CurrentProfile);
    }

    public void ShowAddTask()
    {
        if (CurrentProfile is not null)
        {
            CurrentViewModel = new AddTaskViewModel(this, _todoRepository, CurrentProfile);
        }
    }

    public void ShowEditTask(TodoItem item)
    {
        if (CurrentProfile is not null)
        {
            CurrentViewModel = new EditTaskViewModel(this, _todoRepository, CurrentProfile, item);
        }
    }
}
