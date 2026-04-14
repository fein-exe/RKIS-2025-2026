using System;
using TodoApp.Exceptions;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Commands
{
    public class UpdateCommand : IUndoableCommand
    {
        private int _index;
        private string _newText;
        private string? _oldText;
        private TodoItem? _item;

        public UpdateCommand(int index, string newText)
        {
            _index = index;
            _newText = newText;
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

            if (string.IsNullOrWhiteSpace(_newText))
            {
                throw new InvalidArgumentException("Текст задачи не может быть пустым");
            }

            _item = todos[_index - 1];
            _oldText = _item.Text;
            _item.UpdateText(_newText);
            AppInfo.TodoRepo.Update(_item);
            Console.WriteLine("Задача обновлена.");
        }

        public void Unexecute()
        {
            if (_item == null || _oldText == null) return;
            _item.UpdateText(_oldText);
            AppInfo.TodoRepo.Update(_item);
            Console.WriteLine("Отменено обновление задачи");
        }
    }
}