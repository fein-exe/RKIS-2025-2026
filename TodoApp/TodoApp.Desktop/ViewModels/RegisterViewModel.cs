using System.Windows.Input;
using TodoApp.Data;
using TodoApp.Desktop.Services;
using TodoApp.Models;

namespace TodoApp.Desktop.ViewModels;

public class RegisterViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly ProfileRepository _profileRepository;
    private string _login = string.Empty;
    private string _password = string.Empty;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _birthYear = string.Empty;

    public RegisterViewModel(MainViewModel mainViewModel, ProfileRepository profileRepository)
    {
        _mainViewModel = mainViewModel;
        _profileRepository = profileRepository;
        RegisterCommand = new RelayCommand(_ => Register());
        ShowLoginCommand = new RelayCommand(_ => _mainViewModel.ShowLogin());
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

    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    public string LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
    }

    public string BirthYear
    {
        get => _birthYear;
        set => SetProperty(ref _birthYear, value);
    }

    public ICommand RegisterCommand { get; }

    public ICommand ShowLoginCommand { get; }

    private void Register()
    {
        ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "\u0412\u0432\u0435\u0434\u0438\u0442\u0435 \u043B\u043E\u0433\u0438\u043D \u0438 \u043F\u0430\u0440\u043E\u043B\u044C.";
            return;
        }

        if (!int.TryParse(BirthYear, out var birthYear) || birthYear < 1900 || birthYear > DateTime.Now.Year)
        {
            ErrorMessage = "\u0412\u0432\u0435\u0434\u0438\u0442\u0435 \u043A\u043E\u0440\u0440\u0435\u043A\u0442\u043D\u044B\u0439 \u0433\u043E\u0434 \u0440\u043E\u0436\u0434\u0435\u043D\u0438\u044F.";
            return;
        }

        if (_profileRepository.LoginExists(Login.Trim()))
        {
            ErrorMessage = "\u0422\u0430\u043A\u043E\u0439 \u043B\u043E\u0433\u0438\u043D \u0443\u0436\u0435 \u0437\u0430\u043D\u044F\u0442.";
            return;
        }

        var profile = new Profile(Login.Trim(), Password, FirstName.Trim(), LastName.Trim(), birthYear);
        _profileRepository.Add(profile);
        _mainViewModel.SignIn(profile);
    }
}
