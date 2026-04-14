using System;
using TodoApp.Commands;
using TodoApp.Exceptions;
using TodoApp.Services;

namespace TodoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать в TodoApp!");
            Console.WriteLine("Используется база данных SQLite\n");

            while (AppInfo.CurrentProfile == null)
            {
                Console.WriteLine("1. Вход");
                Console.WriteLine("2. Регистрация");
                Console.WriteLine("3. Выход");
                Console.Write("\nВыберите действие: ");
                
                string choice = Console.ReadLine();
                
                try
                {
                    if (choice == "1")
                    {
                        Console.Write("Логин: ");
                        string login = Console.ReadLine();
                        Console.Write("Пароль: ");
                        string password = Console.ReadLine();
                        
                        var command = new LoginCommand(login, password);
                        command.Execute();
                    }
                    else if (choice == "2")
                    {
                        Console.Write("Логин: ");
                        string login = Console.ReadLine();
                        Console.Write("Пароль: ");
                        string password = Console.ReadLine();
                        Console.Write("Имя: ");
                        string firstName = Console.ReadLine();
                        Console.Write("Фамилия: ");
                        string lastName = Console.ReadLine();
                        Console.Write("Год рождения: ");
                        
                        if (int.TryParse(Console.ReadLine(), out int birthYear))
                        {
                            var command = new RegisterCommand(login, password, firstName, lastName, birthYear);
                            command.Execute();
                        }
                        else
                        {
                            Console.WriteLine("Неверный год рождения");
                        }
                    }
                    else if (choice == "3")
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Неверный выбор");
                    }
                }
                catch (AuthenticationException ex)
                {
                    Console.WriteLine($"Ошибка авторизации: {ex.Message}");
                }
                catch (DuplicateLoginException ex)
                {
                    Console.WriteLine($"Ошибка регистрации: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
                
                Console.WriteLine();
            }
            
            Console.WriteLine($"\nВведите 'help' для списка команд или 'exit' для выхода.\n");

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (input.ToLower() == "exit")
                    break;

                try
                {
                    var command = CommandParser.Parse(input);
                    command?.Execute();
                }
                catch (TaskNotFoundException ex)
                {
                    Console.WriteLine($"Ошибка задачи: {ex.Message}");
                }
                catch (AuthenticationException ex)
                {
                    Console.WriteLine($"Ошибка авторизации: {ex.Message}");
                }
                catch (InvalidCommandException ex)
                {
                    Console.WriteLine($"Ошибка команды: {ex.Message}");
                }
                catch (InvalidArgumentException ex)
                {
                    Console.WriteLine($"Ошибка аргументов: {ex.Message}");
                }
                catch (ProfileNotFoundException ex)
                {
                    Console.WriteLine($"Ошибка профиля: {ex.Message}");
                }
                catch (DuplicateLoginException ex)
                {
                    Console.WriteLine($"Ошибка регистрации: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Неожиданная ошибка: {ex.Message}");
                }
            }
        }
    }
}