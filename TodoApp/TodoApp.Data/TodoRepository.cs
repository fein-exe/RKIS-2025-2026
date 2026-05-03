using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Data;

public class TodoRepository
{
    private readonly AppDbContext _context;

    public TodoRepository()
        : this(new AppDbContext())
    {
    }

    public TodoRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<TodoItem> GetAll(Guid profileId)
    {
        return _context.Todos
            .AsNoTracking()
            .Where(todo => todo.ProfileId == profileId)
            .OrderByDescending(todo => todo.LastUpdate)
            .ToList();
    }

    public TodoItem? GetById(int id, Guid profileId)
    {
        return _context.Todos.FirstOrDefault(todo => todo.Id == id && todo.ProfileId == profileId);
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
        var item = GetById(id, profileId);
        if (item is null)
        {
            return;
        }

        _context.Todos.Remove(item);
        _context.SaveChanges();
    }

    public void SetStatus(int id, TodoStatus status, Guid profileId)
    {
        var item = GetById(id, profileId);
        if (item is null)
        {
            return;
        }

        item.SetStatus(status);
        _context.SaveChanges();
    }
}
