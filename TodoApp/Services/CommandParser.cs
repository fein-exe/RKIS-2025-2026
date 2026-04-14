#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TodoApp.Commands;
using TodoApp.Exceptions;
using TodoApp.Models;

namespace TodoApp.Services
{
    public static class CommandParser
    {
        private static Dictionary<string, Func<string[], ICommand>> _commandHandlers;

        static CommandParser()
        {
            InitializeHandlers();
        }

        private static void InitializeHandlers()
        {
            _commandHandlers = new Dictionary<string, Func<string[], ICommand>>
            {
                ["help"] = args => new HelpCommand(),
                ["profile"] = args => ParseProfileCommand(args),
                ["add"] = args => ParseAddCommand(args),
                ["view"] = args => ParseViewCommand(args),
                ["read"] = args => ParseReadCommand(args),
                ["status"] = args => ParseStatusCommand(args),
                ["update"] = args => ParseUpdateCommand(args),
                ["delete"] = args => ParseDeleteCommand(args),
                ["undo"] = args => new UndoCommand(),
                ["redo"] = args => new RedoCommand(),
                ["search"] = args => ParseSearchCommand(args),
                ["login"] = args => ParseLoginCommand(args),
                ["register"] = args => ParseRegisterCommand(args),
                ["load"] = args => ParseLoadCommand(args),
            };
        }

        public static ICommand Parse(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString))
            {
                throw new InvalidCommandException("Команда не может быть пустой");
            }

            var parts = SplitCommand(inputString);
            if (parts.Length == 0)
            {
                throw new InvalidCommandException("Не удалось разобрать команду");
            }

            string command = parts[0].ToLower();
            var args = parts.Skip(1).ToArray();

            if (_commandHandlers.ContainsKey(command))
            {
                try
                {
                    return _commandHandlers[command](args);
                }
                catch (Exception ex) when (ex is InvalidArgumentException || ex is TaskNotFoundException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new InvalidCommandException($"Ошибка при выполнении команды '{command}': {ex.Message}");
                }
            }

            throw new InvalidCommandException($"Неизвестная команда: '{command}'. Введите 'help' для справки.");
        }

        private static ICommand ParseProfileCommand(string[] args)
        {
            bool logout = args.Any(a => a == "-o" || a == "--out");
            return new ProfileCommand(logout);
        }

        private static ICommand ParseAddCommand(string[] args)
        {
            bool isMultiline = args.Any(a => a == "-m" || a == "--multiline");

            if (isMultiline)
            {
                return new AddCommand("", true);
            }

            string text = string.Join(" ", args);
            text = text.Trim('"');

            return new AddCommand(text, false);
        }

        private static ICommand ParseViewCommand(string[] args)
        {
            bool showIndex = args.Any(a => a == "-i" || a == "--index");
            bool showStatus = args.Any(a => a == "-s" || a == "--status");
            bool showDate = args.Any(a => a == "-d" || a == "--update-date");
            bool showAll = args.Any(a => a == "-a" || a == "--all");

            if (showAll)
                return new ViewCommand(true, true, true);

            return new ViewCommand(showIndex, showStatus, showDate);
        }

        private static ICommand ParseReadCommand(string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidArgumentException("Используйте: read <индекс>");
            }

            if (!int.TryParse(args[0], out int index))
            {
                throw new InvalidArgumentException("Индекс должен быть числом");
            }

            return new ReadCommand(index);
        }

        private static ICommand ParseStatusCommand(string[] args)
        {
            if (args.Length < 2)
            {
                throw new InvalidArgumentException("Используйте: status <индекс> <статус>");
            }

            if (!int.TryParse(args[0], out int index))
            {
                throw new InvalidArgumentException("Индекс должен быть числом");
            }

            string statusStr = args[1].ToLower();
            if (Enum.TryParse<TodoStatus>(statusStr, ignoreCase: true, out var status))
            {
                return new StatusCommand(index, status);
            }

            throw new InvalidArgumentException("Неизвестный статус. Доступные: NotStarted, InProgress, Completed, Postponed, Failed");
        }

        private static ICommand ParseUpdateCommand(string[] args)
        {
            if (args.Length < 2)
            {
                throw new InvalidArgumentException("Используйте: update <индекс> \"новый текст\"");
            }

            if (!int.TryParse(args[0], out int index))
            {
                throw new InvalidArgumentException("Индекс должен быть числом");
            }

            string newText = string.Join(" ", args.Skip(1)).Trim('"');
            return new UpdateCommand(index, newText);
        }

        private static ICommand ParseDeleteCommand(string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidArgumentException("Используйте: delete <индекс>");
            }

            if (!int.TryParse(args[0], out int index))
            {
                throw new InvalidArgumentException("Индекс должен быть числом");
            }

            return new DeleteCommand(index);
        }

        private static ICommand ParseSearchCommand(string[] args)
        {
            var parameters = new Dictionary<string, string>();
            
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    string key = args[i].Substring(2);
                    
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                    {
                        parameters[key] = args[i + 1];
                        i++;
                    }
                    else
                    {
                        parameters[key] = "true";
                    }
                }
                else
                {
                    throw new InvalidArgumentException($"Неизвестный параметр: {args[i]}. Используйте флаги --");
                }
            }
            
            return new SearchCommand(parameters);
        }

        private static ICommand ParseLoginCommand(string[] args)
        {
            if (args.Length < 2)
            {
                throw new InvalidArgumentException("Используйте: login <логин> <пароль>");
            }

            return new LoginCommand(args[0], args[1]);
        }

        private static ICommand ParseRegisterCommand(string[] args)
        {
            if (args.Length < 5)
            {
                throw new InvalidArgumentException("Используйте: register <логин> <пароль> <имя> <фамилия> <год рождения>");
            }

            if (!int.TryParse(args[4], out int birthYear))
            {
                throw new InvalidArgumentException("Год рождения должен быть числом");
            }

            return new RegisterCommand(args[0], args[1], args[2], args[3], birthYear);
        }

        private static ICommand ParseLoadCommand(string[] args)
        {
            if (args.Length < 2)
            {
                throw new InvalidArgumentException("Используйте: load <количество_загрузок> <размер_файла>");
            }

            if (!int.TryParse(args[0], out int downloadsCount))
            {
                throw new InvalidArgumentException("Количество загрузок должно быть числом");
            }

            if (!int.TryParse(args[1], out int fileSize))
            {
                throw new InvalidArgumentException("Размер файла должен быть числом");
            }

            if (downloadsCount <= 0)
            {
                throw new InvalidArgumentException("Количество загрузок должно быть больше 0");
            }

            if (fileSize <= 0)
            {
                throw new InvalidArgumentException("Размер файла должен быть больше 0");
            }

            return new LoadCommand(downloadsCount, fileSize);
        }

        private static string[] SplitCommand(string input)
        {
            var result = new List<string>();
            var regex = new Regex(@"[^\s""]+|""([^""]*)""");
            var matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                if (match.Groups[1].Success)
                {
                    result.Add(match.Groups[1].Value);
                }
                else
                {
                    result.Add(match.Value);
                }
            }

            return result.ToArray();
        }
    }
}