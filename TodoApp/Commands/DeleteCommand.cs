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
        private TodoList? _todos;

        public DeleteCommand(int index)
        {
            _index = index;
        }

        public void Execute()
        {
            _todos = AppInfo.GetCurrentTodoList();
            if (_todos == null)
            {
                throw new AuthenticationException("Пользователь не авторизован");
            }

            _deletedItem = _todos[_index];

            if (_deletedItem == null)
            {
                throw new TaskNotFoundException($"Задача с индексом {_index} не найдена");
            }

            _todos.Delete(_index);
            Console.WriteLine($"Задача удалена: {_deletedItem.Text}");
        }

        public void Unexecute()
        {
            _todos = AppInfo.GetCurrentTodoList();
            if (_todos == null || _deletedItem == null) return;

            _todos.Add(_deletedItem);
            Console.WriteLine("Отменено удаление задачи");
        }
    }
}