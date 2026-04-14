using System;
using TodoApp.Exceptions;
using TodoApp.Services;

namespace TodoApp.Commands
{
    public class LoginCommand : ICommand
    {
        private readonly string _login;
        private readonly string _password;

        public LoginCommand(string login, string password)
        {
            _login = login;
            _password = password;
        }

        public void Execute()
        {
            if (string.IsNullOrWhiteSpace(_login) || string.IsNullOrWhiteSpace(_password))
            {
                throw new InvalidArgumentException("Логин и пароль не могут быть пустыми");
            }

            var profile = AppInfo.ProfileRepo.GetByLoginAndPassword(_login, _password);
            
            if (profile != null)
            {
                AppInfo.CurrentProfile = profile;
                Console.WriteLine($"Добро пожаловать, {profile.GetInfo()}");
            }
            else
            {
                throw new AuthenticationException("Неверный логин или пароль");
            }
        }
    }
}