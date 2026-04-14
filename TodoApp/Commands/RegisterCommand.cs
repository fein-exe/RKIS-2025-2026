using System;
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
            var existing = FileManager.LoadAllProfiles().Find(p => p.Login == _login);
            if (existing != null)
            {
                Console.WriteLine("Пользователь с таким логином уже существует");
                return;
            }

            var profile = new Profile(_login, _password, _firstName, _lastName, _birthYear);
            FileManager.SaveProfile(profile);
            
            Console.WriteLine($"Пользователь {_login} успешно зарегистрирован");
        }
    }
}