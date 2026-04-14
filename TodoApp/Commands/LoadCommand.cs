using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.Exceptions;

namespace TodoApp.Commands
{
    public class LoadCommand : ICommand
    {
        private readonly int _downloadsCount;
        private readonly int _fileSize;
        private static readonly object _consoleLock = new object();
        private string[] _progressLines;

        public LoadCommand(int downloadsCount, int fileSize)
        {
            _downloadsCount = downloadsCount;
            _fileSize = fileSize;
        }

        public void Execute()
        {
            RunAsync().Wait();
        }

        private async Task RunAsync()
        {
            Console.WriteLine($"\nЗапуск {_downloadsCount} параллельных загрузок...\n");

            _progressLines = new string[_downloadsCount];

            for (int i = 0; i < _downloadsCount; i++)
            {
                _progressLines[i] = $"Загрузка {i + 1}: [--------------------] 0%";
                Console.WriteLine(_progressLines[i]);
            }

            var tasks = new List<Task>();

            for (int i = 0; i < _downloadsCount; i++)
            {
                int index = i;
                tasks.Add(DownloadAsync(index));
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("\n\nВсе загрузки завершены.");
        }

        private async Task DownloadAsync(int index)
        {
            var random = new Random();
            
            for (int progress = 0; progress <= _fileSize; progress++)
            {
                int percent = (progress * 100) / _fileSize;
                
                string bar = GetProgressBar(percent);
                string downloadBar = $"Загрузка {index + 1}: [{bar}] {percent}%";
                
                lock (_consoleLock)
                {
                    _progressLines[index] = downloadBar;
                    Console.Clear();
                    Console.WriteLine($"\nЗапуск {_downloadsCount} параллельных загрузок...\n");
                    for (int i = 0; i < _downloadsCount; i++)
                    {
                        Console.WriteLine(_progressLines[i]);
                    }
                }
                
                await Task.Delay(random.Next(10, 50));
            }
        }

        private string GetProgressBar(int percent)
        {
            int filledCount = percent / 5;
            int emptyCount = 20 - filledCount;
            
            string filled = new string('#', filledCount);
            string empty = new string('-', emptyCount);
            
            return $"{filled}{empty}";
        }
    }
}