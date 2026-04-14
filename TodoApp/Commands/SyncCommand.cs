using System;
using System.Linq;
using TodoApp.Exceptions;
using TodoApp.Services;

namespace TodoApp.Commands
{
    public class SyncCommand : ICommand
    {
        private readonly bool _pull;
        private readonly bool _push;

        public SyncCommand(bool pull, bool push)
        {
            _pull = pull;
            _push = push;
        }

        public void Execute()
        {
            if (AppInfo.CurrentProfile == null)
            {
                throw new AuthenticationException("Пользователь не авторизован");
            }

            if (!_pull && !_push)
            {
                throw new InvalidArgumentException("Используйте: sync --pull или sync --push");
            }

            if (_pull && _push)
            {
                throw new InvalidArgumentException("Нельзя использовать --pull и --push одновременно");
            }

            if (_pull)
            {
                PullData();
            }
            else if (_push)
            {
                PushData();
            }
        }

        private void PullData()
        {
            try
            {
                Console.WriteLine("Синхронизация с сервером (PULL)...");
                
                // Для синхронизации нужно реализовать API клиент
                // Пока выводим сообщение
                Console.WriteLine("Функция синхронизации временно отключена при работе с БД");
                Console.WriteLine("Синхронизация завершена");
            }
            catch (Exception ex) when (ex.Message.Contains("недоступен"))
            {
                Console.WriteLine("Ошибка: сервер недоступен");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка синхронизации: {ex.Message}");
            }
        }

        private void PushData()
        {
            try
            {
                Console.WriteLine("Синхронизация с сервером (PUSH)...");
                
                // Для синхронизации нужно реализовать API клиент
                // Пока выводим сообщение
                Console.WriteLine("Функция синхронизации временно отключена при работе с БД");
                Console.WriteLine("Синхронизация завершена");
            }
            catch (Exception ex) when (ex.Message.Contains("недоступен"))
            {
                Console.WriteLine("Ошибка: сервер недоступен");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка синхронизации: {ex.Message}");
            }
        }
    }
}