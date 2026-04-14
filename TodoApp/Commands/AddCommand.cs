using System;
using System.Collections.Generic;
using TodoApp.Exceptions;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Commands
{
    public class AddCommand : IUndoableCommand
    {
        private string _text;
        private bool _isMultiline;
        private TodoItem? _addedItem;

        public AddCommand(string text, bool isMultiline)
        {
            _text = text;
            _isMultiline = isMultiline;
        }

        public void Execute()
        {
            if (AppInfo.CurrentProfile == null)
            {
                throw new AuthenticationException("Пользователь не авторизован");
            }

            if (_isMultiline)
            {
                _text = ReadMultilineInput();
            }

            if (string.IsNullOrWhiteSpace(_text))
            {
                throw new InvalidArgumentException("Текст задачи не может быть пустым");
            }

            _addedItem = new TodoItem(_text);
            AppInfo.TodoRepo.Add(_addedItem, AppInfo.CurrentProfile.Id);
            
            // Добавляем команду в стек отмены
            AppInfo.UndoStack.Push(this);
            AppInfo.RedoStack.Clear();
            
            Console.WriteLine($"Задача добавлена: {_text}");
        }

        public void Unexecute()
        {
            if (_addedItem == null) return;
            AppInfo.TodoRepo.Delete(_addedItem.Id, AppInfo.CurrentProfile.Id);
            Console.WriteLine("Отменено добавление задачи");
        }

        private string ReadMultilineInput()
        {
            var lines = new List<string>();
            Console.WriteLine("Введите строки задачи (завершите вводом '!end'):");

            while (true)
            {
                Console.Write("> ");
                string? line = Console.ReadLine();
                if (line == "!end")
                    break;
                if (!string.IsNullOrEmpty(line))
                    lines.Add(line);
            }

            return string.Join("\n", lines);
        }
    }
}