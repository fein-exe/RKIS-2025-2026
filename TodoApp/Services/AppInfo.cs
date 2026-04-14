using System;
using System.Collections.Generic;
using TodoApp.Commands;
using TodoApp.Models;

namespace TodoApp.Services
{
    public static class AppInfo
    {
        public static Profile? CurrentProfile { get; set; }
        public static Stack<IUndoableCommand> UndoStack { get; set; } = new();
        public static Stack<IUndoableCommand> RedoStack { get; set; } = new();
        
        public static TodoRepository TodoRepo { get; set; } = new();
        public static ProfileRepository ProfileRepo { get; set; } = new();

        public static List<TodoItem> GetCurrentTodos()
        {
            if (CurrentProfile == null) return new List<TodoItem>();
            return TodoRepo.GetAll(CurrentProfile.Id);
        }

        public static void ClearUndoRedo()
        {
            UndoStack.Clear();
            RedoStack.Clear();
        }
    }
}