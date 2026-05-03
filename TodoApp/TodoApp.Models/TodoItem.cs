using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApp.Models;

public class TodoItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Text { get; private set; } = string.Empty;

    [Required]
    public TodoStatus Status { get; set; } = TodoStatus.NotStarted;

    [Required]
    public DateTime LastUpdate { get; set; } = DateTime.Now;

    [Required]
    public Guid ProfileId { get; set; }

    [ForeignKey(nameof(ProfileId))]
    public Profile? Profile { get; set; }

    public TodoItem()
    {
    }

    public TodoItem(string text)
    {
        UpdateText(text);
        Status = TodoStatus.NotStarted;
    }

    public void UpdateText(string newText)
    {
        Text = newText.Trim();
        LastUpdate = DateTime.Now;
    }

    public void SetStatus(TodoStatus status)
    {
        Status = status;
        LastUpdate = DateTime.Now;
    }

    [NotMapped]
    public string ShortText
    {
        get
        {
            var oneLineText = Text.Replace("\r", " ").Replace("\n", " ");
            return oneLineText.Length > 30 ? oneLineText[..30] + "..." : oneLineText;
        }
    }

    public string GetShortInfo() => ShortText;

    public string GetFullInfo() => $"\u0422\u0435\u043A\u0441\u0442: {Text}\n\u0421\u0442\u0430\u0442\u0443\u0441: {Status}\n\u041F\u043E\u0441\u043B\u0435\u0434\u043D\u0435\u0435 \u0438\u0437\u043C\u0435\u043D\u0435\u043D\u0438\u0435: {LastUpdate:yyyy-MM-dd HH:mm:ss}";
}
