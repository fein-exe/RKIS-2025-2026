using System;
using TodoApp.Exceptions;
using TodoApp.Models;
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
            var todos = AppInfo.GetCurrentTodoList();
            if (todos == null)
            {
                throw new AuthenticationException("Пользователь не авторизован");
            }

            var item = todos[_index];

            if (item == null)
            {
                throw new TaskNotFoundException($"Задача с индексом {_index} не найдена");
            }

            Console.WriteLine(item.GetFullInfo());
        }
    }
}