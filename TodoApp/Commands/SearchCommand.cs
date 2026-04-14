using System;
using System.Collections.Generic;
using System.Linq;
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
            if (todos == null || todos.Count == 0)
            {
                Console.WriteLine("Ничего не найдено");
                return;
            }

            var query = todos.GetAll().AsEnumerable();

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
                if (DateTime.TryParse(_parameters["from"], out DateTime fromDate))
                {
                    query = query.Where(t => t.LastUpdate.Date >= fromDate.Date);
                }
                else
                {
                    Console.WriteLine($"Ошибка: неверный формат даты '{_parameters["from"]}'. Используйте yyyy-MM-dd");
                    return;
                }
            }
            
            if (_parameters.ContainsKey("to"))
            {
                if (DateTime.TryParse(_parameters["to"], out DateTime toDate))
                {
                    query = query.Where(t => t.LastUpdate.Date <= toDate.Date);
                }
                else
                {
                    Console.WriteLine($"Ошибка: неверный формат даты '{_parameters["to"]}'. Используйте yyyy-MM-dd");
                    return;
                }
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

            if (_parameters.ContainsKey("top"))
            {
                if (int.TryParse(_parameters["top"], out int top) && top > 0)
                {
                    query = query.Take(top);
                }
                else
                {
                    Console.WriteLine($"Ошибка: параметр top должен быть положительным числом. Получено: '{_parameters["top"]}'");
                    return;
                }
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