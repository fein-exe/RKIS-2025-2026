using System;
using TodoApp.Exceptions;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Commands
{
    public class StatusCommand : IUndoableCommand
    {
        private int _index;
        private TodoStatus _newStatus;
        private TodoStatus _oldStatus;
        private TodoItem? _item;

        public StatusCommand(int index, TodoStatus status)
        {
            _index = index;
            _newStatus = status;
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

            _item = todos[_index - 1];
            _oldStatus = _item.Status;
            _item.SetStatus(_newStatus);
            AppInfo.TodoRepo.Update(_item);
            Console.WriteLine($"Статус задачи изменён на: {_newStatus}");
        }

        public void Unexecute()
        {
            if (_item == null) return;
            _item.SetStatus(_oldStatus);
            AppInfo.TodoRepo.Update(_item);
            Console.WriteLine("Отменено изменение статуса");
        }
    }
}