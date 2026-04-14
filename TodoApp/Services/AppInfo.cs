using System;
using System.Collections.Generic;
using TodoApp.Commands;
using TodoApp.Interfaces;
using TodoApp.Models;

namespace TodoApp.Services
{
    public static class AppInfo
    {
        public static List<Profile> Profiles { get; set; } = new();
        public static Profile? CurrentProfile { get; set; }
        public static Dictionary<Guid, TodoList> UserTodos { get; set; } = new();
        public static Stack<IUndoableCommand> UndoStack { get; set; } = new();
        public static Stack<IUndoableCommand> RedoStack { get; set; } = new();
        public static IDataStorage? DataStorage { get; set; }

        public static TodoList? GetCurrentTodoList()
        {
            if (CurrentProfile != null && UserTodos.ContainsKey(CurrentProfile.Id))
            {
                return UserTodos[CurrentProfile.Id];
            }
            return null;
        }

        public static void ClearUndoRedo()
        {
            UndoStack.Clear();
            RedoStack.Clear();
        }
    }
}