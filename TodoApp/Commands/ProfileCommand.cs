using System;
using TodoApp.Services;

namespace TodoApp.Commands
{
    public class ProfileCommand : ICommand
    {
        private bool _logout;

        public ProfileCommand(bool logout = false)
        {
            _logout = logout;
        }

        public void Execute()
        {
            if (_logout)
            {
                if (AppInfo.CurrentProfile == null)
                {
                    Console.WriteLine("Вы не авторизованы");
                    return;
                }
                
                if (AppInfo.CurrentProfile != null)
                {
                    var todoList = AppInfo.GetCurrentTodoList();
                    if (todoList != null)
                    {
                        AppInfo.DataStorage.SaveTodos(AppInfo.CurrentProfile.Id, todoList.GetAll());
                    }
                }
                
                AppInfo.CurrentProfile = null;
                AppInfo.ClearUndoRedo();
                Console.WriteLine("Вы вышли из профиля.");
            }
            else
            {
                var profile = AppInfo.CurrentProfile;
                if (profile != null)
                {
                    Console.WriteLine($"Текущий профиль: {profile.GetInfo()}");
                }
                else
                {
                    Console.WriteLine("Профиль не выбран.");
                }
            }
        }
    }
}