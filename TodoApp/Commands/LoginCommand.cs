using System;
using System.Linq;
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

            var profile = AppInfo.Profiles.FirstOrDefault(p => p.Login == _login && p.Password == _password);
            
            if (profile != null)
            {
                AppInfo.CurrentProfile = profile;
                
                if (!AppInfo.UserTodos.ContainsKey(profile.Id))
                {
                    try
                    {
                        var todos = AppInfo.DataStorage.LoadTodos(profile.Id);
                        var todoList = new TodoList();
                        foreach (var item in todos)
                        {
                            todoList.Add(item);
                        }
                        
                        todoList.OnTodoAdded += (item) => SaveTodos(profile.Id);
                        todoList.OnTodoDeleted += (item) => SaveTodos(profile.Id);
                        todoList.OnTodoUpdated += (item) => SaveTodos(profile.Id);
                        todoList.OnStatusChanged += (item) => SaveTodos(profile.Id);
                        
                        AppInfo.UserTodos[profile.Id] = todoList;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Ошибка загрузки задач: {ex.Message}", ex);
                    }
                }
                
                Console.WriteLine($"Добро пожаловать, {profile.GetInfo()}");
            }
            else
            {
                throw new AuthenticationException("Неверный логин или пароль");
            }
        }

        private void SaveTodos(Guid userId)
        {
            var todoList = AppInfo.UserTodos[userId];
            if (todoList != null)
            {
                AppInfo.DataStorage.SaveTodos(userId, todoList.GetAll());
            }
        }
    }
}