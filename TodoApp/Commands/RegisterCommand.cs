using System;
using TodoApp.Exceptions;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Commands
{
    public class RegisterCommand : ICommand
    {
        private readonly string _login;
        private readonly string _password;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly int _birthYear;

        public RegisterCommand(string login, string password, string firstName, string lastName, int birthYear)
        {
            _login = login;
            _password = password;
            _firstName = firstName;
            _lastName = lastName;
            _birthYear = birthYear;
        }

        public void Execute()
        {
            if (string.IsNullOrWhiteSpace(_login) || string.IsNullOrWhiteSpace(_password))
            {
                throw new InvalidArgumentException("Логин и пароль не могут быть пустыми");
            }

            if (_birthYear < 1900 || _birthYear > DateTime.Now.Year)
            {
                throw new InvalidArgumentException($"Год рождения должен быть от 1900 до {DateTime.Now.Year}");
            }

            if (AppInfo.ProfileRepo.LoginExists(_login))
            {
                throw new DuplicateLoginException($"Пользователь с логином '{_login}' уже существует");
            }

            var profile = new Profile(_login, _password, _firstName, _lastName, _birthYear);
            AppInfo.ProfileRepo.Add(profile);
            
            Console.WriteLine($"Пользователь {_login} успешно зарегистрирован");
        }
    }
}