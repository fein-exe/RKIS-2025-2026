using System;
using System.Collections.Generic;
using System.Linq;
using TodoApp.Exceptions;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Commands
{
    public class SearchCommand : ICommand
    {
        private readonly Dictionary<string, string> _parameters;

        public SearchCommand(Dictionary<string, string> parameters)
        {
            _parameters = parameters;
        }

        public void Execute()
        {
            var todos = AppInfo.GetCurrentTodoList();
            if (todos == null)
            {
                throw new AuthenticationException("Пользователь не авторизован");
            }

            if (todos.Count == 0)
            {
                Console.WriteLine("Ничего не найдено");
                return;
            }

            var query = todos.GetAll().AsEnumerable();

            var validFlags = new[] { "contains", "starts-with", "ends-with", "from", "to", "status", "sort", "desc", "top" };
            
            foreach (var key in _parameters.Keys)
            {
                if (!validFlags.Contains(key))
                {
                    throw new InvalidArgumentException($"Неизвестный флаг: --{key}");
                }
            }

            if (_parameters.ContainsKey("contains"))
            {
                string text = _parameters["contains"].ToLower();
                query = query.Where(t => t.Text.ToLower().Contains(text));
            }
            
            if (_parameters.ContainsKey("starts-with"))
            {
                string text = _parameters["starts-with"].ToLower();
                query = query.Where(t => t.Text.ToLower().StartsWith(text));
            }
            
            if (_parameters.ContainsKey("ends-with"))
            {
                string text = _parameters["ends-with"].ToLower();
                query = query.Where(t => t.Text.ToLower().EndsWith(text));
            }

            if (_parameters.ContainsKey("from"))
            {
                if (!DateTime.TryParse(_parameters["from"], out DateTime fromDate))
                {
                    throw new InvalidArgumentException($"Неверный формат даты '{_parameters["from"]}'. Используйте yyyy-MM-dd");
                }
                query = query.Where(t => t.LastUpdate.Date >= fromDate.Date);
            }
            
            if (_parameters.ContainsKey("to"))
            {
                if (!DateTime.TryParse(_parameters["to"], out DateTime toDate))
                {
                    throw new InvalidArgumentException($"Неверный формат даты '{_parameters["to"]}'. Используйте yyyy-MM-dd");
                }
                query = query.Where(t => t.LastUpdate.Date <= toDate.Date);
            }

            if (_parameters.ContainsKey("status"))
            {
                string status = _parameters["status"];
                query = query.Where(t => t.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            bool isDescending = _parameters.ContainsKey("desc");
            string sortBy = _parameters.ContainsKey("sort") ? _parameters["sort"].ToLower() : "";
            
            if (sortBy == "text")
            {
                query = isDescending ? query.OrderByDescending(t => t.Text) : query.OrderBy(t => t.Text);
            }
            else if (sortBy == "date")
            {
                query = isDescending ? query.OrderByDescending(t => t.LastUpdate) : query.OrderBy(t => t.LastUpdate);
            }
            else if (!string.IsNullOrEmpty(sortBy))
            {
                throw new InvalidArgumentException($"Неизвестное поле сортировки '{sortBy}'. Используйте 'text' или 'date'");
            }

            if (_parameters.ContainsKey("top"))
            {
                if (!int.TryParse(_parameters["top"], out int top) || top <= 0)
                {
                    throw new InvalidArgumentException($"Параметр top должен быть положительным числом. Получено: '{_parameters["top"]}'");
                }
                query = query.Take(top);
            }

            var results = query.ToList();

            if (!results.Any())
            {
                Console.WriteLine("Ничего не найдено");
                return;
            }

            DisplayResults(results);
        }

        private void DisplayResults(List<TodoItem> todos)
        {
            Console.WriteLine("\n╔══════╦═════════════════════════════════════╦════════════════╦══════════════════════╗");
            Console.WriteLine("║ INDEX ║               ЗАДАЧА                ║     СТАТУС     ║     ПОСЛЕДНЕЕ        ║");
            Console.WriteLine("║       ║                                     ║                ║     ИЗМЕНЕНИЕ        ║");
            Console.WriteLine("╠══════╬═════════════════════════════════════╬════════════════╬══════════════════════╣");

            for (int i = 0; i < todos.Count; i++)
            {
                var todo = todos[i];
                string shortText = todo.Text.Length > 30 ? todo.Text.Substring(0, 27) + "..." : todo.Text;
                shortText = shortText.Replace("\n", " ");
                
                Console.WriteLine("║ {0,-4} ║ {1,-31} ║ {2,-14} ║ {3,-18} ║", 
                    i + 1, 
                    shortText, 
                    todo.Status, 
                    todo.LastUpdate.ToString("yyyy-MM-dd HH:mm"));
                
                if (i < todos.Count - 1)
                {
                    Console.WriteLine("╠══════╬═════════════════════════════════════╬════════════════╬══════════════════════╣");
                }
            }
            
            Console.WriteLine("╚══════╩═════════════════════════════════════╩════════════════╩══════════════════════╝\n");
        }
    }
}