using System;
using TodoApp.Exceptions;
using TodoApp.Services;

namespace TodoApp.Commands
{
    public class ReadCommand : ICommand
    {
        private int _index;

        public ReadCommand(int index)
        {
            _index = index;
        }

        public void Execute()
        {
            if (AppInfo.CurrentProfile == null)
            {
                throw new AuthenticationException("Пользователь не авторизован");
            }

            var todos = AppInfo.GetCurrentTodos();
            if (_index < 1 || _index > todos.Count)
            {
                throw new TaskNotFoundException($"Задача с индексом {_index} не найдена");
            }

            var item = todos[_index - 1];
            Console.WriteLine(item.GetFullInfo());
        }
    }
}