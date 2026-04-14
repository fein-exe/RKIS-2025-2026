using System;
using System.Collections.Generic;
using System.Linq;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services
{
    public class ProfileRepository
    {
        private readonly AppDbContext _context;

        public ProfileRepository()
        {
            _context = new AppDbContext();
        }

        public ProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Profile> GetAll()
        {
            return _context.Profiles.ToList();
        }

        public Profile? GetById(Guid id)
        {
            return _context.Profiles.Find(id);
        }

        public Profile? GetByLogin(string login)
        {
            return _context.Profiles.FirstOrDefault(p => p.Login == login);
        }

        public Profile? GetByLoginAndPassword(string login, string password)
        {
            return _context.Profiles.FirstOrDefault(p => p.Login == login && p.Password == password);
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
            var profile = _context.Profiles.Find(id);
            if (profile != null)
            {
                _context.Profiles.Remove(profile);
                _context.SaveChanges();
            }
        }

        public bool LoginExists(string login)
        {
            return _context.Profiles.Any(p => p.Login == login);
        }
    }
}