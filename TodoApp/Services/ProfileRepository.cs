using System;
using System.Collections.Generic;
using System.Linq;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services
{
    public class ProfileRepository
    {
        public List<Profile> GetAll()
        {
            using var context = new AppDbContext();
            return context.Profiles.ToList();
        }

        public Profile? GetById(Guid id)
        {
            using var context = new AppDbContext();
            return context.Profiles.Find(id);
        }

        public Profile? GetByLogin(string login)
        {
            using var context = new AppDbContext();
            return context.Profiles.FirstOrDefault(p => p.Login == login);
        }

        public Profile? GetByLoginAndPassword(string login, string password)
        {
            using var context = new AppDbContext();
            return context.Profiles.FirstOrDefault(p => p.Login == login && p.Password == password);
        }

        public void Add(Profile profile)
        {
            using var context = new AppDbContext();
            context.Profiles.Add(profile);
            context.SaveChanges();
        }

        public void Update(Profile profile)
        {
            using var context = new AppDbContext();
            context.Profiles.Update(profile);
            context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            using var context = new AppDbContext();
            var profile = context.Profiles.Find(id);
            if (profile != null)
            {
                context.Profiles.Remove(profile);
                context.SaveChanges();
            }
        }

        public bool LoginExists(string login)
        {
            using var context = new AppDbContext();
            return context.Profiles.Any(p => p.Login == login);
        }
    }
}