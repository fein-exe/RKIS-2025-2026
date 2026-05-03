using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Data;

public class ProfileRepository
{
    private readonly AppDbContext _context;

    public ProfileRepository()
        : this(new AppDbContext())
    {
    }

    public ProfileRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<Profile> GetAll()
    {
        return _context.Profiles.AsNoTracking().OrderBy(profile => profile.Login).ToList();
    }

    public Profile? GetById(Guid id)
    {
        return _context.Profiles.Find(id);
    }

    public Profile? GetByLogin(string login)
    {
        return _context.Profiles.FirstOrDefault(profile => profile.Login == login);
    }

    public Profile? GetByLoginAndPassword(string login, string password)
    {
        return _context.Profiles.FirstOrDefault(profile => profile.Login == login && profile.Password == password);
    }

    public void Add(Profile profile)
    {
        _context.Profiles.Add(profile);
        _context.SaveChanges();
    }

    public void Update(Profile profile)
    {
        _context.Profiles.Update(profile);
        _context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var profile = GetById(id);
        if (profile is null)
        {
            return;
        }

        _context.Profiles.Remove(profile);
        _context.SaveChanges();
    }

    public bool LoginExists(string login)
    {
        return _context.Profiles.Any(profile => profile.Login == login);
    }
}
