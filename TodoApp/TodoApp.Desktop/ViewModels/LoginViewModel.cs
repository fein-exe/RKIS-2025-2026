using System.Windows.Input;
using TodoApp.Data;
using TodoApp.Desktop.Services;

namespace TodoApp.Desktop.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly ProfileRepository _profileRepository;
    private string _login = string.Empty;
    private string _password = string.Empty;

    public LoginViewModel(MainViewModel mainViewModel, ProfileRepository profileRepository)
    {
        _mainViewModel = mainViewModel;
        _profileRepository = profileRepository;
        LoginCommand = new RelayCommand(_ => SignIn());
        ShowRegisterCommand = new RelayCommand(_ => _mainViewModel.ShowRegister());
    }

    public string Login
    {
        get => _login;
        set => SetProperty(ref _login, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public ICommand LoginCommand { get; }

    public ICommand ShowRegisterCommand { get; }

    private void SignIn()
    {
        ErrorMessage = string.Empty;
        var profile = _profileRepository.GetByLoginAndPassword(Login.Trim(), Password);
        if (profile is null)
        {
            ErrorMessage = "\u041D\u0435\u0432\u0435\u0440\u043D\u044B\u0439 \u043B\u043E\u0433\u0438\u043D \u0438\u043B\u0438 \u043F\u0430\u0440\u043E\u043B\u044C.";
            return;
        }

        _mainViewModel.SignIn(profile);
    }
}
