using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApp.Models
{
    public class TodoItem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Text { get; private set; } = string.Empty;
        
        [Required]
        public TodoStatus Status { get; set; }
        
        [Required]
        public DateTime LastUpdate { get; set; }
        
        [Required]
        public Guid ProfileId { get; set; }
        
        [ForeignKey("ProfileId")]
        public virtual Profile Profile { get; set; } = null!;

        public TodoItem() { }

        public TodoItem(string text)
        {
            Text = text;
            Status = TodoStatus.NotStarted;
            LastUpdate = DateTime.Now;
        }

        public void UpdateText(string newText)
        {
            Text = newText;
            LastUpdate = DateTime.Now;
        }

        public void SetStatus(TodoStatus status)
        {
            Status = status;
            LastUpdate = DateTime.Now;
        }

        [NotMapped]
        public string ShortText => Text.Length > 30 ? Text.Replace("\n", " ").Substring(0, 30) + "..." : Text;

        public string GetShortInfo() => ShortText;

        public string GetFullInfo()
        {
            return $"Текст: {Text}\nСтатус: {Status}\nПоследнее изменение: {LastUpdate:yyyy-MM-dd HH:mm:ss}";
        }
    }
}