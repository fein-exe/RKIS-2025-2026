using System;
using TodoApp.Exceptions;
using TodoApp.Models;
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

            var profile = FileManager.LoadProfile(_login, _password);
            if (profile != null)
            {
                AppInfo.CurrentProfile = profile;
                
                if (!AppInfo.UserTodos.ContainsKey(profile.Id))
                {
                    string filePath = FileManager.GetTodoFilePath(profile.Id);
                    var todoList = FileManager.LoadTodos(filePath);
                    AppInfo.UserTodos[profile.Id] = todoList;
                    
                    todoList.OnTodoAdded += (item) => FileManager.SaveTodoList(item);
                    todoList.OnTodoDeleted += (item) => FileManager.SaveTodoList(item);
                    todoList.OnTodoUpdated += (item) => FileManager.SaveTodoList(item);
                    todoList.OnStatusChanged += (item) => FileManager.SaveTodoList(item);
                }
                
                Console.WriteLine($"Добро пожаловать, {profile.GetInfo()}");
            }
            else
            {
                throw new AuthenticationException("Неверный логин или пароль");
            }
        }
    }
}