using System;
using System.Collections.Generic;
using System.Linq;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services
{
    public class TodoRepository
    {
        private readonly AppDbContext _context;

        public TodoRepository()
        {
            _context = new AppDbContext();
        }

        public TodoRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<TodoItem> GetAll(Guid profileId)
        {
            return _context.Todos.Where(t => t.ProfileId == profileId).ToList();
        }

        public TodoItem? GetById(int id, Guid profileId)
        {
            return _context.Todos.FirstOrDefault(t => t.Id == id && t.ProfileId == profileId);
        }

        public void Add(TodoItem item, Guid profileId)
        {
            item.ProfileId = profileId;
            _context.Todos.Add(item);
            _context.SaveChanges();
        }

        public void Update(TodoItem item)
        {
            _context.Todos.Update(item);
            _context.SaveChanges();
        }

        public void Delete(int id, Guid profileId)
        {
            var item = _context.Todos.FirstOrDefault(t => t.Id == id && t.ProfileId == profileId);
            if (item != null)
            {
                _context.Todos.Remove(item);
                _context.SaveChanges();
            }
        }

        public void SetStatus(int id, TodoStatus status, Guid profileId)
        {
            var item = _context.Todos.FirstOrDefault(t => t.Id == id && t.ProfileId == profileId);
            if (item != null)
            {
                item.SetStatus(status);
                _context.Todos.Update(item);
                _context.SaveChanges();
            }
        }
    }
}