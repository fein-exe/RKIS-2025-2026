using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class Profile
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Login { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Range(1900, 2026)]
        public int BirthYear { get; set; }
        
        public virtual ICollection<TodoItem> Todos { get; set; } = new List<TodoItem>();

        public Profile() { }

        public Profile(string login, string password, string firstName, string lastName, int birthYear)
        {
            Id = Guid.NewGuid();
            Login = login;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            BirthYear = birthYear;
        }

        public string GetInfo()
        {
            int age = DateTime.Now.Year - BirthYear;
            return $"{FirstName} {LastName}, {age} лет";
        }
    }
}