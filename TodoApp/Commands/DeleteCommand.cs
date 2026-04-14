using System;
using TodoApp.Exceptions;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Commands
{
    public class DeleteCommand : IUndoableCommand
    {
        private int _index;
        private TodoItem? _deletedItem;

        public DeleteCommand(int index)
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

            _deletedItem = todos[_index - 1];
            AppInfo.TodoRepo.Delete(_deletedItem.Id, AppInfo.CurrentProfile.Id);
            Console.WriteLine($"Задача удалена: {_deletedItem.Text}");
        }

        public void Unexecute()
        {
            if (_deletedItem == null) return;
            AppInfo.TodoRepo.Add(_deletedItem, AppInfo.CurrentProfile.Id);
            Console.WriteLine("Отменено удаление задачи");
        }
    }
}