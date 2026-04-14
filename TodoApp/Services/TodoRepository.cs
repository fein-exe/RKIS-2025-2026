using System;
using System.Collections.Generic;
using System.Linq;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services
{
    public class TodoRepository
    {
        public List<TodoItem> GetAll(Guid profileId)
        {
            using var context = new AppDbContext();
            return context.Todos.Where(t => t.ProfileId == profileId).ToList();
        }

        public TodoItem? GetById(int id, Guid profileId)
        {
            using var context = new AppDbContext();
            return context.Todos.FirstOrDefault(t => t.Id == id && t.ProfileId == profileId);
        }

        public void Add(TodoItem item, Guid profileId)
        {
            using var context = new AppDbContext();
            item.ProfileId = profileId;
            context.Todos.Add(item);
            context.SaveChanges();
        }

        public void Update(TodoItem item)
        {
            using var context = new AppDbContext();
            context.Todos.Update(item);
            context.SaveChanges();
        }

        public void Delete(int id, Guid profileId)
        {
            using var context = new AppDbContext();
            var item = context.Todos.FirstOrDefault(t => t.Id == id && t.ProfileId == profileId);
            if (item != null)
            {
                context.Todos.Remove(item);
                context.SaveChanges();
            }
        }

        public void SetStatus(int id, TodoStatus status, Guid profileId)
        {
            using var context = new AppDbContext();
            var item = context.Todos.FirstOrDefault(t => t.Id == id && t.ProfileId == profileId);
            if (item != null)
            {
                item.SetStatus(status);
                context.Todos.Update(item);
                context.SaveChanges();
            }
        }
    }
}